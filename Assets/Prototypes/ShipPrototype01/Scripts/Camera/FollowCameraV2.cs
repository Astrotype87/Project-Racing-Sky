using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Archived
{
    // Second iteration of ship follow camera
    public class FollowCameraV2 : MonoBehaviour
    {
        // REFERENCES
        public Transform cameraTransform;
        public ShipPhysicsV2 shipPhysics;
        
        [Header("Camera View")]
        public CameraView closeView;
        public CameraView farView;
        public CameraView cockpitView;
        public ViewMode viewMode;
        
        [Header("Camera Damping")]
        public FollowDamping followDamping;
        public OrbitDamping orbitDamping;
        
        [Header("Camera Effects")]
        [Range(0, 100)] public float KPHPerFOV;
        public bool debugCamera;
        
        [Header("Extra Options")]
        public Vector3 startFollowOffset;       // For cosmetics, similar to wipeout after restarting the race, the camera comes from the right
        public Vector3 startOrbitOffset;        // For cosmetics, similar to wipeout after restarting the race, the camera comes from the right
        public Vector3 extraOffset;
        public Vector3 extraOrbit;
        public Vector3 extraRotation;
        [Range(0, 2)] public float timeScale;
        
        // PROPERTIES
        private bool IsLockedToGround => shipPhysics.IsOnGround && shipPhysics.LockToGround;
        
        
        
        // PRIVATE VARIABLES
        private Camera mainCamera;
        private Vector3 pivotPoint;
        private Vector3 pivotOffset;
        private Vector3 cameraPosition;
        private Quaternion cameraRotation;
        private bool hasStarted;
        
        // FOLLOW DAMP
        private Vector3 followDampPosition;         // Damped target position in global coordinates
        private Vector3 followDampVelocity;         // Used in Mathf.SmoothDamp
        private Vector3 lastFollowDampOffset;       // Previous calculation of next follow damp offset
        private Vector3 nextFollowDampOffset;       // Local offset between follow damp position and target's position
        private Vector3 followDampOffset;           // Applied follow damp offset, interpolated from last to next follow damp offset based on fixed update lapse
        
        // ORBIT DAMP
        private Vector3 orbitDampRotation;          // Damped target rotation in global rotation
        private Vector3 orbitDampVelocity;          // Used in Mathf.SmoothDampAngle
        private Vector3 lastOrbitDampOffset;        // Previous calculation of next orbit damp offset
        private Vector3 nextOrbitDampOffset;        // Local offset between orbit damp position and target's rotation
        private Vector3 orbitDampOffset;            // Applied orbit damp offset, interpolated from last to next orbit damp offset based on fixed update lapse
        
        
        
        // ENUMS, STRUCTS & CONSTRUCTOR
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
        
        [Serializable] public struct FollowDamping
        {
            public bool enabled;
            [Range(0, 2)] public float xDamping;
            [Range(0, 2)] public float yDamping;
            [Range(0, 2)] public float zDamping;
            [Range(0, 1)] public float xScale;
            [Range(0, 1)] public float yScale;
            [Range(0, 1)] public float zScale;
            
            public Vector3 damping => new Vector3(xDamping, yDamping, zDamping);
            public Vector3 scale => new Vector3(xScale, yScale, zScale);
            
            public void SetDamping(float x, float y, float z) {xDamping = x; yDamping = y; zDamping = z;}
            public void SetScale(float x, float y, float z) {xScale = x; yScale = y; zScale = z;}
        }
        
        [Serializable] public struct OrbitDamping
        {
            public bool enabled;
            [Range(0, 2)] public float pitchDamping;
            [Range(0, 2)] public float yawDamping;
            [Range(0, 2)] public float rollDamping;
            [Range(0, 1)] public float pitchScale;
            [Range(0, 1)] public float yawScale;
            [Range(0, 1)] public float rollScale;
            
            public Vector3 damping => new Vector3(pitchDamping, yawDamping, rollDamping);
            public Vector3 scale => new Vector3(pitchScale, yawScale, rollScale);
            
            public void SetDamping(float pitch, float yaw, float roll) {pitchDamping = pitch; yawDamping = yaw; rollDamping = roll;}
            public void SetScale(float pitch, float yaw, float roll) {pitchScale = pitch; yawScale = yaw; rollScale = roll;}
        }
        
        public FollowCameraV2()
        {
            closeView.SetCameraView(new Vector3(0, 3.25f, -10), new Vector3(10, 0, 0), 50, 78);
            farView.SetCameraView(new Vector3(0, 5f, -12.5f), new Vector3(10, 0, 0), 50, 78);
            cockpitView.SetCameraView(new Vector3(0, 0.7f, -2.67f), new Vector3(0, 0, 0), 0, 78);
            
            followDamping.SetDamping(0.15f, 0.05f, 0f);
            followDamping.SetScale(0.55f, 0.4f, 1f);
            followDamping.enabled = true;
            
            orbitDamping.SetDamping(0.1f, 0.2f, 0f);
            orbitDamping.SetScale(0.1f, 0.2f, 1f);
            orbitDamping.enabled = false;
            
            KPHPerFOV = 100;
            timeScale = 1;
        }
        
        
        
        // MONOBEHAVIOUR METHODS
        
        private void Awake()
        {
            mainCamera = Camera.main;
            ResetDamping(startFollowOffset, startOrbitOffset);
        }
        
        private void Update()
        {
            // Toggle between three view modes when 'C' is pressed
            if (Input.GetKeyDown(KeyCode.C))
            {
                // Toggle viewmode
                viewMode = (ViewMode) ((int) viewMode == 2 ? 0 : (int) viewMode + 1);
                // If switching from close to far, don't reset damping
                if (viewMode != ViewMode.Far) ResetDamping(Vector3.zero, Vector3.zero);
            }
        }
        
        private void FixedUpdate()
        {
            UpdateFollowDamping(GetCameraView(viewMode));
            UpdateOrbitDamping(GetCameraView(viewMode));
            
            if (!hasStarted) hasStarted = true;
        }
        
        private void LateUpdate()
        {
            CameraView currentCameraView = GetCameraView(viewMode);
            
            Time.timeScale = timeScale;
            
            UpdateInterpolation(currentCameraView);
            UpdateFollowCamera(currentCameraView);
            UpdateFOV(currentCameraView);
        }
        
        private void OnDrawGizmos()
        {
            if (!debugCamera) return;
            DrawCameraGizmos(GetCameraView(viewMode));
            
            void DrawCameraGizmos(CameraView view)
            {
                DrawLineSphere(view.target.position, view.target.TransformPoint(view.offset), new Color(1, 1, 1, 0.25f), new Color(1, 1, 1, 0.25f), 0.5f);
                DrawLineSphere(view.target.TransformPoint(view.offset), view.target.TransformPoint(view.offset + pivotPoint), new Color(1, 1, 1, 0.5f), Color.white, 0.5f);
                DrawLineSphere(view.target.TransformPoint(view.offset + pivotPoint), cameraPosition, new Color(1, 1, 0, 0.5f), Color.blue, 0.25f);
            }
            
            void DrawLineSphere(Vector3 start, Vector3 end, Color c1, Color c2, float radius)
            {
                Gizmos.color = c1; Gizmos.DrawLine(start, end);
                Gizmos.color = c2; Gizmos.DrawSphere(end, radius);
            }
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
        
        private void ResetDamping(Vector3 startFollowOffset, Vector3 startOrbitOffset)
        {
            followDampPosition = GetCameraView(viewMode).target.position + startFollowOffset;
            lastFollowDampOffset = startFollowOffset;
            orbitDampRotation = GetCameraView(viewMode).target.eulerAngles + startOrbitOffset;
            lastOrbitDampOffset = startOrbitOffset;
        }
        
        private void UpdateFollowDamping(CameraView view)
        {
            // Store previously calculated nextFollowDampOffset to be used in interpolation
            if (hasStarted) lastFollowDampOffset = nextFollowDampOffset;
            
            // Convert damped postion to local position
            followDampPosition = view.target.InverseTransformPoint(followDampPosition);
            // Apply damping in three separate axis relative to ship direction
            followDampPosition = new Vector3(
                Mathf.SmoothDamp(followDampPosition.x, 0.0f, ref followDampVelocity.x, followDamping.damping.x),
                Mathf.SmoothDamp(followDampPosition.y, 0.0f, ref followDampVelocity.y, followDamping.damping.y),
                Mathf.SmoothDamp(followDampPosition.z, 0.0f, ref followDampVelocity.z, followDamping.damping.z));
            // Convert damped position back to global position
            followDampPosition = view.target.TransformPoint(followDampPosition);
            
            // Get global offset between target position and damped target position
            nextFollowDampOffset = followDampPosition - view.target.position;
            // Convert to local offset and filter each axis when ship is not locked to ground.
            nextFollowDampOffset = new Vector3(
                Vector3.Dot(nextFollowDampOffset, IsLockedToGround ? view.target.right :
                    Vector3.Scale(view.target.right, new Vector3(1, 0, 1))),
                Vector3.Dot(nextFollowDampOffset, IsLockedToGround ? view.target.up :
                    Vector3.Scale(view.target.up, new Vector3(0, 1, 0))),
                Vector3.Dot(nextFollowDampOffset, view.target.forward));
            // Scale the follow damp offset
            nextFollowDampOffset = Vector3.Scale(nextFollowDampOffset, followDamping.scale);
        }
        
        private void UpdateOrbitDamping(CameraView view)
        {
            // Store previously calculated nextOrbitDampOffset to be used in interpolation
            if (hasStarted) lastOrbitDampOffset = nextOrbitDampOffset;
            
            // Get offset between target rotation and damped target rotation, convert degree range to -180 to 180
            orbitDampRotation = (Quaternion.Euler(orbitDampRotation) * Quaternion.Inverse(view.target.rotation)).eulerAngles;
            for (int i = 0; i < 3; i++) orbitDampRotation[i] = Mathf.DeltaAngle(0, orbitDampRotation[i]);
            
            // Convert damped rotation to local rotation
            orbitDampRotation = view.target.InverseTransformDirection(orbitDampRotation);
            // Apply damping in three euler axis relative to global euler axis
            orbitDampRotation = new Vector3(
                Mathf.SmoothDampAngle(orbitDampRotation.x, 0.0f, ref orbitDampVelocity.x, orbitDamping.damping.x),
                Mathf.SmoothDampAngle(orbitDampRotation.y, 0.0f, ref orbitDampVelocity.y, orbitDamping.damping.y),
                Mathf.SmoothDampAngle(orbitDampRotation.z, 0.0f, ref orbitDampVelocity.z, orbitDamping.damping.z));
            // Convert damped rotation back to global rotation
            orbitDampRotation = view.target.TransformDirection(orbitDampRotation);
            
            // Sum up the target rotation and damped rotation offset
            orbitDampRotation = (Quaternion.Euler(orbitDampRotation) * view.target.rotation).eulerAngles;
            
            // Get offset between target rotation and damped target rotation, convert degree range to -180 to 180
            nextOrbitDampOffset = (Quaternion.Euler(orbitDampRotation) * Quaternion.Inverse(view.target.rotation)).eulerAngles;
            for (int i = 0; i < 3; i++) nextOrbitDampOffset[i] = Mathf.DeltaAngle(0, nextOrbitDampOffset[i]);
            // Convert damped rotation to local rotation
            nextOrbitDampOffset = view.target.InverseTransformDirection(nextOrbitDampOffset);
            // Scale the orbit damp offset
            nextOrbitDampOffset = Vector3.Scale(nextOrbitDampOffset, orbitDamping.scale);
        }
        
        private void UpdateInterpolation(CameraView view)
        {
            // Calculate the time passed since the last FixedUpdate() call
            float Repeat(double t, double length) => (float) Math.Clamp(t - Math.Floor(t / length) * length, 0.0, length);
            float fixedUpdateLapse = Repeat(Time.timeAsDouble, Time.fixedDeltaTime) / Time.fixedDeltaTime;
            
            // Interpolate from last to next follow damp offset based on fixed update lapse
            followDampOffset = !followDamping.enabled ? Vector3.zero:
                Vector3.Lerp(lastFollowDampOffset, nextFollowDampOffset, fixedUpdateLapse) * (viewMode == ViewMode.Cockpit ? 0 : 1);
            
            // // Interpolate from last to next orbit damp offset based on fixed update lapse where it wraps around 360
            // if (orbitDamping.enabled)
            //     for (int i = 0; i < 3; i++) orbitDampOffset[i] = (viewMode == ViewMode.Cockpit ? 0 : 1) *
            //         Mathf.LerpAngle(lastOrbitDampOffset[i], nextFollowDampOffset[i], fixedUpdateLapse);
            // else orbitDampOffset = Vector3.zero;
            
            orbitDampOffset = !orbitDamping.enabled ? Vector3.zero:
                Vector3.Lerp(lastOrbitDampOffset, nextOrbitDampOffset, fixedUpdateLapse) * (viewMode == ViewMode.Cockpit ? 0 : 1);
        }
        
        private void UpdateFollowCamera(CameraView view)
        {
            // Calculate pivot point and offset
            pivotPoint = Vector3.forward * view.pivotDistance;
            pivotOffset = pivotPoint + Quaternion.Euler(orbitDampOffset + extraOrbit) * -pivotPoint;
            
            // Apply camera offset and rotation
            cameraPosition = view.target.TransformPoint(view.offset + pivotOffset + followDampOffset + extraOffset);
            cameraRotation = view.target.rotation * Quaternion.Euler(view.rotation + orbitDampOffset + extraOrbit + extraRotation);
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
