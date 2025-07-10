using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class ElementAbsorber : MonoBehaviour
{
    public Inside_Sun_Manager sunManager;

    [Header("Settings")]
    [SerializeField] bool allowHighRankToPass;
    public int maxAbsorbableRank = 5;
    public List<Element> elements;

    [Header("References")]
    [SerializeField] Collider2D col;
    [SerializeField] TextMeshPro rankLabel;
    public Collider2D Col => col;
    Crucible crucible;


    public event Action OnFedCorrectElement;
    public event Action OnFedIncorrectElement;

    private void Awake()
    {
        if (allowHighRankToPass && rankLabel)
            rankLabel.text = TextUtil.ToRoman(maxAbsorbableRank);

        crucible = FindFirstObjectByType<Crucible>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Orb>(out var orb))
            return;

        if (allowHighRankToPass && orb.Rank >= maxAbsorbableRank)
            return;

        if (elements.Contains(orb.Element) && crucible)
        {
            if (GameManager.Instance != null) { crucible.SatisfyRequirement(orb.Element, orb.Rank); }
            OnFedCorrectElement?.Invoke();
        }
        else
        {
            OnFedIncorrectElement?.Invoke();
            if (GameManager.Instance != null) { GameManager.Instance.Stats.AddLostElement(orb.Rank, orb.Element); }
        }

        if (GameManager.Instance != null || elements.Count == 0)
        {
            orb.BeConsumed(transform);
            return;
        }

        if (elements.Contains(orb.Element))
        {
            sunManager.AddElemement(orb.Rank, orb.Element);
            orb.BeConsumed(transform);
            return;
        }
    }

    public void SetRankLimit(int maxAbsorbableRank)
    {
        this.maxAbsorbableRank = maxAbsorbableRank;
        allowHighRankToPass = true;

        if (rankLabel)
            rankLabel.text = TextUtil.ToRoman(maxAbsorbableRank);
    }
}
