using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : Entity {


    public override void Initialize() {
        if (initialized)
            return;




        initialized = true;
    }
    public override void Tick() {
        throw new System.NotImplementedException();
    }
    public override void CleanUp() {


    }

}
