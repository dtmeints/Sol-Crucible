using TMPro;
using UnityEngine;
using System.Collections;

public class UILevelButton : MonoBehaviour, IClickable
{
    [SerializeField] D_Level level;
    [SerializeField] TextMeshPro levelNumberText;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] D_Element element;
    [ColorUsage(true, true), SerializeField] Color flashColorA, flashColorB;
    MaterialPropertyBlock MPB;

    private void Start()
    {
        if (level == null)
        {
            Debug.LogError("No Level Assigned To Button");
            gameObject.SetActive(false);
            return;
        }
        gameObject.name = level.name;
        if (level.LevelIndex() >= 0)
            levelNumberText.text = "Lv. \n" + TextUtil.ToRoman(level.LevelIndex() + 1);
        else levelNumberText.text = TextUtil.ToRoman(level.ChallengeLevel);
        spriteRenderer.color = element.color;
        MPB = new MaterialPropertyBlock();
    }

    public void StartLevel()
    {
        if (level.LevelIndex() >= 0)
            _ = GameManager.Instance.StartLevelInSequence(level.LevelIndex());
        else _ = GameManager.Instance.StartLevel(level);
    }

    void IClickable.ActivateClickEffect()
    {
        StartLevel();
    }

    public void ActivateMouseOverEffect()
    {
        StartCoroutine(Co_Flash(.4f));
    }

    public IEnumerator Co_Flash(float intensity)
    {
        float startTime = Time.time;
        float duration = .2f;

        while (Time.time - startTime < duration)
        {
            MPB.SetColor("_AddColor", Color.Lerp(flashColorA, flashColorB, (Time.time - startTime) / duration) * intensity);
            spriteRenderer.SetPropertyBlock(MPB);
            yield return null;
        }

        duration = .5f;
        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            MPB.SetColor("_AddColor", Color.Lerp(flashColorB, flashColorA, (Time.time - startTime) / duration) * intensity);
            spriteRenderer.SetPropertyBlock(MPB);
            yield return null;
        }

        MPB.SetColor("_AddColor", flashColorA);
        spriteRenderer.SetPropertyBlock(MPB);
    }
}
