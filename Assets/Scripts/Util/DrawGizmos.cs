using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    enum GizmosType { Cube, Sphere, Line }
    enum DrawType { None, Always, Selected }
    
    [SerializeField] GizmosType gizmosType;
    [SerializeField] DrawType drawType;
    [SerializeField] Color color = Color.yellow;
    
    [SerializeField, ShowIf("isCube")] Vector3 cubeSize = Vector3.one;
    [SerializeField, ShowIf("isSphere")] float sphereSize = 1;
    [SerializeField, ShowIf("isLine")] float lineSize = 1;
    [SerializeField, ShowIf("isLine")] Vector3 lineDirection = Vector2.up;

    bool isCube;
    bool isSphere;
    bool isLine;
    
    void OnValidate() {
        isCube = gizmosType == GizmosType.Cube;
        isSphere = gizmosType == GizmosType.Sphere;
        isLine = gizmosType == GizmosType.Line;
    }

    void OnDrawGizmos() {
        if (drawType is not DrawType.Always) return;
        Draw();
    }
    
    void OnDrawGizmosSelected() {
        if (drawType is not DrawType.Selected) return;
        Draw();
    }

    void Draw() {
        Gizmos.color = color;
        switch (gizmosType) {
            case GizmosType.Cube:
                Gizmos.DrawWireCube(transform.position, cubeSize);
                break;
            case GizmosType.Sphere:
                Gizmos.DrawWireSphere(transform.position, sphereSize);
                break;
            case GizmosType.Line:
                Gizmos.DrawLine(transform.position, transform.position + lineDirection * lineSize);
                break;
        }
    }
}