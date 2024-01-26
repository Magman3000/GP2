using UnityEngine;

public class CoordinatorHUD : Entity
{
    private Player _player;
    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }
    
    public void SetPlayerReference(Player player)
    {
        _player = player;
    }
    
    //TODO: Implement methods for the buttons
    private void TrapOne()
    {
        //Calls a function on the coordinator
    }
    private void TrapTwo()
    {
        //Calls a function on the coordinator
    }
    private void TrapThree()
    {
        //Calls a function on the coordinator
    }
    private void TrapFour()
    {
        //Calls a function on the coordinator
    }
}
