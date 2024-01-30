using Initialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private bool initialized = false;

    GameInstance gameInstanceRef;
    Level levelRef;

    public void Initialize(GameInstance game, Level level)
    {
        if (initialized)
            return;



        gameInstanceRef = game;
        levelRef = level;
        initialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //notify x in levelRef
        }

    }
}
