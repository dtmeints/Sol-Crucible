using UnityEngine;

public enum Element
{
    Air, Fire, Water, Earth
}

public static class ElementExtensions
{
    public static Color Color(this Element element)
    {
        return element switch {
            Element.Air => new Color(0.6745098f, 1, 0.6509804f, 1),
            Element.Fire => new Color(1, 0.05709351f, 0, 1),
            Element.Water => new Color(0.5647059f, 0.945098f, 0.9921569f, 1),
            Element.Earth => new Color(1, 0.7882353f, 0.1098039f, 1),
            _ => UnityEngine.Color.white
        } ;
    }

    public static Element Random(this Element element)
    {
        Element[] allElements = (Element[])System.Enum.GetValues(typeof(Element));

        return allElements[UnityEngine.Random.Range(0, allElements.Length)];    
    }
}