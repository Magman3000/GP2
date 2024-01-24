using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MyUtility.Utility;

public class ConnectionMenu : Entity {


    private const string hostWaitingMessage = "Waiting for player 2 to join...";
    private const string clientSearchingMessage = "Searching for players to connect with...";


    private Button hostButtonComp = null;
    private Button clientButtonComp = null;
    private TextMeshProUGUI statusTextComp = null;


    private bool performingTask = false;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        SetupReferences();
        gameInstanceRef = game;
        initialized = true;
    }
    private void SetupReferences() {

        //Host Button
        Transform hostButtonTransform = transform.Find("HostButton");
        if (!Validate(hostButtonTransform, "ConnectionMenu failed to get reference to HostButton transform", ValidationLevel.ERROR, false))
            return;
        hostButtonComp = hostButtonTransform.GetComponent<Button>();
        if (!Validate(hostButtonComp, "ConnectionMenu failed to get reference to host Button component", ValidationLevel.ERROR, false))
            return;

        //Client Button
        Transform clientButtonTransform = transform.Find("ClientButton");
        if (!Validate(clientButtonTransform, "ConnectionMenu failed to get reference to ClientButton transform", ValidationLevel.ERROR, false))
            return;
        clientButtonComp = clientButtonTransform.GetComponent<Button>();
        if (!Validate(clientButtonComp, "ConnectionMenu failed to get reference to client Button component", ValidationLevel.ERROR, false))
            return;

        //Status Text
        Transform statusTextTransform = transform.Find("StatusText");
        if (!Validate(statusTextTransform, "ConnectionMenu failed to get reference to status text transform", ValidationLevel.ERROR, false))
            return;
        statusTextComp = statusTextTransform.GetComponent<TextMeshProUGUI>();
        if (!Validate(statusTextComp, "ConnectionMenu failed to get reference to status Text component", ValidationLevel.ERROR, false))
            return;
    }


    public void HostButton() {
        gameInstanceRef.GetNetcode().StartAsHost();
        statusTextComp.gameObject.SetActive(true);
        hostButtonComp.gameObject.SetActive(false);
        clientButtonComp.gameObject.SetActive(false);
        statusTextComp.text = hostWaitingMessage;
        performingTask = true;
    }
    public void ClientButton() {
        gameInstanceRef.GetNetcode().StartAsClient();
        statusTextComp.gameObject.SetActive(true);
        hostButtonComp.gameObject.SetActive(false);
        clientButtonComp.gameObject.SetActive(false);
        statusTextComp.text = clientSearchingMessage;
        performingTask = true;
    }
    public void BackButton() {
        if (performingTask) {
            gameInstanceRef.GetNetcode().StopNetworking();
            statusTextComp.gameObject.SetActive(false);
            hostButtonComp.gameObject.SetActive(true);
            clientButtonComp.gameObject.SetActive(true);
            performingTask = false;
            return;
        }

        gameInstanceRef.Transition(GameInstance.GameState.MAIN_MENU);
    }
}
