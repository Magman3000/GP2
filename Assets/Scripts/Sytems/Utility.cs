using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;


namespace MyUtility { 
    public class Utility {

        public enum ValidationLevel {
            DEBUG,
            WARNING,
            ERROR
        }

        public static void Log(object mesage) {
            Debug.Log("[" + Time.frameCount + "]\t" + mesage);
        }
        public static void Warning(object mesage) {
            Debug.LogWarning("[" + Time.frameCount + "]\t" + mesage);
        }
        public static void Error(object mesage) {
            Debug.LogError("[" + Time.frameCount + "]\t" + mesage);
        }

        public static bool Validate(object target, string message, ValidationLevel level = ValidationLevel.DEBUG, bool abortOnFail = false) {
            if (target != null)
                return true;

            if (level == ValidationLevel.DEBUG)
                Log(message);
            else if (level == ValidationLevel.WARNING)
                Warning(message);
            else if (level == ValidationLevel.ERROR)
                Error(message);

            if (abortOnFail)
                GameInstance.AbortApplication();

            return false;
        }

        public static float LinearConversion(float value, float oldMin, float oldMax, float newMin, float newMax) {

            float oldRange = oldMax - oldMin;
            float newRange = newMax - newMin;
            return (((value -  oldMin) * newRange) / oldRange) + newMin;
        }

    }
}
