using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;
using System;

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
        CONNECTION_MENU,
        LEVEL_SELECT_MENU,
        WIN_MENU,
        LOSE_MENU,
        PLAYING,
        PAUSED
    }

    [SerializeField] private bool initializeOnStartup = true;
    [SerializeField] private bool debugging = true;


    //Addressables Labels
    private const string essentialAssetsLabel       = "Essential";
    private const string levelsBundleLabel          = "LevelsBundle";



    public ApplicationStatus currentApplicationStatus = ApplicationStatus.ERROR;
    public GameState currentGameState = GameState.ERROR;



    private bool initialized = false;
    private bool initializationInProgress = false;
    private bool assetsLoadingInProgress = false;
    private bool gamePaused = false;

    private int powerSavingFrameTarget = 20;
    private int gameplayFrameTarget = 60;

    private AsyncOperationHandle<IList<GameObject>> loadedAssetsHandle;
    private AsyncOperationHandle<ScriptableObject> levelsBundleHandle;

    private AsyncOperationHandle<GameObject> currentLoadedLevelHandle;
    private GameObject currentLoadedLevel = null;

    private Resolution deviceResolution;


    //Entities
    private GameObject soundSystem;
    private GameObject eventSystem;
    private GameObject netcode;

    private GameObject player;
    private GameObject mainCamera;
    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject creditsMenu;
    private GameObject connectionMenu;
    private GameObject levelSelectMenu;
    private GameObject winMenu;
    private GameObject loseMenu;
    private GameObject pauseMenu;
    private GameObject fadeTransition;

    //Scripts
    private SoundSystem soundSystemScript;
    private Player playerScript;
    private MainCamera mainCameraScript;
    private Netcode netcodeScript;
    private MainMenu mainMenuScript;
    private OptionsMenu optionsMenuScript;
    private ConnectionMenu connectionMenuScript;
    private FadeTransition fadeTransitionScript;



    public void Initialize() {
        if (!initializeOnStartup)
            return;

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
        SetupApplicationInitialSettings();
        initializationInProgress = true;
        LoadAssets();
    }


    //ResourceManagment
    private void LoadAssets() {
        LoadEssentials();
        LoadLevelsBundle();
        assetsLoadingInProgress = true;
        currentApplicationStatus = ApplicationStatus.LOADING_ASSETS;
    }
    private void LoadEssentials() {
        if (debugging)
            Log("Started loading essential assets!");

        loadedAssetsHandle = Addressables.LoadAssetsAsync<GameObject>(essentialAssetsLabel, AssetLoadedCallback);
        loadedAssetsHandle.Completed += FinishedLoadingAssetsCallback;
    }
    private void LoadLevelsBundle() {
        if (debugging)
            Log("Started loading levels bundle!");
        
        levelsBundleHandle = Addressables.LoadAssetAsync<ScriptableObject>(levelsBundleLabel);
        //TODO: Validate
        levelsBundleHandle.Completed += FinishedLoadingLevelsBundleCallback;
    }
    private bool CheckAssetsLoadingStatus() {

        bool result = true;

        result &= loadedAssetsHandle.IsDone;
        result &= levelsBundleHandle.IsDone;

        assetsLoadingInProgress = !result;
        return result;
    }


    private void SetupApplicationInitialSettings() {
        Input.gyro.enabled = true;
        deviceResolution = Screen.currentResolution;
        if (debugging) {
            Log("Application started on device.");
            Log("Device information:\nScreen Width: [" + deviceResolution.width 
                + "]\nScreen Height: [" + deviceResolution.height + "]\nRefresh Rate: [" 
                + deviceResolution.refreshRateRatio + "]");
        }

        //Framerate

    }
    private void SetApplicationTargetFrameRate(int target) {
        Application.targetFrameRate = target;
        if (debugging)
            Log("Framerate has been set to " + target + "!");
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
        if (debugging)
            Log("Destroyed all entities successfully!");

        //TODO: Add messages for each of these two

        //TODO: TEST OUT THE STATE OF A HANDLE BEFORE LOADING AND AFTER UNLOADING! IMPORTANT!
        //Note:
        //-on unloading, it becomes invalid! the handle!

        if (currentLoadedLevelHandle.IsValid()) {
            ValidateAndDestroy(currentLoadedLevel);
            Addressables.Release(currentLoadedLevelHandle);
            if (debugging)
                Log("Level was destroyed and unloaded successfully!");
        }

        if (levelsBundleHandle.IsValid()) {
            Addressables.Release(levelsBundleHandle);
            if (debugging)
                Log("Levels bundle was unloaded successfully!");
        }

        if (loadedAssetsHandle.IsValid()) {
            Addressables.Release(loadedAssetsHandle);
            if (debugging)
                Log("Assets were unloaded successfully!");
        }

        if (debugging) {
            Log("Released all resources successfully!");
            Log("Application has been stopped!");
        }
    }
    private void ValidateAndDestroy(GameObject target) {
        if (target)
            Destroy(target);
    }

    private void UpdateDebuggingCode() {
        //Level Loading
        if (Input.GetKeyDown(KeyCode.W))
            LoadLevel("Test");
        if (Input.GetKeyDown(KeyCode.Q))
            UnloadLevel();

        //Input
        //Input.gyro.enabled = true;
        //Input.GetTouch(0).
        //Log(Input.gyro.rotationRate);

        //Input.GetTouch(0).
    }


    //Update/Tick
    void Update() {

        UpdateDebuggingCode();

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
            CheckAssetsLoadingStatus();
            if (debugging)
                Log("Loading Assets In Progress...");
            return;
        }

        initialized = true;
        initializationInProgress = false;
        currentApplicationStatus = ApplicationStatus.RUNNING;
        SetGameState(GameState.MAIN_MENU);
        if (debugging)
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
            case GameState.LEVEL_SELECT_MENU:
                SetupLevelSelectMenuState();
                break;
            case GameState.CONNECTION_MENU:
                SetupConnectionMenuState();
                break;
            case GameState.WIN_MENU:
                SetupWinMenuState();
                break;
            case GameState.LOSE_MENU:
                SetupLoseMenuState();
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

        switch (state) {
            case GameState.MAIN_MENU:
                fadeTransitionScript.StartTransition(SetupMainMenuState);
                break;
            case GameState.OPTIONS_MENU:
                fadeTransitionScript.StartTransition(SetupOptionsMenuState);
                break;
            case GameState.CREDITS_MENU:
                fadeTransitionScript.StartTransition(SetupCreditsMenuState);
                break;
            case GameState.LEVEL_SELECT_MENU:
                fadeTransitionScript.StartTransition(SetupLevelSelectMenuState);
                break;
            case GameState.CONNECTION_MENU:
                fadeTransitionScript.StartTransition(SetupConnectionMenuState);
                break;
            case GameState.WIN_MENU:
                fadeTransitionScript.StartTransition(SetupWinMenuState);
                break;
            case GameState.LOSE_MENU:
                fadeTransitionScript.StartTransition(SetupLoseMenuState);
                break;
            case GameState.PLAYING:
                fadeTransitionScript.StartTransition(SetupStartState);
                break;
            case GameState.PAUSED:
                Warning("Use PauseGame/UnpauseGame instead of calling Transition(GameState.PAUSED)");
                break;
        }
    }



    public void PauseGame() {
        gamePaused = true;
        Time.timeScale = 0.0f;

    }
    public void UnpauseGame() {
        gamePaused = false;
        Time.timeScale = 1.0f;

    }

    //State Setup
    private void SetupMainMenuState() {
        currentGameState = GameState.MAIN_MENU;
        HideAllMenus();
        mainMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupOptionsMenuState() {
        currentGameState = GameState.OPTIONS_MENU;
        HideAllMenus();
        optionsMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupCreditsMenuState() {
        currentGameState = GameState.CREDITS_MENU;
        HideAllMenus();
        creditsMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupConnectionMenuState() {
        currentGameState = GameState.CONNECTION_MENU;
        HideAllMenus();
        connectionMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupLevelSelectMenuState() {
        currentGameState = GameState.LEVEL_SELECT_MENU;
        HideAllMenus();
        levelSelectMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupStartState() {
        currentGameState = GameState.PLAYING;
        HideAllMenus();
        SetApplicationTargetFrameRate(gameplayFrameTarget); //Make sure to call this the moment the gameplay state is ready!

    }
    private void SetupWinMenuState() {
        currentGameState = GameState.WIN_MENU;
        HideAllMenus();
        winMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupLoseMenuState() {
        currentGameState = GameState.LOSE_MENU;
        HideAllMenus();
        loseMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }


    //State Update
    private void UpdateStatelessSystems() {
        soundSystemScript.Tick();
    }
    private void UpdatePlayingState() {
        playerScript.Tick();
    }
    private void UpdateFixedPlayingState() {
        playerScript.FixedTick();
    }



    //Level Loading - TODO: Move into LevelManagement class along with related vars
    public bool LoadLevel(string levelName) {
        if (currentLoadedLevel) {
            Error("Failed to load level!\nUnload current level first before loading a new one!");
            return false;
        }
        if (!levelsBundleHandle.IsValid()) {
            Error("Failed to load level!\nLevels bundle was not loaded!");
            return false;
        }

        LevelEntry requestedLevel = new LevelEntry();
        bool levelFound = false;
        foreach (LevelEntry level in ((LevelsBundle)levelsBundleHandle.Result).Entries) {
            if (level.key == levelName) {
                requestedLevel = level;
                levelFound = true;
            }
        }

        if (!levelFound) {
            Error("Failed to load level!\nRequested level was not found in the levels bundle!");
            return false;
        }

        currentLoadedLevelHandle = Addressables.LoadAssetAsync<GameObject>(requestedLevel.asset);
        if (currentLoadedLevelHandle.IsDone) {
            Error("Failed to load level!\nRequested level was not found in the addressables!");
            return false;
        }
        currentLoadedLevelHandle.Completed += FinishedLoadingLevelCallback;

        if (debugging)
            Log("Started loading level associated with key [" + levelName + "]");

        return true;
    }
    public bool UnloadLevel() {
        if (!currentLoadedLevel) {
            Warning("Unable to unload level!\nNo levels are currently loaded!");
            return false;
        }

        ValidateAndDestroy(currentLoadedLevel);
        Addressables.Release(currentLoadedLevelHandle);

        if (debugging)
            Log("Started unloading current level!");

        return true;
    }
    private bool CreateLevel(GameObject asset) {
        if (currentLoadedLevel) {
            Error("Failed to create level\nThere is currently a level loaded already!");
            return false;
        }

        currentLoadedLevel = Instantiate(asset);
        if (!currentLoadedLevel) {
            Error("Failed to create level\nInstantiation failed!");
            return false;
        }

        return true;
    }


    private void HideAllMenus() {
        //Add all GUI here!
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
        connectionMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }

    //Unused
    private void SetCursorState(bool state) {
        UnityEngine.Cursor.visible = state;
        if (state)
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        else
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public bool IsDebuggingEnabled() { return debugging; }

    

    //Getters
    public Netcode GetNetcode() { return netcodeScript; }



    //Callbacks
    private void AssetLoadedCallback(GameObject asset) {
        if (debugging)
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
        else if (asset.CompareTag("EventSystem")) {
            Log("Started creating " + asset.name + " entity");
            eventSystem = Instantiate(asset);
        }
        else if (asset.CompareTag("Netcode")) {
            Log("Started creating " + asset.name + " entity");
            netcode = Instantiate(asset);
            netcodeScript = netcode.GetComponent<Netcode>();
            netcodeScript.Initialize(this);
            Validate(netcodeScript, "Netcode component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("MainMenu")) {
            Log("Started creating " + asset.name + " entity");
            mainMenu = Instantiate(asset);
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
            mainMenuScript.Initialize(this);
            Validate(mainMenuScript, "MainMenu component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("OptionsMenu")) {
            Log("Started creating " + asset.name + " entity");
            optionsMenu = Instantiate(asset);
            optionsMenuScript = optionsMenu.GetComponent<OptionsMenu>();
            optionsMenuScript.Initialize(this);
            Validate(optionsMenuScript, "OptionMenu component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("CreditsMenu")) {
            Log("Started creating " + asset.name + " entity");
            creditsMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("ConnectionMenu")) {
            Log("Started creating " + asset.name + " entity");
            connectionMenu = Instantiate(asset);
            connectionMenuScript = connectionMenu.GetComponent<ConnectionMenu>();
            connectionMenuScript.Initialize(this);
        }
        else if (asset.CompareTag("LevelSelectMenu")) {
            Log("Started creating " + asset.name + " entity");
            levelSelectMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("LoseMenu")) {
            Log("Started creating " + asset.name + " entity");
            loseMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("WinMenu")) {
            Log("Started creating " + asset.name + " entity");
            winMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("PauseMenu")) {
            Log("Started creating " + asset.name + " entity");
            pauseMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("FadeTransition")) {
            Log("Started creating " + asset.name + " entity");
            fadeTransition = Instantiate(asset);
            fadeTransitionScript = fadeTransition.GetComponent<FadeTransition>();
            fadeTransitionScript.Initialize(this);
        }
        else
            Warning("Loaded an asset that was not recognized!\n[" + asset.name + "]");
    }

    private void FinishedLoadingAssetsCallback(AsyncOperationHandle<IList<GameObject>> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (debugging)
                Log("Finished loading assets successfully!");
            //assetsLoadingInProgress = false; //Move this to function that checks the status of the assets handle and the levels bundle handle!
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            AbortApplication("Failed to load assets!\nCheck if label is correct.");
        }
    }
    private void FinishedLoadingLevelsBundleCallback(AsyncOperationHandle<ScriptableObject> handle) {
        //NOTE: Could ditch even assinging the callbacks depending on showSystemMessages if its only debugging code in them
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (debugging)
                Log("Finished loading levels bundle successfully!");
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            AbortApplication("Failed to load levels bundle!\nCheck if label is correct.");
        }
    }
    private void FinishedLoadingLevelCallback(AsyncOperationHandle<GameObject> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (debugging)
                Log("Finished loading level successfully!");

            bool result = CreateLevel(handle.Result);
            if (!result)
                Error("Failed to create level!\nCheck asset for any errors!");
            //Other script stuff!

            //Start game or any registered callback for level loading
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            Error("Failed to load level!"); //going back to main menu. message
            //Go back to some other state
        }
    }
}
