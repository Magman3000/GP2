using System;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using static MyUtility.Utility;


public class RelayManager {

    private bool initialized = false;


    private const uint maximumAllowedClients = 2;
    public string currentJoinCode = string.Empty;
    public bool signedIn = false;
    public bool test = false;

    private Netcode netcodeRef = null;
    private Allocation hostAllocation = null;
    private JoinAllocation clientAllocation = null;
    private RelayServerData relayServerData;

    public void Initialize(Netcode netcode) {
        if (initialized) {
            Warning("Attempted to initialize RelayManager after it was already initialized!");
            return;
        }


        InitializeUnityServices();
        //SignIn();

        netcodeRef = netcode;
        
    }
    public void Tick() {



        //Use callbacks to do this instead!
        if (initialized && !signedIn && !test) {
            test = true;
            SignIn();
        }
    }



    private async void InitializeUnityServices() {
        await UnityServices.InitializeAsync();
        initialized = true;
    }
    private async void SignIn() {
        AuthenticationService.Instance.SignedIn += SignInCallback;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    public async void CreateRelay(Action<string> codeCallback) {

        try {
            hostAllocation = await RelayService.Instance.CreateAllocationAsync((int)maximumAllowedClients - 1, null); //-Host
            currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
            Log(currentJoinCode);
            codeCallback(currentJoinCode);

            relayServerData = new RelayServerData(hostAllocation, "dtls");
            //Approach 1
            netcodeRef.GetUnityTransport().SetRelayServerData(
                relayServerData
            );

            //netcodeRef.StartHost();
            netcodeRef.EnableNetworking();

        } catch(RelayServiceException exception) {
            Error("Relay exception caught at creating relay!\n" + exception.Message);
        }

    }
    public async void JoinRelay(string code) {

        try {
            if (netcodeRef.IsDebugLogEnabled())
                Log("Joining relay with code " + code);
            clientAllocation =  await RelayService.Instance.JoinAllocationAsync(code);


            RelayServerData relayServerData2 = new RelayServerData(clientAllocation, "dtls");
            netcodeRef.GetUnityTransport().SetRelayServerData(relayServerData2);
            //netcodeRef.StartAsClient();
            netcodeRef.EnableNetworking();
        }
        catch (RelayServiceException exception) {
            Error("Relay exception caught!\n" + exception.Message);
        }
    }


    public static bool IsSignedIn() { return AuthenticationService.Instance.IsSignedIn; }


    private void SignInCallback() {
        if (netcodeRef.IsDebugLogEnabled())
            Log("Signed in using ID " + AuthenticationService.Instance.PlayerId);
        signedIn = true;
    }
}
