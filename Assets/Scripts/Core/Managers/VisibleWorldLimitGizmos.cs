using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VisibleWorldLimitGizmos : MonoBehaviour
{
    [SerializeField, InlineEditor] GameSettingsData gameSettingsData;
    [SerializeField] bool drawVisibleBackgroundEdges = true;
    [SerializeField] Color visibleBackgroundEdges = Color.green;
    
    [SerializeField] bool drawVisibleGameplanEdges = true;
    [SerializeField] Color visibleGameplanEdges = Color.green;

    Camera gizmosCamera;

    void OnDrawGizmos() => DrawGizmos();
    void DrawGizmos() {
        transform.localPosition = Vector3.zero;
        gameSettingsData ??= GameSettingsData.LoadDefaultSettings();
        
        if (!enabled) return;

        if (!gizmosCamera)
            gizmosCamera = GetComponent<Camera>();

        float pixelHeight = gizmosCamera.pixelHeight;
        float pixelWidth = gizmosCamera.pixelWidth;
        float halfWorldSize = gameSettingsData.worldSize * 0.5f;
        float backgroundLimitZ = gameSettingsData.starBackgroundShaderZ;

        if (drawVisibleBackgroundEdges) 
            DrawStuff(backgroundLimitZ, halfWorldSize, pixelHeight, pixelWidth, visibleBackgroundEdges);
        
        if (drawVisibleGameplanEdges)
            DrawStuff(0, halfWorldSize, pixelHeight, pixelWidth, visibleGameplanEdges);
    }

    void DrawStuff(float distance, float halfWorldSize, float pixelHeight, float pixelWidth, Color color) {
        Gizmos.color = color;
        gizmosCamera.fieldOfView = gameSettingsData.camera.minMaxFOV.y;
        transform.position = new Vector2(-halfWorldSize, -halfWorldSize);
        Vector3 bl = gizmosCamera.ScreenToWorldPoint(new Vector3(0, 0, distance));
        transform.position = new Vector2(-halfWorldSize, halfWorldSize);
        Vector3 tl = gizmosCamera.ScreenToWorldPoint(new Vector3(0, pixelHeight, distance));
        transform.position = new Vector2(halfWorldSize, halfWorldSize);
        Vector3 tr = gizmosCamera.ScreenToWorldPoint(new Vector3(pixelWidth, pixelHeight, distance));
        transform.position = new Vector2(halfWorldSize, -halfWorldSize);
        Vector3 br = gizmosCamera.ScreenToWorldPoint(new Vector3(pixelWidth, 0, distance));

        float height = Mathf.Abs(bl.y - tl.y);
        float width = Mathf.Abs(bl.x - br.x);
        float lineSize = 0.5f;

        Gizmos.DrawRay(bl, Vector2.right * (width * lineSize));
        Gizmos.DrawRay(bl, Vector2.up * (height * lineSize));

        Gizmos.DrawRay(tl, Vector2.right * (width * lineSize));
        Gizmos.DrawRay(tl, Vector2.down * (height * lineSize));

        Gizmos.DrawRay(tr, Vector2.left * (width * lineSize));
        Gizmos.DrawRay(tr, Vector2.down * (height * lineSize));

        Gizmos.DrawRay(br, Vector2.left * (width * lineSize));
        Gizmos.DrawRay(br, Vector2.up * (height * lineSize));
    }
}