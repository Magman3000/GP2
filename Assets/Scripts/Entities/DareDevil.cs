using Initialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static MyUtility.Utility;

public class Daredevil {

    bool initialized = false;

    GameInstance gameInstanceRef;
    Player playerRef;
    DaredevilStats stats;
    
    public bool boosting = false;
    
    private float currentSpeed;


    public void Initialize(GameInstance game, Player player) {
        if (initialized)
            return;

        playerRef = player;
        stats = playerRef.GetDaredevilStats();

        gameInstanceRef = game;
        initialized = true;
    }
    public void Tick() {
        if (!initialized) {
            Warning("Player attempted to tick daredevil while it was not initialized");
            return;
        }


    }
    public void FixedTick() {
        if (!initialized) {
            Warning("Player attempted to fixed tick daredevil while it was not initialized");
            return;
        }




    }

    private void UpdateVelocity() {
        playerRef.rigidbodyComp.velocity = playerRef.transform.forward * currentSpeed;
    }

    private void IncreaseCurrentSpeed() {

        currentSpeed += stats.GetAccelerationSpeed() * Time.deltaTime;
        if (currentSpeed > stats.GetMaxSpeed() && !boosting)
            currentSpeed = stats.GetMaxSpeed();

        if (boosting && currentSpeed > stats.GetMaxBoostSpeed())
            currentSpeed = stats.GetMaxBoostSpeed();
    }
    private void DecreaseCurrentSpeed() {
        currentSpeed -= stats.GetDeccelerationSpeed() * Time.deltaTime;
        if (currentSpeed < 0.0f)
            currentSpeed = 0.0f;
    }


    private void TurnRight() {
        playerRef.transform.eulerAngles += new Vector3(0, stats.GetTurnSpeed(), 0) * Time.deltaTime;
    }
    private void TurnLeft() {
        playerRef.transform.eulerAngles -= new Vector3(0, stats.GetTurnSpeed(), 0) * Time.deltaTime;
    }


    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / stats.GetMaxSpeed(); }
}
