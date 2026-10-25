using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    [DefaultExecutionOrder(ScriptOrder.CameraShake)]
    public class CameraShake : MonoBehaviour
    {
        [Header("Camera Shake")]
        [SerializeField] private ShakeConstant idleShake;      // Applies only when stationary and on ground
        [SerializeField] private ShakeConstant speedShake;     // Shake increases as the ship go faster
        // [SerializeField] private ShakeConstant scrapeShake;    // Applies when ship is scraping walls
        [Space(5)]
        [SerializeField] private ShakeImpulse jumpShake;
        // [SerializeField] private ShakeComponent collisionShake; // Applies when ship collides
        
        [Header("Editor")]
        public bool alwaysRefreshInspector;
        
        [Header("References")]
        [SerializeField] private Ship.Ship targetShip;
        [SerializeField] private CameraData cameraData;
        
        
        // PRIVATE GETTERS
        private Ship.InputData inputData => targetShip.InputData;
        private Vector3 localVelocityKPH => targetShip.transform.InverseTransformVector(targetShip.Rigidbody.linearVelocity) * 3.6f;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            if (!targetShip) { Debug.LogWarning("[CamShake] Missing Ship reference!"
                + " Will not respond to InputData events until Ship reference is set before playmode start."); return; }
            inputData.OnJump += () => StartCoroutine(jumpShake.OnImpulse(1));
            inputData.OnJumpPad += value => StartCoroutine(jumpShake.OnImpulse(1));
        }
        
        private void LateUpdate()
        {
            if (!targetShip) { Debug.LogWarning("[CamShake] Missing Ship reference!"); return; }
            UpdateCameraShake();
        }
        
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                idleShake.Reset();
                speedShake.Reset();
            }
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(Ship.Ship targetShip, CameraData cameraData)
        {
            this.targetShip = targetShip;
            this.cameraData = cameraData;
        }
        
        public void SetShakeConfig(ShakeConfig shakeConfig)
        {
            this.idleShake = shakeConfig.idleShake;
            this.speedShake = shakeConfig.speedShake;
            this.jumpShake = shakeConfig.jumpShake;
        }
        
        
        // CAMERA METHODS
        private void UpdateCameraShake()
        {
            float abDragInput = Mathf.Clamp01(inputData.airbrakeLeftInput + inputData.airbrakeRightInput - 1);
            float abDragAmpFactor = 1 + abDragInput * 2;
            float abDragFreqFactor = 1 - abDragInput * 0.25f;
            
            // Update each shake profiles
            UpdateShakeConstant(idleShake, localVelocityKPH.z);
            UpdateShakeConstant(speedShake, localVelocityKPH.z, abDragAmpFactor, abDragFreqFactor);
            
            UpdateShakeImpulse(jumpShake);
            
            // Local method
            void UpdateShakeConstant(ShakeConstant shake, float unit, float ampMod = 1, float freqMod = 1)
            {
                shake.Update(Time.deltaTime, unit, ampMod, freqMod);
                cameraData.shakePosition += shake.PositionOutput;
                cameraData.shakeRotation += shake.RotationOutput;
            }
            
            void UpdateShakeImpulse(ShakeImpulse shake)
            {
                shake.Update(Time.deltaTime);
                cameraData.shakePosition += shake.PositionOutput;
                cameraData.shakeRotation += shake.RotationOutput;
            }
            
        }
        
        
        
    }
}
