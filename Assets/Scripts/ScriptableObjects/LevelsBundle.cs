using System;
using UnityEngine;
using UnityEngine.AddressableAssets;



[Serializable]
public struct LevelEntry {
    public string key;
    public AssetReference asset;
}

[CreateAssetMenu(fileName = "LevelsBundle", menuName = "Level/LevelsBundle", order = 0)]
public class LevelsBundle : ScriptableObject {

    public LevelEntry[] Entries;
}
