using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class Chain_Visuals : MonoBehaviour
{
    public Transform Origin;
    public Transform Target;

    [Header("Colors")]
    public Color FireColor;
    public Color AirColor;
    public Color WaterColor;
    public Color EarthColor;

    [Space(10)]
    public Color DefaultColor;

    private Color targetColor;

    void Start()
    {
        GetComponent<LineRenderer>().material.SetColor("_Color", DefaultColor);
        targetColor = DefaultColor;
    }

    void FixedUpdate()
    {
        UpdateChain();
    }

    void UpdateChain()
    {
        if (Target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        LineRenderer chainLine = GetComponent<LineRenderer>();

        Vector3 start = Origin.position;
        Vector3 end = Target.position;
        Vector3 midpoint = (start + end) * .5f;

        chainLine.SetPositions(new Vector3[] { start, midpoint, end });

        Color chainColor = Color.Lerp(chainLine.material.GetColor("_Color"), targetColor, 0.1f);
        chainLine.material.SetColor("_Color", chainColor);
    }

    public void SetColor(int resourceIndex)
    {
        if (resourceIndex == 0) targetColor = FireColor;
        if (resourceIndex == 1) targetColor = AirColor;
        if (resourceIndex == 2) targetColor = WaterColor;
        if (resourceIndex == 3) targetColor = EarthColor;
    }

    public void ResetColor()
    {
        targetColor = DefaultColor;
    }
}
