using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;

public class Level : Entity {


    public Obstacle.ObstacleActivationState currentObstacleState = Obstacle.ObstacleActivationState.NONE;





    public List<Obstacle> registeredObstacles = new List<Obstacle>();

    private GameObject obstaclesParent = null;
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
        //Spawn Point
        Transform spawnPointTransform = transform.Find("SpawnPoint");
        if (Validate(spawnPointTransform, "No spawn point was found!\nSpawn point set to 0.0.0!", ValidationLevel.WARNING)) {
            spawnPoint = spawnPointTransform.position;
            spawnPointTransform.gameObject.SetActive(false);
        }

        //Obstacles
        Transform obstacleParentTransform = transform.Find("Obstacles");
        if (Validate(obstacleParentTransform, "No obstacles parent was found!\nObstacles activation state will be unused!", ValidationLevel.WARNING)) {
            obstaclesParent = obstacleParentTransform.gameObject;
            InitializeObstacles();
        }
        //Initial state? Either decided by entity or applied here! InitialObstaclesState [SerializeField]
        CheckTree(transform);
    }

    private void CheckTree(Transform parent) {
        foreach (Transform child in parent) {
            Log(child.name + " was checked!");
            Obstacle component = child.GetComponent<Obstacle>();
            if (component) {
                registeredObstacles.Add(component);
                Log(component + " was added to the list!");
            }
            
            if (child.childCount > 0)
                CheckTree(child);
        }
    }


    public void SetCurrentObstacleState(Obstacle.ObstacleActivationState state) {
        if (currentObstacleState == state)
            return;

        Log("State changed to " + state.ToString());
        currentObstacleState = state;
        UpdateObstacles();
    }
    private void InitializeObstacles() {
        if (!obstaclesParent) //Ditch this to instead registering all of them since now its just obstacle class
            return;

        foreach (var child in obstaclesParent.GetComponentsInChildren<Obstacle>()) //SET THE ACTIVATION THING TOO!
            child.Initialize(gameInstanceRef);
    }
    private void UpdateObstacles() {
        if (!obstaclesParent)
            return;

        foreach(var child in obstaclesParent.GetComponentsInChildren<Obstacle>()) {
            if (child.GetObstacleActivationState() == currentObstacleState)
                child.SetActivationState(true);
            else
                child.SetActivationState(false);
        }
    }



    public Vector3 GetSpawnPoint() { return spawnPoint; }
}
