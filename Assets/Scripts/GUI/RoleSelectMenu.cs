using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;

public class RoleSelectMenu : Entity {

    public bool player1Ready = false;
    public bool player2Ready = false;

    public Player.PlayerIdentity player1Identity = Player.PlayerIdentity.NONE;
    public Player.PlayerIdentity player2Identity = Player.PlayerIdentity.NONE;

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
    public void SetupMenuStartState() {
        player1Ready = false;
        player2Ready = false;

        player1Identity = Player.PlayerIdentity.NONE;
        player2Identity = Player.PlayerIdentity.NONE;

        //Update GUI
        UpdateGUI();
    }
    private void UpdateGUI() {

    }




    public void ReadyButton() {
        //Check if both ready and check if both are different roles
        long clientID = gameInstanceRef.GetNetcode().GetClientID();
        if (clientID == Netcode.INVALID_CLIENT_ID) {
            Error("Invalid client ID detected at RoleSelectMenu!");
            return;
        }

        if (clientID == 0)
            Log("Client 0 is ready!");
        else if (clientID == 1)
            Log("Client 1 is ready!");
    }
    public void CoordinatorButton() {
        long clientID = gameInstanceRef.GetNetcode().GetClientID();
        if (clientID == Netcode.INVALID_CLIENT_ID) {
            Error("Invalid client ID detected at RoleSelectMenu!");
            return;
        }

        if (clientID == 0)
            Log("Client 0 is Coordinator!");
        else if (clientID == 1)
            Log("Client 1 is Coordinator!");
    }
    public void DaredevilButton() {
        long clientID = gameInstanceRef.GetNetcode().GetClientID();
        if (clientID == Netcode.INVALID_CLIENT_ID) {
            Error("Invalid client ID detected at RoleSelectMenu!");
            return;
        }

        if (clientID == 0)
            Log("Client 0 is Daredevil!");
        else if (clientID == 1)
            Log("Client 1 is Daredevil!");
    }
}
