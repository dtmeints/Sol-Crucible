using UnityEngine;
using UnityEngine.UI;

public class Meter_Display : MonoBehaviour
{
    public float Amount;

    public GameObject Meter;

    [Header("Colors")]
    public Color StartColor;
    public Color EndColor;

    //full = width: 113.64, x: 841.6
    //empty = width: 0, x: 784.78

    Vector3 fullPos = new Vector3(841.6f, 270.8f, 0);
    Vector2 fullSize = new Vector3(113.64f, 19.62f);

    Vector3 emptyPos = new Vector3(784.78f, 270.8f, 0);
    Vector2 emptySize = new Vector3(0f, 19.62f);

    void Update()
    {
        Meter.GetComponent<RectTransform>().localPosition = Vector3.Lerp(emptyPos, fullPos, Amount);
        Meter.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(emptySize, fullSize, Amount);
        Meter.GetComponent<Image>().color = Color.Lerp(StartColor, EndColor, Amount);
    }
}
