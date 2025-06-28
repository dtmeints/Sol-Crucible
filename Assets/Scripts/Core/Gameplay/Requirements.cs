using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Requirements
{
    public bool Randomize;
    public int TotalToRequire = 60;

    [SerializeField] int airRequired, fireRequired, earthRequired, waterRequired;
    public int AirRequired => airRequired;
    public int FireRequired => fireRequired;
    public int EarthRequired => earthRequired;
    public int WaterRequired => waterRequired;

    public int GetRequiredCountByElement(Element element)
    {
        return element switch
        {
            Element.Air => airRequired,
            Element.Fire => fireRequired,
            Element.Water => waterRequired,
            Element.Earth => earthRequired
        };
    }

    public int OriginalTotal { get; private set; }
    public int CurrentHeld { get; private set; }
    public float Completeness { get; private set; }

    public Dictionary<Element, int> CurrentHeldByElement { get; private set; }

    public Requirements(bool randomized, int total)
    {
        if (randomized)
            CreateRandomRequirements(total);

        Initialize();
    }

    public void Initialize()
    {
        CurrentHeldByElement = new()
        {
            {   Element.Air, 0 },
            {   Element.Fire, 0 },
            {   Element.Earth, 0 },
            {   Element.Water, 0 },
        };

        OriginalTotal = airRequired + fireRequired + waterRequired + earthRequired;
        RecalculateCurrent();
    }

    public void CreateRandomRequirements(int totalToDistribute)
    {
        //generate first 3 requirements
        List<int> segments = new()
        {
            UnityEngine.Random.Range(totalToDistribute / 4, totalToDistribute / 3),
            UnityEngine.Random.Range(totalToDistribute / 6, totalToDistribute / 4),
            UnityEngine.Random.Range(totalToDistribute / 5, totalToDistribute / 3)
        };

        //assign remaining at random index
        foreach (var segment in segments)
            totalToDistribute -= segment;
        segments.Insert(UnityEngine.Random.Range(0, 3), totalToDistribute);

        //assign 
        airRequired = segments[0];
        fireRequired = segments[1];
        earthRequired = segments[2];
        waterRequired = segments[3];
    }

    public void RecalculateCurrent()
    {
        CurrentHeld = 0;
        foreach (var rank in CurrentHeldByElement.Values)
            CurrentHeld += rank;

        Completeness = (float)CurrentHeld / (float)OriginalTotal;
    }

    public void Satisfy(Element element, int count)
    {
        CurrentHeldByElement[element] = Mathf.Clamp(CurrentHeldByElement[element] + count, 0, GetRequiredCountByElement(element));
        RecalculateCurrent();
    }
}