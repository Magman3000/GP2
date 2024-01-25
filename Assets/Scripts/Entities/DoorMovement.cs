using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    public enum ObstacleState
    {
        error, active, inactive
    }
    [SerializeField] Vector3 secondPosition;
    [SerializeField] Vector3 secondRotation;
    [SerializeField] float interpolationRatio;
    [SerializeField] float interpolationLimit;
    public Boolean moving = false;
    Vector3 initialPosition;
    Vector3 initialRotation;
    ObstacleState doorState = new ObstacleState();
    // Start is called before the first frame update
    void Initialize()
    {
        doorState = ObstacleState.inactive;
        initialRotation = gameObject.transform.rotation.eulerAngles;
        initialPosition = gameObject.transform.position;

    }
    private void Update()
    {
        if (doorState == ObstacleState.active && moving)
        {
            Vector3 postionCalculation = Vector3.Lerp(transform.position, secondPosition, interpolationRatio * Time.deltaTime);
            transform.position = postionCalculation;
            Vector3 rotationCalculation = Vector3.Lerp(transform.eulerAngles, secondRotation, interpolationRatio * Time.deltaTime);
            transform.eulerAngles = rotationCalculation;
            float positionDistance = Vector3.Distance(postionCalculation, secondPosition);
            float rotationDistance = Vector3.Distance(rotationCalculation, secondRotation);
            if (positionDistance < interpolationLimit && rotationDistance < interpolationLimit)
            {
                transform.position = secondPosition;
                transform.eulerAngles = secondRotation;
                moving = false;
            }
        }
        else if (doorState == ObstacleState.inactive && moving)
        {
            Vector3 postionCalculation = Vector3.Lerp(transform.position, initialPosition, interpolationRatio * Time.deltaTime);
            transform.position = postionCalculation;
            Vector3 rotationCalculation = Vector3.Lerp(transform.eulerAngles, initialRotation, interpolationRatio * Time.deltaTime);
            transform.eulerAngles = rotationCalculation;
            float positionDistance = Vector3.Distance(postionCalculation, initialPosition);
            float rotationDistance = Vector3.Distance(rotationCalculation, initialRotation);
            if (positionDistance < interpolationLimit && rotationDistance < interpolationLimit)
            {
                transform.position = initialPosition;
                transform.eulerAngles = initialRotation;
                moving = false;
            }
        }
    }

    public void StateChange(ObstacleState state)
    {
        doorState = state;
        if (doorState == ObstacleState.active)
        {
            moving = true;
        }
        else if (doorState == ObstacleState.inactive)
        {
            moving = true;
        }
        else if (doorState == ObstacleState.error)
        {
            Debug.Log("Error");
        }
    }
}
