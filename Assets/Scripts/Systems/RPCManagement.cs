using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using static MyUtility.Utility;
using static UnityEngine.GraphicsBuffer;

public class RPCManagement : NetworkedEntity {


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
        gameInstanceRef.GetRoleSelectMenu().ReceiveReadyCheckRPC(value);
    }


    [ServerRpc (RequireOwnership = false)]
    public void UpdateRoleSelectionServerRpc(ulong senderID, Player.Identity identity) {
        Netcode netcodeRef = gameInstanceRef.GetNetcode();
        var targetID = netcodeRef.GetOtherClient(senderID); //Do more elegant solution
        if (targetID == senderID) {
            Log("Other client look up failed!");
            return;
        }

        var clientParams = CreateClientRpcParams(targetID);
        RelayRoleSelectionClientRpc(senderID, identity, clientParams);
    }
    [ClientRpc]
    public void RelayRoleSelectionClientRpc(ulong senderID, Player.Identity identity, ClientRpcParams paramsPack) {
        gameInstanceRef.GetRoleSelectMenu().ReceiveRoleSelectionRPC(identity);
    }


    [ServerRpc (RequireOwnership = true)]
    public void ConfirmRoleSelectionServerRpc(ulong senderID) {
        Netcode netcodeRef = gameInstanceRef.GetNetcode();
        var targetID = netcodeRef.GetOtherClient(senderID); //Do more elegant solution
        if (targetID == senderID) {
            Log("Other client look up failed!");
            return;
        }

        var clientParams = CreateClientRpcParams(targetID);
        RelayRoleSelectionConfirmationClientRpc(senderID, clientParams);
    }
    [ClientRpc]
    public void RelayRoleSelectionConfirmationClientRpc(ulong senderID, ClientRpcParams paramsPack) {
        gameInstanceRef.GetLevelManagement().QueueLevelLoadKey("DebugLevel"); //Temporary
        gameInstanceRef.StartGame();
    }



}
