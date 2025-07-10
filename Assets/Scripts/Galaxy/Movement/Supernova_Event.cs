using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Supernova_Event : MonoBehaviour
{
    public float PushSpeed = 0.05f;
    public Vector2 PushRange = new Vector2(4f, 8f);
    public float SunAvoidDistance = 3f;

    public GameObject SunPrefab;
    public GameObject InsideSunPrefab;

    [ColorUsage(true, true)]
    public Color startColor;
    [ColorUsage(true, true)]
    public Color endColor;

    private GameObject OldSun;
    private GameObject NewSun;

    private Vector3 oldPushPos;
    private Vector3 newPushPos;

    private int posTries;
    private float PushDistance;

    private bool pushAway;

    public void StartSupernova(GameObject oldSun, Supernova_Button button)
    {
        StartCoroutine(Supernova(oldSun, button));
    }

    public IEnumerator Supernova(GameObject oldSun, Supernova_Button button)
    {
        Sun_Script sunScript = oldSun.GetComponent<Sun_Script>();

        sunScript.DisableGameplay();
        sunScript.CanEnterSun = false;

        yield return new WaitForSeconds(0.25f);

        sunScript.supernovaGrow = true;

        CurrentResources r = oldSun.GetComponent<CurrentResources>();
        for (int i = 0; i < r.Resources.Length; i++)
        {
            r.Resources[i] -= 25;
        }

        yield return new WaitForSeconds(5f);

        sunScript.supernovaGrow = false;

        button.SupernovaHappening = false;

        sunScript.CanEnterSun = true;

        Debug.Log("Supernova Happening");
        GameObject newSun = Instantiate(SunPrefab);
        newSun.transform.position = oldSun.transform.position;
        NewSun = newSun;
        OldSun = oldSun;

        GameObject newInside = Instantiate(InsideSunPrefab);
        //newInside.GetComponent<Inside_Sun_Manager>().SunSprite = NewSun.GetComponent<Sun_Script>().SunSprite;
        newInside.GetComponent<Inside_Sun_Manager>().SunResources = NewSun.GetComponent<CurrentResources>();

        NewSun.GetComponent<Sun_Script>().SunInside = newInside.GetComponent<Inside_Sun_Manager>();
        NewSun.GetComponent<Sun_Script>().Supernova = this;

        NewSun.GetComponent<Sun_Script>().SunSprite.material.SetColor("_AddColor", Color.Lerp(startColor, endColor, Random.Range(0, 1f)));

        newInside.SetActive(false);

        PushDistance = Random.Range(PushRange.x, PushRange.y);

        posTries = 0;

        GetSunPos();

        Planet_Script[] planets = GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().ActivePlanets.ToArray();
        foreach (Planet_Script planet in planets)
        {
            if (planet.gameObject.GetComponent<Planet_Movement>().Target == OldSun.transform)
            {
                if (Random.Range(0, 2) == 0)
                {
                    planet.gameObject.GetComponent<Planet_Movement>().Target = NewSun.transform;
                }
            }
        }

        pushAway = true;

        yield return new WaitForSeconds(20f);

        pushAway = false;
    }

    void FixedUpdate()
    {
        if (pushAway)
        {
            OldSun.transform.position = Vector3.Lerp(OldSun.transform.position, oldPushPos, PushSpeed);
            NewSun.transform.position = Vector3.Lerp(NewSun.transform.position, newPushPos, PushSpeed);
        }
    }

    void GetSunPos()
    {
        Vector2 moveDir = Random.insideUnitCircle.normalized;
        // Vector2 moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        // moveDir = moveDir.normalized;

        Vector3 tempOldPos = moveDir * PushDistance;
        tempOldPos += OldSun.transform.position;

        Vector3 tempNewPos = moveDir * -1 * PushDistance;
        tempNewPos += NewSun.transform.position;

        if (CheckPosOverlap(tempOldPos) && CheckPosOverlap(tempNewPos))
        {
            oldPushPos = tempOldPos;
            newPushPos = tempNewPos;
        }
        else
        {
            if (posTries < 100)
            {
                posTries += 1;
                GetSunPos();
            }
            else
            {
                Debug.Log("Failed to find Supernova Path");
                oldPushPos = tempOldPos;
                newPushPos = tempNewPos;
            }
        }
    }

    bool CheckPosOverlap(Vector3 pos)
    {
        Sun_Script[] suns = GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().ActiveSuns.ToArray();

        foreach (Sun_Script sun in suns)
        {
            if (Vector3.Distance(sun.gameObject.transform.position, pos) <= SunAvoidDistance)
            {
                return false;
            }
        }

        if (Mathf.Abs(pos.x) > 25 || Mathf.Abs(pos.y) > 20)
        {
            return false;
        }

        return true;
    }
}
