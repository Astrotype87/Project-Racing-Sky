using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Archived
{
    // Basic version of ship camera 
    // Press 'C' to toggle between three view modes
    public class ToggleFollowCamera : MonoBehaviour
    {
        // REFERENCES
        public Transform cameraTransform;
        public ShipPhysicsV2 shipPhysics;
        
        [Header("Camera View")]
        public CameraView closeView;
        public CameraView farView;
        public CameraView cockpitView;
        public ViewMode viewMode;
        
        [Header("Camera Effects")]
        [Range(0, 100)] public float KPHPerFOV;
        
        [Header("Extra Settings")]
        public Vector3 extraOffset;
        public Vector3 extraOrbit;
        public Vector3 extraRotate;
        
        // PRIVATE
        private Camera mainCamera;
        private Vector3 pivotPoint;
        private Vector3 pivotOffset;
        private Vector3 cameraPosition;
        private Quaternion cameraRotation;
        
        
        // ENUMS, STRUCT & CONSTRUCTOR
        public enum ViewMode {Close, Far, Cockpit}
        
        [Serializable] public struct CameraView
        {
            public Transform target;
            public Vector3 offset;
            public Vector3 rotation;
            public float pivotDistance;
            [Range(1e-05f, 179)] public float baseFOV;
            
            public void SetCameraView(Vector3 offset, Vector3 rotation, float pivotDistance, float baseFOV)
            {
                this.offset = offset; this.rotation = rotation;
                this.pivotDistance = pivotDistance; this.baseFOV = baseFOV;
            }
        }
        
        public ToggleFollowCamera()
        {
            closeView.SetCameraView(new Vector3(0, 3.25f, -10), new Vector3(10, 0, 0), 25, 78);
            farView.SetCameraView(new Vector3(0, 5f, -12.5f), new Vector3(10, 0, 0), 25, 78);
            cockpitView.SetCameraView(new Vector3(0, 0.7f, -2.67f), new Vector3(0, 0, 0), 0, 78);
            
            KPHPerFOV = 100;
        }
        
        
        
        // MONOBEHAVIOUR METHODS
        
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                viewMode = (ViewMode) ((int) viewMode == 2 ? 0 : (int) viewMode + 1);
        }
        
        private void LateUpdate()
        {
            UpdateFollowCamera(GetCameraView(viewMode));
            UpdateFOV(GetCameraView(viewMode));
        }
        
        // CAMERA METHODS
        
        private CameraView GetCameraView(ViewMode viewMode)
        {
            switch(viewMode)
            {
                case ViewMode.Close: return closeView;
                case ViewMode.Far: return farView;
                case ViewMode.Cockpit: return cockpitView;
                default: return closeView;
            }
        }
        
        private void UpdateFollowCamera(CameraView view)
        {
            // Calculate pivot point and offset
            pivotPoint = Vector3.forward * view.pivotDistance;
            pivotOffset = pivotPoint + Quaternion.Euler(extraOrbit) * -pivotPoint;
            
            // Apply camera offset and rotation
            cameraPosition = view.target.TransformPoint(view.offset + pivotOffset + extraOffset);
            cameraRotation = view.target.rotation * Quaternion.Euler(view.rotation + extraOrbit + extraRotate);
            cameraTransform.SetPositionAndRotation(cameraPosition, cameraRotation);
        }
        
        private void UpdateFOV(CameraView view)
        {
            // Increase FOV depending on ship forward speed
            mainCamera.fieldOfView = viewMode == ViewMode.Cockpit ? view.baseFOV:
                view.baseFOV + (shipPhysics.ForwardSpeedKPH / KPHPerFOV * 0.78f);
        }
    }
    
}
