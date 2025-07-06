using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class RightMenuUI : MonoBehaviour
{
    [SerializeField] ItemPlacer itemPlacer;
    [SerializeField] RectTransform panel;
    [SerializeField] RectTransform buttonArrow;
    [ShowInInspector, ReadOnly] bool open;
    const float kToggleDuration = 0.15f;
    
    #if UNITY_EDITOR
    [Button("Toggle")]
    void EditorToggle() {
        panel.anchoredPosition = new Vector2(Mathf.Approximately(panel.anchoredPosition.x, 0) ? panel.rect.width : 0, 0);
        open = Mathf.Approximately(panel.anchoredPosition.x, 0);
        buttonArrow.rotation = Quaternion.Euler(0, 0, open ? -90 : 90);
    }
    #endif
    
    void Start() {
        panel.anchoredPosition = new Vector2(0, 0);
        open = true;
        buttonArrow.rotation = Quaternion.Euler(0, 0, -90);
    }
    public void ToggleMenu() {
        panel.DOAnchorPosX(open ? panel.rect.width : 0, kToggleDuration).Play();
        open = !open;
        buttonArrow.rotation = Quaternion.Euler(0, 0, open ? -90 : 90);
        
        if (open)
            itemPlacer.Clear();
    }
}
