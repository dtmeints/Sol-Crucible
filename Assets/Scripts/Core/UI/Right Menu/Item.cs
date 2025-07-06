using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPlaceableItem
{
    [SerializeField, LabelWidth(70)] ItemPlacer itemPlacer;
    [field: SerializeField, InlineEditor, HideLabel] public ItemData Data { get; private set; }
    
    protected Vector3 mouseWorldPos => mainCamera.ScreenToWorldPoint(new(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(mainCamera.transform.position.z)));
    
    Camera mainCamera;
    Button button;
    
    protected virtual void Reset() => itemPlacer = transform.root.GetComponentInChildren<ItemPlacer>();
    protected virtual void Awake() {
        mainCamera = Camera.main;
        button = GetComponent<Button>();
        button.onClick.AddListener(() => itemPlacer.SelectItem(this));
    }

    public virtual bool CanPlaceHere() => true;

    public virtual Transform Place() {
        return null;
    }
}