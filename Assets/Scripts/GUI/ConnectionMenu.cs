using System.Net;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MyUtility.Utility;

public class ConnectionMenu : Entity {

    private enum ConnectionMenuState {
        SELECT_MODE,
        HOST_SELECTED,
        CLIENT_SELECTED
    }


    private const string hostWaitingMessage = "Waiting for player 2 to join...";
    private const string clientSearchingMessage = "Enter code to connect!";

    private ConnectionMenuState currentState = ConnectionMenuState.SELECT_MODE;


    private Button hostButtonComp = null;
    private Button clientButtonComp = null;
    private TextMeshProUGUI statusTextComp = null;
    private TextMeshProUGUI localHostComp = null; //Could be reused for the join code later

    private TMP_InputField connectionCodeInputComp = null;


    private bool networkingActivated = false;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        gameInstanceRef = game;

        SetupReferences();
        SetupStartState();
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

        //Connection Code Input Field
        Transform connectionCodeTransform = transform.Find("ConnectionCodeInputField");
        if (!Validate(connectionCodeTransform, "ConnectionMenu failed to get reference to ConnectionCodeInputField transform", ValidationLevel.ERROR, false))
            return;
        connectionCodeInputComp = connectionCodeTransform.GetComponent<TMP_InputField>();
        if (!Validate(connectionCodeInputComp, "ConnectionMenu failed to get reference to InputField component", ValidationLevel.ERROR, false))
            return;

        //Status Text
        Transform statusTextTransform = transform.Find("StatusText");
        if (!Validate(statusTextTransform, "ConnectionMenu failed to get reference to status text transform", ValidationLevel.ERROR, false))
            return;
        statusTextComp = statusTextTransform.GetComponent<TextMeshProUGUI>();
        if (!Validate(statusTextComp, "ConnectionMenu failed to get reference to status Text component", ValidationLevel.ERROR, false))
            return;

        //LocalHost Text
        Transform localHostTextTransform = transform.Find("LocalHost");
        if (!Validate(localHostTextTransform, "ConnectionMenu failed to get reference to LocalHost text transform", ValidationLevel.ERROR, false))
            return;
        localHostComp = localHostTextTransform.GetComponent<TextMeshProUGUI>();
        if (!Validate(localHostComp, "ConnectionMenu failed to get reference to LocalHost Text component", ValidationLevel.ERROR, false))
            return;
    }
    private void SetupStartState() {
        localHostComp.gameObject.SetActive(false);
        statusTextComp.gameObject.SetActive(false);
        hostButtonComp.gameObject.SetActive(true);
        clientButtonComp.gameObject.SetActive(true);
        connectionCodeInputComp.gameObject.SetActive(false);
        networkingActivated = false;
        connectionCodeInputComp.characterLimit = 15; //This should be the limit on ip address decrypted values XXX.XXX.XXX.XXX
    }

    public void SearchButton() {
        //Netcode netcodeRef = gameInstanceRef.GetNetcode();
        //string code = IPAddress.Broadcast.ToString();
        //IPAddress address = netcodeRef.GetLocalIPAddress();
        //
        //code = address.GetAddressBytes()[0].ToString() + "." + address.GetAddressBytes()[1].ToString() + "." + address.GetAddressBytes()[2].ToString() + "." + "255";
        //Log("Broadcasting at " + code);
        //netcodeRef.StartAsClient(code);
    }
    public void ConfirmConnectionCode() {

        Netcode netcodeRef = gameInstanceRef.GetNetcode();
        //string connectionCode = netcodeRef.DecryptConnectionCode(connectionCodeInputComp.text);
        if (connectionCodeInputComp.text != null)
            netcodeRef.StartAsClient(connectionCodeInputComp.text);
        else {
            if (gameInstanceRef.IsDebuggingEnabled())
                Warning("Invalid code received after decryption");
        }
            
        if (gameInstanceRef.IsDebuggingEnabled())
            Log("Attempting to connect to " + connectionCodeInputComp.text);
    }

    public void UpdateConnectionCode(string code) {
        localHostComp.text = "Join Code: " + code;
    }
    public void HostButton() {
        gameInstanceRef.GetNetcode().StartGlobalHost(UpdateConnectionCode);
        statusTextComp.gameObject.SetActive(true);
        localHostComp.gameObject.SetActive(true);
        hostButtonComp.gameObject.SetActive(false);
        clientButtonComp.gameObject.SetActive(false);
        connectionCodeInputComp.gameObject.SetActive(false);
        statusTextComp.text = hostWaitingMessage;



        
        
        networkingActivated = true;
    }
    public void ClientButton() {
        //gameInstanceRef.GetNetcode().StartAsClient();
        connectionCodeInputComp.gameObject.SetActive(true);
        statusTextComp.gameObject.SetActive(true);
        localHostComp.gameObject.SetActive(false);
        hostButtonComp.gameObject.SetActive(false);
        clientButtonComp.gameObject.SetActive(false);
        statusTextComp.text = clientSearchingMessage;
        networkingActivated = true;
    }
    public void BackButton() {

        if (currentState == ConnectionMenuState.HOST_SELECTED) {
            currentState = ConnectionMenuState.SELECT_MODE;


        }
        if (currentState == ConnectionMenuState.CLIENT_SELECTED) {
            currentState = ConnectionMenuState.SELECT_MODE;


        }
        if (currentState == ConnectionMenuState.SELECT_MODE)
            gameInstanceRef.Transition(GameInstance.GameState.MAIN_MENU);



        if (networkingActivated) {
            gameInstanceRef.GetNetcode().StopNetworking();
            statusTextComp.gameObject.SetActive(false);
            localHostComp.gameObject.SetActive(false);
            connectionCodeInputComp.gameObject.SetActive(false); //Reset its text too!
            hostButtonComp.gameObject.SetActive(true);
            clientButtonComp.gameObject.SetActive(true);
            networkingActivated = false;
        }
        else
            gameInstanceRef.Transition(GameInstance.GameState.MAIN_MENU);
    }
}
