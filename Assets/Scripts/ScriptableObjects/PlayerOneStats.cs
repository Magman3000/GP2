using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerOneStats", menuName = "Player/PlayerOneStats", order = 0)]
public class PlayerOneStats : ScriptableObject
{
    [Header("Speed Settings")]
    [Tooltip("Speed of acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float accelerationSpeed = 1;
    [Tooltip("Speed of deAcceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float deccelerationSpeed = 1;
    [Tooltip("Maximum amount of speed attainable")]
    [Range(0.1f, 500.0f)][SerializeField] private float maxSpeed = 1;

    [Tooltip("How much you turn left and right each turning action")]
    [Range(0.1f, 500.0f)][SerializeField] private float turnSpeed = 1;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float boostAccelerationMultiplier = 1;
    [Tooltip("The amount of boost charges")]
    [Range(0.1f, 500.0f)][SerializeField] private int boostCharges = 1;
    [Tooltip("The duration of the boost")]
    [Range(0.1f, 500.0f)][SerializeField] private float boostDuration = 1;
    [Range(0.1f, 500.0f)][SerializeField] private float maxBoostSpeed = 1;
    private bool boosting = false;

    public float GetAccelerationSpeed()
    {
        return accelerationSpeed;
    }

    public float GetDeccelerationSpeed()
    {
        return deccelerationSpeed;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetTurnSpeed()
    {
        return turnSpeed;
    }

    public float GetBoostAccelerationMultiplier()
    {
        return boostAccelerationMultiplier;
    }

    public int GetBoostCharges()
    {
        return boostCharges;
    }

    public float GetBoostDuration()
    {
        return boostDuration;
    }

    public float GetMaxBoostSpeed()
    {
        return maxBoostSpeed;
    }


}
