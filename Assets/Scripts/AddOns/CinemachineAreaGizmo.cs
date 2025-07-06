using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Unity.Cinemachine
{
    public class CinemachineAreaGizmo : CinemachineExtension
    {
        struct FrustumEdgePoints
        {
            public Vector3 botLeft;
            public Vector3 topLeft;
            public Vector3 topRight;
            public Vector3 botRight;
            public FrustumEdgePoints(Vector3 botLeft, Vector3 topLeft, Vector3 topRight, Vector3 botRight) {
                this.botLeft = botLeft;
                this.topLeft = topLeft;
                this.topRight = topRight;
                this.botRight = botRight;
            }
            public FrustumEdgePoints(Vector3 point) : this(point, point, point, point) { }
        }

        [Serializable]
        public struct CameraAreaByDistance
        {
            [ReadOnly] public float distance;
            public Color color;
            public bool drawEdgeLines;
            public Color edgeLinesColor;

            public CameraAreaByDistance(float distance) : this(distance, Color.white) { }
            public CameraAreaByDistance(Color color) : this(0, color) { }
            public CameraAreaByDistance(float distance, Color color) {
                this.distance = distance;
                this.color = color;
                drawEdgeLines = true;
                edgeLinesColor = Color.white;
            }
        }
        
        [SerializeField, InlineEditor, HideLabel] GameSettingsData gameSettingsData;
        [SerializeField] CameraAreaByDistance gameplayPlan;
        [SerializeField] CameraAreaByDistance farthestBackground;
        
        CinemachineCamera cinemachineCamera;
        CinemachinePositionComposer positionComposer;
        Camera mainCamera;

        void Reset() {
            cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            positionComposer = GetComponent<CinemachinePositionComposer>();
            mainCamera = Camera.main;
        }
        protected override void Awake() {
            base.Awake();
            if (!cinemachineCamera || !positionComposer) {
                cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
                positionComposer = GetComponent<CinemachinePositionComposer>();
            }
            
            mainCamera = Camera.main;
        }
        
        void OnDrawGizmos() => DrawGizmos();
        void DrawGizmos() {
            if (!enabled) return;
            
            if (!mainCamera)
                mainCamera = Camera.main;

            #if UNITY_EDITOR
            if (!gameSettingsData) 
                gameSettingsData = GameSettingsData.LoadDefaultSettings();
            #endif
            
            transform.position = new(transform.position.x, transform.position.y, gameSettingsData.camera.cameraZ);
            float pixelHeight = mainCamera.pixelHeight;
            float pixelWidth = mainCamera.pixelWidth;
            FrustumEdgePoints lastEdgePoints = new(transform.position);
            
            Gizmos.color = gameplayPlan.color;
            FrustumEdgePoints previousEdgePoints = lastEdgePoints;
            gameplayPlan.distance = Mathf.Abs(gameSettingsData.camera.cameraZ);
            lastEdgePoints = DrawCameraAreaByDistance(gameplayPlan.distance, pixelHeight, pixelWidth);
            
            if (gameplayPlan.drawEdgeLines) {
                Gizmos.color = gameplayPlan.edgeLinesColor;
                Gizmos.DrawLine(previousEdgePoints.botLeft, lastEdgePoints.botLeft);
                Gizmos.DrawLine(previousEdgePoints.topLeft, lastEdgePoints.topLeft);
                Gizmos.DrawLine(previousEdgePoints.topRight, lastEdgePoints.topRight);
                Gizmos.DrawLine(previousEdgePoints.botRight, lastEdgePoints.botRight);    
            }
            
            Gizmos.color = farthestBackground.color;
            previousEdgePoints = lastEdgePoints;
            farthestBackground.distance = Mathf.Abs(gameSettingsData.starBackgroundShaderZ);
            lastEdgePoints = DrawCameraAreaByDistance(farthestBackground.distance, pixelHeight, pixelWidth);
            
            if (farthestBackground.drawEdgeLines) {
                Gizmos.color = farthestBackground.edgeLinesColor;
                Gizmos.DrawLine(previousEdgePoints.botLeft, lastEdgePoints.botLeft);
                Gizmos.DrawLine(previousEdgePoints.topLeft, lastEdgePoints.topLeft);
                Gizmos.DrawLine(previousEdgePoints.topRight, lastEdgePoints.topRight);
                Gizmos.DrawLine(previousEdgePoints.botRight, lastEdgePoints.botRight);    
            }
        }
        
        FrustumEdgePoints DrawCameraAreaByDistance(float distance, float pixelHeight, float pixelWidth) {
            Vector3 bl = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, distance));
            Vector3 tl = mainCamera.ScreenToWorldPoint(new Vector3(0, pixelHeight, distance));
            Vector3 tr = mainCamera.ScreenToWorldPoint(new Vector3(pixelWidth, pixelHeight, distance));
            Vector3 br = mainCamera.ScreenToWorldPoint(new Vector3(pixelWidth, 0, distance));

            float height = Mathf.Abs(bl.y-tl.y);
            float width = Mathf.Abs(bl.x-br.x);
            float lineSize = 0.3f;
            
            Gizmos.DrawRay(bl, Vector2.right * (width * lineSize));
            Gizmos.DrawRay(bl, Vector2.up * (height * lineSize));
            
            Gizmos.DrawRay(tl, Vector2.right * (width * lineSize));
            Gizmos.DrawRay(tl, Vector2.down * (height * lineSize));
            
            Gizmos.DrawRay(tr, Vector2.left * (width * lineSize));
            Gizmos.DrawRay(tr, Vector2.down * (height * lineSize));
            
            Gizmos.DrawRay(br, Vector2.left * (width * lineSize));
            Gizmos.DrawRay(br, Vector2.up * (height * lineSize));
            
            return new(bl, tl, tr, br);
        }
    }
}