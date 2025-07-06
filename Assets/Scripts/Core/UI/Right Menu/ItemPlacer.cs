using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ItemPlacer : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public static bool FirstSol = true;
    
    public static event Action<Transform> OnFirstSol = delegate { };
    
    [SerializeField, LabelWidth(90)] RightMenuUI rightMenuUI;
    [SerializeField, LabelWidth(90)] Image previewImage;
    [SerializeField, ReadOnly, HideLabel, Title("Selected Item"), InlineEditor] 
    ItemData selectedItemData;
    
    Item selectedItem;
    Camera mainCamera;
    Color validPlacementColor;
    Color invalidPlacementColor;

    void Awake() => mainCamera = Camera.main;

    void Start() {
        FirstSol = true;
        validPlacementColor = previewImage.color;
        validPlacementColor.a = 0.5f;
        invalidPlacementColor = new(1, 0.3f, 0.3f, 0.5f);
    }
    
    public void SelectItem(Item info) {
        selectedItem = info;
        selectedItemData = info.Data;
        previewImage.sprite = selectedItemData.Image;
        previewImage.color = validPlacementColor;
        Cursor.visible = false;
        rightMenuUI.ToggleMenu(); 
    }

    void Update() {
        if (!selectedItem) return;

        previewImage.rectTransform.position = Input.mousePosition;
        bool canPlaceHere = selectedItem.CanPlaceHere();
        previewImage.color = canPlaceHere ? validPlacementColor : invalidPlacementColor;

        if (Input.GetMouseButtonDown(0) && canPlaceHere) {
            Transform newSol = selectedItem.Place();
            if (FirstSol) {
                OnFirstSol.Invoke(newSol);
                FirstSol = false;
            }
            Clear();
        }
        else if (Input.GetMouseButtonDown(1))
            Clear();
    }

    public void Clear() {
        selectedItem = null;
        selectedItemData = null;
        previewImage.color = Color.clear;
        Cursor.visible = true;
    }

    void OnDrawGizmos() {
        if (GameSettings.Instance.Data.FirstSolMaxDistanceDebug && FirstSol) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Vector2.zero, GameSettings.Instance.Data.FirstSolMaxDistance);
        }
    }
}