using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandstorm : MonoBehaviour
{
    [SerializeField] private float diruptionRatio = 1.5f;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) 
            return;

        other.transform.forward += (other.transform.position - transform.position).normalized * diruptionRatio * Time.deltaTime;
    }
}