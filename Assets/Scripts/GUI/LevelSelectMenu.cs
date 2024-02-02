using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MyUtility.Utility;

public class LevelSelectMenu : Entity {
    //SerializeFields
    [SerializeField] private LevelsBundle levelsBundle;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private float xCenterOffset = 0.0f;
    [SerializeField] private Vector2 defaultGuiElementSize = new Vector2(200, 400);

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
    private float canvasCenterX = 0.0f;
    private float guiElementWidth = 0.0f;
    private float spacing = 0.0f;
    private bool isSnapped = false;

    //Holding button
    private bool isButtonHeld = false;
    private float timer = 0.0f;
    private float timeToHold = 1.0f;

    private int middleElementIndex = 0;

    //I am using these for testing, cuz the Initialize method is not being called
    //private void Start() {
    //    SetupReferences();
    //    SetupGUIElements();
    //}

    //private void Update() {
    //    ButtonTimer();
    //    UpdateTimerBar();
    //    StopScrollAtEdges();
    //    var currentItem = CalculateCurrentItem();
    //    middleElementIndex = currentItem;
    //    Snap();
    //    UpdateLevelName();
    //}

    public override void Initialize(GameInstance game) {
        if (initialized)
            return;
        gameInstanceRef = game;
        initialized = true;
        SetupReferences();
        SetupGUIElements();
    }


    //public override void Tick()
    //{
    //    if (!initialized)
    //    {
    //        Error("");
    //        return;
    //    }
    //    ButtonTimer();
    //    UpdateTimerBar();
    //    StopScrollAtEdges();
    //    var currentItem = CalculateCurrentItem();
    //    middleElementIndex = currentItem;
    //    Snap();
    //    UpdateLevelName();
    //}


    public void SetupMenuStartingState() {
        player1Ready = false;
        player2Ready = false;
    }

    private void SetupReferences() {
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

    private void SetupGUIElements() {
        guiElements = new RectTransform[levelsBundle.Entries.Length];
        var index = 0;
        foreach (var level in levelsBundle.Entries) {
            var levelUIObject = new GameObject();

            var levelUIRectTransform = levelUIObject.AddComponent<RectTransform>();
            levelUIRectTransform.SetParent(contentRectTransform);
            levelUIRectTransform.gameObject.name =
                "Button" + index; // Just for testing since all have the same name //level.name;
            levelUIRectTransform.sizeDelta = defaultGuiElementSize;
            guiElements[index] = levelUIRectTransform;
            index++;

            var levelUIButton = levelUIObject.AddComponent<Button>();
            var levelUIImage = levelUIObject.AddComponent<Image>();
            levelUIImage.sprite = level.preview;
        }

        guiElementWidth = guiElements[0].sizeDelta.x;
        spacing = contentRectTransform.GetComponent<HorizontalLayoutGroup>().spacing;
        contentRectTransform.GetComponent<ContentSizeFitter>().enabled = false;
    }

    //Button holding
    private void ButtonTimer() {
        if (!isButtonHeld)
            return;

        timer += Time.deltaTime;

        if (!(timer >= timeToHold))
            return;

        TimerOver();
        timer = 0.0f;
        isButtonHeld = false;
    }

    private void TimerOver() {
        FinalizeVote();
    }

    private void FinalizeVote() {
        var levelData = levelsBundle.Entries[middleElementIndex];
        var levelKey = levelData.key;
        Log(levelKey);
        //If not selectable return
    }

    public void OnPointerExit() {
        isButtonHeld = false;
        timer = 0.0f;
        timeBar.gameObject.SetActive(false);
    }

    public void OnPointerEnter() {
        isButtonHeld = true;
        timeBar.gameObject.SetActive(true);
    }

    //Scrollling
    private void StopScrollAtEdges() {
        if (guiElements[0].position.x >= canvasCenterX)
            scrollRect.velocity = Vector2.zero;
        if (guiElements[^1].position.x <= canvasCenterX)
            scrollRect.velocity = Vector2.zero;
    }

    private int CalculateCurrentItem() {
        var currentItem = Mathf.RoundToInt(-(contentRectTransform.localPosition.x / (guiElementWidth + spacing)));
        if (currentItem < 0)
            currentItem = 0;
        if (currentItem > guiElements.Length - 1)
            currentItem = guiElements.Length - 1;
        return currentItem;
    }

    private void Snap() {
        if (!isSnapped && scrollRect.velocity.magnitude < 30f) {
            var localPosition = contentRectTransform.localPosition;
            var x = middleElementIndex * (guiElementWidth + spacing) + xCenterOffset;
            var updatedPosition = new Vector3(-x, localPosition.y, localPosition.z);

            localPosition = Vector3.Lerp(localPosition, updatedPosition, 10f * Time.deltaTime);
            contentRectTransform.localPosition = localPosition;

            isSnapped = Math.Abs(localPosition.x - -x) < 0.1f;
        }
        else {
            isSnapped = false;
        }
    }

    //
    private void UpdateTimerBar() {
        if (!isButtonHeld)
            return;
        timeBar.value = timer / timeToHold;
    }

    private void UpdateLevelName() {
        levelNameText.text = guiElements[middleElementIndex].name;
    }

    public void DebugLevelButton() {
        gameInstanceRef.StartGame("DebugLevel");
    }
}