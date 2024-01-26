using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampSpeedBoost : MonoBehaviour
{
    [SerializeField] float boostMultiplier = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
            return;


    }
}
