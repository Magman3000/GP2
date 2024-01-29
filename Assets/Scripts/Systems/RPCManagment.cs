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




}
