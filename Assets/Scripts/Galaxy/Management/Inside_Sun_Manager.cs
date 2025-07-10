using UnityEngine;
using TMPro;

public class Inside_Sun_Manager : MonoBehaviour
{
    public SpriteRenderer SunSprite;
    public CurrentResources SunResources;

    public TextMeshPro[] ResourceText;

    public Meter_Display[] Meters;

    public OrbManager orbManager;

    [HideInInspector] public bool CanSupernova;

    void Start()
    {
        SunSprite.material = SunResources.gameObject.GetComponent<Sun_Script>().SunSprite.material;
    }

    public void EnableOrbManager()
    {
        orbManager.SetInstance();
    }

    public void DisableOrbManager()
    {
        orbManager.UnsetInstance();
    }

    void Update()
    {
        int aboveThreshold = 0;

        for (int i = 0; i < Meters.Length; i++)
        {
            Meters[i].Amount = SunResources.Resources[i] / 25f;

            if (SunResources.Resources[i] >= 25)
            {
                aboveThreshold += 1;
            }
        }

        if (aboveThreshold == 4)
        {
            CanSupernova = true;
        }
        else
        {
            CanSupernova = false;
        }
    }

    public void Disable()
    {
        SunResources.gameObject.GetComponent<Sun_Script>().DisableGameplay();
    }

    public void ResetOrbs()
    {
        Orb[] AllOrbs = GetComponentsInChildren<Orb>();
        foreach (Orb orb in AllOrbs)
        {
            orb.BeConsumed(orb.gameObject.transform);
        }
    }

    public void AddElemement(int rank, Element element)
    {
        switch (element)
        {
            case Element.Fire:
                SunResources.Resources[0] += rank;
                break;
            case Element.Air:
                SunResources.Resources[1] += rank;
                break;
            case Element.Water:
                SunResources.Resources[2] += rank;
                break;
            case Element.Earth:
                SunResources.Resources[3] += rank;
                break;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < ResourceText.Length; i++)
        {
            ResourceText[i].text = SunResources.Resources[i].ToString();
        }
    }
}
