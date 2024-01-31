using System.Xml.Serialization;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using static MyUtility.Utility;

public class CoordinatorHUD : Entity {

    
    private Player playerRef;
    private Coordinator coordinatorRef;

    private Slider boostCrankSlider;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        SetupReferences();
        gameInstanceRef = game;
        initialized = true;
    }
    private void SetupReferences() {
        Transform boostCrankTransform = transform.Find("BoostCrank");
        Validate(boostCrankTransform, "BoostCrank transform not found!", ValidationLevel.ERROR, true);
        boostCrankSlider = boostCrankTransform.GetComponent<Slider>();
        Validate(boostCrankSlider, "BoostCrank slider component not found!", ValidationLevel.ERROR, true);
    }
    
    public void SetPlayerReference(Player player) {
        playerRef = player;
        if (playerRef)
            coordinatorRef = playerRef.GetCoordinatorData();
    }




    public void BoostCrankSlider() {
        float value = boostCrankSlider.value;
        if (value == boostCrankSlider.maxValue) { //If boost is not activated
            Log("ON!");
        }
        else if (value == boostCrankSlider.minValue) { //If boost is activated
            Log("OFF!");
        }
    }
}
