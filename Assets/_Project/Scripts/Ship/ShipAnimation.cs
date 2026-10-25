using System;
using UnityEngine;
using PrimeTween;

namespace ProjectRacingSky.Ship
{
    using Utility;
    
    // TODO: ShipAnimation refactor this spaghetti code
    [DefaultExecutionOrder(ScriptOrder.ShipAnimation)]
    public class ShipAnimation : MonoBehaviour
    {
        // FIELDS & PROPERTIES
        #region INSPECTOR
        [Header("References")]
        [SerializeField] private GameObject shipCollider;
        
        [Header("Roll Animation")]
        [SerializeField] private AngleOffset steerRoll;
        [SerializeField] private AngleOffset strafeRoll;
        [SerializeField] private AngleOffset abTorqueRoll;
        
        [Header("Hover Animation")]
        [SerializeField] private Noise3D hoverPosition;
        [SerializeField] private Noise3D hoverRotation;
        [SerializeField] [Range(0, 1)] private float idleFadeInOut;
        
        [Header("Airbrake Animation")]
        [SerializeField] [Range(-90, 90)] private float airbrakeAngle;
        [SerializeField] [Range(0, 4)] private float airbrakeOpenTime;
        [SerializeField] [Range(0, 4)] private float airbrakeCloseTime;
        [SerializeField] [Range(0, 1)] private float airbrakeSmooth;
        
        [Header("Canopy Animation")]
        [SerializeField] [Range(0, 90)] private float canopyAngle;
        [SerializeField] [Range(0, 10)] private float canopyOpenTime;
        
        #endregion
        
        #region FIELDS
        
        // PRIVATE GETTERS
        private Transform modelTransform => shipModel.ModelTransform;
        private Transform[] airbrakeLeft => shipModel.AirbrakeLeft;
        private Transform[] airbrakeRight => shipModel.AirbrakeRight;
        private Transform[] airbrakeFull => shipModel.AirbrakeFull;
        private Transform canopy => shipModel.Canopy;
        
        // PRIVATE FIELDS: REFERENCES
        private ShipModel shipModel;
        private InputData inputData;
        private PhysicsData physicsData;
        private VectorData vectorData;
        
        // PRIVATE FIELDS: STEER
        private float targetSteerRoll;
        private float currentSteerRoll;
        private float currentSteerVelocity;
        private Vector3 appliedSteerRoll;
        
        // PRIVATE FIELDS: STRAFE
        private float targetStrafeRoll;
        private float currentStrafeRoll;
        private float currentStrafeVelocity;
        private Vector3 appliedStrafeRoll;
        
        // PRIVATE FIELDS: AIRBRAKE TORQUE ROLL
        private float targetAbTorqueRoll;
        private float currentAbTorqueRoll;
        private float currentAbTorqueVelocity;
        private Vector3 appliedAbTorqueRoll;
        
        // PRIVATE FIELDS: HOVER
        private float hoverAmount;
        private float hoverAmountVelocity;
        private Vector3 appliedHoverPos;
        private Vector3 appliedHoverRot;
        
        // PRIVATE FIELDS: AIRBRAKE
        private float smoothLeftAngle, smoothRightAngle;
        private float targetLeftAngle, targetRightAngle;
        private float p_targetLeftAngle, p_targetRightAngle;
        private float leftAngleVelocity, rightAngleVelocity;
        
        // PRIVATE FIELDS: CANOPY
        private bool isCanopyOpen;
        private bool isCanopyStillMoving;
        
        #endregion
        
        
        // CONSTRUCTOR & FINALIZER
        public ShipAnimation()
        {
            idleFadeInOut = 0.75f;
            
            airbrakeAngle = 45;
            airbrakeOpenTime = 0.25f;
            airbrakeCloseTime = 1f;
            airbrakeSmooth = 0.075f;
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            InitializeShipAnimation();
        }
        
        private void LateUpdate()
        {
            UpdateShipAnimation();
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(ShipModel shipModel, InputData inputData,
            PhysicsData physicsData, VectorData vectorData)
        {
            this.shipModel = shipModel;
            
            this.inputData = inputData;
            this.physicsData = physicsData;
            this.vectorData = vectorData;
        }
        
        public void SetShipModel(ShipModel shipModel)
        {
            this.shipModel = shipModel;
        }
        
        public void SetAnimationConfig(AnimationConfig animationConfig)
        {
            // TODO: Empty SetAnimationConfig
        }
        
        
        // SHIP ANIMATION
        private void InitializeShipAnimation()
        {
            inputData.OnCanopy += () => OpenCloseCanopyAnimation();
        }
        
        private void UpdateShipAnimation()
        {
            CalculateRollAnimation();
            CalculateHoverAnimation();
            CalculateAirbrakeAnimation();
            
            // Apply move and rotate hover animation, and ship roll animation
            Vector3 localPosition = appliedHoverPos * hoverAmount;
            Quaternion localRotation = Quaternion.Euler(appliedHoverRot * hoverAmount
                + appliedSteerRoll + appliedStrafeRoll + appliedAbTorqueRoll);
            modelTransform.SetLocalPositionAndRotation(localPosition, localRotation);
            
            if (shipCollider)
                shipCollider.transform.SetLocalPositionAndRotation(Vector3.zero, localRotation);
            
            // Apply airbrake angle local rotation
            for (int i = 0; i < airbrakeLeft.Length; i++)
                airbrakeLeft[i].localRotation = Quaternion.Euler(smoothLeftAngle * -airbrakeAngle, 0, 0);
            
            for (int i = 0; i < airbrakeRight.Length; i++)
                airbrakeRight[i].localRotation = Quaternion.Euler(smoothRightAngle * -airbrakeAngle, 0, 0);
        }
        
        
        // CALCULATE ANIMATION
        private void CalculateRollAnimation()
        {
            // TODO: Encapsulate into data structure, spaghetti code
            
            // Steer roll animation
            targetSteerRoll = Mathf.MoveTowards(targetSteerRoll, inputData.steerInput, Time.deltaTime / steerRoll.time);
            currentSteerRoll = Mathf.SmoothDamp(currentSteerRoll, targetSteerRoll, ref currentSteerVelocity, steerRoll.smooth);
            appliedSteerRoll = new Vector3(0, 0, steerRoll.angle * -currentSteerRoll);
            
            // Strafe roll animation
            targetStrafeRoll = Mathf.MoveTowards(targetStrafeRoll, inputData.strafeInput, Time.deltaTime / strafeRoll.time);
            currentStrafeRoll = Mathf.SmoothDamp(currentStrafeRoll, targetStrafeRoll, ref currentStrafeVelocity, strafeRoll.smooth);
            appliedStrafeRoll = new Vector3(0, 0, strafeRoll.angle * -currentStrafeRoll);
            
            // Airbrake torque roll animation
            float forwardFactor = Mathf.Clamp(vectorData.localVelocityKPH.z / 100, -1, 1).NaNInfSafe(1);
            float abTorqueInput = inputData.airbrakeRightInput - inputData.airbrakeLeftInput;
            float abTorqueFactor = abTorqueInput * Mathf.Abs(inputData.steerInput) * forwardFactor;
            
            targetAbTorqueRoll = Mathf.MoveTowards(targetAbTorqueRoll, abTorqueFactor, Time.deltaTime / abTorqueRoll.time);
            currentAbTorqueRoll = Mathf.SmoothDamp(currentAbTorqueRoll, targetAbTorqueRoll, ref currentAbTorqueVelocity, abTorqueRoll.smooth);
            appliedAbTorqueRoll = new Vector3(0, 0, abTorqueRoll.angle * -currentAbTorqueRoll);
        }
        
        private void CalculateHoverAnimation()
        {
            // Factors that changes hover animation intensity
            float speedFactor = 1f - Mathf.Clamp01(vectorData.localVelocityKPH.z / 200);
            float groundFactor = physicsData.isOnGround ? 1 : 0;
            float hoverFactor = physicsData.control.hover;
            
            // Smoothly damp towards target hover amount
            float targetHoverAmount = speedFactor * groundFactor * hoverFactor;
            hoverAmount = Mathf.SmoothDamp(hoverAmount, targetHoverAmount, ref hoverAmountVelocity, idleFadeInOut);
            
            appliedHoverPos = hoverPosition.GetValue(Time.timeAsDouble, 1, 1);
            appliedHoverRot = hoverRotation.GetValue(Time.timeAsDouble, 1, 1);
        }
        
        private void CalculateAirbrakeAnimation()
        {
            // TODO: Full airbrake animation
            
            // Avoid null reference
            if (airbrakeLeft == null || airbrakeRight == null) return;
            
            // Airbrake open and close speed
            float leftTime = targetLeftAngle >= p_targetLeftAngle ? airbrakeOpenTime : airbrakeCloseTime;
            float rightTime = targetRightAngle >= p_targetRightAngle ? airbrakeOpenTime : airbrakeCloseTime;
            p_targetLeftAngle = targetLeftAngle;
            p_targetRightAngle = targetRightAngle;
            
            // Calculate target airbrake angle
            targetLeftAngle = Mathf.MoveTowards(targetLeftAngle, inputData.airbrakeLeftInput, Time.deltaTime / leftTime);
            targetRightAngle = Mathf.MoveTowards(targetRightAngle, inputData.airbrakeRightInput, Time.deltaTime / rightTime);
            
            // Calculate smoothed airbrake angle
            smoothLeftAngle = Mathf.SmoothDamp(smoothLeftAngle, targetLeftAngle, ref leftAngleVelocity, airbrakeSmooth);
            smoothRightAngle = Mathf.SmoothDamp(smoothRightAngle, targetRightAngle, ref rightAngleVelocity, airbrakeSmooth);
        }
        
        
        // EVENT-BASED ANIMATION
        private void OpenCloseCanopyAnimation()
        {
            // Wait for canopy to finish opening/closing
            if (isCanopyStillMoving) return;
            
            if (!isCanopyOpen)
                Tween.LocalRotation(canopy, new Vector3(-canopyAngle, 0, 0), canopyOpenTime, Ease.OutSine)
                    .OnComplete(() => isCanopyStillMoving = false);
            else
                Tween.LocalRotation(canopy, new Vector3(0, 0, 0), canopyOpenTime, Ease.InOutQuad)
                    .OnComplete(() => isCanopyStillMoving = false);
            
            isCanopyOpen = !isCanopyOpen;
            isCanopyStillMoving = true;
        }
        
        
        // STRUCTS
        [Serializable]
        public class AngleOffset
        {
            [Range(0, 90)] public float angle;
            public float time;
            public float smooth;
        }
        
        
    }
}
