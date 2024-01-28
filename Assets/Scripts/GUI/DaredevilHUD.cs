using UnityEngine;

public class DaredevilHUD : Entity {

    private Player playerRef;
    private Daredevil daredevilRef;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }   
    
    public void SetPlayerReference(Player player) {
        playerRef = player;
        if (playerRef)
            daredevilRef = playerRef.GetDaredevilData();
    }
    
    //TODO: Implement methods for the buttons
    public void Move()
    {
        //Calls a function on the daredevil
    }
}
