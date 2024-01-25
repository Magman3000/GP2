using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : Entity {

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        gameInstanceRef = game;
        initialized = true;
    }


    public void DebugLevelButton() {
        gameInstanceRef.StartGame("DebugLevel");
    }

}
