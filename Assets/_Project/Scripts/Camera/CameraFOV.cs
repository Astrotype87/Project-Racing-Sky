using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using System;
    using Utility;
    
    [DefaultExecutionOrder(ScriptOrder.CameraFOV)]
    public class CameraFOV : MonoBehaviour
    {
        [Header("Constant FOV Effect")]
        [SerializeField] private FOVLayer speedFOV;
        [SerializeField] private FOVLayer thrustFOV;
        [SerializeField] private FOVLayer afterburnerFOV;
        [SerializeField] private FOVLayer brakeFOV;
        
        [Header("Event FOV Effect")]
        [SerializeField] private FOVLayer boostFOV;
        [SerializeField] private FOVLayer speedPadFOV;
        
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
            
            // SUBSCRIBE FOV LAYERS
            inputData.OnBoost += () => StartCoroutine(boostFOV.OnImpulse(1));
            inputData.OnSpeedPad += value => StartCoroutine(speedPadFOV.OnImpulse(speedPadFactor(value)));
            
            float speedPadFactor(Vector3 direction) => Mathf.Max(Vector3.Dot(targetShip.Transform.forward, direction), -0.5f);
        }
        
        private void LateUpdate()
        {
            if (!targetShip) { Debug.LogWarning("[CamFOV] Missing Ship reference!"); return; }
            UpdateCameraFOV();
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(Ship.Ship targetShip, CameraData cameraData)
        {
            this.targetShip = targetShip;
            this.cameraData = cameraData;
        }
        
        public void SetFOVConfig(FOVConfig fOVConfig)
        {
            this.speedFOV = fOVConfig.speedFOV;
            this.thrustFOV = fOVConfig.thrustFOV;
            this.afterburnerFOV = fOVConfig.afterburnerFOV;
            this.brakeFOV = fOVConfig.brakeFOV;
            this.boostFOV = fOVConfig.boostFOV;
            this.speedPadFOV = fOVConfig.speedPadFOV;
        }
        
        
        // CAMERA METHODS
        private void UpdateCameraFOV()
        {
            // Get ship factors
            float forwardSpeed = localVelocityKPH.z;
            float abDragInput = Mathf.Clamp01(inputData.airbrakeLeftInput + inputData.airbrakeRightInput - 1);
            float afterburnerInput = inputData.afterburnerInput;
            
            // Calculate current FOVs
            float currentSpeedFOV = this.speedFOV.UpdateFOV(forwardSpeed, Time.deltaTime);
            float currentThrustFOV = this.thrustFOV.UpdateFOV(inputData.thrustInput, Time.deltaTime);
            float currentAftbnrFOV = this.afterburnerFOV.UpdateFOV(afterburnerInput, Time.deltaTime);
            float currentBrakeFOV = this.brakeFOV.UpdateFOV(abDragInput, Time.deltaTime);
            
            // Calculate event-based FOVs
            float currentBoostFOV = this.boostFOV.UpdateFOV(0, Time.deltaTime);
            float currentSpeedPadFOV = this.speedPadFOV.UpdateFOV(0, Time.deltaTime);
            
            // Apply FOV
            float appliedNormalFOV = currentSpeedFOV + currentThrustFOV + currentAftbnrFOV - currentBrakeFOV;
            float appliedEventFOV = currentBoostFOV + currentSpeedPadFOV;
            cameraData.fOVOffset = appliedNormalFOV + appliedEventFOV;
        }
        
        
        
    }
}
