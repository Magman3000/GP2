using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaredevilStats", menuName = "Player/DaredevilStats", order = 0)]
public class DaredevilStats : ScriptableObject {

    [Header("Speed Settings")]

    [Tooltip("Speed of acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] public float accelerationRate = 200.0f;

    [Tooltip("Speed of deceleration")]
    [Range(0.1f, 500.0f)][SerializeField] public float decelerationRate = 150.0f;

    [Tooltip("Maximum amount of speed attainable")]
    [Range(0.1f, 1000.0f)][SerializeField] public float maxSpeed = 300.0f;
    [Range(0.1f, 5000.0f)][SerializeField] public float maxReverseSpeed = 100.0f;
    [Range(0.1f, 1000.0f)][SerializeField] public float reverseRate = 50.0f;
    [Range(0.1f, 1000.0f)][SerializeField] public float brakeRate = 200.0f;


    [Tooltip("How much you turn left and right each turning action")]
    [Range(0.1f, 5000.0f)][SerializeField] public float turnRate = 500.0f;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase acceleration")]
    [Range(0.1f, 500.0f)][SerializeField] public float maxBoostSpeed = 300.0f;
    [Range(0.1f, 500.0f)][SerializeField] public float boostAccelerationRate = 300.0f;
    [Range(0.1f, 500.0f)][SerializeField] public float boostDecelerationRate = 400.0f;



}
