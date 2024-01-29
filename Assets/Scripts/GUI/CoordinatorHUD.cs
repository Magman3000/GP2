using UnityEngine;

public class CoordinatorHUD : Entity
{
    public enum SymbolCode {
        CODE_1 = 0,
    }


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
    public void CodeKeyButton(int keycode)
    {
        var code = (SymbolCode)keycode;

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
