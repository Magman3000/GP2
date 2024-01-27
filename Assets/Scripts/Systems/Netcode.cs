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

    public static ulong INVALID_CLIENT_ID = 0;
    public const uint DEFAULT_SERVER_PORT = 6312;

    public enum NetworkingState {
        NONE = 0,
        LOCAL_CLIENT,
        GLOBAL_CLIENT,
        LOCAL_HOST,
        GLOBAL_HOST
    }

    [SerializeField] private bool enableNetworkLog = true;


    public NetworkingState currentState = NetworkingState.NONE;



    private const uint clientsLimit = 2;
    private uint connectedClients = 0;
    public ulong clientID = INVALID_CLIENT_ID;

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

        return encryptor.Encrypt(localIPAddress.GetAddressBytes());
        //return relayManager.currentJoinCode;
    }
    public string DecryptConnectionCode(string targetCode) {

        return encryptor.Decrypt(targetCode);
    }





    public IPAddress GetLocalIPAddress() {
        return localIPAddress;
    }


    private void DisconnectAllClients() {
        foreach(var client in networkManagerRef.ConnectedClients) {
            if (client.Value.ClientId != clientID)
                networkManagerRef.DisconnectClient(client.Value.ClientId, "Server Shutdown");
        }
    }
    public void StopNetworking() {

        networkManagerRef.Shutdown();
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Networking has stopped!");

        connectedClients = 0; //This kinda does it. 
        clientID = INVALID_CLIENT_ID;
        currentState = NetworkingState.NONE;
        if (IsHost())
            DisconnectAllClients();

        //Destory entities from game instance side? might not be required. No need to destroy anything!
    }
    public bool EnableNetworking() {

        if (currentState == NetworkingState.LOCAL_CLIENT || currentState == NetworkingState.GLOBAL_CLIENT)
            return networkManagerRef.StartClient();
        else if (currentState == NetworkingState.LOCAL_HOST || currentState == NetworkingState.GLOBAL_HOST)
            return networkManagerRef.StartHost();

        return false;
    }




    public bool StartLocalClient(string address) {
        if (enableNetworkLog)
            Log("Attempting to connect to..." + address);

        unityTransportRef.SetConnectionData(address, (ushort)DEFAULT_SERVER_PORT);
        currentState = NetworkingState.LOCAL_CLIENT;
        unityTransportRef.ConnectionData.Address = address;
        return EnableNetworking();
    }
    public bool StartGlobalClient(string targetAddress) {

        currentState = NetworkingState.GLOBAL_CLIENT;
        relayManager.JoinRelay(targetAddress);
        return true; //Start as client on code being received! callable by connection menu
    }



    public bool StartLocalHost() {
        currentState = NetworkingState.LOCAL_HOST;
        unityTransportRef.SetConnectionData(localIPAddress.ToString(), (ushort)DEFAULT_SERVER_PORT);
        return EnableNetworking();
    }
    public bool StartGlobalHost(Action<string> codeCallback) {
        currentState = NetworkingState.GLOBAL_HOST;
        relayManager.CreateRelay(codeCallback);
        return true;
    }





    public bool IsDebugLogEnabled() { return enableNetworkLog; }
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


    public UnityTransport GetUnityTransport() { return unityTransportRef; }
    public RelayManager GetRelayManager() { return relayManager; }



    //Break it into host code and client code!�
    //Callbacks
    private void OnClientConnectedCallback(ulong ID) {
        if (enableNetworkLog)
            Log("Client " + ID + " has connected!");

        if (IsHost() && networkManagerRef.ConnectedClients.Count == 1)
            clientID = ID;

        connectedClients++; //Disconnecting doesnt trigger this on relay for some reason

        //Need to do stuff with the client ID - Server Auth
        if (GetConnectedClientsCount() == 1)
            gameInstanceRef.CreatePlayerEntity(Player.PlayerIdentity.PLAYER_1);
        else if (GetConnectedClientsCount() == 2)
            gameInstanceRef.CreatePlayerEntity(Player.PlayerIdentity.PLAYER_2);
    }
    private void OnClientDisconnectCallback(ulong ID) {
        if (enableNetworkLog)
            Log("Disconnection request received from " + ID);

        connectedClients--;

        //HMMMM gotta start thinking about authority
        if (connectedClients != 2) //Technically any disconnection should interrupt.
            gameInstanceRef.InterruptGame();
    }
    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (enableNetworkLog)
            Log("Connection request received from " + request.ClientNetworkId);

        
        response.CreatePlayerObject = false;
        response.Approved = true;
        if (enableNetworkLog)
            Log("Connection request was accepted!");
    }
}
