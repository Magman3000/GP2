using System;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;
using static MyUtility.Utility;

public class Netcode : Entity {


    public static ulong INVALID_CLIENT_ID = 0;

    private const uint clientsLimit = 2;
    private ulong clientID = INVALID_CLIENT_ID;

    private NetworkManager networkManagerRef = null;
    
    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        networkManagerRef = GetComponent<NetworkManager>();


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



    public void StopNetworking() {

        networkManagerRef.Shutdown();
        //Destory entities from game instance side? might not be required.
    }

    public bool StartAsClient() {

        return networkManagerRef.StartClient();
    }        
    public bool StartAsHost() {

        return networkManagerRef.StartHost();
    }

    public new bool IsHost() {
        return networkManagerRef.IsHost;
    }
    public new bool IsClient() {
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
        Log("Connection request received from " + ID);
    }
    private void OnClientDisconnectCallback(ulong ID) {
        Log("Disconnection request received from " + ID);
    }
    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        Log("Connection request received from " + request.ClientNetworkId);

    }
}
