using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameSettings", menuName = "Data/Settings/Game")]
public class GameSettingsData : ScriptableObject
{
    public static readonly string kSettingsPath = "Assets/Data/Settings/GameSettings.asset";
    #if UNITY_EDITOR
    public static GameSettingsData LoadDefaultSettings() => AssetDatabase.LoadAssetAtPath<GameSettingsData>(kSettingsPath);
    #endif
    
    [field: SerializeField, InlineEditor] public CameraSettingsData camera { get; private set; }
    [field: SerializeField, FoldoutGroup("World & Camera")] public float worldSize { get; private set; } = 1000;
    [field: SerializeField, FoldoutGroup("World & Camera")] public float starBackgroundShaderZ { get; private set; } = 1000;
    [field: SerializeField, FoldoutGroup("First Sol")] public float FirstSolMaxDistance { get; private set; } = 5;
    [field: SerializeField, FoldoutGroup("First Sol")] public bool FirstSolMaxDistanceDebug { get; private set; } = true;
}