using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using static MyUtility.Utility;

public class RPCManagment : NetworkedEntity {


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {
        if (!initialized)
            return;




    }


    [ServerRpc (RequireOwnership = true)]
    public void ConfirmConnectionServerRpc() {
        Log("Server rpc ConfirmConnectionServerRpc called!");
        RelayConnectionConfirmationClientRpc();
    }

    [ClientRpc]
    public void RelayConnectionConfirmationClientRpc() {
        Log("Client rpc RelayConnectionConfirmationClientRpc called!");
        gameInstanceRef.ConfirmAllClientsConnected();
    }




    [ServerRpc (RequireOwnership = true)]
    public void UpdateReadyCheckServerRpc(ulong senderID, bool value) {
        ClientRpcParams clientRpcParams = new ClientRpcParams();
        clientRpcParams.Send = new ClientRpcSendParams();

        Netcode netcodeRef = gameInstanceRef.GetNetcode();
        var targetID = netcodeRef.GetOtherClient(senderID);
        if (targetID == senderID) {
            Log("Other client look up failed!");
            return;
        }

        clientRpcParams.Send.TargetClientIds = new ulong[] { targetID };



        RelayReadyCheckClientRpc(senderID, value, clientRpcParams);
    }

    [ClientRpc]
    public void RelayReadyCheckClientRpc(ulong senderID, bool value, ClientRpcParams paramsPack) {
        //if (senderID == (ulong)Netcode.GetClientID())
        //    return;

        Log("I received rpc " + Netcode.GetClientID() + "\nValue : " + value);

    }








}
