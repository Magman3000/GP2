using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MyUtility.Utility;

public class RoleSelectMenu : Entity {

    public bool player1Ready = false;
    public bool player2Ready = false;

    public Player.PlayerIdentity player1Identity = Player.PlayerIdentity.NONE;
    public Player.PlayerIdentity player2Identity = Player.PlayerIdentity.NONE;



    private Image client1ReadyCheck = null;
    private Image client2ReadyCheck = null;


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        SetupReferences();
        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {
        if (!initialized)
            return;





    }
    public void SetupReferences() {

        //Ready Button
        Transform readyButtonTransform = transform.Find("ReadyButton");
        Validate(readyButtonTransform, "Failed to get ReadyButton transform", ValidationLevel.ERROR, true);

        //Client 1 Ready Check
        Transform client1Transform = readyButtonTransform.Find("Client1ReadyCheck");
        Validate(client1Transform, "Failed to get Client1ReadyCheck transform", ValidationLevel.ERROR, true);
        client1ReadyCheck = client1Transform.GetComponent<Image>();
        Validate(client1ReadyCheck, "Failed to get Image1 component", ValidationLevel.ERROR, true);

        //Client 2 Ready Check
        Transform client2Transform = readyButtonTransform.Find("Client2ReadyCheck");
        Validate(client2Transform, "Failed to get Client2ReadyCheck transform", ValidationLevel.ERROR, true);
        client2ReadyCheck = client2Transform.GetComponent<Image>();
        Validate(client2ReadyCheck, "Failed to get Image2 component", ValidationLevel.ERROR, true);


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


    public void ReceiveReadyCheckRPC(ulong senderID, bool value) {

    }


    public void ReadyButton() {
        //Check if both ready and check if both are different roles
        long clientID = Netcode.GetClientID();

        if (player1Ready)
            player1Ready = false;
        else if (!player1Ready)
            player1Ready = true;

        gameInstanceRef.GetRPCManagment().UpdateReadyCheckServerRpc((ulong)clientID, player1Ready);

        //if (clientID == 0)
        //    Log("Client 0 is ready!");
        //else if (clientID == 1)
        //    Log("Client 1 is ready!");
    }
    public void CoordinatorButton() {
        long clientID = Netcode.GetClientID();


        if (clientID == 0)
            Log("Client 0 is Coordinator!");
        else if (clientID == 1)
            Log("Client 1 is Coordinator!");
    }
    public void DaredevilButton() {
        long clientID = Netcode.GetClientID();


        if (clientID == 0)
            Log("Client 0 is Daredevil!");
        else if (clientID == 1)
            Log("Client 1 is Daredevil!");
    }
}
