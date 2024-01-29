using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchableProps : MonoBehaviour
{
    [Range(1.0f, 100.0f)][SerializeField] float launchMultiplier;
    Rigidbody rigidBody;
    private void Initialize()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(CompareTag("Player"))
        {
            Vector3 direction = (transform.position - other.transform.position) * launchMultiplier;
            rigidBody.AddForce(direction);
        }
    }
}
