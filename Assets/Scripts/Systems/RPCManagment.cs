using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using static MyUtility.Utility;
using static UnityEngine.GraphicsBuffer;

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
    private ClientRpcParams CreateClientRpcParams(ulong targetID) {
        ClientRpcParams clientRpcParams = new ClientRpcParams();
        clientRpcParams.Send = new ClientRpcSendParams();
        clientRpcParams.Send.TargetClientIds = new ulong[] { targetID };
        return clientRpcParams;
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






    [ServerRpc (RequireOwnership = false)]
    public void UpdateReadyCheckServerRpc(ulong senderID, bool value) {
        Netcode netcodeRef = gameInstanceRef.GetNetcode();
        var targetID = netcodeRef.GetOtherClient(senderID); //Do more elegant solution
        if (targetID == senderID) {
            Log("Other client look up failed!");
            return;
        }

        var clientParams = CreateClientRpcParams(targetID);
        RelayReadyCheckClientRpc(senderID, value, clientParams);
    }

    [ClientRpc]
    public void RelayReadyCheckClientRpc(ulong senderID, bool value, ClientRpcParams paramsPack) {
        //if (senderID == (ulong)Netcode.GetClientID())
        //    return;

        Log("I received rpc from " + senderID + "\nValue : " + value);
        gameInstanceRef.GetRoleSelectMenu().ReceiveReadyCheckRPC(senderID, value);
    }








}
