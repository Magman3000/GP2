using Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinator
{
    private float boostTimer = 0.0f;
    
    private int boostCharges;
    bool initialized;
    GameInstance gameInstanceRef;
    Player playerRef;
    
    
    
    public void Initialize(GameInstance game, Player player)
    {
        if (initialized)
            return;



        playerRef = player;
        gameInstanceRef = game;
        initialized = true;
    }
    public void Tick()
    {



    }
    public void FixedTick()
    {

    }

    private void RestoreCharges()
    {
        boostCharges = playerRef.playerOneStats.GetBoostCharges();
    }

    private void SpeedBoost()
    {
        if (playerRef.playerOneStats.GetBoostCharges() <= 0 || playerRef._dareDevil.boosting)
            return;


        boostCharges -= 1;
        playerRef._dareDevil.boosting = true;
        
        boostTimer = playerRef.playerOneStats.GetBoostCharges();


    }
    private void BoostTimer()
    {
        if (boostTimer <= 0.0f)
            return;

        boostTimer -= Time.deltaTime;
        if (boostTimer <= 0.0f)
        {
            boostTimer = 0.0f;
            playerRef._dareDevil.boosting = false;
            
        }

    }
}
