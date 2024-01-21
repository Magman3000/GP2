using MyUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    protected bool initialized = false;

    /// <summary>
    /// Should be called at entity instantiation.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called instead of Update by MonoBehaviour.
    /// </summary>
    public abstract void Tick();

    /// <summary>
    /// Called instead of FixedUpdate by MonoBehaviour.
    /// </summary>
    public virtual void FixedTick() {
        Debug.LogWarning("Fixed tick called on an entity that does not provide implementation for FixedTick!");
    }

    /// <summary>
    /// Called to clean up resources allocated by this entity.
    /// </summary>
    public virtual void CleanUp() {
        Utility.Log(name + " cleaned up successfully!");
    }
}
