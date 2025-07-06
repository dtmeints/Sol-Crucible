using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Data/Settings/Camera")]
public class CameraSettingsData : ScriptableObject
{
    [field: SerializeField] public float cameraZ { get; private set; } = -5;
    [field: SerializeField] public int defaultFOV { get; private set; } = 100;
    [field: SerializeField] public Vector2Int minMaxFOV { get; private set; } = new(10, 125);
    [field: SerializeField] public Vector2 minMaxPanSpeed { get; private set; } = new(0.25f, 2.5f);
    [SerializeField] AnimationCurve panSpeedCurve = AnimationCurve.Constant(0, 1, 1);
    [field: SerializeField] public float zoomSpeed { get; private set; } = 10f;
    
    public float GetPanSpeed(CinemachineCamera camera) => Mathf.Lerp(minMaxPanSpeed.x, minMaxPanSpeed.y, panSpeedCurve.Evaluate(Mathf.InverseLerp(minMaxFOV.x, minMaxFOV.y, camera.Lens.FieldOfView)));
}