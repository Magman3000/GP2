using Unity.Netcode;
using static MyUtility.Utility;

public class Netcode : NetworkedEntity {

    private const uint clientsLimit = 2;


    
    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {
        if (!initialized) {
            Error("Attempted to tick Netcode while it was not initialized!");
            return;
        }




    }

    public void StartNetworking() {
        
    }
    public void StopNetworking() {

        NetworkManager.Shutdown();
    }

    public bool StartAsClient() {

        return NetworkManager.StartClient();
    }        
    public bool StartAsHost() {

        return NetworkManager.StartHost();
    }

    public new bool IsHost() {
        return NetworkManager.IsHost;
    }
    public new bool IsClient() {
        return NetworkManager.IsClient;
    }
    public uint GetConnectedClientsCount() {
        return (uint)NetworkManager.ConnectedClients.Count;
    }
}
