using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerOneStats", menuName = "Player/PlayerOneStats", order = 0)]
public class PlayerOneStats : ScriptableObject
{
    [Header("Speed Settings")]
    [Tooltip("Speed of acceleration")]
    [SerializeField] private float accelerationSpeed;
    [Tooltip("Speed of deAcceleration")]
    [SerializeField] private float deccelerationSpeed;
    [Tooltip("Maximum amount of speed attainable")]
    [SerializeField] private float maxSpeed;

    [Tooltip("How much you turn left and right each turning action")]
    [SerializeField] private float turnSpeed;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase acceleration")]
    [SerializeField] private float boostAccelerationMultiplier;
    [Tooltip("The amount of boost charges")]
    [SerializeField] private int boostCharges;
    [Tooltip("The duration of the boost")]
    [SerializeField] private float boostDuration;
    [SerializeField] private float maxBoostSpeed;

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
