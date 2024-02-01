using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;

public class Level : Entity {


    Vector3 spawnPoint = Vector3.zero;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        SetupReferences();
        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {
        if (!initialized)
            return;




    }
    public void SetupReferences() {
        Transform spawnPointTransform = transform.Find("SpawnPoint");
        if (Validate(spawnPointTransform, "No spawn point was found!\nSpawn point set to 0.0.0!", ValidationLevel.WARNING)) {
            spawnPoint = spawnPointTransform.position;
            spawnPointTransform.gameObject.SetActive(false);
        }
    }


    public Vector3 GetSpawnPoint() { return spawnPoint; }


}
