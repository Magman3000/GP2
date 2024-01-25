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
    Vector3 position;
    Quaternion rotation;
    ObstacleState doorState = new ObstacleState();
    // Start is called before the first frame update
    void Initialize()
    {
        doorState = ObstacleState.inactive;
        position = gameObject.transform.position;

    }

    public void StateChange(ObstacleState state)
    {
        doorState = state; 
        if(doorState == ObstacleState.active)
        {
            transform.eulerAngles = secondRotation;
            transform.position = secondPosition;
        }
        else if(doorState == ObstacleState.inactive)
        {
            transform.rotation = rotation;
            transform.position = position;
        }
        else if(doorState == ObstacleState.error)
        {
            ErrorManager.Error(gameObject);
        }
    }
}
