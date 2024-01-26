using UnityEngine;

public class DaredevilHUD : Entity
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
}
