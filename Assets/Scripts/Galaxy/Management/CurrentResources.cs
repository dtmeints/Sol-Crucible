using System.Collections.Generic;
using System;
using UnityEngine;

public class CurrentResources : MonoBehaviour
{
    // 0 = FIRE, 1 = AIR, 2 = WATER, 3 = EARTH

    [Header("Resources")]
    public int[] Resources = new int[4];

    public CurrentResources Target;

    private float resourceTimer;

    private float transportTime;

    void Start()
    {
        transportTime = GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().TransportTime;
    }


    void FixedUpdate()
    {
        if (Target != null)
        {
            resourceTimer += Time.deltaTime;

            if (resourceTimer > transportTime)
            {
                SendResource();
                resourceTimer = 0;
            }

            if (resourceTimer > 0.5f)
            {
                if (GetComponent<Sun_Script>() != null) GetComponent<Sun_Script>().ActiveChain.ResetColor();
                if (GetComponent<Planet_Script>() != null) GetComponent<Planet_Script>().ActiveChain.ResetColor();
            }
        }
        else
        {
            resourceTimer = 0;
        }
    }


    private void SendResource()
    {
        int targetResource = 0;
        int highestResouce = 0;

        for (int i = 0; i < Resources.Length; i++)
        {
            if (Resources[i] > highestResouce)
            {
                highestResouce = Resources[i];
                targetResource = i;
            }
        }

        if (highestResouce == 0) return;

        GetComponent<Sun_Script>().ActiveChain.SetColor(targetResource);

        Target.Resources[targetResource] += 1;
        Resources[targetResource] -= 1;
    }
}
