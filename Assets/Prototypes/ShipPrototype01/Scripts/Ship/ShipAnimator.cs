using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    public class ShipAnimator : MonoBehaviour
    {
        public Transform shipMesh;
        public ShipInputScript shipInput;
        public ShipPhysicsV2 shipPhysics;
        public Transform airbrakeLeftMesh;
        public Transform airbrakeRightMesh;
        
        [Header("Steer Animation")]
        [Range(0, 90)] public float rollAngle;
        [Range(0, 1)] public float rollTime;
        [Range(0, 1)] public float rollSmooth;
        
        [Header("Hover Animation")]
        public float moveSpeed;
        public Vector3 moveOffset;
        public float rotateSpeed;
        public Vector3 rotateOffset;
        [Range(0, 1)] public float idleFadeInOut;
        
        [Header("Airbrake Animation")]
        [Range(0, 90)] public float airbrakeAngle;
        [Range(0, 4)] public float openSpeed;
        [Range(0, 4)] public float closeSpeed;
        [Range(0, 1)] public float airbrakeSmooth;
        
        // PRIVATE VARIABLES
        private float targetRollFactor;
        private float currentRollFactor;
        private float currentRollVelocity;
        
        private const float PI2 = Mathf.PI * 2;
        private float targetIntensity;
        private float hoverIntensity;
        private float hoverIntensityVelocity;
        
        private float moveTime;
        private float rotateTime;
        private Vector3 applyMove;
        private Vector3 applyRotate;
        private Vector3 applyRoll;
        
        private float smoothLeftAngle, smoothRightAngle;
        private float targetLeftAngle, targetRightAngle;
        private float p_targetLeftAngle, p_targetRightAngle;
        private float leftAngleVelocity, rightAngleVelocity;
        
        public ShipAnimator()
        {
            rollAngle = 30;
            rollTime = 0.1f;
            rollSmooth = 0.33f;
            
            moveSpeed = 0.5f;
            moveOffset = new Vector3(0.15f, 0.25f, 0.1f);
            rotateSpeed = 0.25f;
            rotateOffset = new Vector3(1, 0.5f, 2);
            idleFadeInOut = 0.75f;
            
            airbrakeAngle = 45;
            openSpeed = 0.25f;
            closeSpeed = 1f;
        }
        
        private void Awake()
        {
            moveTime = Random.Range(0, PI2);
            rotateTime = Random.Range(0, PI2);
        }
        
        private void Update()
        {
            UpdateShipAnimation();
            UpdateAirbrakeAnimation();
        }
        
        private void UpdateShipAnimation()
        {
            // Steering roll animation
            targetRollFactor = Mathf.MoveTowards(targetRollFactor, shipInput.SteerInput, Time.deltaTime / rollTime);
            currentRollFactor = Mathf.SmoothDamp(currentRollFactor, targetRollFactor, ref currentRollVelocity, rollSmooth);
            
            // The slower the ship, the stronger the hover animation
            targetIntensity = (1f - Mathf.Clamp01(shipPhysics.ShipSpeedKPH / 200)) * (shipPhysics.IsOnGround ? 1 : 0);
            hoverIntensity = Mathf.SmoothDamp(hoverIntensity, targetIntensity, ref hoverIntensityVelocity, idleFadeInOut);
            
            // Generate random number smoothly between -1 and 1
            // Uses 2d perlin noise, x and y position virtually moves in circle for seamless animation loop
            float timeGen(ref float t, float f, float r) => t += (t < PI2) ? Time.deltaTime * f / r : -t;
            float numGen(float t, float o, float r) => (Mathf.PerlinNoise((Mathf.Sin(t) + o) * r, (Mathf.Cos(t) + o) * r) - 0.5f) * 2;
            
            // Update move and rotate time
            timeGen(ref moveTime, moveSpeed, 100);
            timeGen(ref rotateTime, rotateSpeed, 100);
            
            // Calculate move and rotate offset, and roll
            applyMove = Vector3.Scale(new Vector3(numGen(moveTime, 1, 100), numGen(moveTime, 2, 100), numGen(moveTime, 3, 100)), moveOffset);
            applyRotate = Vector3.Scale(new Vector3(numGen(rotateTime, 4, 100), numGen(rotateTime, 5, 100), numGen(rotateTime, 6, 100)), rotateOffset);
            applyRoll = new Vector3(0, 0, rollAngle * -currentRollFactor);
            
            // Apply move and rotate hover animation, and ship roll animation
            shipMesh.localPosition = applyMove * hoverIntensity;
            shipMesh.localRotation = Quaternion.Euler(applyRotate * hoverIntensity + applyRoll);
        }
        
        private void UpdateAirbrakeAnimation()
        {
            if (airbrakeLeftMesh == null || airbrakeRightMesh == null) return; // Avoid null ref
            
            // Airbrake open and close speed
            float leftTime = targetLeftAngle >= p_targetLeftAngle ? openSpeed : closeSpeed;
            float rightTime = targetRightAngle >= p_targetRightAngle ? openSpeed : closeSpeed;
            p_targetLeftAngle = targetLeftAngle;
            p_targetRightAngle = targetRightAngle;
            
            // Calculate target airbrake angle
            targetLeftAngle = Mathf.MoveTowards(targetLeftAngle, shipInput.AirbrakeLeftInput, Time.deltaTime / leftTime);
            targetRightAngle = Mathf.MoveTowards(targetRightAngle, shipInput.AirbrakeRightInput, Time.deltaTime / rightTime);
            
            // Calculate smoothed airbrake angle
            smoothLeftAngle = Mathf.SmoothDamp(smoothLeftAngle, targetLeftAngle, ref leftAngleVelocity, airbrakeSmooth);
            smoothRightAngle = Mathf.SmoothDamp(smoothRightAngle, targetRightAngle, ref rightAngleVelocity, airbrakeSmooth);
            
            // Apply airbrake angle local rotation
            airbrakeLeftMesh.localRotation = Quaternion.Euler(smoothLeftAngle * -airbrakeAngle, 0, 0);
            airbrakeRightMesh.localRotation = Quaternion.Euler(smoothRightAngle * -airbrakeAngle, 0, 0);
        }
    }
    
}
