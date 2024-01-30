using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;

public class LevelSelectMenu : Entity {

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        gameInstanceRef = game;
        initialized = true;
    }

    public override void Tick() {
        if (!initialized) {
            Error("");
            return;
        }







    }
    public void SetupMenuStartingState() {



    }











    public void DebugLevelButton() {
        gameInstanceRef.StartGame("DebugLevel");
    }
}
