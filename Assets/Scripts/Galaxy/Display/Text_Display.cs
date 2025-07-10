using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class Text_Display : MonoBehaviour
{
    [Header("Variables")]
    public string Content;
    public Sprite Icon;

    [Header("References")]
    public TextMeshProUGUI Text;
    public SpriteRenderer IconSprite;

    private bool isShowing;
    private float timer;


    void Start()
    {
        IconSprite.sprite = Icon;
    }


    void Update()
    {
        if (timer <= 1.1f)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 1f && !isShowing)
        {
            GetComponent<Animator>().SetBool("IsShowing", false);
        }
    }

    void FixedUpdate()
    {
        Text.text = Content;
    }

    public void Show()
    {
        if (!isShowing)
        {
            isShowing = true;
            timer = 0;

            GetComponent<Animator>().SetBool("IsShowing", true);
        }
    }

    public void Hide()
    {
        isShowing = false;
    }
}
