using System;
using System.Collections;
using UnityEngine;

public class StarElement : MonoBehaviour
{
    static readonly int AddColor = Shader.PropertyToID("_AddColor");
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
    Camera mainCamera;

    void Awake() {
        mpb = new MaterialPropertyBlock();
        visibleCompleteness = 0;
        mainCamera = Camera.main;
        //SetRequirements(GameManager.Instance.CurrentLevel.Requirements);
    }
    
    void Start() => OnUpdateReadouts?.Invoke(Requirements);

    void Update() {

        
        if (Input.GetMouseButtonDown(0)) {
            SR.transform.localScale = Vector3.MoveTowards(SR.transform.localScale, Vector3.one * 12, .1f);
            SR.GetPropertyBlock(mpb);
            mpb.SetColor(AddColor, Vector4.MoveTowards(mpb.GetColor(AddColor), endColor, .1f));
            SR.SetPropertyBlock(mpb);
            return;
        }

        //UpdateCompletenessVisuals();
    }

    public void SetRequirements(Requirements requirements) {
        requirements.Initialize();
        if (requirements.Randomize || requirements == null) {
            int totalToRequire = requirements != null ? requirements.TotalToRequire : 60;
            requirements = new Requirements(true, totalToRequire);
        }

        Requirements = requirements;
        GameManager.Instance.Stats.requirements = Requirements;
        OnUpdateReadouts?.Invoke(Requirements);
    }

    public void SatisfyRequirement(Element element, int count) {
        Debug.Log("Satisfying " + element + " by " + count);
        Requirements.Satisfy(element, count);

        StartCoroutine(Co_Flash(1));
        OnUpdateReadouts?.Invoke(Requirements);
        OnElementSupplied?.Invoke(element);
        AudioManager.Instance.PlayPianoChord();
        CheckGameEnd();
    }

    void UpdateCompletenessVisuals() {
        visibleCompleteness = Mathf.MoveTowards(visibleCompleteness, Requirements.Completeness, .01f);
        SR.GetPropertyBlock(mpb);
        mpb.SetColor(AddColor, gradient.Evaluate(visibleCompleteness));
        SR.SetPropertyBlock(mpb);
        SR.transform.localScale = Vector3.one * Mathf.Lerp(1f, 4f, visibleCompleteness);

        attractor.SetPullStrength(visibleCompleteness);
    }

    private void CheckGameEnd() {
        if (Requirements.Completeness >= 1f && GameManager.Instance.GameStarted)
            GameManager.Instance.EndGame();
    }

    public IEnumerator Co_Flash(float intensity) {
        float startTime = Time.time;
        float duration = .2f;

        while (Time.time - startTime < duration) {
            SR.color = Color.Lerp(startColor, flashColor, (Time.time - startTime) / duration) * intensity;
            yield return null;
        }

        duration = .5f;
        startTime = Time.time;

        while (Time.time - startTime < duration) {
            SR.color = Color.Lerp(flashColor, startColor, (Time.time - startTime) / duration) * intensity;

            yield return null;
        }

        SR.color = startColor;
    }
}