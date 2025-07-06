using Sirenix.OdinInspector;
using UnityEngine;

public class GameSettings : Singleton<GameSettings>
{
    [field: SerializeField, HideLabel, InlineEditor] public GameSettingsData Data { get; private set; } = null;
}