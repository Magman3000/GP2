using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static MyUtility.Utility;

public class LevelSelectMenu : Entity
{
    //Temp
    [SerializeField] private GameObject levelPrefab; 
    
    //SerializeFields
    [SerializeField] private LevelsBundle levelsBundle;
    
    //Ref
    private Slider timeBar;         
    
    public bool player1Ready = false;
    public bool player2Ready = false;

    //Level votes
    public string player1LevelVote = "";
    public string player2LevelVote = "";

    //Scrolling
    private RectTransform[] guiElements;
    private float scrollSpeed = 200f;

    //Holding button
    private bool isButtonHeld = false;
    private float timer = 0.0f;
    private float timeToHold = 1.0f;


    //Lifecycle methods
    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;


        gameInstanceRef = game;
        initialized = true;

        SetupReferences();
        SetUpGUIElements();
    }

    public override void Tick()
    {
        if (!initialized)
        {
            Error("");
            return;
        }

        ButtonTimer();
    }

    
    public void SetupMenuStartingState()
    {
        player1Ready = false;
        player2Ready = false;
    }

    public void SetupReferences()
    {
        //Time bar
        var timerBarTransform = transform.Find("TimerBar");
        Validate(timerBarTransform, "Failed to find TimerBar transform", ValidationLevel.ERROR, true);
        timeBar = timerBarTransform.GetComponent<Slider>();
        Validate(timeBar, "Failed to find TimerBar component", ValidationLevel.ERROR, true);
        timeBar.gameObject.SetActive(false);
    }
    
    private void SetUpGUIElements()
    {
        foreach (var level in levelsBundle.Entries)
        {
            var levelUITransform = Instantiate(levelPrefab, transform).transform;

            //Set name and preview
            var levelNameTransform = levelUITransform.Find("LevelName");
            Validate(levelNameTransform, "Failed to find LevelName transform", ValidationLevel.ERROR, true);
            var levelNameText = levelNameTransform.GetComponent<TextMeshProUGUI>();
            Validate(levelNameText, "Failed to find LevelName component", ValidationLevel.ERROR, true);
            levelNameText.text = level.name;

            var levelPreviewTransform = levelUITransform.Find("LevelPreview");
            Validate(levelPreviewTransform, "Failed to find LevelPreview transform", ValidationLevel.ERROR, true);
            var levelPreviewImage = levelPreviewTransform.GetComponent<Image>();
            Validate(levelPreviewImage, "Failed to find LevelPreview component", ValidationLevel.ERROR, true);
            levelPreviewImage.sprite = level.preview;

            //Set up button listeners
            var voteButtonTransform = levelUITransform.Find("VoteButton");
            Validate(voteButtonTransform, "Failed to find VoteButton transform", ValidationLevel.ERROR, true);
            var eventTrigger = voteButtonTransform.GetComponent<EventTrigger>();
            Validate(eventTrigger, "Failed to find EventTrigger component", ValidationLevel.ERROR, true);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(pointerDownEntry);

            var pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUpEntry.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data, level.key); });
            eventTrigger.triggers.Add(pointerUpEntry);
        }
    }

    private void UpdateTimerBar()
    {
        if (!isButtonHeld)
            return;
        timeBar.value = timer / timeToHold;
    }

    private void ButtonTimer()
    {
        if (!isButtonHeld)
            return;

        timer += Time.deltaTime;

        if (!(timer >= timeToHold))
            return;

        timer = 0.0f;
        isButtonHeld = false;
    }

    private void OnPointerUpDelegate(PointerEventData arg0, string levelKey)
    {
        isButtonHeld = false;
        timer = 0.0f;
        timeBar.gameObject.SetActive(false);
    }

    private void OnPointerDownDelegate(PointerEventData arg0)
    {
        isButtonHeld = true;
        timeBar.gameObject.SetActive(true);
    }


    private void ProcessTouchInput()
    {
        var touchDelta = Pointer.current.delta.ReadValue();
        if (guiElements[^1].anchoredPosition.x <= 0 && touchDelta.x < 0)
            return;
        if (guiElements[0].anchoredPosition.x >= 0 && touchDelta.x > 0)
            return;
        var delta = touchDelta.x * Time.deltaTime * scrollSpeed;
        Scroll(delta);
    }

    private void Scroll(float delta)
    {
        foreach (var child in guiElements)
        {
            child.anchoredPosition += new Vector2(delta, 0);
        }
    }


    public void DebugLevelButton()
    {
        gameInstanceRef.StartGame("DebugLevel");
    }
}