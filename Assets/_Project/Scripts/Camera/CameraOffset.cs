using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using System;
    using UnityEngine.Video;
    using Utility;
    
    // TODO: CameraOffset refactor this spaghetti code
    [DefaultExecutionOrder(ScriptOrder.CameraOffset)]
    public class CameraOffset : MonoBehaviour
    {
        [Header("Offset Effects")]
        [SerializeField] private OffsetLayer thrustOffset;
        [SerializeField] private OffsetLayer afterburnerOffset;
        [SerializeField] private OffsetLayer airbrakeOffset;
        [SerializeField] private OffsetLayer steerOffset;
        [SerializeField] private OffsetLayer strafeOffset;
        [SerializeField] private OffsetLayer abTorqueOffset;
        [Space(5)]
        [SerializeField] private OffsetLayer boostOffset;
        [SerializeField] private OffsetLayer speedPadOffset;
        
        [Header("References")]
        [SerializeField] private Ship.Ship targetShip;
        [SerializeField] private CameraData cameraData;
        
        
        // PRIVATE GETTERS
        private Ship.InputData inputData => targetShip.InputData;
        private Ship.MechanicsData mechanicsData => targetShip.MechanicsData;
        private Vector3 localVelocityKPH => targetShip.transform.InverseTransformVector(targetShip.Rigidbody.linearVelocity) * 3.6f;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            if (!targetShip) return;
            inputData.OnBoost += () => StartCoroutine(boostOffset.OnImpulse(1));
            inputData.OnSpeedPad += value => StartCoroutine(speedPadOffset.OnImpulse(speedPadFactor(value)));
            
            float speedPadFactor(Vector3 direction) => Mathf.Max(Vector3.Dot(targetShip.Transform.forward, direction), -0.5f);
        }
        
        private void LateUpdate()
        {
            if (!targetShip) { Debug.LogWarning("[CamFOV] Missing Ship reference!"); return; }
            UpdateCameraOffset();
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(Ship.Ship targetShip, CameraData cameraData)
        {
            this.targetShip = targetShip;
            this.cameraData = cameraData;
        }
        
        public void SetOffsetConfig(OffsetConfig offsetConfig)
        {
            this.thrustOffset = offsetConfig.thrustOffset;
            this.afterburnerOffset = offsetConfig.afterburnerOffset;
            this.airbrakeOffset = offsetConfig.airbrakeOffset;
            this.steerOffset = offsetConfig.steerOffset;
            this.strafeOffset = offsetConfig.strafeOffset;
            this.abTorqueOffset = offsetConfig.abTorqueOffset;
            this.boostOffset = offsetConfig.boostOffset;
            this.speedPadOffset = offsetConfig.speedPadOffset;
        }
        
        // CAMERA METHODS
        private void UpdateCameraOffset()
        {
            // Get ship factors
            float forwardSpeed = localVelocityKPH.z;
            float forwardFactor = Mathf.Clamp(forwardSpeed / 200, -1, 1).NaNInfSafe(1);
            
            float abDragInput = Mathf.Clamp01(inputData.airbrakeLeftInput + inputData.airbrakeRightInput - 1);
            float afterburnerInput = inputData.afterburnerInput;
            
            // Airbrake torque factor
            float abTorqueInput = inputData.airbrakeRightInput - inputData.airbrakeLeftInput;
            float abTorqueFactor = abTorqueInput * Mathf.Abs(inputData.steerInput) * forwardFactor;
            
            // Update offsets
            thrustOffset.UpdateOffset(inputData.thrustInput, Time.deltaTime);
            afterburnerOffset.UpdateOffset(afterburnerInput, Time.deltaTime);
            airbrakeOffset.UpdateOffset(abDragInput, Time.deltaTime);
            steerOffset.UpdateOffset(inputData.steerInput, Time.deltaTime);
            strafeOffset.UpdateOffset(inputData.strafeInput, Time.deltaTime);
            abTorqueOffset.UpdateOffset(abTorqueFactor, Time.deltaTime);
            boostOffset.UpdateOffset(0, Time.deltaTime);
            speedPadOffset.UpdateOffset(0, Time.deltaTime);
            
            // Update Y corrections
            steerOffset.UpdateYCorrection(1);
            abTorqueOffset.UpdateYCorrection(1);
            
            // Apply position offset
            cameraData.offsetPosition = thrustOffset.PosOffsetOutput + afterburnerOffset.PosOffsetOutput
                + airbrakeOffset.PosOffsetOutput + steerOffset.PosOffsetOutput + strafeOffset.PosOffsetOutput
                + abTorqueOffset.PosOffsetOutput + boostOffset.PosOffsetOutput + speedPadOffset.PosOffsetOutput;
            
            // Apply rotation offset
            cameraData.offsetRotation = thrustOffset.RotOffsetOutput + afterburnerOffset.RotOffsetOutput
                + airbrakeOffset.RotOffsetOutput + steerOffset.RotOffsetOutput + strafeOffset.RotOffsetOutput
                + abTorqueOffset.RotOffsetOutput + boostOffset.RotOffsetOutput + speedPadOffset.RotOffsetOutput;
        }
        
        
        
    }
}
