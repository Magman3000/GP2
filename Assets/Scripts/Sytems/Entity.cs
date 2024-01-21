using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    protected bool initialized = false;

    public abstract void Initialize();
    public abstract void Tick();
    public virtual void FixedTick() {
        Debug.LogWarning("Fixed tick called on an entity that does not provide implementation for FixedTick!");
    }
    public virtual void CleanUp() {
        Debug.LogWarning("clean up called on an entity that does not provide implementation for FixedTick!");
    }
}
