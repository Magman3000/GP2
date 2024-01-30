using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;
using Unity.Services.Authentication;
using UnityEngine.EventSystems;

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
        ROLE_SELECT_MENU,
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
    private const string loadingScreenLabel         = "LoadingScreen";



    public ApplicationStatus currentApplicationStatus = ApplicationStatus.ERROR;
    public GameState currentGameState = GameState.ERROR;



    private bool initialized = false;
    private bool initializationInProgress = false;
    private bool assetsLoadingInProgress = false;
    private bool gameStarted = false;
    private bool gamePaused = false;

    private int powerSavingFrameTarget = 20;
    private int gameplayFrameTarget = -1;

    private AsyncOperationHandle<IList<GameObject>> loadedAssetsHandle;
    private AsyncOperationHandle<GameObject> loadingScreenHandle;

    private Resolution deviceResolution;


    //Entities
    private GameObject soundSystem;
    private GameObject eventSystem;
    private GameObject netcode;

    private GameObject player;
    private GameObject daredevilHUD;
    private GameObject coordinatorHUD;

    private GameObject mainCamera;
    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject creditsMenu;
    private GameObject connectionMenu;
    private GameObject roleSelectMenu;
    private GameObject levelSelectMenu;
    private GameObject winMenu;
    private GameObject loseMenu;

    private GameObject pauseMenu;
    private GameObject fadeTransition;
    private GameObject loadingScreen;

    //Scripts
    private GameObject rpcManagement;
    private RPCManagement rpcManagementScript;
    private SoundSystem soundSystemScript;
    private LevelManagement levelManagementScript = new LevelManagement();
    private Player playerScript;

    private DaredevilHUD daredevilHUDScript;
    private CoordinatorHUD coordinatorHUDScript;

    private MainCamera mainCameraScript;
    private Netcode netcodeScript;
    private MainMenu mainMenuScript;
    private OptionsMenu optionsMenuScript;
    private CreditsMenu creditsMenuScript;
    private ConnectionMenu connectionMenuScript;
    private RoleSelectMenu roleSelectMenuScript;
    private LevelSelectMenu levelSelectMenuScript;
    private FadeTransition fadeTransitionScript;
    private LoadingScreen loadingScreenScript;



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
        levelManagementScript.Initialize(this); //So it starts loading level bundle concurrently.
        LoadAssets();
    }


    //ResourceManagment
    private void LoadAssets() {
        LoadLoadingScreen();
        LoadEssentials();
        assetsLoadingInProgress = true;
        currentApplicationStatus = ApplicationStatus.LOADING_ASSETS;
    }
    private void LoadLoadingScreen() {
        if (debugging)
            Log("Started loading LoadingScreen!");

        loadingScreenHandle = Addressables.LoadAssetAsync<GameObject>(loadingScreenLabel);
        if (!loadingScreenHandle.IsValid()) {
            QuitApplication("Failed to load loading screen\nCheck if label is correct!");
            return;
        }
        loadingScreenHandle.Completed += FinishedLoadingLoadingScreenCallback;
    }
    private void LoadEssentials() {
        if (debugging)
            Log("Started loading essential assets!");

        loadedAssetsHandle = Addressables.LoadAssetsAsync<GameObject>(essentialAssetsLabel, AssetLoadedCallback);
        loadedAssetsHandle.Completed += FinishedLoadingAssetsCallback;
    }
    private bool CheckAssetsLoadingStatus() {
        assetsLoadingInProgress = !loadedAssetsHandle.IsDone;
        return loadedAssetsHandle.IsDone;
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
    //This is kinda problamatic consedering i cant free the resources from here! cause its static
    public void QuitApplication(object message = null) {
        UnloadResources();
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
        if (!initializeOnStartup)
            return;

        //Steps:
        //-Call CleanUp on all entities
        //-Destroy gameobjects
        //-Release resources


        //ADD THE REST OF THE ENTITIES ! CLEAN UP

        //Needs reworking after networking solution
        if (playerScript) //Temp
            playerScript.CleanUp("Player cleaned up successfully!");


        mainCameraScript.CleanUp("MainCamera cleaned up successfully!");
        soundSystemScript.CleanUp("SoundSystem cleaned up successfully!");
        levelManagementScript.CleanUp();


        //Needed to guarantee destruction of all entities before attempting to release resources.
        ValidateAndDestroy(player);
        ValidateAndDestroy(mainCamera);
        ValidateAndDestroy(soundSystem);
        ValidateAndDestroy(eventSystem);
        ValidateAndDestroy(daredevilHUD);
        ValidateAndDestroy(coordinatorHUD);
        ValidateAndDestroy(fadeTransition);
        ValidateAndDestroy(loadingScreen);

        ValidateAndDestroy(rpcManagement);
        ValidateAndDestroy(netcode);
        ValidateAndDestroy(mainMenu);
        ValidateAndDestroy(optionsMenu);
        ValidateAndDestroy(creditsMenu);
        ValidateAndDestroy(connectionMenu);
        ValidateAndDestroy(levelSelectMenu);
        ValidateAndDestroy(roleSelectMenu);
        ValidateAndDestroy(pauseMenu);
        ValidateAndDestroy(winMenu);
        ValidateAndDestroy(loseMenu);

        //ADD REST OF THE MENUS!



        if (debugging)
            Log("Destroyed all entities successfully!");

        //TODO: Add messages for each of these two





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


    //Update/Tick
    void Update() {
        if (!initializeOnStartup)
            return;

        switch (currentApplicationStatus) {
            case ApplicationStatus.LOADING_ASSETS:
                UpdateApplicationLoadingAssetsState();
                break;
            case ApplicationStatus.RUNNING:
                UpdateApplicationRunningState(); 
                break;
            case ApplicationStatus.ERROR:
                QuitApplication("Attempted to update application while status was ERROR");
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
            if (loadingScreenScript)
                loadingScreenScript.UpdateLoadingBar(loadedAssetsHandle.PercentComplete);
            if (debugging)
                Log("Loading Assets In Progress...");
            return;
        }
        else if (loadingScreenScript.IsLoadingProcessRunning())
            loadingScreenScript.FinishLoadingProcess();


        SetupDependencies();

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
            case GameState.CONNECTION_MENU:
                UpdateConnectionMenuState();
                break;
        }
    }
    private void SetupDependencies() {
        mainCameraScript.SetPlayerReference(playerScript);
        daredevilHUDScript.SetPlayerReference(playerScript);
        coordinatorHUDScript.SetPlayerReference(playerScript);
        playerScript.SetCoordinatorHUD(coordinatorHUDScript);
        playerScript.SetDaredevilHUD(daredevilHUDScript);

        if (debugging)
            Log("All dependencies has been setup!");
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
            case GameState.ROLE_SELECT_MENU:
                SetupRoleSelectMenuState();
                break;
            case GameState.WIN_MENU:
                SetupWinMenuState();
                break;
            case GameState.LOSE_MENU:
                SetupLoseMenuState();
                break;
            case GameState.PLAYING:
                Warning("Use StartGame instead of calling SetGameState(GameState.PLAYING)");
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
            case GameState.ROLE_SELECT_MENU:
                fadeTransitionScript.StartTransition(SetupRoleSelectMenuState);
                break;
            case GameState.WIN_MENU:
                fadeTransitionScript.StartTransition(SetupWinMenuState);
                break;
            case GameState.LOSE_MENU:
                fadeTransitionScript.StartTransition(SetupLoseMenuState);
                break;
            case GameState.PLAYING:
                Warning("Use StartGame instead of calling Transition(GameState.PLAYING)");
                break;
            case GameState.PAUSED:
                Warning("Use PauseGame/UnpauseGame instead of calling Transition(GameState.PAUSED)");
                break;
        }
    }



    public void PauseGame() {
        gamePaused = true;
        //Time.timeScale = 0.0f; 

    }
    public void UnpauseGame() {
        gamePaused = false;
        //Time.timeScale = 1.0f; ITS A MULTIPLAYER GAME

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
        connectionMenuScript.SetupStartState();
        connectionMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupRoleSelectMenuState() {
        currentGameState = GameState.ROLE_SELECT_MENU;
        HideAllMenus();
        roleSelectMenuScript.SetupMenuStartState();
        roleSelectMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupLevelSelectMenuState() {
        currentGameState = GameState.LEVEL_SELECT_MENU;
        HideAllMenus();
        levelSelectMenuScript.SetupMenuStartingState();
        levelSelectMenu.SetActive(true);
        SetApplicationTargetFrameRate(powerSavingFrameTarget);

    }
    private void SetupStartState() {
        currentGameState = GameState.PLAYING;
        HideAllMenus();
        SetApplicationTargetFrameRate(gameplayFrameTarget); //Make sure to call this the moment the gameplay state is ready!



        player.SetActive(true);
        //Enable controls! turn on and enable huds for each (SetActive(true) pretty much)


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
        netcodeScript.Tick();
    }
    private void UpdateConnectionMenuState() {
        connectionMenuScript.Tick();
    }
    private void UpdatePlayingState() {
        mainCameraScript.Tick();
        playerScript.Tick();
    }
    private void UpdateFixedPlayingState() {
        playerScript.FixedTick();
    }



    public bool StartGame() {
        if (!levelManagementScript.LoadQueuedLevelKey())
            return false;

        fadeTransitionScript.StartTransition(SetupStartState); //Will probably be swtiched or combines with loading screen
        gameStarted = true;
        return true;
    }
    public bool StartGame(string levelName) {
        if (!levelManagementScript.LoadLevel(levelName))
            return false;

        fadeTransitionScript.StartTransition(SetupStartState); //Will probably be swtiched or combines with loading screen
        gameStarted = true;
        return true;
    }
    public void InterruptGame() {
        //In case of player disconnection!
        if (levelManagementScript.IsLevelLoaded())
            levelManagementScript.UnloadLevel();

        gameStarted = false;
        player.SetActive(false);

        if (gamePaused)
            UnpauseGame();

        //Should also do some clean ups that i forgot where.
        //-I put a comment regarding it there tho.

        Transition(GameState.MAIN_MENU);
    }



    //Level Loading - TODO: Move into LevelManagement class along with related vars


    private void HideAllMenus() {
        //Add all GUI here!
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
        connectionMenu.SetActive(false);
        roleSelectMenu.SetActive(false);
        pauseMenu.SetActive(false);
        //? huds?
    }


    public bool IsDebuggingEnabled() { return debugging; }


    //Getters
    public Netcode GetNetcode() { return netcodeScript; }
    public RPCManagement GetRPCManagement() { return rpcManagementScript; }
    public LevelManagement GetLevelManagement() { return levelManagementScript; }

    public RoleSelectMenu GetRoleSelectMenu() { return roleSelectMenuScript; }
    public Player GetPlayer() { return playerScript; }


    //RPC related
    public void ConfirmAllClientsConnected() {
        Transition(GameState.ROLE_SELECT_MENU);
    }




    //Callbacks
    private void AssetLoadedCallback(GameObject asset) {
        if (debugging)
            Log(asset.name + " has been loaded successfully!");

        //Notes:
        //Any asset that get loaded will be used to construct a gameobject while the rest of the assets are still being loaded
        //Taking advantage of multithreading


        //TODO: Do something about this!

        if (asset.CompareTag("Player")) {
            player = Instantiate(asset);
            player.SetActive(false);
            playerScript = player.GetComponent<Player>();
            Validate(playerScript, "Player component is missing on entity!", ValidationLevel.ERROR, true);
            playerScript.Initialize(this);
        }
        else if (asset.CompareTag("MainCamera")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            mainCamera = Instantiate(asset);
            mainCameraScript = mainCamera.GetComponent<MainCamera>();
            mainCameraScript.Initialize(this);
            //mainCameraScript.SetPlayerReference(player1Script); //Cant guarantee order!
            Validate(mainCameraScript, "MainCamera component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("SoundSystem")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            soundSystem = Instantiate(asset);
            soundSystemScript = soundSystem.GetComponent<SoundSystem>();
            soundSystemScript.Initialize(this);
            Validate(soundSystemScript, "SoundSystem component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("RPCManagement")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            rpcManagement = Instantiate(asset);
            rpcManagementScript = rpcManagement.GetComponent<RPCManagement>();
            rpcManagementScript.Initialize(this);
            Validate(rpcManagementScript, "RpcManagement component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("EventSystem")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            eventSystem = Instantiate(asset);
        }
        else if (asset.CompareTag("Netcode")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            netcode = Instantiate(asset);
            netcodeScript = netcode.GetComponent<Netcode>();
            netcodeScript.Initialize(this);
            Validate(netcodeScript, "Netcode component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("MainMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            mainMenu = Instantiate(asset);
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
            mainMenuScript.Initialize(this);
            Validate(mainMenuScript, "MainMenu component is missing on entity!", ValidationLevel.ERROR, true);
        }
        else if (asset.CompareTag("OptionsMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            optionsMenu = Instantiate(asset);
            optionsMenuScript = optionsMenu.GetComponent<OptionsMenu>();
            Validate(optionsMenuScript, "OptionMenu component is missing on entity!", ValidationLevel.ERROR, true);
            optionsMenuScript.Initialize(this);
        }
        else if (asset.CompareTag("CreditsMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            creditsMenu = Instantiate(asset);
            creditsMenuScript = creditsMenu.GetComponent<CreditsMenu>();
            Validate(creditsMenuScript, "CreditsMenu component is missing on entity!", ValidationLevel.ERROR, true);
            creditsMenuScript.Initialize(this);
        }
        else if (asset.CompareTag("ConnectionMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            connectionMenu = Instantiate(asset);
            connectionMenuScript = connectionMenu.GetComponent<ConnectionMenu>();
            Validate(connectionMenuScript, "ConnectionMenu component is missing on entity!", ValidationLevel.ERROR, true);
            connectionMenuScript.Initialize(this);
        }
        else if (asset.CompareTag("RoleSelectMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            roleSelectMenu = Instantiate(asset);
            roleSelectMenuScript = roleSelectMenu.GetComponent<RoleSelectMenu>();
            Validate(roleSelectMenuScript, "RoleSelectMenu component is missing on entity!", ValidationLevel.ERROR, true);
            roleSelectMenuScript.Initialize(this);
        }
        else if (asset.CompareTag("LevelSelectMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            levelSelectMenu = Instantiate(asset);
            levelSelectMenuScript = levelSelectMenu.GetComponent<LevelSelectMenu>();
            Validate(levelSelectMenuScript, "LevelSelectMenu component is missing on entity!", ValidationLevel.ERROR, true);
            levelSelectMenuScript.Initialize(this);
        }
        else if (asset.CompareTag("LoseMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            loseMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("WinMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            winMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("DaredevilHUD")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            daredevilHUD = Instantiate(asset);
            daredevilHUDScript = daredevilHUD.GetComponent<DaredevilHUD>();
            Validate(daredevilHUDScript, "DaredevilHUD component is missing on entity!", ValidationLevel.ERROR, true);
            daredevilHUDScript.Initialize(this);
        }
        else if (asset.CompareTag("CoordinatorHUD")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            coordinatorHUD = Instantiate(asset);
            coordinatorHUDScript = coordinatorHUD.GetComponent<CoordinatorHUD>();
            Validate(coordinatorHUDScript, "CoordinatorHUD component is missing on entity!", ValidationLevel.ERROR, true);
            coordinatorHUDScript.Initialize(this);
        }
        else if (asset.CompareTag("PauseMenu")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            pauseMenu = Instantiate(asset);
        }
        else if (asset.CompareTag("FadeTransition")) {
            if (debugging)
                Log("Started creating " + asset.name + " entity");
            fadeTransition = Instantiate(asset);
            fadeTransitionScript = fadeTransition.GetComponent<FadeTransition>();
            fadeTransitionScript.Initialize(this);
        }
        else
            Warning("Loaded an asset that was not recognized!\n[" + asset.name + "]");
    }



    private void FinishedLoadingLoadingScreenCallback(AsyncOperationHandle<GameObject> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (debugging)
                Log("Finished loading LoadingScreen successfully!");

            loadingScreen = Instantiate(handle.Result);
            loadingScreen.SetActive(false);
            loadingScreenScript = loadingScreen.GetComponent<LoadingScreen>();
            loadingScreenScript.Initialize(this);
            if (assetsLoadingInProgress)
                loadingScreenScript.StartLoadingProcess(LoadingScreen.LoadingProcess.LOADING_ASSETS);

            //Start Using it?
            if (debugging)
                Log("Created " + handle.Result.name);
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            QuitApplication("Failed to load LoadingScreen!\nCheck if label is correct.");
        }
    }
    private void FinishedLoadingAssetsCallback(AsyncOperationHandle<IList<GameObject>> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (debugging)
                Log("Finished loading assets successfully!");
            //assetsLoadingInProgress = false; //Move this to function that checks the status of the assets handle and the levels bundle handle!
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            QuitApplication("Failed to load assets!\nCheck if label is correct.");
        }
    }

}
