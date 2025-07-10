using System.Collections.Generic;
using System;
using UnityEngine;

public class Galaxy_Manager : MonoBehaviour
{
    public int Energy;
    public int EnergyPerFling;

    [Space(10)]
    public float TransportTime;
    public float EnergyGenerateTime;

    [Space(10)]
    public Energy_Display EnergyDisplay;

    [HideInInspector] public bool ActiveGameplay;
    [HideInInspector] public Sun_Script ActiveSun;

    [HideInInspector] public List<Planet_Script> ActivePlanets = new List<Planet_Script>();
    [HideInInspector] public List<Sun_Script> ActiveSuns = new List<Sun_Script>();


    void Start()
    {
        Select_Script.isDraggingChain = false;
    }


    public void Fling()
    {
        //Lose energy
        Energy -= EnergyPerFling;
    }


    public void FocusGameplay(Sun_Script sun)
    {
        ActiveSun = sun;
        ActiveGameplay = true;
        Camera.main.gameObject.GetComponent<Camera_Controller>().CanDrag = false;
        Camera.main.orthographicSize = 5.55f;

        EnergyDisplay.Gameplay();
    }

    public void UnfocusGameplay()
    {
        ActiveSun = null;
        ActiveGameplay = false;
        Camera.main.gameObject.GetComponent<Camera_Controller>().CanDrag = true;

        EnergyDisplay.Galaxy();
    }
}
