using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Collections.Generic;
using System;
using System.Linq;

public static class Reactions
{
    static Dictionary<(Element, Element), Action<Orb, Orb>> ReactionDict = new()
    {
        {(Element.Air, Element.Fire), AirVsFire },
        {(Element.Air, Element.Water), AirVsWater },
        {(Element.Air, Element.Earth), AirVsEarth },
        {(Element.Fire, Element.Water), FireVsWater },
        {(Element.Fire, Element.Earth), FireVsEarth },
        {(Element.Water, Element.Earth), WaterVsEarth },
        {(Element.Water, Element.Water), Self },
        {(Element.Air, Element.Air), Self },
        {(Element.Fire, Element.Fire), Self },
        {(Element.Earth, Element.Earth), Self },
    };

    public static bool React(Orb orb1, Orb orb2, Vector2 collisionVelocity)
    {
        if ((int)orb1.Element > (int)orb2.Element)
            return false;

        if (ReactionDict.TryGetValue((orb1.Element, orb2.Element), out var reaction))
        {
            reaction?.Invoke(orb1, orb2);
            if (GameManager.Instance != null) { GameManager.Instance.Stats.collisionsMade++; }
        }
        return true;
    }

    public static void Self(Orb orb1, Orb orb2)
    {
        if (orb1.twinOriginal == orb2 && (Time.time - orb1.timeOfTwinning < 2f || Time.time - orb2.timeOfTwinning < 2f))
            return;

        Orb higherOrb = orb1.Rank >= orb2.Rank ? orb1 : orb2;
        Orb lowerOrb = orb1 != higherOrb ? orb1 : orb2;

        higherOrb.AddRank(lowerOrb.Rank);
        lowerOrb.BeConsumed(higherOrb.transform);
        lowerOrb.RB.linearVelocity = (higherOrb.transform.position - lowerOrb.transform.position).normalized * 2f;
        higherOrb.ActivateVFX();
    }

    public static void AirVsFire(Orb airOrb, Orb fireOrb)
    {
        // air speed is doubled
        // fire receives air's rank
        // fire is split

        airOrb.RB.linearVelocity *= 2;
        fireOrb.AddRank(airOrb.Rank);

        _ = CreateTwin(airOrb);
    }

    public static void AirVsWater(Orb airOrb, Orb waterOrb)
    {
        //water orb doubles speed,
        //air receives water orb rank,
        //water orb rank is halved
        waterOrb.RB.linearVelocity *= 2;
        airOrb.AddRank(waterOrb.Rank);
        waterOrb.AddRank(-Mathf.FloorToInt(waterOrb.Rank / 2));
        
    }

    public static void AirVsEarth(Orb airOrb, Orb earthOrb)
    {
        //earth receives half of air rank,
        //air is consumed
        //earth is split
        earthOrb.AddRank(Mathf.FloorToInt(airOrb.Rank / 2));
        airOrb.BeConsumed(earthOrb.transform);
        //airOrb.AddRank(-Mathf.FloorToInt(airOrb.Rank / 2));

        _ = CreateTwin(earthOrb);
    }

    public static void FireVsWater(Orb fireOrb, Orb waterOrb)
    {
        // water receives fire rank
        // fire is consumed
        // water is split

        waterOrb.AddRank(fireOrb.Rank);
        fireOrb.BeConsumed(waterOrb.transform);

        
        _ = CreateTwin(waterOrb);
    }

    public static void FireVsEarth(Orb fireOrb, Orb earthOrb)
    {
        //earth speed doubles
        //earth rank is halved
        //fire receives (halved) earth rank
        // fire is split

        earthOrb.RB.linearVelocity *= 2;
        earthOrb.AddRank(-Mathf.FloorToInt(earthOrb.Rank / 2));
        fireOrb.AddRank(earthOrb.Rank);


        _ = CreateTwin(fireOrb);
    }
   
    public static void WaterVsEarth(Orb waterOrb, Orb earthOrb)
    {
        // earth receives water's rank
        // water is consumed
        earthOrb.AddRank(waterOrb.Rank);

        waterOrb.BeConsumed(earthOrb.transform);
    }


    public static async Task CreateTwin(Orb orb)
    {
        if (OrbManager.Instance.Count >= OrbManager.Instance.MaxOrbCount)
            return;

        await Awaitable.WaitForSecondsAsync(.1f);

        int rank = orb.Rank;
        Vector2 currentVelocity = orb.RB.linearVelocity;

        orb.RB.linearVelocity = Vector2.Perpendicular(currentVelocity) * 1.2f;

        Vector2 twinPosition = (Vector2)orb.transform.position - (orb.RB.linearVelocity.normalized * orb.transform.localScale);
        Orb twin = OrbManager.Instance.SpawnOrb(orb.Element, orb.Rank, twinPosition);

        orb.SetRank(Mathf.CeilToInt((float)rank / (float)2));
        twin.SetRank(Mathf.CeilToInt((float)rank / (float)2));

        twin.twinOriginal = orb;

        twin.timeOfTwinning = Time.time;
        orb.timeOfTwinning = Time.time;

        await Awaitable.WaitForSecondsAsync(.1f);
        twin.RB.linearVelocity = -orb.RB.linearVelocity;
    }
}
