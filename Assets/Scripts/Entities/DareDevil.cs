using Initialization;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static MyUtility.Utility;

public class Daredevil {

    bool initialized = false;
    
    private enum turnDirections
    {
        NONE = 0,
        LEFT,
        RIGHT
    }

    GameInstance gameInstanceRef;
    Player playerRef;
    DaredevilStats stats;
    Rigidbody playerRigidbody;

    private float currentSpeed;
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


    }
    public void FixedTick() {
        if (!initialized) {
            Warning("Player attempted to fixed tick daredevil while it was not initialized");
            return;
        }


    }

    private void UpdateVelocity()
    {
        playerRigidbody.velocity = playerRef.transform.forward * currentSpeed;
    }

    private void UpdateBoostBool()
    {
        speedBoostBool = playerRef.GetBoostCheck();
    }

    private void IncreaseCurrentSpeed() {

        currentSpeed += stats.GetAccelerationSpeed() * Time.deltaTime;
        if (currentSpeed > stats.GetMaxSpeed() && !speedBoostBool)
            currentSpeed = stats.GetMaxSpeed();

        if (speedBoostBool && currentSpeed > stats.GetMaxBoostSpeed())
            currentSpeed = stats.GetMaxBoostSpeed();
    }
    private void DecreaseCurrentSpeed() {
        currentSpeed -= stats.GetDeccelerationSpeed() * Time.deltaTime;
        if (currentSpeed < 0.0f)
            currentSpeed = 0.0f;
    }


    private void Turning(Enum turn) {
        if (turn.Equals(0))
        {
            Warning("No turning direction is inputted");
            return;
        }


        if (turn.Equals(1))
        {
            playerRef.transform.eulerAngles += new Vector3(0, stats.GetTurnSpeed(), 0) * Time.deltaTime;
        } else
        {
            playerRef.transform.eulerAngles -= new Vector3(0, stats.GetTurnSpeed(), 0) * Time.deltaTime;
        }


    }


    public void SetBoostBool(bool boosting) { speedBoostBool = boosting; }
    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / stats.GetMaxSpeed(); }
}
