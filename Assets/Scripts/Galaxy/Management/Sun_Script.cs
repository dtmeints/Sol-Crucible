using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class Sun_Script : MonoBehaviour
{
    public Inside_Sun_Manager SunInside;
    public SpriteRenderer SunSprite;
    public Supernova_Event Supernova;

    [Header("Prefabs")]
    public GameObject Chain;


    [HideInInspector] public Chain_Visuals ActiveChain;

    [HideInInspector] public bool CanEnterSun;


    private Transform chainTarget;

    private Vector3 chainEndPos;
    private GameObject myChain;

    public float chainHealth = 0.4f;

    private Color goodChainColor = new Color(0.238f, 0.67f, 0.196f, 0.27f);
    private Color badChainColor = new Color(0.67f, 0.233f, 0.196f, 0.27f);

    private Vector3 camPos;

    private Vector3 originalScale;

    [HideInInspector] public bool supernovaGrow;


    void Start()
    {
        GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().ActiveSuns.Add(this);
        CanEnterSun = true;
        originalScale = SunSprite.gameObject.transform.localScale;
        SpawnChain();
    }

    public void EnableGameplay()
    {
        if (!CanEnterSun) return;
        Camera.main.gameObject.GetComponent<Animator>().SetTrigger("Fade");
        camPos = Camera.main.gameObject.transform.position;
        StartCoroutine(EnableWait());
    }

    private IEnumerator EnableWait()
    {
        yield return new WaitForSeconds(0.25f);

        SunInside.gameObject.SetActive(true);
        SunInside.EnableOrbManager();
        //Camera.main.gameObject.GetComponent<Camera_Movement>().enabled = false;
        Camera.main.gameObject.transform.position = new Vector3(SunInside.gameObject.transform.position.x, SunInside.gameObject.transform.position.y, Camera.main.gameObject.transform.position.z);

        GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().FocusGameplay(this);
    }

    public void DisableGameplay()
    {
        Camera.main.gameObject.GetComponent<Animator>().SetTrigger("Fade");
        StartCoroutine(DisableWait());
    }

    private IEnumerator DisableWait()
    {
        yield return new WaitForSeconds(0.25f);

        SunInside.DisableOrbManager();
        SunInside.gameObject.SetActive(false);
        Camera.main.gameObject.transform.position = camPos;
        //Camera.main.gameObject.GetComponent<Camera_Movement>().enabled = true;

        GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>().UnfocusGameplay();
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

    void FixedUpdate()
    {
        Transform spriteTrans = SunSprite.gameObject.transform;
        if (supernovaGrow)
        {
            spriteTrans.localScale += new Vector3(0.02f, 0.02f, 0f);
        }
        else
        {
            spriteTrans.localScale = originalScale;
        }
    }
}
