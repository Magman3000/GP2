using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaredevilStats", menuName = "Player/DaredevilStats", order = 0)]
public class DaredevilStats : ScriptableObject {

    [Header("Speed Settings")]
    [Tooltip("Speed of acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float accelerationSpeed = 1.0f;
    [Tooltip("Speed of deAcceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float deccelerationSpeed = 1.0f;
    [Tooltip("Maximum amount of speed attainable")]
    [Range(0.1f, 500.0f)][SerializeField] private float maxSpeed = 1.0f;

    [Tooltip("How much you turn left and right each turning action")]
    [Range(0.1f, 500.0f)][SerializeField] private float turnSpeed = 1.0f;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float boostAccelerationMultiplier = 1.0f;
    [Tooltip("The amount of boost charges")]
    [Range(0.1f, 500.0f)][SerializeField] private int boostCharges = 1;
    [Tooltip("The duration of the boost")]
    [Range(0.1f, 500.0f)][SerializeField] private float boostDuration = 1.0f;
    [Range(0.1f, 500.0f)][SerializeField] private float maxBoostSpeed = 1.0f;
    

    public float GetAccelerationSpeed() { return accelerationSpeed; }
    public float GetDeccelerationSpeed() { return deccelerationSpeed; }
    public float GetMaxSpeed() { return maxSpeed; }
    public float GetTurnSpeed() { return turnSpeed; }
    public float GetBoostAccelerationMultiplier() { return boostAccelerationMultiplier; }
    public int GetBoostCharges() { return boostCharges; }
    public float GetBoostDuration() { return boostDuration; }
    public float GetMaxBoostSpeed() { return maxBoostSpeed; }
}
