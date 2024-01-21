using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MyUtility { 
    public class Utility {

        public static void Log(object mesage) {
            Debug.Log(mesage);
        }
        public static void Warning(object mesage) {
            Debug.LogWarning(mesage);
        }
        public static void Error(object mesage) {
            Debug.LogError(mesage);
        }


        public static void AbortApplication(object message) {

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            Error(message);
#else
    Application.Quit();

#endif
        }
    }
}
