using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GravityPullController : MonoBehaviour
{
    ParticleSystem particle;
    ParticleSystem.EmissionModule emission;
    Camera mainCamera;
    bool pulling;
    
    void Awake() {
        particle = GetComponent<ParticleSystem>();
        emission = particle.emission;
        emission.enabled = false;
        mainCamera = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MoveToMouse();
            emission.enabled = true;
            pulling = true;
        }

        if (pulling && Input.GetMouseButton(0)) {
            MoveToMouse();            
        }
        else if (Input.GetMouseButtonUp(0)) {
            emission.enabled = false;
            pulling = false;
        }
    }

    void MoveToMouse() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        transform.position = mainCamera.ScreenToWorldPoint(mousePos);
    }
}