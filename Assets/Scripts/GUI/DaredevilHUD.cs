using UnityEngine;
using TMPro;
using static MyUtility.Utility;

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

    private TMP_Text scoreText;
    private TMP_Text timeLimitText;

    

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;




        gameInstanceRef = game;
        SetupReferences();
        initialized = true;
    }

    public override void Tick() {
        if (!initialized)
            return;

        UpdateCurrentScore();
        UpdateTimeLimit();
    }
    public void SetupStartState() {

    }


    public void SetPlayerReference(Player player) {
        playerRef = player;
        if (playerRef)
            daredevilRef = playerRef.GetDaredevilData();
    }

    private void SetupReferences()
    {
        //finding the text for the score
        Transform scoreBackgroundTransform = transform.Find("Score");
        Validate(scoreBackgroundTransform, "ScoreTransform transform not found!", ValidationLevel.ERROR, true);
        Transform scoreTextTransform = scoreBackgroundTransform.transform.Find("ScoreTMP");
        Validate(scoreTextTransform, "ScoreTransform transform not found!", ValidationLevel.ERROR, true);
        scoreText = scoreTextTransform.GetComponent<TMP_Text>();
        Validate(scoreText, "ScoreTransform transform not found!", ValidationLevel.ERROR, true);

        //finding the text for the time limit
        Transform timeLimitBackgroundTransform = transform.Find("TimeLimit");
        Validate(timeLimitBackgroundTransform, "ScoreTransform transform not found!", ValidationLevel.ERROR, true);
        Transform timeLimitTextTransform = timeLimitBackgroundTransform.transform.Find("TimeLimitTMP");
        Validate(timeLimitTextTransform, "TimeLimitTransform transform not found!", ValidationLevel.ERROR, true);
        timeLimitText = timeLimitTextTransform.GetComponent<TMP_Text>();
        Validate(timeLimitText, "TimeLimitText transform not found!", ValidationLevel.ERROR, true);


        
    }

    //calling scoreSystem through gameInstance to change the score
    public void UpdateCurrentScore() {
        scoreText.text = "Score: " + gameInstanceRef.GetScoreSystem().GetCurrentScore();
    }

    //getting the current time limit from calling level from levelmanagment 
    public void UpdateTimeLimit() {
        timeLimitText.text = "Seconds Left: " + gameInstanceRef.GetLevelManagement().GetCurrentLoadedLevel().GetCurrentTimeLimit();
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
