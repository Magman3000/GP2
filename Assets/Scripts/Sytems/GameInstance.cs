using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;
using System.Xml.Serialization;
using System;
using UnityEngine.UIElements;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;

public class GameInstance : MonoBehaviour {

    public enum ApplicationStatus {
        ERROR = 0,
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

    [SerializeField] public static bool showSystemMessages = true;


    //Addressables Labels
    private const string levelsBundleLabel          = "LevelsBundle";
    private const string soundSystemResourceLabel   = "SoundSystem";
    private const string playerResourceLabel        = "Player";
    private const string mainCameraResourceLabel    = "MainCamera";

    private const string mainMenuResourceLabel      = "MainMenu";
    private const string optionsMenuResourceLabel   = "OptionsMenu";
    private const string creditsMenuResourceLabel   = "CreditsMenu";



    public ApplicationStatus currentApplicationStatus = ApplicationStatus.ERROR;
    public GameState currentGameState = GameState.ERROR;



    public bool initialized = false;
    public bool initializationInProgress = false;
    public bool assetsLoadingInProgress = false;

    private AsyncOperationHandle<IList<GameObject>> loadedAssetsHandle;


    //Entities
    private GameObject soundSystem;
    private GameObject player;
    private GameObject mainCamera;
    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject creditsMenu;

    //Scripts
    private SoundSystem soundSystemScript;
    private Player playerScript;
    private MainCamera mainCameraScript;



    public void Initialize() {
        if (initialized) {
            Warning("Attempted to initialize game while its already initialized!");
            return;
        }
        if (initializationInProgress) {
            Warning("Attempted to initialize game while initialization is in progress!");
            return;
        }


        //Loading bundles could be completely skipped
        //Look into other loading strategies and read sebastians notes
        initializationInProgress = true;
        LoadAssets();
    }


    //ResourceManagment
    private void LoadAssets() {
        if (showSystemMessages)
            Log("Started Loading Assets!");

        currentApplicationStatus = ApplicationStatus.LOADING_ASSETS;
        assetsLoadingInProgress = true;

        loadedAssetsHandle = Addressables.LoadAssetsAsync<GameObject>("Essential", AssetLoadedCallback);
        loadedAssetsHandle.Completed += FinishedLoadingAssetsCallback;
    }



    public static void AbortApplication(object message = null) {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        if (message != null)
            Error(message);
#else
    Application.Quit();

#endif
    }
    private void OnDestroy() {
        UnloadResources();
    }
    private void UnloadResources() {
        //Steps:
        //-Call CleanUp on all entities
        //-Destroy gameobjects
        //-Release resources

        playerScript.CleanUp("Player cleaned up successfully!");
        mainCameraScript.CleanUp("MainCamera cleaned up successfully!");
        soundSystemScript.CleanUp("SoundSystem cleaned up successfully!");

        //Needed to guarantee destruction of all entities before attempting to release resources.
        ValidateAndDestroy(player);
        ValidateAndDestroy(mainCamera);
        ValidateAndDestroy(soundSystem);
        if (showSystemMessages)
            Log("Destroyed all entities successfully!");

        if (loadedAssetsHandle.IsValid())
            Addressables.Release(loadedAssetsHandle);

        if (showSystemMessages)
            Log("Released all resources successfully!");
    }
    private void ValidateAndDestroy(GameObject target) {
        if (target)
            Destroy(target);
    }




    //Update/Tick
    void Update() {
        switch (currentApplicationStatus) {

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


    private void UpdateApplicationLoadingAssetsState() {
        if (initialized) {
            Warning("Attempted to update application initialization state while game was already initialized!");
            currentApplicationStatus = ApplicationStatus.RUNNING;
            return;
        }

        if (assetsLoadingInProgress) {
            if (showSystemMessages)
                Log("Loading Assets In Progress...");
            return;
        }

        initialized = true;
        initializationInProgress = false;
        currentApplicationStatus = ApplicationStatus.RUNNING;
        SetGameState(GameState.MAIN_MENU);
        if (showSystemMessages)
            Log("Game successfully initialized!");
    }
    private void UpdateApplicationRunningState() {
        if (currentGameState == GameState.ERROR) {
            Warning("Unable to update game\nCurrent game state is set to ERROR!");
            return;
        }

        UpdateStatelessSystems();
        //Here you can add more states if needed!
        switch (currentGameState) {
            case GameState.MAIN_MENU: 
                UpdateMainMenuState();
                break;
            case GameState.OPTIONS_MENU: 
                UpdateOptionsMenuState();
                break;
            case GameState.CREDITS_MENU:
                UpdateCreditsMenuState();
                break;
            case GameState.PLAYING:
                UpdatePlayingState();
                break;
        }
    }




    private void FixedUpdate() {
        if (currentApplicationStatus != ApplicationStatus.RUNNING)
            return;

        if (currentGameState == GameState.ERROR) {
            Warning("Unable to call fixed-update \nCurrent game state is set to ERROR!");
            return;
        }


        //Here you can add more states if needed!
        switch (currentGameState) {
            case GameState.PLAYING:
                UpdateFixedPlayingState();
                break;
        }
    }




    public void SetGameState(GameState state) {

        switch (state) {
            case GameState.MAIN_MENU:
                SetupMainMenuState();
                break;
            case GameState.OPTIONS_MENU:
                SetupOptionsMenuState();
                break;
            case GameState.CREDITS_MENU:
                SetupCreditsMenuState();
                break;
            case GameState.PLAYING:
                SetupStartState();
                break;
            case GameState.PAUSED:
                Warning("Use PauseGame/UnpauseGame instead of calling SetGameState(GameState.PAUSED)");
                break;
        }
    }
    public void Transition(GameState state) {

    }
    public void PauseGame() {

    }
    public void UnpauseGame() {

    }

    //State Setup
    private void SetupMainMenuState() {
        currentGameState = GameState.MAIN_MENU;
        SetCursorState(true);
        HideAllMenus();
        mainMenu.SetActive(true);

    }
    private void SetupOptionsMenuState() {
        currentGameState = GameState.OPTIONS_MENU;
        SetCursorState(true);
        HideAllMenus();
        optionsMenu.SetActive(true);

    }
    private void SetupCreditsMenuState() {
        currentGameState = GameState.CREDITS_MENU;
        SetCursorState(true);
        HideAllMenus();
        creditsMenu.SetActive(true);

    }
    private void SetupStartState() {
        currentGameState = GameState.PLAYING;
        SetCursorState(false);
        HideAllMenus();


    }


    //State Update
    private void UpdateStatelessSystems() {
        soundSystemScript.Tick();
    }
    private void UpdateMainMenuState() {

    }
    private void UpdateOptionsMenuState() {

    }
    private void UpdateCreditsMenuState() {

    }
    private void UpdatePlayingState() {
        playerScript.Tick();
    }
    private void UpdateFixedPlayingState() {
        playerScript.FixedTick();
    }


    private void HideAllMenus() {
        //Add all GUI here!
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);

    }
    private void SetCursorState(bool state) {
        UnityEngine.Cursor.visible = state;
        if (state)
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        else
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }








    private void AssetLoadedCallback(GameObject asset) {
        if (showSystemMessages)
            Log(asset.name + " has been loaded successfully!");

        //Notes:
        //Any asset that get loaded will be used to construct a gameobject while the rest of the assets are still being loaded
        //Taking advantage of multithreading


        //TODO: Do something about this!

        if (asset.CompareTag("Player")) {
            Log("Started creating " + asset.name + " entity");
            player = Instantiate(asset);
            playerScript = player.GetComponent<Player>();
            playerScript.Initialize(this);
            Validate(playerScript, "Player component is missing on entity!", ValidationLevel.ERROR, true);
            
        }
        else if (asset.CompareTag("MainCamera")) {
            Log("Started creating " + asset.name + " entity");
            mainCamera = Instantiate(asset);
            mainCameraScript = mainCamera.GetComponent<MainCamera>();
            mainCameraScript.Initialize(this);
            Validate(mainCameraScript, "MainCamera component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("SoundSystem")) {
            Log("Started creating " + asset.name + " entity");
            soundSystem = Instantiate(asset);
            soundSystemScript = soundSystem.GetComponent<SoundSystem>();
            soundSystemScript.Initialize(this);
            Validate(soundSystemScript, "SoundSystem component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("MainMenu")) {
            Log("Started creating " + asset.name + " entity");
            mainMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("OptionsMenu")) {
            Log("Started creating " + asset.name + " entity");
            optionsMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("CreditsMenu")) {
            Log("Started creating " + asset.name + " entity");
            creditsMenu = Instantiate(asset);
        }
        else
            Warning("Loaded an asset that was not recognized!");
    }

    private void FinishedLoadingAssetsCallback(AsyncOperationHandle<IList<GameObject>> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (showSystemMessages)
                Log("Finished loading assets successfully!");
            assetsLoadingInProgress = false;
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            AbortApplication("Failed to load assets!");
        }
    }
}
