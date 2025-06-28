using UnityEngine;

[CreateAssetMenu(fileName = "newElement", menuName = "Data/Element")]
public class D_Element : ScriptableObject
{
    public Element element;
    public Color color;
    public Sprite symbol;
}
