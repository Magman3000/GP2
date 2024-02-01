using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using static MyUtility.Utility;

public class CoordinatorHUD : Entity {

    
    private Player playerRef;
    private Coordinator coordinatorRef;

    private Slider boostCrankSlider;
    private Image batteryBar;

    //For testing purposes 
    private float SnapOpenThreshold = 0.5f;
    private Image DragWindowHandle;
    private Image DropWindowBody;
    private float DragWindowClosePosition;

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
    private void SetupReferences() {

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




        DragWindowHandle = transform.Find("DragWindow").GetComponent<Image>();
        DropWindowBody = DragWindowHandle.transform.Find("DragWindowBody").GetComponent<Image>();
        DragWindowClosePosition = DragWindowHandle.rectTransform.localPosition.y; //Could be any other axis. 
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



    public void DragWindowBegin() {
        Log("Begin!");

        var currentPosition = DragWindowHandle.rectTransform.localPosition;
        DragWindowHandle.rectTransform.localPosition = new Vector3(currentPosition.x, Input.mousePosition.y, currentPosition.z);
        //Lock 
        
    }
    public void DragWindowEnd() {
        Log("End!");

        if (DragWindowHandle.rectTransform.localPosition.y >= DropWindowBody.rectTransform.rect.height * SnapOpenThreshold) {
            //Open it
            var currentPosition = DragWindowHandle.rectTransform.localPosition;
            DragWindowHandle.rectTransform.localPosition = new Vector3(currentPosition.x, DropWindowBody.rectTransform.rect.height, currentPosition.z);
        }
        else {
            //Close it
            var currentPosition = DragWindowHandle.rectTransform.localPosition;
            DragWindowHandle.rectTransform.localPosition = new Vector3(currentPosition.x, DragWindowClosePosition, currentPosition.z);
        }
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
}
