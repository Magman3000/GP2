using System;
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


    public bool isMoving = false;
    public bool isReversingAndBraking = false;
    public bool isBoosting = false;


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


        UpdateMovement();
        //Break
        //Reverse
        UpdateRotation();
    }
    public void FixedTick() {
        if (!initialized) {
            Warning("Player attempted to fixed tick daredevil while it was not initialized");
            return;
        }

        UpdateVelocity();
    }
    public void SetupStartState() {

    }


    public void SetBoostState(bool state) { isBoosting = state; }
    public void SetMovementState(bool state) { isMoving = state; }
    public void SetBrakeState(bool state) { isReversingAndBraking = state; }



    private void CheckTerrain() {
        //Check if object is infront by doing 
    }
    private void CheckGroundedState() {
        //Check if grounded using cast that is slightly above then goes down
    }
    private void UpdateGravity() {
        //Custom gravity
    }
    private void UpdateMovement() {
        if (isReversingAndBraking) {
            if (currentSpeed > 0.0f)
                Brake();
            else if (currentSpeed <= 0.0f)
                Reverse();
        }
        else {
            if (isMoving)
                Accelerate();
            else if (!isMoving)
                Decelerate();
        }

        //Log(currentSpeed);
    }
    private void UpdateVelocity() {
        playerRigidbody.velocity = playerRef.transform.forward * (currentSpeed * Time.deltaTime);
    }
    private void UpdateRotation() {
        //float value = Input.gyro.rotationRate.z * stats.GetTurnRate() * Time.deltaTime;
        float value2 = Input.gyro.attitude.z * stats.turnRate * Time.deltaTime;
        //Log("Z " + Input.gyro.attitude.z);
        //Log("X " + Input.gyro.attitude.x);
        //Log("Y " + Input.gyro.attitude.y);

        playerRef.transform.localEulerAngles = new Vector3(playerRef.transform.localEulerAngles.x, value2, playerRef.transform.localEulerAngles.z);
        //playerRef.transform.localEulerAngles += new Vector3(0.0f, value2, 0.0f); //Gyro is offset and off on phone
    }


    private void UpdateSpeed(float rate, float limit, Action callback, bool additive = true) {
        if (additive) {
            currentSpeed += rate * Time.deltaTime;
            if (currentSpeed >= limit) {
                currentSpeed = limit;
                if (callback != null)
                    callback.Invoke();
            }
        }
        else if (!additive) {
            currentSpeed -= rate * Time.deltaTime;
            if (currentSpeed <= limit) {
                currentSpeed = limit;
                if (callback != null)
                    callback.Invoke();
            }
        }
    }



    //Brake
    private void Brake() {
        currentSpeed -= stats.brakeRate * Time.deltaTime;
        if (currentSpeed <= 0.0f) {
            currentSpeed = 0.0f;
            Log("Brake");
        }
    }
    private void Reverse() {
        currentSpeed -= stats.reverseRate * Time.deltaTime;
        if (currentSpeed <= -stats.maxReverseSpeed) {
            currentSpeed = -stats.maxReverseSpeed;
            Log("Reverse");
        }
    }




    public void Accelerate() {

        if (isBoosting) {
            currentSpeed += stats.boostAccelerationRate * Time.deltaTime;
            if (currentSpeed >= stats.maxBoostSpeed) {
                currentSpeed = stats.maxBoostSpeed;
                Log("Boost Accelerate");
            }
        }
        else if (!isBoosting) {

            if (currentSpeed > stats.maxSpeed) { //Boost recovery
                currentSpeed -= stats.boostDecelerationRate * Time.deltaTime;
                if (currentSpeed <= stats.maxSpeed) {
                    currentSpeed = stats.maxSpeed;
                    Log("Boost Recovery");
                }
            }
            else if (currentSpeed < stats.maxSpeed) {
                currentSpeed += stats.accelerationRate * Time.deltaTime;
                if (currentSpeed >= stats.maxSpeed) {
                    currentSpeed = stats.maxSpeed;
                    Log("Accelerate");
                }
            }
        }
    }
    public void Decelerate() {
        //if (currentSpeed <= 0.0f)
            //return;

        //TODO: Add unique behavior for if was boosting
        if (currentSpeed > 0.0f) {
            currentSpeed -= stats.decelerationRate * Time.deltaTime;
            if (currentSpeed <= 0.0f) {
                currentSpeed = 0.0f;
                Log("Normal Decelrate");
            }
        }
        else { 
            if (currentSpeed < 0.0f) {
                currentSpeed += stats.decelerationRate * Time.deltaTime; //COuld be something else. Reverse deceleraton rate
                if (currentSpeed > 0.0f) {
                    currentSpeed = 0.0f;
                    Log("Reverse Decelrate");
                }
            } 
        }


    }


    public void ApplyRampBoost(float value) {
        currentSpeed *= value; //Temp
    }

    
    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / stats.maxSpeed; }
}
