using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Controller : MonoBehaviour
{

    [HideInInspector] public bool CanDrag;

    [Header("Zoom")]
    public float ZoomSpeed;
    public float MinZoom = 1f;
    public float MaxZoom = 50f;

    [Header("Drag")]
    public Vector2 MaxDragPos;
    public Vector2 MinDragPos;

    private Vector3 dragOrigin;
    private Vector3 dragDifference;

    private Camera cam;

    private bool isDragging;

    private float zoom = 5.55f;

    private bool usingMiddleMouse;


    void Start()
    {
        cam = GetComponent<Camera>();
        CanDrag = true;
    }


    void Update()
    {
        Drag();
        Scroll();
    }


    void Drag()
    {
        if (Select_Script.isDraggingChain) return;

        if (Input.GetMouseButtonDown(2))
        {
            usingMiddleMouse = true;
            
            isDragging = true;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(2))
        {
            usingMiddleMouse = false;

            isDragging = false;
        }

        if (Input.GetMouseButtonDown(0) && !usingMiddleMouse)
            {
                if (!CheckMouseOver())
                {
                    isDragging = true;
                    dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }

        if (Input.GetMouseButtonUp(0) && !usingMiddleMouse)
        {
            isDragging = false;
        }
    }

    void Scroll()
    {
        if (!CanDrag) return;

        if (zoom - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed > MinZoom && zoom - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed < MaxZoom)
        {
            zoom -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
        }
    }

    void LateUpdate()
    {
        if (CanDrag)
        {
            cam.orthographicSize = zoom;
        }

        if (!isDragging || !CanDrag) return;

        dragDifference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector3 newPos = dragOrigin - dragDifference;

        if (newPos.x >= MinDragPos.x && newPos.x <= MaxDragPos.x && newPos.y >= MinDragPos.y && newPos.y <= MaxDragPos.y)
        {
            transform.position = dragOrigin - dragDifference;
        }
    }


    bool CheckMouseOver()
    {
        Select_Script[] selects = GameObject.FindObjectsOfType<Select_Script>();

        foreach (Select_Script s in selects)
        {
            if (s.MouseOver) return true;
        }

        return false;
    }
}
