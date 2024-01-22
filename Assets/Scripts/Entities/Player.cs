using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {

    }
    public override void FixedTick() {

    }




}
