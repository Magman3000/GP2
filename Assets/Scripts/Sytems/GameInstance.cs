using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;

public class GameInstance : MonoBehaviour {

    public enum ApplicationStatus {
        ERROR = 0,
        LOADING_BUNDLES,
        LOADING_ASSETS,
        RUNNING
    }
    public enum GameState {
        ERROR = 0,
        MAIN_MENU,
        OPTIONS_MENU,
        CREDITS_MENU,
        PLAYING,
        PAUSED
    }

    [SerializeField] private bool showDebugMessages = true;



    private ApplicationStatus currentApplicationStatus = ApplicationStatus.ERROR;
    private GameState currentGameState = GameState.ERROR;



    private bool initialized = false;
    private bool initializationInProgress = false;


    public void Initialize() {
        if (initialized) {
            Warning("Attempted to initialize game while its already initialized!");
            return;
        }
        if (initializationInProgress) {
            Warning("Attempted to initialize game while initialization is in progress!");
            return;
        }


        if (showDebugMessages)
            Log("Loading Assets In Progress...");

        //Loading bundles could be completely skipped
        //Look into other loading strategies and read sebastians notes
        initializationInProgress = true;
        LoadEssentialBundles();
    }


    //ResourceManagment
    private void LoadEssentialBundles() {

        currentApplicationStatus = ApplicationStatus.LOADING_BUNDLES;
    }
    private bool FinishedLoadingEssentialBundles() {




        return true;
    }

    private void OnDestroy() {
        UnloadResources();
    }
    private void UnloadResources() {
        //Destroy GO then release resource
    }
    private void ValidateAndDestroy(GameObject target) {
        if (target)
            Destroy(target);
    }




    //Update/Tick
    void Update() {
        switch (currentApplicationStatus) {
            case ApplicationStatus.LOADING_BUNDLES:
                UpdateApplicationLoadingBundlesState(); 
                break;

            case ApplicationStatus.LOADING_ASSETS:
                UpdateApplicationLoadingAssetsState();
                break;

            case ApplicationStatus.RUNNING:
                UpdateApplicationRunningState(); 
                break;

            case ApplicationStatus.ERROR:
                AbortApplication("Attempted to update application while status was ERROR");
                break;
        }
    }
    private void UpdateApplicationLoadingBundlesState() {
        if (initialized) {
            Warning("Attempted to update application initialization state while game was already initialized!");
            currentApplicationStatus = ApplicationStatus.RUNNING;
            return;
        }

        if (!FinishedLoadingEssentialBundles())
            return;

        //if asset loading in progress
        //Check status



    }
    private void UpdateApplicationLoadingAssetsState() {

    }
    private void UpdateApplicationRunningState() {

    }




    private void FixedUpdate() {
        if (currentApplicationStatus != ApplicationStatus.RUNNING)
            return;



    }



    void Start() {
        
    }






    public void SetGameState(GameState state) {

    }
    public void Transition(GameState state) {

    }

    public void PauseGame() {

    }
    public void UnpauseGame() {

    }


    private void HideAllMenus() {

    }
    private void SetCursorState(bool state) {
        UnityEngine.Cursor.visible = state;
        if (state)
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        else
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }



}
