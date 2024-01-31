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
    [Range(0.1f, 500.0f)][SerializeField] private float maxReverseSpeed = 1.0f;
    [Range(0.1f, 1.0f)][SerializeField] private float breakSpeed = 0.1f;

    [Tooltip("How much you turn left and right each turning action")]
    [Range(0.1f, 500.0f)][SerializeField] private float turnSpeed = 1.0f;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] private float boostAccelerationMultiplier = 1.0f;
    [Range(0.1f, 500.0f)][SerializeField] private float maxBoostSpeed = 1.0f;
    

    public float GetAccelerationSpeed() { return accelerationSpeed; }
    public float GetDeccelerationSpeed() { return deccelerationSpeed; }
    public float GetMaxSpeed() { return maxSpeed; }
    public float GetMaxReverseSpeed() { return maxReverseSpeed; }

    public float GetBreakSpeed() { return breakSpeed; }
    public float GetTurnSpeed() { return turnSpeed; }
    public float GetBoostAccelerationMultiplier() { return boostAccelerationMultiplier; }
    public float GetMaxBoostSpeed() { return maxBoostSpeed; }
}
