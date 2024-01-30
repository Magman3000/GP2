using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static MyUtility.Utility;

public class LevelSelectMenu : Entity
{
    [SerializeField] private LevelsBundle levelsBundle;
    
    public bool player1Ready = false;
    public bool player2Ready = false;

    public Player.PlayerIdentity player1Identity = Player.PlayerIdentity.NONE;
    public Player.PlayerIdentity player2Identity = Player.PlayerIdentity.NONE;

    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;


        gameInstanceRef = game;
        initialized = true;
        
        SetupMenuStartingState();
        foreach (var level in levelsBundle.Entries)
        {
            //Instantiate the level prefab
            //Set the data e.g.: level name, level description, level image
        }
    }

    public override void Tick()
    {
        if (!initialized)
        {
            Error("");
            return;
        }
        
    }

    private bool IsInteractingWithUI()
    {
        return EventSystem.current.IsPointerOverGameObject() && Pointer.current.press.isPressed;
    }

    public void SetupMenuStartingState()
    {
        player1Ready = false;
        player2Ready = false;
        player1Identity = Player.PlayerIdentity.NONE;
        player2Identity = Player.PlayerIdentity.NONE;
    }


    public void DebugLevelButton()
    {
        gameInstanceRef.StartGame("DebugLevel");
    }
}