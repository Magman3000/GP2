using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;
using static MyUtility.Utility;


//nao skf fd4 gdf
//192.142.0.1

//Requires 256 values 0-255
enum DecryptionCodes {
    Q11 = 0, W11 = 1, E11 = 2, R11 = 4, T11 = 5, Y11 = 6, U11 = 7, I11 = 8, O11 = 9, P11 = 10,
    A12 = 11, S13 = 12, D14 = 13, F15 = 14, G16 = 15, H17 = 16, J18 = 17, K19 = 18, L20 = 19,
    Z21 = 20, X22 = 21, C23 = 22, V24 = 23, B25 = 24, N26 = 25, M27 = 26,

    q11 = 27, w12 = 28, e13 = 29, r14 = 30, t15 = 31, y16 = 32, u17 = 33, i18 = 34, o19 = 35, p20 = 36,
    a21 = 37, s22 = 38, d23 = 39, f24 = 40, g25 = 41, h26 = 42, j27 = 43, k28 = 44, l29 = 45,
    z30 = 46, x31 = 47, c32 = 48, v33 = 49, b34 = 50, n35 = 51, m36 = 52,

    Mq2 = 53, Nw3 = 54, Be4 = 55, Vr5 = 56, Ct6 = 57, Xy7 = 58, Zu8 = 59, Li9 = 60, Ko1 = 61, Jp1 = 62,
    Ha2 = 63, Gs3 = 64, Fd4 = 65, Df5 = 66, Sg6 = 67, Ah7 = 68, Pj8 = 69, Ok9 = 70, Il1 = 71,
    Uz2 = 72, Yx3 = 73, Tc4 = 74, Rv5 = 75, Eb6 = 76, Wn7 = 77, Qm8 = 78,

    MQ2 = 79, NW3 = 80, BE4 = 81, VR5 = 82, CT6 = 83, XY7 = 84, ZU8 = 85, LI9 = 86, KO1 = 87, JP2 = 88,
    HA1 = 89, GS3 = 90, FD4 = 91, DF5 = 92, SG6 = 93, AH7 = 94, PJ8 = 95, OK1 = 96, IL2 = 97,
    UZ3 = 98, YX4 = 99, TC5 = 100, RV6 = 101, EB7 = 102, WN8 = 103, QM9 = 104,


    Q12 = 105, W23 = 106, E34 = 107, R45 = 108, T56 = 109, Y67 = 110, U78 = 111, I89 = 112, O91 = 113, P10 = 114,
    A11 = 115, S12 = 116, D13 = 117, F14 = 118, G15 = 119, H16 = 120, J17 = 121, K18 = 122, L19 = 123,
    Z20 = 124, X21 = 125, C22 = 126, V23 = 127, B24 = 128, N25 = 129, M26 = 130,

    q12 = 131, w22 = 132, e33 = 133, r44 = 134, t55 = 135, y66 = 136, u77 = 137, i88 = 138, o99 = 139, p10 = 140,
    a11 = 141, s12 = 142, d13 = 143, f14 = 144, g15 = 145, h16 = 146, j17 = 147, k18 = 148, l19 = 149,
    z20 = 150, x21 = 151, c22 = 152, v23 = 153, b24 = 154, n25 = 155, m26 = 156,

    Mq1 = 157, Nw2 = 158, Be3 = 159, Vr4 = 160, Ct5 = 161, Xy6 = 162, Zu7 = 163, Li8 = 164, Ko9 = 165, Jp2 = 166,
    Ha1 = 167, Gs1 = 168, Fd1 = 169, Df1 = 170, Sg1 = 171, Ah1 = 172, Pj1 = 173, Ok1 = 174, Il2 = 175,
    Uz3 = 176, Yx2 = 177, Tc2 = 178, Rv2 = 179, Eb2 = 180, Wn2 = 181, Qm2 = 182,

    MQ1 = 183, NW2 = 184, BE3 = 185, VR4 = 186, CT5 = 187, XY6 = 188, ZU7 = 189, LI8 = 190, KO9 = 191, JP1 = 192,
    HA3 = 193, GS1 = 194, FD1 = 195, DF1 = 196, SG1 = 197, AH1 = 198, PJ1 = 199, OK2 = 200, IL1 = 201,
    UZ2 = 202, YX2 = 203, TC2 = 204, RV3 = 205, EB2 = 206, WN2 = 207, QM2 = 208,


    Qq1 = 209, Ww1 = 210, Ee2 = 211, Rr3 = 212, Tt4 = 213, Yy5 = 214, Uu6 = 215, Ii1 = 216, Oo2 = 217, Pp3 = 218,
    Aa4 = 219, Ss5 = 220, Dd6 = 221, Ff7 = 222, Gg8 = 223, Hh9 = 224, Jj1 = 225, Kk2 = 226, Ll3 = 226,
    Zz4 = 227, Xx5 = 228, Cc6 = 229, Vv7 = 230, Bb8 = 231, Nn9 = 232, Mm1 = 233,

    qQ1 = 234, wW2 = 235, eE3 = 236, rR4 = 237, tT5 = 238, yY6 = 239, uU7 = 240, iI8 = 241, oO9 = 242, pP1 = 243,
    aA1 = 244, sS2 = 245, dD3 = 246, fF4 = 247, gG5 = 248, hH6 = 249, jJ7 = 250, kK8 = 251, lL1 = 252,
    zZ2 = 253, xX3 = 254, cC4 = 255
}




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


    public string GetEncryptedLocalHost() {

        string connectionCode = "";
        Byte[] addressBytes = localIPAddress.GetAddressBytes();
        for (int i = 0; i < addressBytes.Length; i++) {
            addressBytes[i] ^= (Byte)(encryptionKey >> 8 * i); //First encryption
            connectionCode += ((DecryptionCodes)addressBytes[i]); //Second encryption
        }

        return connectionCode;
    }
    public string DecryptConnectionCode(string targetCode) {
        if (targetCode.Length != 12) //Double check that its 12 bytes (X.X.X.X = 4 * 3 characters per code)
            return null;


        //ConnectionCode decryption (1st decryption)
        Byte[] parsedAddress = new byte[4];
        int addressCursor = 0;

        Byte[] addressBytes = Encoding.ASCII.GetBytes(targetCode);
        for (int i = 0; i < addressBytes.Length;) { 

            //Pack each 3 bytes together
            Byte[] decryptedBytes = new Byte[3];
            for (int j = 0; j < 3; j++)
                decryptedBytes[j] = addressBytes[i + j];

            //Parse code and get value
            string code = Encoding.ASCII.GetString(decryptedBytes);
            DecryptionCodes parsedCode = (DecryptionCodes)System.Enum.Parse(typeof(DecryptionCodes), code);
            int value = (int)parsedCode;
            parsedAddress[addressCursor] = (byte)value;
            addressCursor++;

            i += 3;//Move 3 bytes each step
        }


        string connectionCode = "";
        //IP Address decryption (2nd decryption)
        for (int i = 0; i < parsedAddress.Length; i++) {
            parsedAddress[i] ^= (Byte)(encryptionKey >> 8 * i);
            if (i == parsedAddress.Length - 1)
                connectionCode += parsedAddress[i];
            else
                connectionCode += parsedAddress[i] + ".";
        }

        return connectionCode;
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
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Client " + ID + " has connected!");

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
