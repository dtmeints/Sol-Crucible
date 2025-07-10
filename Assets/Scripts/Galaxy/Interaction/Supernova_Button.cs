using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Supernova_Button : MonoBehaviour
{
    public Inside_Sun_Manager InsideSun;

    public TextMeshProUGUI Text;

    [Header("Images")]
    public Image Border;
    public Image Backdrop;

    private Sun_Script sunScript;

    [HideInInspector] public bool SupernovaHappening;
    private bool canSupernova;

    float checkTimer;

    private Vector3 targetScale;


    void Start()
    {
        targetScale = new Vector3(1f, 1f, 1f);
        CheckConditions();
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= 0.5f)
        {
            CheckConditions();
        }
    }

    void FixedUpdate()
    {
        if (canSupernova)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            targetScale = new Vector3(1f, 1f, 1f);
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !SupernovaHappening)
        {
            CheckSupernovaRequirements();
        }
    }

    void OnMouseEnter()
    {
        if (canSupernova)
        {
            targetScale = new Vector3(1.1f, 1.1f, 1f);
        }
    }

    void OnMouseExit()
    {
        if (canSupernova)
        {
            targetScale = new Vector3(1f, 1f, 1f);
        }
    }

    void CheckSupernovaRequirements()
    {
        CurrentResources r = InsideSun.SunResources;

        int aboveThreshold = 0;

        for (int i = 0; i < r.Resources.Length; i++)
        {
            if (r.Resources[i] >= 25)
            {
                aboveThreshold += 1;
            }
        }

        if (aboveThreshold == 4)
        {
            SupernovaHappening = true;
            InsideSun.SunResources.gameObject.GetComponent<Sun_Script>().Supernova.StartSupernova(InsideSun.SunResources.gameObject.gameObject, this);
        }
    }


    void CheckConditions()
    {
        CurrentResources r = InsideSun.SunResources;

        int aboveThreshold = 0;

        for (int i = 0; i < r.Resources.Length; i++)
        {
            if (r.Resources[i] >= 25)
            {
                aboveThreshold += 1;
            }
        }

        if (aboveThreshold == 4)
        {
            Border.color = new Color(Border.color.r, Border.color.g, Border.color.b, 1f);
            Backdrop.color = new Color(Backdrop.color.r, Backdrop.color.g, Backdrop.color.b, 1f);

            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, 1f);

            canSupernova = true;

            GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            Border.color = new Color(Border.color.r, Border.color.g, Border.color.b, 0.01f);
            Backdrop.color = new Color(Backdrop.color.r, Backdrop.color.g, Backdrop.color.b, 0.5f);

            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, 0.1f);

            canSupernova = false;

            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
