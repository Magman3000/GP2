using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;
using static MyUtility.Utility;

public class Netcode : Entity {


    public static ulong INVALID_CLIENT_ID = 0;
    private const UInt32 encryptionKey = 0x04080219;

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
        EncryptionTest();
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
    private void EncryptionTest() {
        IPAddress testAddress = localIPAddress;
        for (int i = 0; i < localIPAddress.GetAddressBytes().Length; i++) {
            Byte encryptionByte = (Byte)(encryptionKey >> 8 * i); //0xff
            Byte dataByte = testAddress.GetAddressBytes()[i];

            Log("Before encryption " + dataByte);
            dataByte ^= encryptionByte;
            Log("After encryption " + dataByte);
            //Log("Encryption Byte " + i + " is " + encryptionByte);

        }
    }


    public string GetEncryptedLocalHost() {
        //65 - 90 A-Z
        //Old Range: 

        Byte[] addressBytes = localIPAddress.GetAddressBytes();
        for (int i = 0; i < addressBytes.Length; i++) {
            addressBytes[i] ^= (Byte)(encryptionKey >> 8 * i);
            //if(addressBytes[i] > )
        }

        
        //

        return Encoding.ASCII.GetString(addressBytes); ;
    }
    public string ApplyEncryptionKey(string address) {


        //for (int i = 0; i < addressBytes.Length; i++) {
        //  addressBytes[i] ^= (Byte)(encryptionKey >> 8 * i);
        //}



    //Make sure its a certain size!
    IPAddress Result = localIPAddress;
        string encryptedAddress = address;

        var addressLength = address.Length;
        Byte[] Bytes = new byte[4];

        IPAddress test = new IPAddress(address[0]);

        for (int i = 0; i < localIPAddress.GetAddressBytes().Length; i++) {
            Byte encryptionByte = (Byte)(encryptionKey >> 8 * i); //0xff
            Byte dataByte = Result.GetAddressBytes()[i];

            Log("Before encryption " + dataByte);
            dataByte ^= encryptionByte;
            Log("After encryption " + dataByte);
            //Log("Encryption Byte " + i + " is " + encryptionByte);

        }

        return encryptedAddress;
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
        Log("Attempting to connect to..." + targetAddress);
        transportLayer.ConnectionData.Address = targetAddress;


        //Log(transportLayer.ConnectionData.Address);
        return networkManagerRef.StartClient(); //Start as client on code being received! callable by connection menu
    }        
    public bool StartAsHost() {

        transportLayer.ConnectionData.Address = "0.0.0.0"; //??
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
