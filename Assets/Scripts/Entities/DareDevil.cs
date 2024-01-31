using Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;

public class Daredevil {

    bool initialized = false;
    public enum TurnDirection {
        NONE = 0,
        LEFT,
        RIGHT
    }



    GameInstance gameInstanceRef;
    Player playerRef;
    DaredevilStats stats;
    Rigidbody playerRigidbody;

    private float currentSpeed = 0.0f;
    private bool gasOn = false;



    private bool speedBoostBool = false;

    public void Initialize(GameInstance game, Player player) {
        if (initialized)
            return;

        playerRef = player;
        stats = playerRef.GetDaredevilStats();
        playerRigidbody = playerRef.GetRigidbody();
        gameInstanceRef = game;
        initialized = true;
    }
    public void Tick() {
        if (!initialized) {
            Warning("Player attempted to tick daredevil while it was not initialized");
            return;
        }


        if (gasOn)
            Accelerate();
        else if (!gasOn && currentSpeed > 0.0f)
            Decelerate();


        UpdateVelocity();
    }
    public void FixedTick() {
        if (!initialized) {
            Warning("Player attempted to fixed tick daredevil while it was not initialized");
            return;
        }


    }




    public void SetGasState(bool state) { gasOn = state; }


    private void UpdateVelocity() {
        playerRigidbody.velocity = playerRef.transform.forward * currentSpeed * Time.deltaTime;
    }

    private void UpdateBoostBool() {
        speedBoostBool = playerRef.GetBoostCheck();
    }

    private void Breaking() {
        if (currentSpeed >= -stats.GetMaxReverseSpeed()/10.0f && currentSpeed <= stats.GetMaxSpeed()/10.0f) {
            playerRigidbody.velocity = Vector3.zero;
            currentSpeed = 0.0f;
            return;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, 0, stats.GetBreakSpeed() * Time.deltaTime);

    }

    public void Accelerate() {

        currentSpeed += stats.GetAccelerationRate() * Time.deltaTime;
        if (currentSpeed >= stats.GetMaxSpeed()) {
            currentSpeed = stats.GetMaxSpeed();
        }


        //currentSpeed += stats.GetAccelerationSpeed() * Time.deltaTime;
        //if (currentSpeed > stats.GetMaxSpeed() && !speedBoostBool)
        //    currentSpeed = stats.GetMaxSpeed();

        //if (speedBoostBool && currentSpeed > stats.GetMaxBoostSpeed())
        //    currentSpeed = stats.GetMaxBoostSpeed();
    }

    public void Decelerate() {
        currentSpeed -= stats.GetDecelerationRate() * Time.deltaTime;
        if (currentSpeed <= 0.0f) {
            currentSpeed = 0.0f;

        }
    }


    public void Turn(TurnDirection turn) {
        if (turn == TurnDirection.NONE)
            return;

        float turnResult = stats.GetTurnRate() * Time.deltaTime;
        Vector3 turnVector = new Vector3(0.0f, turnResult, 0.0f);

        if (turn == TurnDirection.LEFT)
            playerRef.transform.eulerAngles += turnVector;
        else if (turn == TurnDirection.RIGHT)
            playerRef.transform.eulerAngles -= turnVector;
    }


    
    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / stats.GetMaxSpeed(); }
}
