using System;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Ship;
    using Utility;
    
    [DefaultExecutionOrder(ScriptOrder.CameraDamping)]
    public class CameraDamping : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Camera Damping")]
        [SerializeField] private FollowDamping followDamping;
        [SerializeField] private OrbitDamping orbitDamping;
        
        [Header("Extra Options")]
        [SerializeField] private Vector3 startFollowOffset;     // For cosmetics, similar to wipeout after restarting the race
        [SerializeField] private Vector3 startOrbitOffset;      // The camera comes from different place on start
        [SerializeField] private Vector3 toggleFollowOffset;    // The camera comes from different place after
        [SerializeField] private Vector3 toggleOrbitOffset;     //   switching camera view from Cockpit to Close
        
        [Header("References")]
        [SerializeField] private Ship targetShip;
        [SerializeField] private CameraData cameraData;
        
        
        // PRIVATE GETTERS
        private Transform target => cameraData.cameraView.followModel ? targetShip.ModelTransform : targetShip.Transform;
        private bool isLockedToGround => targetShip.PhysicsData.isOnGround && targetShip.InputData.lockToGround;
        
        // PRIVATE VARIABLES
        private bool hasStarted;
        private bool resetDamping;
        private Vector3 resetFollowOffset;
        private Vector3 resetOrbitOffset;
        
        // FOLLOW DAMP
        private Vector3 followDampPosition;         // Damped target position in global coordinates
        private Vector3 followDampVelocity;         // Used in Mathf.SmoothDamp
        private Vector3 lastFollowDampOffset;       // Previous calculation of next follow damp offset
        private Vector3 nextFollowDampOffset;       // Local offset between follow damp position and target's position
        private Vector3 followDampOffset;           // Applied local follow damp offset, interpolated from last to next follow damp offset based on fixed update lapse
        
        // ORBIT DAMP
        private Vector3 orbitDampRotation;          // Damped target rotation in global rotation
        private Vector3 orbitDampVelocity;          // Used in Mathf.SmoothDampAngle
        private Vector3 lastOrbitDampOffset;        // Previous calculation of next orbit damp offset
        private Vector3 nextOrbitDampOffset;        // Local offset between orbit damp position and target's rotation
        private Vector3 orbitDampOffset;            // Applied local orbit damp offset, interpolated from last to next orbit damp offset based on fixed update lapse
        
        
        // CONSTRUCTOR
        public CameraDamping()
        {
            followDamping.SetDamping(0.1f, 0.05f, 0f);
            followDamping.SetScale(0.25f, 1f, 1f);
            followDamping.enabled = true;
            
            orbitDamping.SetDamping(0.1f, 0.25f, 0.15f);
            orbitDamping.SetScale(0.05f, 0.08f, 0.5f);
            orbitDamping.enabled = true;
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            resetDamping = true;
            SetDampingReset(startFollowOffset, startOrbitOffset);
        }
        
        private void FixedUpdate()
        {
            if (!targetShip) return;
            UpdateFollowDamping();
            UpdateOrbitDamping();
            
            if (!hasStarted) hasStarted = true;
        }
        
        private void LateUpdate()
        {
            if (!targetShip) return;
            UpdateInterpolation();
            UpdateCameraDamping();
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(Ship targetShip, CameraData cameraData)
        {
            this.targetShip = targetShip;
            this.cameraData = cameraData;
        }
        
        public void SetDampingConfig(DampingConfig dampingConfig)
        {
            this.followDamping = dampingConfig.followDamping;
            this.orbitDamping = dampingConfig.orbitDamping;
        }
        
        public void SetDampingReset(Vector3 followOffset, Vector3 orbitOffset)
        {
            resetDamping = true;
            
            followDampPosition = target.position + followOffset;
            orbitDampRotation = target.eulerAngles + orbitOffset;
            
            resetFollowOffset = followOffset;
            resetOrbitOffset = orbitOffset;
        }
        
        public void SetDampingReset()
        {
            SetDampingReset(toggleFollowOffset, toggleOrbitOffset);
        }
        
        
        // CAMERA METHODS
        private void UpdateFollowDamping()
        {
            // Store previously calculated nextFollowDampOffset to be used in interpolation
            if (hasStarted) lastFollowDampOffset = nextFollowDampOffset;
            
            // Convert damped position to local position
            followDampPosition = target.InverseTransformPoint(followDampPosition);
            // Apply damping in three separate axis relative to ship direction
            followDampPosition = new Vector3(
                Mathf.SmoothDamp(followDampPosition.x, 0.0f, ref followDampVelocity.x, followDamping.damping.x),
                Mathf.SmoothDamp(followDampPosition.y, 0.0f, ref followDampVelocity.y, followDamping.damping.y),
                Mathf.SmoothDamp(followDampPosition.z, 0.0f, ref followDampVelocity.z, followDamping.damping.z));
            // Convert damped position back to global position
            followDampPosition = target.TransformPoint(followDampPosition);
            
            // Get global offset between target position and damped target position
            nextFollowDampOffset = followDampPosition - target.position;
            // Convert to local offset and filter each axis when ship is not locked to ground.
            nextFollowDampOffset = new Vector3(
                Vector3.Dot(nextFollowDampOffset, target.right),
                Vector3.Dot(nextFollowDampOffset, target.up),
                Vector3.Dot(nextFollowDampOffset, target.forward));
            
            // nextFollowDampOffset = new Vector3(
            //     Vector3.Dot(nextFollowDampOffset, IsLockedToGround ? Target.right :
            //         Vector3.Scale(Target.right, new Vector3(1, 0, 1))),
            //     Vector3.Dot(nextFollowDampOffset, IsLockedToGround ? Target.up :
            //         Vector3.Scale(Target.up, new Vector3(0, 1, 0))),
            //     Vector3.Dot(nextFollowDampOffset, Target.forward));
            
            // Scale the follow damp offset
            nextFollowDampOffset = Vector3.Scale(nextFollowDampOffset, followDamping.scale);
        }
        
        private void UpdateOrbitDamping()
        {
            // Store previously calculated nextOrbitDampOffset to be used in interpolation
            if (hasStarted) lastOrbitDampOffset = nextOrbitDampOffset;
            
            // Get offset between target rotation and damped target rotation, convert degree range to -180 to 180
            orbitDampRotation = (Quaternion.Euler(orbitDampRotation) * Quaternion.Inverse(target.rotation)).eulerAngles;
            for (int i = 0; i < 3; i++) orbitDampRotation[i] = Mathf.DeltaAngle(0, orbitDampRotation[i]);
            
            // Convert damped rotation to local rotation
            orbitDampRotation = target.InverseTransformDirection(orbitDampRotation);
            // Apply damping in three euler axis relative to global euler axis
            orbitDampRotation = new Vector3(
                Mathf.SmoothDampAngle(orbitDampRotation.x, 0.0f, ref orbitDampVelocity.x, orbitDamping.damping.x),
                Mathf.SmoothDampAngle(orbitDampRotation.y, 0.0f, ref orbitDampVelocity.y, orbitDamping.damping.y),
                Mathf.SmoothDampAngle(orbitDampRotation.z, 0.0f, ref orbitDampVelocity.z, orbitDamping.damping.z));
            // Convert damped rotation back to global rotation
            orbitDampRotation = target.TransformDirection(orbitDampRotation);
            
            // Sum up the target rotation and damped rotation offset
            orbitDampRotation = (Quaternion.Euler(orbitDampRotation) * target.rotation).eulerAngles;
            
            // Get offset between target rotation and damped target rotation, convert degree range to -180 to 180
            nextOrbitDampOffset = (Quaternion.Euler(orbitDampRotation) * Quaternion.Inverse(target.rotation)).eulerAngles;
            for (int i = 0; i < 3; i++) nextOrbitDampOffset[i] = Mathf.DeltaAngle(0, nextOrbitDampOffset[i]);
            // Convert damped rotation to local rotation
            nextOrbitDampOffset = target.InverseTransformDirection(nextOrbitDampOffset);
            // Scale the orbit damp offset
            nextOrbitDampOffset = Vector3.Scale(nextOrbitDampOffset, orbitDamping.scale);
        }
        
        private void UpdateInterpolation()
        {
            // Calculate the time passed since the last FixedUpdate() call
            float Repeat(double t, double length) => (float) Math.Clamp(t - Math.Floor(t / length) * length, 0.0, length);
            float fixedUpdateLapse = Repeat(Time.timeAsDouble, Time.fixedDeltaTime) / Time.fixedDeltaTime;
            
            // Interpolate from last to next follow damp offset based on fixed update lapse
            followDampOffset = !followDamping.enabled ? Vector3.zero:
                Vector3.Lerp(lastFollowDampOffset, nextFollowDampOffset, fixedUpdateLapse);
            
            // // Interpolate from last to next orbit damp offset based on fixed update lapse where it wraps around 360
            // if (orbitDamping.enabled)
            //     for (int i = 0; i < 3; i++) orbitDampOffset[i] = (viewMode == ViewMode.Cockpit ? 0 : 1) *
            //         Mathf.LerpAngle(lastOrbitDampOffset[i], nextFollowDampOffset[i], fixedUpdateLapse);
            // else orbitDampOffset = Vector3.zero;
            
            orbitDampOffset = !orbitDamping.enabled ? Vector3.zero:
                Vector3.Lerp(lastOrbitDampOffset, nextOrbitDampOffset, fixedUpdateLapse);
        }
        
        private void UpdateCameraDamping()
        {
            if (resetDamping) ResetDamping();
            
            cameraData.followDampOffset = followDampOffset;
            cameraData.orbitDampOffset = orbitDampOffset;
        }
        
        private void ResetDamping()
        {
            followDampPosition = target.position + resetFollowOffset;
            orbitDampRotation = target.eulerAngles + resetOrbitOffset;
            
            lastFollowDampOffset = resetFollowOffset;
            nextFollowDampOffset = resetFollowOffset;
            
            lastOrbitDampOffset = resetOrbitOffset;
            nextFollowDampOffset = resetFollowOffset;
            
            resetDamping = false;
        }
        
        
        
    }
    
}
