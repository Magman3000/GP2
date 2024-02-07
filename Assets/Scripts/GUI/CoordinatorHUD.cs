using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using static MyUtility.Utility;

public class CoordinatorHUD : Entity {

    
    private Player playerRef;
    private Coordinator coordinatorRef;
    private Vector3 daredevilPosition = Vector3.zero;

    private Slider boostCrankSlider;
    private Image batteryBar;
    private TMP_Text scoreText;
    private TMP_Text timeLimitText;

    private const float distanceTrashHold = 10f;


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

        UpdateRadar();
    }
    private void SetupReferences()
    {

        //BoostCrank
        Transform boostCrankTransform = transform.Find("BoostCrank");
        Validate(boostCrankTransform, "BoostCrank transform not found!", ValidationLevel.ERROR, true);
        boostCrankSlider = boostCrankTransform.GetComponent<Slider>();
        Validate(boostCrankSlider, "BoostCrank slider component not found!", ValidationLevel.ERROR, true);


        //BatteryBar batteryBar
        Transform batteryBarTransform = transform.Find("BatteryBar");
        Validate(batteryBarTransform, "BatteryBar transform not found!", ValidationLevel.ERROR, true);
        Transform batteryBarFillTransform = batteryBarTransform.Find("BatteryBarFill");
        Validate(batteryBarFillTransform, "BatteryBarFill transform not found!", ValidationLevel.ERROR, true);
        batteryBar = batteryBarFillTransform.GetComponent<Image>();
        Validate(batteryBar, "batteryBar component not found!", ValidationLevel.ERROR, true);

        //Dragwindow
        Transform DragWindowTransform = transform.Find("DragWindow");
        Validate(DragWindowTransform, "DragWindowTransform transform not found!", ValidationLevel.ERROR, true);
        Transform DragWindowBodyTransform = DragWindowTransform.transform.Find("DragWindowBody");
        Validate(DragWindowBodyTransform, "DragWindowBodyTransform transform not found!", ValidationLevel.ERROR, true);
        //Score inside dragwindow
        Transform ScoreTextTransform = DragWindowBodyTransform.transform.Find("ScoreTMP");
        Validate(ScoreTextTransform, "ScoreTextTransform transform not found!", ValidationLevel.ERROR, true);
        timeLimitText = ScoreTextTransform.GetComponent<TMP_Text>();
        Validate(timeLimitText, "timeLimitText transform not found!", ValidationLevel.ERROR, true);
        //TimeLimit inside dragwindow
        Transform TimeLimitTextTransform = DragWindowBodyTransform.transform.Find("TimeLimitTMP");
        Validate(TimeLimitTextTransform, "TimeLimitTextTransform transform not found!", ValidationLevel.ERROR, true);
        scoreText = TimeLimitTextTransform.GetComponent<TMP_Text>();
        Validate(timeLimitText, "scoreText transform not found!", ValidationLevel.ERROR, true);
    }

    //calling scoreSystem through gameInstance to change the score
    public void UpdateCurrentScore()
    {
        scoreText.text = "Score: " + gameInstanceRef.GetScoreSystem().GetCurrentScore();
    }

    //getting the current time limit from calling level from levelmanagment 
    public void UpdateTimeLimit()
    {
        timeLimitText.text = "Seconds Left: " + gameInstanceRef.GetLevelManagement().GetCurrentLoadedLevel().GetCurrentTimeLimit();
    }

    public void SetupStartState() {
        batteryBar.fillAmount = coordinatorRef.GetStats().batteryLimit; //Could be changed to starting battery limit later!
    }

    public void SetPlayerReference(Player player) {
        playerRef = player;
        if (playerRef)
            coordinatorRef = playerRef.GetCoordinatorData();
    }

    public void RelayBoostState(bool state) {
        Log("Boost : " + state);
        gameInstanceRef.GetRPCManagement().SetBoostStateServerRpc(Netcode.GetClientID(), state);
    }
    public void UpdatePowerBar(float percentage) {
        batteryBar.fillAmount = percentage;
    }



   public void ObstacleActivationStateButton(int index) {
        if (index < 0) {
            Warning("Invalid index received at ObstacleActivationStateButton: " + index);
            return;
        }

        var currentLevel = gameInstanceRef.GetLevelManagement().GetCurrentLoadedLevel();
        if (!currentLevel) {
            Warning("ObstacleActivationStateButton was called while no level was loaded!");
            return;
        }
        if (currentLevel.currentObstacleState == (Obstacle.ObstacleActivationState)index)
            return;

        gameInstanceRef.GetRPCManagement().SetObstacleActivationStateServerRpc(Netcode.GetClientID(), (Obstacle.ObstacleActivationState)index);
   }

    public void BoostCrankSlider() {
        float value = boostCrankSlider.value;
        if (value == boostCrankSlider.maxValue) { //If boost is not activated
            RelayBoostState(true);
            coordinatorRef.SetBoostState(true);
        }
        else if (value == boostCrankSlider.minValue) { //If boost is activated
            RelayBoostState(false);
            coordinatorRef.SetBoostState(false);
        }
    }
    
    public void SetDaredevilPosition(Vector3 position) {
        daredevilPosition = position;
    }

    private void UpdateRadar() {
        var playerPosition = daredevilPosition;
        var currentLevel = gameInstanceRef.GetLevelManagement().GetCurrentLoadedLevel();
        foreach (var obstacle in currentLevel.registeredObstacles) {
            if (!obstacle)
                continue;
            var distance = Vector3.Distance(playerPosition, obstacle.transform.position);

            if (distance < distanceTrashHold) {
                PingRadar(daredevilPosition);
            }
        }
    }

    private void PingRadar(Vector3 position) {
        Log("Pinging radar!");
    }
}
