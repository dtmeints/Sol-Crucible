using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Controls;

[CreateAssetMenu(fileName = "Input Reader", menuName = "Data/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    Controls controls;
    public Vector2 mousePosition;

    public Vector2 clickPosition;
    public Vector2 releasePosition;
    public bool isHeld;

    public event Action<Vector2> OnClick;
    public event Action<Vector2> OnRelease;
    public event Action OnPause;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
    }

    void IPlayerActions.OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isHeld = true;
            OnClick?.Invoke(mousePosition);
            clickPosition = mousePosition;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isHeld = false;
            releasePosition = mousePosition;
            OnRelease?.Invoke(mousePosition);
        }
    }

    void IPlayerActions.OnMousePosition(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void IPlayerActions.OnRestart(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            GameManager.Instance.StartLevelInSequence(GameManager.Instance.CurrentLevelIndex);
        }
    }

    void IPlayerActions.OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            OnPause?.Invoke();
        }
    }

    void IPlayerActions.OnDrag(InputAction.CallbackContext context)
    {
        
    }
}
