using TMPro;
using UnityEngine;

public class RequirementReadout : MonoBehaviour
{
    public Element element;
    [SerializeField] TextMeshProUGUI textMeshUI;
    [SerializeField] TextMeshPro textMesh;


    private void Start()
    {

    }

    public void UpdateValues(Requirements requirements)
    {
        int required = requirements.GetRequiredCountByElement(this.element);
        int current = requirements.CurrentHeldByElement[this.element];
        if (textMeshUI != null)
            textMeshUI.text = current + " / " + required;

        if (textMesh != null)
            textMesh.text = current + " / " + required;
    }

    private void OnEnable()
    {
        Crucible.OnUpdateReadouts += UpdateValues;
    }

    private void OnDisable()
    {
        Crucible.OnUpdateReadouts -= UpdateValues;
    }
}
