using System;
using UnityEngine;

public class DoorMovement : MonoBehaviour {
    public enum ObstacleState { ERROR = 0, 
        ACTIVE, 
        INACTIVE 
    }


    [SerializeField] Vector3 openedPosition;
    [SerializeField] Vector3 openedRotation;

    [SerializeField] float interpolationRatio;
    [SerializeField] float interpolationLimit;


    public bool moving = false;

    Vector3 initialPosition;
    Vector3 initialRotation;
    ObstacleState doorState = ObstacleState.ERROR;



    void init()
    {
        //TODO: To func! - Negative rotation requires testing
        doorState = ObstacleState.INACTIVE;
        initialRotation = gameObject.transform.rotation.eulerAngles;
        initialPosition = gameObject.transform.position;

    }
    private void Update()
    {
        if (doorState == ObstacleState.ACTIVE && moving)
        {
            DoorMove(openedPosition, openedRotation);
        }
        else if (doorState == ObstacleState.INACTIVE && moving)
        {
            DoorMove(initialPosition, initialRotation);
        }
    }

    public void StateChange(ObstacleState state)
    {
        if (state == ObstacleState.ERROR) {
            Debug.Log("Door Error");
            return;
        }

        doorState = state;
        moving = true;
    }
    private void DoorMove(Vector3 destination, Vector3 toRotation)
    {
        Vector3 postionCalculation = Vector3.Lerp(transform.position, destination, interpolationRatio * Time.deltaTime);
        transform.position = postionCalculation;
        Vector3 rotationCalculation = Vector3.Lerp(transform.eulerAngles, toRotation, interpolationRatio * Time.deltaTime);
        transform.eulerAngles = rotationCalculation;
        float positionDistance = Vector3.Distance(postionCalculation, destination);
        float rotationDistance = Vector3.Distance(rotationCalculation, toRotation);
        if (positionDistance < interpolationLimit && rotationDistance < interpolationLimit)
        {
            transform.position = destination;
            transform.eulerAngles = toRotation;
            moving = false;
        }
    }
}
