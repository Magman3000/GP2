using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static MyUtility.Utility;

[Serializable]
public struct LevelOption {
    public int index;
    public Image progressBar;
    public Image previewImage;
    public Button button;
    public string name;
    public string levelKey;
}


public class LevelSelectMenu : Entity {
    
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI levelNameText;

    //Ref
    public LevelOption[] levelOptions;

    private LevelsBundle levelsBundle;
    private ScrollRect scrollRectComp;
    private HorizontalLayoutGroup layoutGroupComp;

    private Slider timeBar;

    //Ready checks
    public bool player1Ready = false;
    public bool player2Ready = false;

    //Level votes
    public string player1LevelVote = "";
    public string player2LevelVote = "";

    //Holding button
    private bool isButtonHeld = false;
    private float timer = 0.0f;
    private float timeToHold = 1.0f;

    private int selectedElementIndex = 0;


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        gameInstanceRef = game;
        SetupReferences();

        initialized = true;
    }

    public override void Tick() {
        if (!initialized) {
            Error("");
            return;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            scrollRectComp.content.localPosition = Vector2.zero;
        }

        ButtonTimer();
        UpdateTimerBar(selectedElementIndex);
    }

    public void SetupMenuStartingState() {
        player1Ready = false;
        player2Ready = false;
        //Menu gets reconstructed on each opening of the menu
        levelsBundle = gameInstanceRef.GetLevelManagement().GetLevelsBundle();
        SetupGUIElements();
    }

    private void SetupReferences() {
        Transform scrollViewTransform = transform.Find("ScrollView");
        scrollRectComp = scrollViewTransform.GetComponent<ScrollRect>();
        layoutGroupComp = scrollRectComp.content.gameObject.GetComponent<HorizontalLayoutGroup>();
    }

    private void SetupGUIElements() {
        if (!levelsBundle) {
            Warning("Invalid levels bundle!\nUnable to construct GUI elements.");
            return;
        }

        levelOptions = new LevelOption[levelsBundle.Entries.Length];
        for (int i = 0; i < levelsBundle.Entries.Length + 1; i++) {
            GameObject newOption = Instantiate(buttonPrefab, scrollRectComp.content);
            Transform levelButton = newOption.transform.Find("LevelButton");
            if (i == levelsBundle.Entries.Length) {
                newOption.GetComponent<Image>().enabled = false;
                levelButton.GetComponent<Image>().enabled = false;
                newOption.name = "EmptyOption";
                continue;
            }


            LevelOption levelOption = new LevelOption();
            levelOption.progressBar = newOption.GetComponent<Image>();
            levelOption.previewImage = levelButton.GetComponent<Image>();
            levelOption.button = levelButton.GetComponent<Button>();
            levelOption.name = levelsBundle.Entries[i].name;
            levelOption.levelKey = levelsBundle.Entries[i].key;

            levelOption.progressBar.fillAmount = 0.0f;
            levelOption.previewImage.sprite = levelsBundle.Entries[i].preview;
            levelOption.index = i;

            EventTrigger eventTrigger = levelButton.GetComponent<EventTrigger>();

            EventTrigger.Entry OnPointerEnter = new EventTrigger.Entry();
            OnPointerEnter.eventID = EventTriggerType.PointerDown;
            OnPointerEnter.callback.AddListener((eventData) => { OnStartClicking(levelOption.index); });
            
            EventTrigger.Entry OnPointerExit = new EventTrigger.Entry();
            OnPointerExit.eventID = EventTriggerType.PointerUp;
            OnPointerExit.callback.AddListener((eventData) => { OnEndClicking(levelOption.index); });

            eventTrigger.triggers.Add(OnPointerEnter);
            eventTrigger.triggers.Add(OnPointerExit);

            levelOptions[i] = levelOption;
        }
    }

    //Button holding
    private void ButtonTimer() {
        if (!isButtonHeld)
            return;

        timer += Time.deltaTime;

        if (timer >= timeToHold) {

            TimerOver();
            levelOptions[selectedElementIndex].progressBar.fillAmount = 1.0f;
            levelNameText.text = levelOptions[selectedElementIndex].name;
            timer = 0.0f;
            isButtonHeld = false;
        }
    }

    private void TimerOver() {
        FinalizeVote();
    }

    private void FinalizeVote() {
        LevelOption levelData = levelOptions[selectedElementIndex];
        string levelKey = levelData.levelKey;
        //Log(levelKey);
    }

    private void UpdateTimerBar(int index) {
        if (!isButtonHeld)
            return;
        levelOptions[index].progressBar.fillAmount = timer / timeToHold;
    }

    public void DebugLevelButton() {
        gameInstanceRef.StartGame("DebugLevel");
    }


    public void OnStartClicking(int index) {
        LevelOption option = levelOptions[index];

        float middleIndex = (float)levelOptions.Length / 2;
        float indexDiff = middleIndex - index - .5f;

        float spacingTotal = layoutGroupComp.spacing * indexDiff + 1;
        float totalWidth = option.progressBar.rectTransform.rect.width * indexDiff + 1;

        Vector3 scrollRectPosition = scrollRectComp.content.localPosition;
        Vector3 resultRect = new Vector3(spacingTotal + totalWidth, scrollRectPosition.y, scrollRectPosition.z);
        scrollRectComp.content.localPosition = resultRect;
        scrollRectComp.velocity = Vector2.zero;
        
        selectedElementIndex = index;
        isButtonHeld = true;
    }
    
    public void OnEndClicking(int index) {
        levelOptions[index].progressBar.fillAmount = 0.0f;
        isButtonHeld = false;
    }
}