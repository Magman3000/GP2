using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CoordinatorStats", menuName = "Player/CoordinatorStats", order = 0)]
public class CoordinatorStats : ScriptableObject
{
    [Range(0.1f, 500.0f)][SerializeField] private float boostDuration = 1.0f;
    [Range(0.1f, 500.0f)][SerializeField] private float boostCooldown = 1.0f;
    [Tooltip("The amount of boost charges")]
    [Range(0.1f, 500.0f)][SerializeField] private int boostCharges = 1;



    public float GetBoostDuration() { return boostDuration; }
    public float GetboostCooldown() { return boostCooldown; }
    public int GetBoostCharges() { return boostCharges; }
}
