using UnityEngine;

public class Input_Handler : MonoBehaviour
{
    public Vector2 MousePosition;
    public Vector2 ClickPosition;
    public Vector2 ReleasePosition;
    public bool IsHeld;

    void Update()
    {
        MousePosition = Input.mousePosition;

        IsHeld = Input.GetMouseButton(0);

        if (Input.GetMouseButtonDown(0))
        {
            ClickPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleasePosition = Input.mousePosition;
        }
    }
}
