using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class Vortex : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] D_Element element;
    [SerializeField] RequirementReadout readout;
    [SerializeField] SpriteRenderer symbol;
    [SerializeField] TextMeshPro nameReadout;
    [SerializeField] ElementAbsorber elementAbsorber;
    [SerializeField] LineRenderer chain;
    Crucible crucible;

    [Header("Settings")]
    [SerializeField] float chainPulseDuration;
    Color startColor;
    Color flashColor;
    Color goneColor;
    MaterialPropertyBlock mpb;

    [Header("In-Game")]
    [SerializeField] bool isComplete;

    private void Awake()
    {
        if(GameManager.Instance != null)readout.element = element.element;
        symbol.sprite = element.symbol;
        nameReadout.text = element.element.ToString();
        elementAbsorber.elements = new List<Element>() { element.element };
        crucible = FindAnyObjectByType<Crucible>();

        Vector3 start = Physics2D.ClosestPoint(crucible.transform.position, elementAbsorber.Col);
        Vector3 end = crucible.transform.position;
        Vector3 midpoint = (start + end) * .5f; 
        chain.SetPositions(new Vector3[] {start, midpoint, end});

        mpb = new();

        startColor = new Color(element.color.r, element.color.g, element.color.b, .2f);
        flashColor = element.color;
        goneColor = new Color(element.color.r, element.color.g, element.color.b, 0f);
        chain.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", goneColor);
        chain.SetPropertyBlock(mpb);
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;

        if (crucible.Requirements.GetRequiredCountByElement(this.element.element) <= 0)
        {
            chain.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", goneColor);
            chain.SetPropertyBlock(mpb);
        }
        else
        {
            chain.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", startColor);
            chain.SetPropertyBlock(mpb);
        }
    }

    public void EvaluateCompleted(Requirements requirements)
    {
        if (GameManager.Instance == null) return;
        if (requirements.CurrentHeldByElement[element.element] >= requirements.GetRequiredCountByElement(element.element))
            {
                if (!isComplete)
                {
                    isComplete = true;
                    StartCoroutine(Co_FadeChain(.5f));
                }
            }
    }
    private void StartFlash()
    {
        if (isComplete) return;

        StartCoroutine(Co_ColorPulse(startColor, flashColor, chainPulseDuration));
    }

    private IEnumerator Co_ColorPulse(Color startColor, Color endColor, float duration)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            chain.GetPropertyBlock(mpb);
            Color colorToSet = Color.Lerp(startColor, endColor, ((Time.time - startTime) / duration).ToExponentialPulse());
            mpb.SetColor("_Color", colorToSet);
            chain.SetPropertyBlock(mpb);
            yield return null;
        }
    }

    private IEnumerator Co_FadeChain(float duration)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            chain.GetPropertyBlock(mpb);
            Color colorToSet = Color.Lerp(startColor, goneColor, ((Time.time - startTime) / duration));
            mpb.SetColor("_Color", colorToSet);
            chain.SetPropertyBlock(mpb);
            yield return null;
        }
        mpb.SetColor("_Color", goneColor);
        chain.SetPropertyBlock(mpb);
    }

    private void OnEnable()
    {
        elementAbsorber.OnFedCorrectElement += StartFlash;
        Crucible.OnUpdateReadouts += EvaluateCompleted;
    }

    private void OnDisable()
    {
        elementAbsorber.OnFedCorrectElement -= StartFlash;
        Crucible.OnUpdateReadouts -= EvaluateCompleted;
    }


    
}

public static class MathHelpers
{
    public static float ToExponentialPulse(this float value)
    {
        return -4 * Mathf.Pow(value - .5f, 2) + 1;
    }
}
