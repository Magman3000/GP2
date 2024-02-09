using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinStateTrigger : MonoBehaviour
{
    //[SerializeField] GameInstance.GameState gameState = GameInstance.GameState.WIN_MENU;
    public GameObject winMenu;
    public float timer = 0;
    /*
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
    */

    private void Start()
    {
        winMenu = GameObject.Find("WinMenu(Clone)");
        winMenu.SetActive(false);
    }
    private void Update()
    {
        timer += Time.deltaTime;
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
