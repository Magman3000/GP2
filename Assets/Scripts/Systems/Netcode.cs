using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using static MyUtility.Utility;






public class Netcode : Entity {

    [SerializeField] private bool enableDebugLog = true;
    public static ulong INVALID_CLIENT_ID = 0;

    private const uint clientsLimit = 2;
    private uint connectedClients = 0;
    private ulong clientID = INVALID_CLIENT_ID;

    private IPAddress localIPAddress = null;
    
    private Encryptor encryptor;

    private NetworkManager networkManagerRef = null;
    private UnityTransport unityTransportRef = null;

    private RelayManager relayManager = null;


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        networkManagerRef = GetComponent<NetworkManager>();
        unityTransportRef = networkManagerRef.GetComponent<UnityTransport>();

        relayManager = new RelayManager();
        relayManager.Initialize(this);

        //transportLayer.ConnectionData.Address = "192.0.0.1";

        encryptor = new Encryptor();
        QueuryOwnIPAddress();
        SetupCallbacks();
        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {
        if (!initialized) {
            Error("Attempted to tick Netcode while it was not initialized!");
            return;
        }


        relayManager.Tick();
    }
    private void SetupCallbacks() {
        networkManagerRef.OnClientConnectedCallback += OnClientConnectedCallback;
        networkManagerRef.OnClientDisconnectCallback += OnClientDisconnectCallback;
        networkManagerRef.ConnectionApprovalCallback += ConnectionApprovalCallback;
    }
    private void QueuryOwnIPAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var address in host.AddressList) {
            if (address.AddressFamily == AddressFamily.InterNetwork) {
                localIPAddress = address;
                return;
            }
        }
    }


    public string GetEncryptedLocalHost() {

        //return encryptor.Encrypt(localIPAddress.GetAddressBytes());
        return relayManager.currentJoinCode;
    }
    public string DecryptConnectionCode(string targetCode) {

        return encryptor.Decrypt(targetCode);
    }





    public IPAddress GetLocalIPAddress() {
        return localIPAddress;
    }

    public void StopNetworking() {

        networkManagerRef.Shutdown();
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Networking has stopped!");
        clientID = INVALID_CLIENT_ID;
        //Destory entities from game instance side? might not be required.
    }

    public bool StartAsClient(string targetAddress) {
        //localIPAddress.ToString();
        //Log("Attempting to connect to..." + targetAddress);
        //unityTransportRef.ConnectionData.Address = targetAddress;
        //Log(transportLayer.ConnectionData.Address);


        relayManager.JoinRelay(targetAddress);
        return true; //Start as client on code being received! callable by connection menu
    }
    public bool StartAsClient() {

        return networkManagerRef.StartClient();
    }




    public bool StartGlobalHost(Action<string> codeCallback) {

        //transportLayer.ConnectionData.Address = "0.0.0.0"; //??
        // unityTransportRef.ConnectionData.ServerListenAddress = localIPAddress.ToString();

        //Log("Host started listening on " + unityTransportRef.ConnectionData.Address);

        relayManager.CreateRelay(codeCallback);
        return true;
    }
    public void StartHost() {
        networkManagerRef.StartHost();
    }

    public bool IsHost() {
        return networkManagerRef.IsHost;
    }
    public bool IsClient() {
        return networkManagerRef.IsClient;
    }
    public uint GetConnectedClientsCount() {
        return connectedClients;
    }
    public ulong GetClientID() {
        return clientID;
    }
    public bool IsDebugLogEnabled() { return enableDebugLog; }

    public UnityTransport GetUnityTransport() { return unityTransportRef; }


    //Callbacks
    private void OnClientConnectedCallback(ulong ID) {
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Client " + ID + " has connected!");

        connectedClients++;

        //Need to do stuff with the client ID
        if (GetConnectedClientsCount() == 1)
            gameInstanceRef.CreatePlayerEntity(Player.PlayerIdentity.PLAYER_1);
        else if (GetConnectedClientsCount() == 2)
            gameInstanceRef.CreatePlayerEntity(Player.PlayerIdentity.PLAYER_2);
    }
    private void OnClientDisconnectCallback(ulong ID) {
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Disconnection request received from " + ID);
    }
    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Connection request received from " + request.ClientNetworkId);

        
        response.CreatePlayerObject = false;
        response.Approved = true;
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Connection request was accepted!");
    }
}
