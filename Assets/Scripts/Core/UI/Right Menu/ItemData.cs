using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData_", menuName = "Data/Item Data")]
public class ItemData : ScriptableObject
{
    [field: SerializeField, HorizontalGroup("a"), LabelText("Name & Price"), LabelWidth(80)] public string Name { get; private set; }        
    [field: SerializeField, HorizontalGroup("a"), HideLabel] public int Cost { get; private set; }
    
    [field: SerializeField, HorizontalGroup("b"), BoxGroup("b/Prefab"), PreviewField(ObjectFieldAlignment.Center, Height = 150), HideLabel] public GameObject Prefab { get; private set; }
    [field: SerializeField, BoxGroup("b/UI Image"), PreviewField(ObjectFieldAlignment.Center, Height = 150), HideLabel] public Sprite Image { get; private set; }        
}