using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static MyUtility.Utility;

public class WinMenu : Entity
{
    private TMP_Text scoreText;
    private TMP_Text finalTimeText;

    private Level levelRef;
    private ScoreSystem scoreSystemRef;
    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }

    private void SetupReferences()
    {


        //finding the transform for the score text and then the text component
        Transform scoreTextTransform = transform.Find("Score");
        Validate(scoreTextTransform, "ScoreTransform transform not found!", ValidationLevel.ERROR, true);
        scoreText = scoreTextTransform.GetComponent<TMP_Text>();
        Validate(scoreText, "ScoreTransform transform not found!", ValidationLevel.ERROR, true);

        //finding the transform for the time text and then the text component
        Transform timeLimitTextTransform = transform.Find("SecondsLeft");
        Validate(timeLimitTextTransform, "TimeLimitTransform transform not found!", ValidationLevel.ERROR, true);
        finalTimeText = timeLimitTextTransform.GetComponent<TMP_Text>();
        Validate(finalTimeText, "TimeLimitText transform not found!", ValidationLevel.ERROR, true);

        //setting references to the currentScore variable and currentTimeLimit variable
        levelRef = gameInstanceRef.GetLevelManagement().GetCurrentLoadedLevel();
        scoreSystemRef = gameInstanceRef.GetScoreSystem();
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Your Final Score: " + scoreSystemRef.GetCurrentScore();
    }

    public void UpdateTimeText()
    {
        finalTimeText.text = "You Had " + levelRef.GetCurrentTimeLimit() + "Seconds Left!";
    }
}
