using Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DareDevil
{

    bool initialized = false;
    GameInstance gameInstanceRef;
    Player playerRef;
    
    public bool boosting = false;
    
    private float currentSpeed;


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

    private void UpdateVelocity()
    {
        playerRef.rigidbody.velocity = playerRef.transform.forward * currentSpeed;
    }

    private void IncreaseCurrentSpeed()
    {

        currentSpeed += playerRef.playerOneStats.GetAccelerationSpeed() * Time.deltaTime;
        if (currentSpeed > playerRef.playerOneStats.GetMaxSpeed() && !boosting)
        {
            currentSpeed = playerRef.playerOneStats.GetMaxSpeed();
        }
        if (boosting && currentSpeed > playerRef.playerOneStats.GetMaxBoostSpeed())
        {
            currentSpeed = playerRef.playerOneStats.GetMaxBoostSpeed();
        }

    }
    private void DecreaseCurrentSpeed()
    {
        currentSpeed -= playerRef.playerOneStats.GetDeccelerationSpeed() * Time.deltaTime;
        if (currentSpeed < 0.0f)
        {
            currentSpeed = 0.0f;
        }
    }


    private void TurnRight()
    {
        playerRef.transform.eulerAngles += new Vector3(0, playerRef.playerOneStats.GetTurnSpeed(), 0) * Time.deltaTime;
    }
    private void TurnLeft()
    {
        playerRef.transform.eulerAngles -= new Vector3(0, playerRef.playerOneStats.GetTurnSpeed(), 0) * Time.deltaTime;
    }


    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / playerRef.playerOneStats.GetMaxSpeed(); }

}
