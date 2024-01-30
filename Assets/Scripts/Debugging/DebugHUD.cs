using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour {


    private TextMeshProUGUI fpsText;


    void Start() {
        SetupReferences();
    }
    private void SetupReferences() {
        fpsText = transform.Find("FPS").GetComponent<TextMeshProUGUI>();
    }
    void Update() {

        if (fpsText)
            fpsText.text = "FPS " + (int)(1.0f / Time.unscaledDeltaTime);

    }
}
