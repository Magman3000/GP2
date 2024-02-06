using UnityEngine;
using TMPro;

public class DaredevilHUD : Entity {

    public enum DaredevilKeyCode
    {
        MoveLeft = 0,
        MoveRight = 1,
        Accelerate = 2,
        Decelerate = 3
    }
    
    private Player playerRef;
    private Daredevil daredevilRef;
    public TMP_Text scoreText;
    public TMP_Text timeLimitText;

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }


    public void SetupStartState() {

    }


    public void SetPlayerReference(Player player) {
        playerRef = player;
        if (playerRef)
            daredevilRef = playerRef.GetDaredevilData();
    }



    public void UpdateCurrentScore(float score) {
        scoreText.text = ("Score: " + score);
    }

    public void UpdateTimeLimit(float timeLimit) {
        timeLimitText.text = ("Seconds Left: " + timeLimit);
    }

    public void BrakeOnEvent() {
        daredevilRef.SetBrakeState(true);
    }
    public void BrakeOffEvent() {
        daredevilRef.SetBrakeState(false);
    }
    public void GasOnEvent() {
        daredevilRef.SetMovementState(true);
    }
    public void GasOffEvent() {
        daredevilRef.SetMovementState(false);
    }
}
