﻿using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Data/Settings/Game")]
public class GameSettingsData : ScriptableObject
{
    public static readonly string kSettingsPath = "Data Objects/GameSettings.asset";
    public static GameSettingsData LoadDefaultSettings() => Resources.Load<GameSettingsData>(kSettingsPath);
    
    [field: SerializeField, InlineEditor] public CameraSettingsData camera { get; private set; }
    [field: SerializeField, FoldoutGroup("World & Camera")] public float worldSize { get; private set; } = 1000;
    [field: SerializeField, FoldoutGroup("World & Camera")] public float starBackgroundShaderZ { get; private set; } = 1000;
    [field: SerializeField, FoldoutGroup("First Sol")] public float FirstSolMaxDistance { get; private set; } = 5;
    [field: SerializeField, FoldoutGroup("First Sol")] public bool FirstSolMaxDistanceDebug { get; private set; } = true;
}