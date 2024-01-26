using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : Entity
{
    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;


        gameInstanceRef = game;
        initialized = true;
    }






}
