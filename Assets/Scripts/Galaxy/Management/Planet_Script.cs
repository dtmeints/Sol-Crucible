using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class Planet_Script : MonoBehaviour
{
    [Header("Settings")]
    public float SpeedScaling;
    public bool CanDie;

    [Header("References")]
    public Warning_Display Warning;
    public SpriteRenderer PlanetTexture;
    public Select_Script Select;

    [Header("Prefabs")]
    public GameObject Chain;

    [Space(10)]

    // 0 = FIRE, 1 = AIR, 2 = WATER, 3 = EARTH

    public int[] Requirements = new int[4];

    [Space(10)]
    public int Population;

    //Decides how much of a certain resource the planet requires (percentage based)
    [HideInInspector] public List<float> resourceWeight = new List<float>() { 0f, 0f, 0f, 0f };

    private List<float> resourceDecimals = new List<float>() { 0f, 0f, 0f, 0f };

    [HideInInspector] public int amountPerPerson;
    private float consumePerPerson;

    private float PopulationShrinkTimer;
    private float PopulationGrowTimer;
    private float ConsumeResourceTimer;
    private float DownGradeCheckTimer;

    private float energyTimer;

    private bool warning;

    [HideInInspector] public Chain_Visuals ActiveChain;

    private GameObject myChain;

    private float energyGenerateTime;


    void Start()
    {
        RandomizeResourceWeight();
        UpdateRequirements();
        SpawnChain();

        amountPerPerson = 3;
        consumePerPerson = 0.5f;

        GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().ActivePlanets.Add(this);

        energyGenerateTime = GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().EnergyGenerateTime;
    }

    void FixedUpdate()
    {
        PopulationShrinkTimer += Time.deltaTime;
        PopulationGrowTimer += Time.deltaTime;

        if (PopulationShrinkTimer >= 20f * SpeedScaling)
        {
            UpdatePopulation(false);
            PopulationShrinkTimer = 0;
        }

        if (PopulationGrowTimer >= 10f * SpeedScaling)
        {
            UpdatePopulation(true);
            PopulationGrowTimer = 0;

        }

        ConsumeResourceTimer += Time.deltaTime;

        if (ConsumeResourceTimer >= 30f / Population * SpeedScaling)
        {
            for (int i = 0; i < resourceWeight.Count; i++)
            {
                if (GetComponent<CurrentResources>().Resources[i] <= 0) continue;

                float n = resourceWeight[i] * consumePerPerson;
                int round = Mathf.FloorToInt(n);
                float decimals = n - round;

                GetComponent<CurrentResources>().Resources[i] -= round;

                resourceDecimals[i] += decimals;

                if (resourceDecimals[i] >= 1f)
                {
                    GetComponent<CurrentResources>().Resources[i] -= 1;
                    resourceDecimals[i] -= 1f;
                }
            }

            ConsumeResourceTimer = 0;
        }

        DownGradeCheckTimer += Time.deltaTime;

        if (DownGradeCheckTimer >= 5f)
        {
            CheckDownGrade();
        }

        energyTimer += Time.deltaTime;

        if (energyTimer >= energyGenerateTime / Population && GetComponent<Planet_Movement>().Target != null)
        {
            GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().Energy += 1;
            energyTimer = 0;
        }
    }


    void UpdatePopulation(bool growCheck)
    {
        int[] resources = GetComponent<CurrentResources>().Resources;

        int above = 0;
        int below = 0;

        for (int i = 0; i < resources.Length; i++)
        {
            int n = resources[i] - Requirements[i];

            if (n >= Mathf.FloorToInt(resourceWeight[i] * 1 * amountPerPerson))
            {
                above += 1;
            }
            else if (n < 0 && !growCheck)
            {
                if (n < Mathf.FloorToInt(resourceWeight[i] * -1 * amountPerPerson))
                {
                    below += 1;
                }
            }
        }

        Debug.Log(above);

        if (below > 0)
        {
            if (!CanDie && Population == 1) return;

            Population -= 1;
            Warning.Show();
            UpdateRequirements();
            return;
        }

        if (above == 4)
        {
            Population += 1;
            Warning.Hide();
            UpdateRequirements();
            PopulationShrinkTimer = 0;
            return;
        }
    }


    void CheckDownGrade()
    {
        int[] resources = GetComponent<CurrentResources>().Resources;

        int below = 0;

        for (int i = 0; i < resources.Length; i++)
        {
            int n = resources[i] - Requirements[i];

            if (n < Mathf.FloorToInt(resourceWeight[i] * -0.5f * amountPerPerson))
            {
                below += 1;
            }
        }

        if (below > 0)
        {
            if (!CanDie && Population == 1) { Warning.Hide(); return; }

            Warning.Show();
        }
        else
        {
            Warning.Hide();
        }
    }


    void UpdateRequirements()
    {
        for (int i = 0; i < Requirements.Length; i++)
        {
            Requirements[i] = Mathf.FloorToInt(resourceWeight[i] * amountPerPerson * Population);
        }
    }

    void SpawnChain()
    {
        myChain = Instantiate(Chain);
        myChain.GetComponent<Chain_Visuals>().Origin = gameObject.transform;

        ActiveChain = myChain.GetComponent<Chain_Visuals>();

        if (GetComponent<CurrentResources>().Target != null)
        {
            myChain.GetComponent<Chain_Visuals>().Target = GetComponent<CurrentResources>().Target.gameObject.transform;
        }
        else
        {
            myChain.SetActive(false);
        }
    }


    void RandomizeResourceWeight()
    {
        List<int> orderList = new List<int>() { 0, 1, 2, 3 };

        List<int> randomList = Shuffle(orderList);

        resourceWeight[randomList[0]] = UnityEngine.Random.Range(0.6f, 0.7f);
        resourceWeight[randomList[1]] = UnityEngine.Random.Range(0.05f, 0.2f);
        resourceWeight[randomList[2]] = UnityEngine.Random.Range(0f, 0.1f);
        resourceWeight[randomList[3]] = 1f - resourceWeight[randomList[0]] - resourceWeight[randomList[1]] - resourceWeight[randomList[2]];
    }

    public List<int> Shuffle(List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }


    public void RandomVisuals(Sprite texture, float sizeMultiplier)
    {
        PlanetTexture.sprite = texture;
        PlanetTexture.gameObject.transform.parent.localScale *= sizeMultiplier;
        Select.gameObject.transform.localScale *= sizeMultiplier;

        foreach (Text_Display display in Select.TextDisplays)
        {
            display.gameObject.transform.localPosition *= Mathf.Lerp(sizeMultiplier, 1f, 0.5f);
        }

        int m = UnityEngine.Random.Range(0, 2) * 2 - 1;

        Select.RotateSpeed = UnityEngine.Random.Range(0.09f, 0.2f) * m;
    }   
    
}
