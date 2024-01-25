using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkedEntity {



    public enum PlayerIdentity {
        NONE = 0,
        PLAYER_1, //Daredevil
        PLAYER_2 //Coordinator
    }

    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;

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

    public void AssignPlayerIdentity(PlayerIdentity playerIdentity) {
        assignedPlayerIdentity = playerIdentity;
    }



}
