using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] int limit;
    public void Awake()
    {
        SetAllChildLimits(limit);
    }

    public void SetAllChildLimits(int limit)
    {
        ElementAbsorber[] childAbsorbers = GetComponentsInChildren<ElementAbsorber>();
        foreach (ElementAbsorber absorber in childAbsorbers)
        {
            absorber.SetRankLimit(limit);
        }
    }
}
