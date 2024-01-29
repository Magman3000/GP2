using Initialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MyUtility.Utility;

public class Coordinator
{


    bool initialized;

    private bool speedBoostBool = false;
    private bool boostOnCooldown = false;
    private int boostCharges = 0;
    private float boostTimer = 0.0f;
    private float boostCooldown = 0.0f;

    GameInstance gameInstanceRef;
    Player playerRef;
    CoordinatorStats stats;


    public void Initialize(GameInstance game, Player player)
    {
        if (initialized)
            return;



        playerRef = player;
        stats = playerRef.GetCoordinatorStats();
        gameInstanceRef = game;
        initialized = true;
    }
    public void Tick()
    {
        if (!initialized)
        {
            Warning("Player attempted to tick Coordinator while it was not initialized");
            return;
        }


    }
    public void FixedTick()
    {
        if (!initialized)
        {
            Warning("Player attempted to fixed tick Coordinator while it was not initialized");
            return;
        }


    }

    private void RestoreCharges()
    {
        boostCharges = stats.GetBoostCharges();
    }

    private void SpeedBoost()
    {
        if (boostCharges <= 0 || speedBoostBool || boostOnCooldown)
            return;


        boostCharges -= 1;
        boostOnCooldown = true;
        speedBoostBool = true;
        playerRef.SetBoostCheck(speedBoostBool);
        boostCooldown = stats.GetboostCooldown();
        boostTimer = stats.GetBoostCharges();


    }
    private void BoostTimer()
    {
        if (boostTimer <= 0.0f)
            return;

        boostTimer -= Time.deltaTime;
        if (boostTimer <= 0.0f)
        {
            boostTimer = 0.0f;
            speedBoostBool = false;
            playerRef.SetBoostCheck(speedBoostBool);

        }

    }

    private void ResetBoostCooldown()
    {
        if (boostCooldown <= 0.0f)
        {
            return;
        }

        boostCooldown -= Time.deltaTime;
        if (boostCooldown <= 0.0f)
        {
            boostCooldown = 0.0f;
            boostOnCooldown = false;
        }

    }
}
