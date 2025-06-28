using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newElementLibrary", menuName = "Data/Element Library")]
public class D_ElementLibrary : ScriptableObject
{
    [SerializeField] private D_Element[] elements;
    public D_Element[] Elements => elements;

    public Color GetColor(Element element)
    {
        foreach (var e in elements)
        {
            if (e.element == element)
                return e.color;
        }
        return Color.white;
    }

    public Sprite GetSymbol(Element element)
    {
        foreach (var e in elements)
        {
            if (e.element == element)
                return e.symbol;
        }
        return elements[0].symbol;
    }
}
