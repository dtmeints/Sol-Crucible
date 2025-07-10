using System.Collections.Generic;
using System;
using UnityEngine;

public class Select_Script : MonoBehaviour
{
    public GameObject Parent;

    [Header("Rotate Settings")]
    public bool Rotate;
    public float RotateSpeed;

    [Space(10)]
    public List<Text_Display> TextDisplays = new List<Text_Display>();


    [HideInInspector] public bool MouseOver;

    public static bool isDraggingChain;


    private SpriteRenderer sr;
    private Color targetColor;

    private bool meDraggingChain;

    private bool mouseButtonStartedDown;

    void Start()
    {
        References();

        targetColor = sr.color;
    }


    void Update()
    {
        CheckChainRelease();
    }

    void CheckChainRelease()
    {
        if (isDraggingChain && meDraggingChain && Input.GetMouseButtonUp(0))
        {
            Select_Script[] allSelectObjects = GameObject.FindObjectsOfType<Select_Script>();

            foreach (Select_Script obj in allSelectObjects)
            {
                //if (obj.Parent.GetComponent<CurrentResources>().Target == Parent.GetComponent<CurrentResources>()) continue;

                if (obj.MouseOver && !obj.meDraggingChain)
                {
                    GameObject objParent = obj.Parent;
                    if (Parent.GetComponent<Sun_Script>() != null) Parent.GetComponent<Sun_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = objParent.transform;
                    if (Parent.GetComponent<Planet_Script>() != null) Parent.GetComponent<Planet_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = objParent.transform;

                    if (Parent.GetComponent<CurrentResources>() != null) Parent.GetComponent<CurrentResources>().Target = objParent.GetComponent<CurrentResources>();

                    if (Parent.GetComponent<Planet_Movement>() != null && Parent.GetComponent<Planet_Movement>().Target == null)
                    {
                        if (objParent.GetComponent<Sun_Script>() != null)
                        {
                            Parent.GetComponent<Planet_Movement>().Target = objParent.transform;
                        }
                        else
                        {
                            Parent.GetComponent<Planet_Movement>().Target = objParent.GetComponent<Planet_Movement>().Target;
                        }

                        Parent.GetComponent<Planet_Movement>().GenerateOrbitStart();
                    }

                    if (Parent.GetComponent<Planet_Movement>() != null && objParent.GetComponent<Planet_Movement>() != null)
                    {
                        if(objParent.GetComponent<Planet_Movement>().Target == null) objParent.GetComponent<Planet_Movement>().Target = Parent.GetComponent<Planet_Movement>().Target;
                    }

                    if (Parent.GetComponent<Sun_Script>() != null && objParent.GetComponent<Planet_Movement>() != null)
                    {
                        if (objParent.GetComponent<Planet_Movement>().Target == null)
                        {
                            objParent.GetComponent<Planet_Movement>().Target = Parent.transform;
                            objParent.GetComponent<Planet_Movement>().GenerateOrbitStart();
                        }
                    }

                    if (Parent.GetComponent<CurrentResources>() != null)
                    {
                        if (objParent.GetComponent<CurrentResources>().Target == Parent.GetComponent<CurrentResources>())
                        {
                            objParent.GetComponent<CurrentResources>().Target = null;

                            if (objParent.GetComponent<Sun_Script>() != null) objParent.GetComponent<Sun_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = null;
                            if (objParent.GetComponent<Planet_Script>() != null) objParent.GetComponent<Planet_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = null;
                        }
                    }

                    isDraggingChain = false;
                    meDraggingChain = false;
                    return;
                }
            }

            if (Parent.GetComponent<Sun_Script>() != null) Parent.GetComponent<Sun_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = null;
            if (Parent.GetComponent<Planet_Script>() != null) Parent.GetComponent<Planet_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = null;

            if (Parent.GetComponent<CurrentResources>() != null) Parent.GetComponent<CurrentResources>().Target = null;

            isDraggingChain = false;
            meDraggingChain = false;
        }
    }

    void FixedUpdate()
    {
        if (Rotate)
        {
            transform.Rotate(new Vector3(0, 0, 1f) * RotateSpeed);
        }

        sr.color = Color.Lerp(sr.color, targetColor, 0.2f);

        for (int i = 0; i < TextDisplays.Count; i++)
        {
            if (i <= 3)
            {
                TextDisplays[i].Content = Parent.GetComponent<CurrentResources>().Resources[i].ToString();
                if (Parent.GetComponent<Planet_Script>() != null)
                {
                    TextDisplays[i].Content += "/" + Mathf.FloorToInt(Parent.GetComponent<Planet_Script>().resourceWeight[i] * Parent.GetComponent<Planet_Script>().amountPerPerson * (Parent.GetComponent<Planet_Script>().Population + 1));
                }
            }
            else
            {
                TextDisplays[i].Content = Parent.GetComponent<Planet_Script>().Population.ToString();
            }

            if (TextDisplays[i].Content.Length <= 5)
            {
                TextDisplays[i].Text.fontSize = 22.6f;
            }
            else if (TextDisplays[i].Content.Length == 6)
            {
                TextDisplays[i].Text.fontSize = 18.6f;
            }
            else if (TextDisplays[i].Content.Length == 7)
            {
                TextDisplays[i].Text.fontSize = 15.5f;
            }
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !isDraggingChain && !mouseButtonStartedDown)
        {
            if (Parent.GetComponent<Sun_Script>() != null)
            {
                Parent.GetComponent<Sun_Script>().EnableGameplay();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseButtonStartedDown = false;
        }
    }


    void OnMouseEnter()
    {
        targetColor = new Color(sr.color.r, sr.color.g, sr.color.b, 0.45f);

        if (!isDraggingChain)
        {
            if (Parent.GetComponent<Planet_Movement>() != null)
            {
                if (Parent.GetComponent<Planet_Movement>().Target != null)
                {
                    foreach (Text_Display display in TextDisplays)
                    {
                        display.Show();
                    }
                }
            }
            else
            {
                foreach (Text_Display display in TextDisplays)
                {
                    display.Show();
                }
            }
            
        }

        if (Input.GetMouseButton(1))
        {
            mouseButtonStartedDown = true;
        }
        
        MouseOver = true;
    }

    void OnMouseExit()
    {
        mouseButtonStartedDown = false;
        
        targetColor = new Color(sr.color.r, sr.color.g, sr.color.b, 0.086f);

        foreach (Text_Display display in TextDisplays)
        {
            display.Hide();
        }

        MouseOver = false;

        if (Input.GetMouseButton(0) && !isDraggingChain && !meDraggingChain)
        {
            isDraggingChain = true;
            meDraggingChain = true;

            if (Parent.GetComponent<Sun_Script>() != null)
            {
                Parent.GetComponent<Sun_Script>().ActiveChain.gameObject.SetActive(true);

                Parent.GetComponent<Sun_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = GameObject.FindWithTag("MouseFollower").transform;
                Parent.GetComponent<Sun_Script>().ActiveChain.GetComponent<Chain_Visuals>().Origin = Parent.transform;
            }

            if (Parent.GetComponent<Planet_Script>() != null)
            {
                Parent.GetComponent<Planet_Script>().ActiveChain.gameObject.SetActive(true);

                Parent.GetComponent<Planet_Script>().ActiveChain.GetComponent<Chain_Visuals>().Target = GameObject.FindWithTag("MouseFollower").transform;
                Parent.GetComponent<Planet_Script>().ActiveChain.GetComponent<Chain_Visuals>().Origin = Parent.transform;
            }
        }
    }


    void References()
    {
        sr = GetComponent<SpriteRenderer>();
    }
}
