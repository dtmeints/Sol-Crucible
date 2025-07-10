using UnityEngine;

public class Warning_Display : MonoBehaviour
{
    public SpriteRenderer Sr;

    public Color WarningColor;

    private Color targetColor;

    void FixedUpdate()
    {
        Sr.color = Color.Lerp(Sr.color, targetColor, 0.1f);
    }

    public void Show()
    {
        targetColor = WarningColor;
    }

    public void Hide()
    {
        targetColor = new Color(Sr.color.r, Sr.color.g, Sr.color.b, 0f);
    }
}
