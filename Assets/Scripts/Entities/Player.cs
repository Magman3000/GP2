using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkedEntity
{
    private Rigidbody rb;

    [Header("Speed Settings")]
    [Tooltip("Speed of acceleration")]
    [SerializeField] private float accelerationSpeed;
    [Tooltip("Speed of deAcceleration")]
    [SerializeField] private float deAccelerationSpeed;
    [Tooltip("Maximum amount of speed attainable")]
    [SerializeField] private float maxSpeed;
    private float currentSpeed;

    [Tooltip("How much you turn left and right each turning action")]
    [SerializeField] private float turnSpeed;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase of speed during the boost")]
    [SerializeField] private float boostMultiplier;
    [Tooltip("The amount of boost charges")]
    [SerializeField] private float boostCharges;
    [Tooltip("The duration of the boost")]
    [SerializeField] private float boostDuration;
    private float timer = 0.0f;
    private bool boosting = false;

    public enum PlayerIdentity
    {
        NONE = 0,
        PLAYER_1, //Daredevil
        PLAYER_2 //Coordinator
    }

    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;

    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick()
    {



    }
    public override void FixedTick()
    {

    }

    public void AssignPlayerIdentity(PlayerIdentity playerIdentity)
    {
        assignedPlayerIdentity = playerIdentity;
    }

    private void GetComponentRigidBody()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void UpdateVelocity()
    {
        rb.velocity = transform.forward * currentSpeed;
    }

    private void IncreaseCurrentSpeed()
    {
        currentSpeed += accelerationSpeed * Time.deltaTime;
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
    }

    private void DecreaseCurrentSpeed()
    {
        currentSpeed += -deAccelerationSpeed * Time.deltaTime;
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
    }

    private void TurnRight()
    {
        transform.eulerAngles += new Vector3(0, turnSpeed, 0) * Time.deltaTime;
    }

    private void TurnLeft()
    {
        transform.eulerAngles += new Vector3(0, -turnSpeed, 0) * Time.deltaTime;
    }

    private void SpeedBoost()
    {
        if (boostCharges > 0 && boosting == false)
        {
            rb.velocity = (transform.forward * currentSpeed) * boostMultiplier;
            boostCharges -= 1;
            boosting = true;
            timer = boostDuration;
        }
    }

    private void BoostTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                boosting = false;
                timer = 0.0f;
            }
        }

    }

}
