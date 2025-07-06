[System.Serializable]
public class Stats 
{
    public int volitionsCast;
    public float duration;
    public int collisionsMade;
    public int airCreated, fireCreated, earthCreated, waterCreated;
    public int airLost, fireLost, earthLost, waterLost;
    public Requirements requirements;

    public int TotalCreated
    {
        get
        {
            return airCreated + fireCreated + earthCreated + waterCreated;
        }
    }
    public int TotalLost
    {
        get
        {
            return airLost + fireLost + earthLost + waterLost;
        }
    }

    public Stats()
    {

    }

    public void AddCreatedElement(int count, Element element)
    {
        switch (element)
        {
            case Element.Air:
                airCreated += count;
                break;
            case Element.Fire:
                fireCreated += count;
                break;
            case Element.Water:
                waterCreated += count;
                break;
            case Element.Earth:
                earthCreated += count;
                break;
        }
    }

    public void AddLostElement(int count, Element element)
    {
        switch (element)
        {
            case Element.Air:
                airLost += count;
                break;
            case Element.Fire:
                fireLost += count;
                break;
            case Element.Water:
                waterLost += count;
                break;
            case Element.Earth:
                earthLost += count;
                break;
        }
    }
}
