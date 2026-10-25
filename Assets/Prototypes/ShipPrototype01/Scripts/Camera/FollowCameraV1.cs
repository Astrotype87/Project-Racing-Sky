using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    // First Version of Ship Camera
    [ExecuteInEditMode]
    public class FollowCameraV1 : MonoBehaviour
    {
        // REFERENCES
        public bool LIVE_UPDATE;
        public Transform cameraTransform;
        public Transform pivotTransform;
        public ShipPhysicsV2 shipPhysics;
        
        [Header("Camera View")]
        public CameraView closeView;
        public CameraView farView;
        public CameraView cockpitView;
        public ViewMode viewMode;
        
        [Header("Camera Damping")]
        public Vector3 offsetDamp;
        public Vector3 offsetSmooth;
        public Vector3 orbitDamp;
        public Vector3 orbitSmooth;
        public Vector3 lookLagSpeed;
        
        [Header("Extra Setting")]
        public Vector3 extraOffset;
        public Vector3 extraRotation;
        public Vector3 extraOrbit;
        [Range(0, 10)] public float extraFOV;
        
        [Header("Camera Effects")]
        [Range(0, 100)] public float KPHPerFOV;
        
        [Header("Debug")]
        public bool debugCamera;
        public float currentFOV;
        [Range(0, 2)] public float timeScale;
        [Range(-1, 144)] public int frameLimiter;
        
        
        // PRIVATE VARIABLES
        private Camera mainCamera;
        private CameraView currentCameraView;
        private bool enableSideOffset;
        private bool isFOVDynamic;
        private Vector3 offsetToPivot;
        private Vector3 targetCamPosition;
        private Quaternion targetCamRotation;
        
        // FIXED UPDATE VELOCITY
        [SerializeField] [Range(0, 1)] private float fixedUpdateLapse;
        private bool hasStarted;
        private Vector3 p_targetPosition;
        private Vector3 p_targetVelocity;
        private Vector3 targetVelocity;
        private Vector3 targetVelocityLerp;
        private Quaternion p_targetRotation;
        private Quaternion p_targetAngularVelocity;
        private Quaternion targetAngularVelocity;
        private Quaternion targetAngularVelocityLerp;
        
        // OFFSET DAMPING
        private Vector3 targetOffset;
        private Vector3 currentOffset;
        private Vector3 offsetDampVelocity;
        
        // ORBIT DAMPING
        [SerializeField] private Vector3 targetOrbitFromQuat;
        [SerializeField] private Vector3 targetOrbit;
        [SerializeField] private Vector3 currentOrbit;
        [SerializeField] private Vector3 orbitDampVelocity;
        
        private float targetSideVelocity;
        Vector3 sideOffset;
        [SerializeField, Range(-100, 100)] private float targetSideOffset;
        [SerializeField, Range(-100, 100)] private float currentSideOffset;
        private float sideOffsetVelocity;
        private bool isEditorUpdate => Application.isPlaying || LIVE_UPDATE;
        
        
        // ENUMS, STRUCT & CONSTRUTOR
        public enum ViewMode {Close, Far, Cockpit}
        
        [Serializable]
        public struct CameraView
        {
            public Transform target;
            public Vector3 offset;
            public Vector3 rotation;
            public float pivotDistance;
            [Range(1e-05f, 179)] public float baseFOV;
        }
        
        public FollowCameraV1()
        {
            closeView.offset = new Vector3(0, 3.25f, -10);
            closeView.rotation = new Vector3(10, 0, 0);
            closeView.pivotDistance = 25;
            closeView.baseFOV = 78;
            
            farView.offset = new Vector3(0, 5f, -12.5f);
            farView.rotation = new Vector3(10, 0, 0);
            farView.pivotDistance = 25;
            farView.baseFOV = 78;
            
            cockpitView.offset = new Vector3(0, 0.7f, -2.67f);
            cockpitView.rotation = new Vector3(0, 0, 0);
            cockpitView.pivotDistance = 0;
            cockpitView.baseFOV = 78;
            
            offsetDamp = new Vector3(0.05f, 0.015f, 0);
            offsetSmooth = new Vector3(0.15f, -0.75f, 0);
            orbitDamp = new Vector3(0, -1, 0);
            orbitSmooth = new Vector3(0, 0.25f, 0);
            
            timeScale = 1;
            KPHPerFOV = 100;
        }
        
        
        // MONOBEHAVIOUR METHODS
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (currentCameraView.target == null) currentCameraView = closeView;
            
            // Prevent sudden camera jolt (The camera jolt is only noticable if using damping systems with non-zero values)
            p_targetPosition = currentCameraView.target.position;
            p_targetRotation = currentCameraView.target.rotation;
            p_targetAngularVelocity = Quaternion.Euler(Vector3.zero);
        }
        
        private void Update()
        {
            if (!isEditorUpdate) return;
            
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            if (Input.GetKeyDown(KeyCode.C))
                viewMode = (ViewMode) (((int) viewMode == 2) ? 0 : (int) viewMode + 1);
            
            enableSideOffset = true;
            isFOVDynamic = true;
            
            switch (viewMode)
            {
                case ViewMode.Close:
                    currentCameraView = closeView; break;
                case ViewMode.Far:
                    currentCameraView = farView; break;
                case ViewMode.Cockpit:
                    currentCameraView = cockpitView;
                    enableSideOffset = false;
                    isFOVDynamic = false;
                    break;
            }
        }
        
        private void FixedUpdate()
        {
            if (!isEditorUpdate) return;
            
            if (!Application.isPlaying) return;
            UpdateTargetVelocity(currentCameraView);
        }
        
        private void LateUpdate()
        {
            if (!isEditorUpdate) return;
            
            if (Application.isPlaying)
            {
                // Time scale & frame rate
                Time.timeScale = timeScale;
                Application.targetFrameRate = frameLimiter;
                
                UpdateInterpolation(currentCameraView);
            }
            
            UpdateFollowCamera(currentCameraView, enableSideOffset);
            UpdateFOV(currentCameraView, isFOVDynamic);
        }
        
        private void OnDrawGizmos()
        {
            if (!debugCamera) return;
            switch (viewMode)
            {
                case ViewMode.Close: DrawCameraGizmos(closeView); break;
                case ViewMode.Far: DrawCameraGizmos(farView); break;
                case ViewMode.Cockpit: DrawCameraGizmos(cockpitView); break;
            }
            
            void DrawCameraGizmos(CameraView view)
            {
                DrawLineSphere(view.target.position, view.target.TransformPoint(view.offset), new Color(1, 1, 1, 0.25f), new Color(1, 1, 1, 0.25f), 0.5f);
                DrawLineSphere(view.target.TransformPoint(view.offset), pivotTransform.position, new Color(1, 1, 1, 0.5f), Color.white, 0.5f);
                DrawLineSphere(pivotTransform.position, targetCamPosition, new Color(1, 1, 0, 0.5f), Color.blue, 0.25f);
            }
            
            void DrawLineSphere(Vector3 start, Vector3 end, Color c1, Color c2, float radius)
            {
                Gizmos.color = c1; Gizmos.DrawLine(start, end);
                Gizmos.color = c2; Gizmos.DrawSphere(end, radius);
            }
        }
        
        
        // CAMERA METHODS
        
        private void UpdateTargetVelocity(CameraView view)
        {
            // Calculate target object's (Fixed Update Based) velocity & angular velocity
            p_targetVelocity = targetVelocity;
            targetVelocity = (view.target.position - p_targetPosition) / Time.fixedDeltaTime;
            p_targetPosition = view.target.position;
            
            // Calculate target object's angular velocity
            if (hasStarted)
            p_targetAngularVelocity = targetAngularVelocity;
            targetAngularVelocity = view.target.rotation * Quaternion.Inverse(p_targetRotation);
            p_targetRotation = view.target.rotation;
            hasStarted = true;
        }
        
        private void UpdateInterpolation(CameraView view)
        {
            // Align LateUpdate() smoothing to FixedUpdate() based target velocity
            float Repeat(double t, double length) => (float) Math.Clamp(t - Math.Floor(t / length) * length, 0.0, length);
            fixedUpdateLapse = Repeat(Time.timeAsDouble, Time.fixedDeltaTime) / Time.fixedDeltaTime;
            
            // Interpolate sync the target velocity and angular velocity
            targetVelocityLerp = Vector3.Lerp(p_targetVelocity, targetVelocity, fixedUpdateLapse);
            targetAngularVelocityLerp = Quaternion.Lerp(p_targetAngularVelocity, targetAngularVelocity, fixedUpdateLapse);
        }
        
        private void UpdateFollowCamera(CameraView view, bool enableSideOffset)
        {
            // Offset damping
            targetOffset = new Vector3(
                Vector3.Dot(targetVelocityLerp, view.target.right),
                Vector3.Dot(targetVelocityLerp, view.target.up),
                Vector3.Dot(targetVelocityLerp, view.target.forward));
            currentOffset = new Vector3(
                Mathf.SmoothDamp(currentOffset.x, -targetOffset.x, ref offsetDampVelocity.x, offsetSmooth.x),
                Mathf.SmoothDamp(currentOffset.y, -targetOffset.y, ref offsetDampVelocity.y, offsetSmooth.y),
                Mathf.SmoothDamp(currentOffset.z, -targetOffset.z, ref offsetDampVelocity.z, offsetSmooth.z));
            Vector3 appliedOffsetDamp = Vector3.Scale(offsetDamp, enableSideOffset ? currentOffset : currentOffset * 0.0625f);
            
            // Orbit damping
            targetOrbitFromQuat = targetAngularVelocityLerp.eulerAngles;
            for (int i = 0; i < 3; i++)
            {
                if (targetOrbitFromQuat[i] > 180) targetOrbitFromQuat[i] -= 360;
                if (targetOrbitFromQuat[i] < -180) targetOrbitFromQuat[i] += 360;
            }
            targetOrbit = new Vector3(
                Vector3.Dot(targetOrbitFromQuat, view.target.right),
                Vector3.Dot(targetOrbitFromQuat, view.target.up),
                Vector3.Dot(targetOrbitFromQuat, view.target.forward));
            currentOrbit = new Vector3(
                Mathf.SmoothDampAngle(currentOrbit.x, targetOrbit.x, ref orbitDampVelocity.x, orbitSmooth.x),
                Mathf.SmoothDampAngle(currentOrbit.y, targetOrbit.y, ref orbitDampVelocity.y, orbitSmooth.y),
                Mathf.SmoothDampAngle(currentOrbit.z, targetOrbit.z, ref orbitDampVelocity.z, orbitSmooth.z));
            Vector3 appliedOrbitDamp = Vector3.Scale(orbitDamp, enableSideOffset ? currentOrbit : Vector3.zero);
            
            // Calculate camera offset, pivot and rotation
            offsetToPivot = view.target.rotation * Vector3.forward * view.pivotDistance;
            pivotTransform.position = view.target.TransformPoint(view.offset + appliedOffsetDamp + extraOffset) + offsetToPivot;
            pivotTransform.rotation = view.target.rotation * Quaternion.Euler(appliedOrbitDamp + extraOrbit);
            
            // Apply camera offset, pivot and rotation
            targetCamPosition = pivotTransform.TransformPoint(Vector3.forward * -view.pivotDistance);
            targetCamRotation = pivotTransform.rotation * Quaternion.Euler(view.rotation + extraRotation);
            cameraTransform.SetPositionAndRotation(targetCamPosition, targetCamRotation);
        }
        
        private void UpdateFOV(CameraView view, bool isFOVDynamic)
        {
            mainCamera.fieldOfView = !isFOVDynamic ? view.baseFOV :
                view.baseFOV + (shipPhysics.ForwardSpeedKPH / KPHPerFOV * 0.78f) + extraFOV;
            currentFOV = mainCamera.fieldOfView;
        }
    }
    
}
