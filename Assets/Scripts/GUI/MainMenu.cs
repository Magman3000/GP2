using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Entity {

    //Could be made non abstract?
    public override void Tick() {
        
    }
    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        gameInstanceRef = game;
        initialized = true;
    }



    public void TestButton() {
        gameInstanceRef.SetGameState(GameInstance.GameState.OPTIONS_MENU);
    }
}
