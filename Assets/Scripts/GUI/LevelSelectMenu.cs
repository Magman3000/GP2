using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MyUtility.Utility;

public class LevelSelectMenu : Entity
{
    //SerializeFields
    [SerializeField] private LevelsBundle levelsBundle;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private TextMeshProUGUI levelNameText;


    //Ref
    private Slider timeBar;

    //Ready checks
    public bool player1Ready = false;
    public bool player2Ready = false;

    //Level votes
    public string player1LevelVote = "";
    public string player2LevelVote = "";

    //Scrolling
    private RectTransform canvasRectTransform;
    private RectTransform[] guiElements;
    private float scrollSpeed = 200f;
    private float canvasCenterX = 0.0f;
    private float guiElementWidth = 0.0f;
    private float spacing = 0.0f;
    private bool isSnapped = false;

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
        SetupGUIElements();
    }

    private void Update()
    {
        if (!initialized)
        {
            Error("");
            return;
        }

        ButtonTimer();
        UpdateTimerBar();
        StopScrollAtEdges();
        var currentItem = CalculateCurrentItem();
        Snap(currentItem);
        UpdateLevelName(currentItem);
    }


    /*public override void Tick()
    {
        if (!initialized)
        {
            Error("");
            return;
        }

        ButtonTimer();
        UpdateTimerBar();
        StopScrollAtEdges();
        var currentItem = CalculateCurrentItem();
        Snap(currentItem);
        UpdateLevelName(currentItem);
    }*/


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

        //Canvas
        canvasRectTransform = GetComponent<RectTransform>();
        Validate(canvasRectTransform, "Failed to find Canvas RectTransform", ValidationLevel.ERROR, true);
        canvasCenterX = canvasRectTransform.rect.width / 2f;
    }

    private void SetupGUIElements()
    {
        guiElements = new RectTransform[levelsBundle.Entries.Length];
        var index = 0;
        foreach (var level in levelsBundle.Entries)
        {
            var levelUIObject = new GameObject();

            var levelUIRectTransform = levelUIObject.AddComponent<RectTransform>();
            levelUIRectTransform.SetParent(contentRectTransform);
            levelUIRectTransform.anchoredPosition = new Vector2(300 * index, 0);
            levelUIRectTransform.sizeDelta = new Vector2(200, 400);
            guiElements[index] = levelUIRectTransform;
            index++;

            var levelUIButton = levelUIObject.AddComponent<Button>();
            var levelUIImage = levelUIObject.AddComponent<Image>();
            levelUIImage.sprite = level.preview;
        }

        guiElementWidth = guiElements[0].sizeDelta.x;
        spacing = contentRectTransform.GetComponent<HorizontalLayoutGroup>().spacing;
    }

    //Button holding
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

    public void OnPointerUpDelegate()
    {
        isButtonHeld = false;
        timer = 0.0f;
        timeBar.gameObject.SetActive(false);
    }

    public void OnPointerDownDelegate()
    {
        isButtonHeld = true;
        timeBar.gameObject.SetActive(true);
    }

    //Scrollling
    private void StopScrollAtEdges()
    {
        if (guiElements[0].position.x >= canvasCenterX)
            scrollRect.velocity = Vector2.zero;
        if (guiElements[^1].position.x <= canvasCenterX)
            scrollRect.velocity = Vector2.zero;
    }

    private int CalculateCurrentItem()
    {
        var currentItem = Mathf.RoundToInt(-(contentRectTransform.localPosition.x / (guiElementWidth + spacing)));
        if (currentItem < 0)
            currentItem = 0;
        if (currentItem > guiElements.Length - 1)
            currentItem = guiElements.Length - 1;
        return currentItem;
    }

    private void Snap(int currentItem)
    {
        if (scrollRect.velocity.magnitude < 30f && !isSnapped)
        {
            var localPosition = contentRectTransform.localPosition;
            var x = currentItem * (guiElementWidth + spacing) + 50;
            var updatedPosition = new Vector3(-x, localPosition.y, localPosition.z);

            localPosition = Vector3.Lerp(localPosition, updatedPosition, 10f * Time.deltaTime);
            contentRectTransform.localPosition = localPosition;

            if (Math.Abs(contentRectTransform.localPosition.x - (-x)) < 0.1f)
                isSnapped = true;
        }
        else
        {
            isSnapped = false;
        }
    }

    //
    private void UpdateTimerBar()
    {
        if (!isButtonHeld)
            return;
        timeBar.value = timer / timeToHold;
    }

    private void UpdateLevelName(int currentItem)
    {
        levelNameText.text = guiElements[currentItem].name;
    }

    public void DebugLevelButton()
    {
        gameInstanceRef.StartGame("DebugLevel");
    }
}