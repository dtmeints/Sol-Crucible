using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraController : MonoBehaviour
{
    [SerializeField, InlineEditor] GameSettingsData gameSettingsData;
    float halfWorldSize => gameSettingsData.worldSize * 0.5f;
    
    [ShowInInspector, ReadOnly] Vector2 currentLimit;

    Camera mainCamera;
    CinemachineCamera cam;
    Vector3 lastMousePosition;
    bool isPanning = false;

    void Reset() => cam = GetComponent<CinemachineCamera>();
    void Start() {
        mainCamera = Camera.main;
        cam ??= GetComponent<CinemachineCamera>();
        cam.Lens.FieldOfView = Mathf.Clamp(cam.Lens.FieldOfView, gameSettingsData.camera.minMaxFOV.x, gameSettingsData.camera.minMaxFOV.y);
        UpdateLimit();
    }

    void UpdateLimit() {
        Vector3 visibleWorldBotLeft = mainCamera.ScreenToWorldPoint(new Vector3(0,0, Mathf.Abs(mainCamera.transform.position.z)));
        float halfVisibleWidth = Mathf.Abs(transform.position.x - visibleWorldBotLeft.x);
        float halfVisibleHeight = Mathf.Abs(transform.position.y - visibleWorldBotLeft.y);
        currentLimit = new Vector3(halfWorldSize, halfWorldSize) - new Vector3(halfVisibleWidth, halfVisibleHeight);
        
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -currentLimit.x, currentLimit.x), Mathf.Clamp(transform.position.y, -currentLimit.y, currentLimit.y), transform.position.z);
    }

    void Update() {
        HandlePanning();
        HandleZooming();
    }

    void HandlePanning() {
        if (Input.GetMouseButtonDown(2)) {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2)) 
            isPanning = false;

        if (!isPanning) return;
        
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

        Vector3 move = new(mouseDelta.x * -1, mouseDelta.y * -1, 0);
        float panSpeed = gameSettingsData.camera.GetPanSpeed(cam);
        move *= panSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + move;
        targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, -currentLimit.x, currentLimit.x), Mathf.Clamp(targetPosition.y, -currentLimit.y, currentLimit.y), transform.position.z);
        transform.position = targetPosition;
        lastMousePosition = Input.mousePosition;
    }

    void HandleZooming() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;
        
        cam.Lens.FieldOfView = Mathf.Clamp(cam.Lens.FieldOfView - scroll * gameSettingsData.camera.zoomSpeed, gameSettingsData.camera.minMaxFOV.x, gameSettingsData.camera.minMaxFOV.y);
        UpdateLimit();
    }
}