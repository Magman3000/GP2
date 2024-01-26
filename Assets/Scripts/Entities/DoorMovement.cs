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
            Vector3 postionCalculation = Vector3.Lerp(transform.position, openedPosition, interpolationRatio * Time.deltaTime);
            transform.position = postionCalculation;
            Vector3 rotationCalculation = Vector3.Lerp(transform.eulerAngles, openedRotation, interpolationRatio * Time.deltaTime);
            transform.eulerAngles = rotationCalculation;
            float positionDistance = Vector3.Distance(postionCalculation, openedPosition);
            float rotationDistance = Vector3.Distance(rotationCalculation, openedRotation);
            if (positionDistance < interpolationLimit && rotationDistance < interpolationLimit)
            {
                transform.position = openedPosition;
                transform.eulerAngles = openedRotation;
                moving = false;
            }
        }
        else if (doorState == ObstacleState.INACTIVE && moving)
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
        if (state == ObstacleState.ERROR) {
            Debug.Log("Door Error");
            return;
        }

        doorState = state;
        moving = true;
    }
}
