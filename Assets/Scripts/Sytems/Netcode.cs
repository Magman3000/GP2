using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using static MyUtility.Utility;

public class Netcode : Entity {


    public static ulong INVALID_CLIENT_ID = 0;

    private const uint clientsLimit = 2;
    private ulong clientID = INVALID_CLIENT_ID;

    private IPAddress localIPAddress = null;
    

    private NetworkManager networkManagerRef = null;
    private UnityTransport transportLayer = null;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        networkManagerRef = GetComponent<NetworkManager>();
        transportLayer = networkManagerRef.GetComponent<UnityTransport>();
        //transportLayer.ConnectionData.Address = "192.0.0.1";

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


    public void StopNetworking() {

        networkManagerRef.Shutdown();
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Networking has stopped!");
        clientID = INVALID_CLIENT_ID;
        //Destory entities from game instance side? might not be required.
    }

    public bool StartAsClient() {
        //localIPAddress.ToString();
        transportLayer.ConnectionData.Address = "192.168.0.255";
        Log(transportLayer.ConnectionData.Address);
        return networkManagerRef.StartClient();
    }        
    public bool StartAsHost() {

        transportLayer.ConnectionData.Address = "0.0.0.0";
        Log(transportLayer.ConnectionData.Address);
        return networkManagerRef.StartHost();
    }

    public bool IsHost() {
        return networkManagerRef.IsHost;
    }
    public bool IsClient() {
        return networkManagerRef.IsClient;
    }
    public uint GetConnectedClientsCount() {
        return (uint)networkManagerRef.ConnectedClients.Count;
    }
    public ulong GetClientID() {
        return clientID;
    }



    //Callbacks
    private void OnClientConnectedCallback(ulong ID) {
        Log("Client " + ID + " has connected!");
    }
    private void OnClientDisconnectCallback(ulong ID) {
        Log("Disconnection request received from " + ID);
    }
    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        Log("Connection request received from " + request.ClientNetworkId);

        
        response.CreatePlayerObject = false;
        response.Approved = true;
    }
}
