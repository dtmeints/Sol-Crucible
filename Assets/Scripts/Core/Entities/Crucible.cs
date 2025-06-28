using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crucible : MonoBehaviour
{
    public Requirements Requirements { get; private set; }
    [SerializeField] SpriteRenderer SR;
    [SerializeField] Color startColor;
    [SerializeField] Color flashColor;
    [SerializeField] Color endColor;
    [SerializeField] Gradient gradient;
    [SerializeField] Attractor attractor;
 
    float visibleCompleteness;
    public static event Action<Element> OnElementSupplied;
    public static event Action<Requirements> OnUpdateReadouts;
 
    MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        visibleCompleteness = 0;
        SetRequirements(GameManager.Instance.CurrentLevel.Requirements);
    }

    private void Start()
    {
        OnUpdateReadouts?.Invoke(Requirements);
    }

    private void Update()
    {
        if (!GameManager.Instance.GameStarted)
            return;

        if (GameManager.Instance.GameStarted && GameManager.Instance.GameEnded)
        {
            SR.transform.localScale = Vector3.MoveTowards(SR.transform.localScale, Vector3.one * 12, .1f);
            SR.GetPropertyBlock(mpb);
            mpb.SetColor("_AddColor", Vector4.MoveTowards(mpb.GetColor("_AddColor"), endColor, .1f));
            SR.SetPropertyBlock(mpb);
            return;
        }

        UpdateCompletenessVisuals();
        
    }

    public void SetRequirements(Requirements requirements)
    {
        requirements.Initialize();
        if (requirements.Randomize || requirements == null)
        {
            int totalToRequire = requirements != null ? requirements.TotalToRequire : 60;
            requirements = new Requirements(true, totalToRequire);
        }

        this.Requirements = requirements;
        GameManager.Instance.Stats.requirements = this.Requirements;
        OnUpdateReadouts?.Invoke(Requirements);
    }

    public void SatisfyRequirement(Element element, int count)
    {
        Debug.Log("Satisfying " + element + " by " + count);
        Requirements.Satisfy(element, count);

        StartCoroutine(Co_Flash(1));
        OnUpdateReadouts?.Invoke(Requirements);
        OnElementSupplied?.Invoke(element);
        AudioManager.Instance.PlayPianoChord();
        CheckGameEnd();
    }

    private void UpdateCompletenessVisuals()
    {
        visibleCompleteness = Mathf.MoveTowards(visibleCompleteness, Requirements.Completeness, .01f);
        SR.GetPropertyBlock(mpb);
        mpb.SetColor("_AddColor", gradient.Evaluate(visibleCompleteness));
        SR.SetPropertyBlock(mpb);
        SR.transform.localScale = Vector3.one * Mathf.Lerp(1f, 4f, visibleCompleteness);

        attractor.SetPullStrength(visibleCompleteness);
    }

    private void CheckGameEnd()
    {
        if (Requirements.Completeness >= 1f && GameManager.Instance.GameStarted)
            GameManager.Instance.EndGame();
    }

    public IEnumerator Co_Flash(float intensity)
    {
        float startTime = Time.time;
        float duration = .2f;

        while (Time.time - startTime < duration)
        {
            SR.color = Color.Lerp(startColor, flashColor, (Time.time - startTime) / duration) * intensity;
            yield return null;
        }

        duration = .5f;
        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            SR.color = Color.Lerp(flashColor, startColor, (Time.time - startTime) / duration) * intensity;

            yield return null;
        }
        SR.color = startColor;
    }

}