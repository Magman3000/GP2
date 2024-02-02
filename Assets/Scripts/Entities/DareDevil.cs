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


    private bool movingForward = false;
    private bool isMoving = false;
    private bool isBoosting = false;



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


        UpdateSpeed();
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


    private void UpdateSpeed() {
        if (isMoving)
            Accelerate();
        else if (!isMoving)
            Decelerate();
    }
    private void UpdateVelocity() {
        playerRigidbody.velocity = playerRef.transform.forward * (currentSpeed * Time.deltaTime);
    }
    private void UpdateRotation() {
        //float value = Input.gyro.rotationRate.z * stats.GetTurnRate() * Time.deltaTime;
        float value2 = Input.gyro.attitude.z * stats.turnRate * Time.deltaTime;
        Log("Z " + Input.gyro.attitude.z);
        Log("X " + Input.gyro.attitude.x);
        Log("Y " + Input.gyro.attitude.y);

        playerRef.transform.localEulerAngles = new Vector3(playerRef.transform.localEulerAngles.x, value2, playerRef.transform.localEulerAngles.z);
        //playerRef.transform.localEulerAngles += new Vector3(0.0f, value2, 0.0f); //Gyro is offset and off on phone
    }




    //Brake
    private void Brake()
    {
        if (currentSpeed == 0.0f)
        {
            return;
        }

        //setting a float to help calculate the needed speed to remove no matter which direction you are going
        if (currentSpeed > 0) {
            movingForward = true;
        } else {
            movingForward = false;
        }

        //removing the needed speed depending on if you are moving forwards or not
        if (movingForward == true){
            currentSpeed -= stats.decelerationRate * Time.deltaTime;
            if (currentSpeed <= 0.0f){
                currentSpeed = 0.0f;
            }
        } else {
            currentSpeed += stats.accelerationRate * Time.deltaTime;
            if (currentSpeed >= 0.0f) {
                currentSpeed = 0.0f;
            }
        }
    }


    private void Reversing()
    {
        currentSpeed -= stats.decelerationRate * Time.deltaTime;
        if (currentSpeed >= stats.maxReverseSpeed)
        {
            currentSpeed = stats.maxReverseSpeed;
        }
    }




    public void Accelerate() {

        if (isBoosting) {
            currentSpeed += stats.boostAccelerationRate * Time.deltaTime;
            if (currentSpeed >= stats.maxBoostSpeed) {
                currentSpeed = stats.maxBoostSpeed;

            }
        }
        else if (!isBoosting) {


            if (currentSpeed > stats.maxSpeed) { //Boost recovery
                currentSpeed -= stats.boostDecelerationRate * Time.deltaTime;
                if (currentSpeed <= stats.maxSpeed) {
                    currentSpeed = stats.maxSpeed;

                }
            }
            else if (currentSpeed < stats.maxSpeed) {
                currentSpeed += stats.accelerationRate * Time.deltaTime;
                if (currentSpeed >= stats.maxSpeed) {
                    currentSpeed = stats.maxSpeed;

                }
            }
        }
    }
    public void Decelerate() {
        if (currentSpeed <= 0.0f)
            return;

        //TODO: Add unique behavior for if was boosting
        currentSpeed -= stats.decelerationRate * Time.deltaTime;
        if (currentSpeed <= 0.0f) {
            currentSpeed = 0.0f;

        }
    }


    public void ApplyRampBoost(float value) {
        currentSpeed *= value; //Temp
    }

    
    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / stats.maxSpeed; }
}
