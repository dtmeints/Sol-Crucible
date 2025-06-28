using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    public UnityEngine.Events.UnityEvent OnClicked;
    void IClickable.ActivateClickEffect()
    {
        OnClicked?.Invoke();
    }

    void IClickable.ActivateMouseOverEffect()
    {
        return;
    }
}
