using UnityEngine;

public class CoordinatorHUD : Entity
{
    public enum CoordinatorKeyCode
    {
        ButtonOne = 0,
        ButtonTwo = 1,
        ButtonThree = 2,
        ButtonFour = 3,
        ButtonFive = 4,
        ButtonSix = 5,
        ButtonSeven = 6,
        ButtonEight = 7,
        ButtonNine = 8,
    }
    
    private Player playerRef;
    private Coordinator coordinatorRef;
    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }
    
    public void SetPlayerReference(Player player)
    {
        playerRef = player;
        if (playerRef)
            coordinatorRef = playerRef.GetCoordinatorData();
    }

    public void OnButtonPress(int code)
    {
        var keyCode = (CoordinatorKeyCode)code;
        //coordinatorRef.HandleInput(keyCode);
    }
}
