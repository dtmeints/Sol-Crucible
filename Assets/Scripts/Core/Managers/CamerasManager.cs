using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    [SerializeField, InlineEditor] GameSettingsData gameSettingsData;
    [SerializeField] CinemachineBrain brain;
    [SerializeField] CinemachineCamera mainCinemachineCamera;
    [SerializeField] CinemachineCamera newSolCinemachineCamera;
    [SerializeField, HideInInspector] CinemachineFollow newSolFollow;
    
    void OnDrawGizmos() {
        if (Application.isPlaying) return;
        Init();
    }

    void Init() {
        gameSettingsData ??= GameSettingsData.LoadDefaultSettings();
        transform.SetZ(gameSettingsData.camera.cameraZ);
        mainCinemachineCamera.transform.localPosition = Vector3.zero;
        mainCinemachineCamera.Lens.FieldOfView = gameSettingsData.camera.defaultFOV;
        
        newSolCinemachineCamera.transform.localPosition = Vector3.zero;
        newSolCinemachineCamera.Lens.FieldOfView = gameSettingsData.camera.defaultFOV;
        newSolFollow ??= newSolCinemachineCamera.GetComponent<CinemachineFollow>();
        newSolFollow.FollowOffset.z = gameSettingsData.camera.cameraZ;
    }
    
    void Awake() {
        Init();
        ItemPlacer.OnFirstSol += FocusOnNewSol;
    }

    void OnDestroy() {
        ItemPlacer.OnFirstSol -= FocusOnNewSol;
    }

    void FocusOnNewSol(Transform newSol) {
        //newSolCinemachineCamera.Follow = newSol;
        //newSolCinemachineCamera.enabled = true;
        //DOVirtual.DelayedCall(brain.DefaultBlend.Time, () => {
        //    mainCinemachineCamera.transform.position = newSolCinemachineCamera.transform.position;
        //    newSolCinemachineCamera.enabled = false;
        //});
    }
}