using System;
using TMPro;
using UnityEngine;

public class UIHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI volitionsText, durationText, tutorialText1, tutorialText2;
    [SerializeField] Animator tutorialAnim;
    [SerializeField] Animator[] airAnimators, fireAnimators, earthAnimators, waterAnimators;

    private void Start()
    {
        tutorialAnim.SetTrigger("show");
        if (GameManager.Instance.CurrentLevel != null)
        {
            tutorialText1.text = GameManager.Instance.CurrentLevel.Text1;
            tutorialText2.text = GameManager.Instance.CurrentLevel.Text2;
        }
    }
    void Update()
    {
        volitionsText.text = "VOLITIONS: " + TextUtil.ToRoman(GameManager.Instance.Stats.volitionsCast);

        TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.Stats.duration);
        durationText.text = string.Format("{0}:{1:D2}", time.Minutes, time.Seconds);
    }

    private void HideTutorials()
    {
        tutorialAnim.SetTrigger("hide");
    }

    public void ActivateElementalReadouts(Element element)
    {
        //Animator[] animators = element switch
        //{
        //    Element.Air => airAnimators,
        //    Element.Fire => fireAnimators,
        //    Element.Water => waterAnimators,
        //    Element.Earth => earthAnimators,
        //};
        //foreach (var anim in animators)
        //{
        //    anim.SetTrigger("active");
        //}
    }

    private void OnEnable()
    {
        GameManager.GameOver += HideTutorials;
        Player.OnHover += ActivateElementalReadouts;
        Crucible.OnElementSupplied += ActivateElementalReadouts;
    }

    private void OnDisable()
    {
        GameManager.GameOver -= HideTutorials;
        Player.OnHover -= ActivateElementalReadouts;
        Crucible.OnElementSupplied -= ActivateElementalReadouts;
    }
}
