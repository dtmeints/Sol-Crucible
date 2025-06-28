using System;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] InputReader input;
    [SerializeField] float maxMagnitude = 3f;
    [SerializeField] float slowDownScale = .1f;
    [SerializeField] DirectionIndicator directionIndicator;


    public IClickable currentClickable;
    public IClickable hoveredClickable;
    public Orb hoveredBall;
    public Orb currentOrb;
    public int volitions;

    public static event Action<Element> OnHover;

    UIOptionsMenu optionsMenu;

    private void Update()
    {
        if (input.isHeld && currentOrb != null)
        {
            directionIndicator.gameObject.SetActive(true);
            directionIndicator.UpdateLine(currentOrb.transform.position, (Vector3)CurrentPushVector(input.mousePosition));
            Time.timeScale = slowDownScale;
        }
        else
        {
            Time.timeScale = 1;
            directionIndicator.gameObject.SetActive(false);
        }
        DoHoverEffect();
        
    }
    private void OnEnable()
    {
        input.OnClick += Click;
        input.OnRelease += Release;
        input.OnPause += TogglePause;

        optionsMenu = FindFirstObjectByType<UIOptionsMenu>(FindObjectsInactive.Include);
    }

    private void OnDisable()
    {
        input.OnClick -= Click;
        input.OnRelease -= Release;
        input.OnPause -= TogglePause;
    }

    private void Click(Vector2 mousePos)
    {
        IClickable clickable = GetClickableAtPoint(mousePos);

        if (clickable != null)
        {
            currentClickable = clickable;
            if (clickable is Orb orb)
            {
                currentOrb = orb;
                return;
            }
        }
        
        currentOrb = null;
    }

    private void Release(Vector2 mousePos)
    {
        TryFlingCurrentOrb(mousePos);
        ActivateClickedEffects(mousePos);
        currentOrb = null;
        currentClickable = null;
    }

    private void DoHoverEffect()
    {
        IClickable lastHoveredClickable = hoveredClickable;

        hoveredClickable = GetClickableAtPoint(input.mousePosition);

        if (hoveredClickable != null && hoveredClickable is Orb orb)
            OnHover?.Invoke(orb.Element);

        if (hoveredClickable != lastHoveredClickable && hoveredClickable != null)
            hoveredClickable.ActivateMouseOverEffect();
    }

    private Vector2 CurrentPushVector(Vector2 mousePos)
    {
        var currentMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -10));
        var clickedMousePos = currentOrb.transform.position;
        Vector2 pushVector = -(currentMousePos - clickedMousePos);
        pushVector = Vector2.ClampMagnitude(pushVector, maxMagnitude) * 5;
        return pushVector;
    }

    private void TryFlingCurrentOrb(Vector2 mousePos)
    {
        if (currentOrb != null)
        {
            currentOrb.Push(CurrentPushVector(mousePos));
            volitions += 1;
            GameManager.Instance.Stats.volitionsCast = volitions;
        }
    }

    private void ActivateClickedEffects(Vector2 mousePos)
    {
        IClickable clickable = GetClickableAtPoint(mousePos);
        if (clickable != null && clickable == currentClickable)
        {
            clickable.ActivateClickEffect();
        }
    }

    private IClickable GetClickableAtPoint(Vector2 mousePos)
    {
        var hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -10)), Vector3.forward, 10f);
        if (hit != null)
        {
            foreach (var hitObj in hit)
            {
                if (hitObj.collider.gameObject.TryGetComponent<IClickable>(out var clickable))
                {
                    return clickable;
                }
            }
        }
        return null;
    }

    private void TogglePause()
    {
        optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeSelf);
    }
}
