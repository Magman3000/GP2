using Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinator
{

    private int boostCharges;
    bool initialized;

    private float boostTimer = 0.0f;

    GameInstance gameInstanceRef;
    Player playerRef;
    
    
    
    public void Initialize(GameInstance game, Player player)
    {
        if (initialized)
            return;



        playerRef = player;
        //boostCharges = playerRef.playerStats.GetBoostCharges(); //??
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
        //boostCharges = playerRef.playerStats.GetBoostCharges();
    }

    private void SpeedBoost()
    {
        //if (playerRef.playerStats.GetBoostCharges() <= 0 || playerRef.daredevil.boosting)
            //return;


        boostCharges -= 1;
        //playerRef.daredevil.boosting = true;
        
        //boostTimer = playerRef.playerStats.GetBoostCharges();


    }
    private void BoostTimer()
    {
        if (boostTimer <= 0.0f)
            return;

        boostTimer -= Time.deltaTime;
        if (boostTimer <= 0.0f)
        {
            boostTimer = 0.0f;
            //playerRef.daredevil.boosting = false;
            
        }

    }
}
