using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinStateTrigger : Entity
{
    //[SerializeField] GameInstance.GameState gameState = GameInstance.GameState.WIN_MENU;
    public GameObject winMenu;
    
    public override void Initialize(GameInstance game)
    {
        if (initialized)
        {
            return;
        }
        winMenu = GameObject.Find("WinMenu(Clone)");
        winMenu.SetActive(false);
        gameInstanceRef = game;
        initialized = true;
    }
    

    public void OnTriggerEnter(Collider other)
    {
       
        if(other.CompareTag("Player"))
        {
            Debug.Log("Trigger Win");
            
            winMenu.SetActive(true);
            //gameInstanceRef.SetGameState(gameState);
        }
    }
}
