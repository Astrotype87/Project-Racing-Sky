using System;
using UnityEngine;

namespace Archived
{
    // Updated Complete Version of Ship Physics
    // More stabilized, with advanced features
    public class ShipPhysicsV3 : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private GroundSensor groundSensor;
        [SerializeField] private Transform[] hoverSensors;
        
        [Header("States")]
        [SerializeField] private bool lockToGround;
        
        [Header("Components")]
        [SerializeField] private HoverModule hoverModule;
        [SerializeField] private GravityModule gravityModule;
        [SerializeField] private ThrustModule thrustModule;
        [SerializeField] private SteerModule steerModule;
        [SerializeField] private AirbrakeModule airbrakeModule;
        [SerializeField] private GripModule gripModule;
        [SerializeField] private PitchModule pitchModule;
        [SerializeField] private RollModule rollModule;
        [SerializeField] private JumpModule jumpModule;
        
        
        // PROPERTIES
        public InputData shipInput {private get; set;}
        public bool shipLockToGround {get => lockToGround;}
        public Vector3 shipPosition {get => transform.position;}
        public LayerMask shipGroundLayer {get => groundLayer;}
        public GroundSensor shipGroundSensor {get => groundSensor;}
        public HoverModule shipHoverModule {get => hoverModule;}
        public Transform[] shipHoverSensors {get => hoverSensors;}
        
        public enum HoverDirection{GroundNormal, HoverSensorDirection}
        public enum GravityMode{GroundNormal, GlobalDown}
        [Serializable] public class GroundSensor
        {
            [SerializeField] private Transform groundSensor;
            [SerializeField] private float sensorRange = 8f;
            
            public Transform sensor {get => groundSensor;}
            public Vector3 position {get => groundSensor.position;}
            public Vector3 direction {get => -groundSensor.up;}
            public Vector3 normal {get => sensorHit.normal;}
            public Vector3 point {get => sensorHit.point;}
            public float distance {get => sensorHit.distance;}
            public float range {get => sensorRange;}
            public bool isHit {get; private set;}
            public bool isOnGround {get; private set;}
            public Vector3 groundNormal {get => groundHit.normal;}
            public LayerMask groundLayer {get; private set;}
            
            private RaycastHit sensorHit;
            private RaycastHit groundHit;
            
            public void UpdateGroundSensor(LayerMask groundLayer)
            {
                this.groundLayer = groundLayer;
                isHit = Physics.Raycast(position, direction, out sensorHit, range, groundLayer);
                isOnGround = Physics.Raycast(position, -normal, out groundHit, range, groundLayer);
            }
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            gravityModule.hoverModule = hoverModule;
            hoverModule.Awake(hoverSensors.Length);
        }
        
        private void Update()
        {
            groundSensor.UpdateGroundSensor(groundLayer);
            UpdateInput();
        }
        
        private void FixedUpdate()
        {
            ApplyPhysics();
        }
        
        
        // CLASS METHODS
        private void ApplyPhysics()
        {
            hoverModule.ApplyHover(shipRigidbody, hoverSensors, groundLayer);
            gravityModule.ApplyGravity(shipRigidbody, hoverSensors, groundLayer, lockToGround);
            thrustModule.ApplyThrust(shipRigidbody);
            steerModule.ApplySteer(shipRigidbody);
            airbrakeModule.ApplyAirbrake(shipRigidbody);
            gripModule.ApplyGrip(shipRigidbody, groundSensor.isOnGround, lockToGround);
            pitchModule.ApplyPitch(shipRigidbody, groundSensor.isOnGround);
            rollModule.ApplyRoll(shipRigidbody, groundSensor.isOnGround, groundSensor.groundNormal);
            jumpModule.ApplyJump(shipRigidbody);
        }
        
        private void UpdateInput()
        {
            thrustModule.UpdateControl(shipInput.thrustInput);
            steerModule.UpdateControl(shipInput.steerInput);
            airbrakeModule.UpdateControl(shipInput.airbrakeLeftInput, shipInput.airbrakeRightInput);
            pitchModule.UpdateControl(shipInput.pitchInput);
            rollModule.UpdateControl(shipInput.rollInput);
            // jumpModule.UpdateControl(shipInput.jumpInput, groundSensor.isOnGround);
        }
        
        
        
        
        // SHIP PHYSICS COMPONENTS: HOVER, GRAVITY, THRUST, STEER, AIRBRAKE, GRIP, PITCH, TILT, JUMP
        [Serializable]
        public class HoverModule
        {
            [SerializeField] private float hoverRange;
            [SerializeField] private float hoverForce;
            [SerializeField] private float hoverDamp;
            [SerializeField] private HoverDirection hoverDirection;
            [SerializeField] private AnimationCurve hoverCurve;
            [SerializeField] [Range(0, 1)] float hoverSpread;
            
            public float shipHoverRange {get => hoverRange;}
            public float shipHoverSpread {get => hoverSpread;}
            public HoverDirection shipHoverDirection {get => hoverDirection;}
            
            private int hoverSensor_Count;
            private bool[] isHoverHit;
            private Vector3[] p_hoverSensor_Pos;
            
            public HoverModule()
            {
                hoverRange = 8f;
                hoverForce = 250;
                hoverDamp = 5;
                hoverDirection = 0;
                hoverCurve = new AnimationCurve(
                    new Keyframe(0, 0, 0, 1),
                    new Keyframe(0.5f, 0.5f, 1, 2),
                    new Keyframe(0.75f, 1, 2, 4),
                    new Keyframe(1, 2, 4, 0)
                );
                hoverSpread = 1;
            }
            
            public void Awake(int count)
            {
                isHoverHit = new bool[count];
                p_hoverSensor_Pos = new Vector3[count];
            }
            
            public void ApplyHover(Rigidbody shipRigidbody, Transform[] hoverSensors, LayerMask groundLayer)
            {
                hoverSensor_Count = hoverSensors.Length;
                for (int i = 0; i < hoverSensors.Length; i++)
                    HoverGenerator(i, shipRigidbody, hoverSensors, groundLayer);
            }
            
            private void HoverGenerator(int i, Rigidbody shipRigidbody, Transform[] hoverSensors, LayerMask groundLayer)
            {
                Vector3 hoverSensor_Pos = Vector3.Lerp(shipRigidbody.position, hoverSensors[i].position, hoverSpread);
                Vector3 hoverSensor_Dir = -hoverSensors[i].up;
                
                if (Physics.Raycast(hoverSensor_Pos, hoverSensor_Dir, out RaycastHit hoverSensor_Hit, hoverRange, groundLayer))
                    hoverSensor_Dir = (hoverDirection == 0) ? -hoverSensor_Hit.normal : hoverSensor_Dir;
                
                if (Physics.Raycast(hoverSensor_Pos, hoverSensor_Dir, out RaycastHit hoverHit, hoverRange, groundLayer))
                {
                    float hoverHit_Distance = hoverHit.distance;
                    float hoverHit_Percent = (hoverRange - hoverHit_Distance) / hoverRange;
                    
                    if (!isHoverHit[i]) p_hoverSensor_Pos[i] = hoverSensor_Pos;
                    float hoverHit_Velocity = Vector3.Dot(hoverSensor_Pos - p_hoverSensor_Pos[i], -hoverSensor_Dir) / Time.fixedDeltaTime;
                    
                    float appliedHoverForce = hoverForce * hoverCurve.Evaluate(hoverHit_Percent);
                    float appliedHoverDamp = hoverDamp * hoverHit_Velocity;
                    
                    float totalHoverForce = (appliedHoverForce - appliedHoverDamp) / hoverSensor_Count;
                    shipRigidbody.AddForceAtPosition(totalHoverForce * -hoverSensor_Dir, hoverSensor_Pos, ForceMode.Acceleration);
                    
                    isHoverHit[i] = true;
                    p_hoverSensor_Pos[i] = hoverSensor_Pos;
                }
                else
                {
                    isHoverHit[i] = false;
                }
            }
        }
        
        
        [Serializable]
        public class GravityModule
        {
            [SerializeField] private float groundGravity;
            [SerializeField] private float airGravity;
            [SerializeField] private GravityMode gravityMode;
            [SerializeField] [Range(0, 1)] private float gravitySpread;
            [SerializeField] [Range(0, 1)] private float hoverPower;
            
            [SerializeField] private bool gravityDebug;
            [SerializeField] private float gravityForce;
            
            public HoverModule hoverModule {private get; set;}
            private float hoverRange {get => hoverModule.shipHoverRange;}
            private HoverDirection hoverDirection {get => hoverModule.shipHoverDirection;}
            
            private bool lockToGround;
            private int hoverSensor_Count;
            private int hoverSensor_HitCount;
            
            public GravityModule()
            {
                groundGravity = 125;
                airGravity = 125;
                gravitySpread = 0f;
                
                hoverPower = 1;
                
                gravityDebug = false;
            }
            
            public void ApplyGravity(Rigidbody shipRigidbody, Transform[] hoverSensors, LayerMask groundLayer, bool lockToGround)
            {
                this.lockToGround = lockToGround;
                hoverSensor_Count = hoverSensors.Length;
                
                for (int i = 0; i < hoverSensor_Count; i++)
                    if (Physics.Raycast(hoverSensors[i].position, -hoverSensors[i].up, hoverRange, groundLayer))
                        hoverSensor_HitCount ++;
                
                for (int i = 0; i < hoverSensors.Length; i++)
                    GravityGenerator(i, shipRigidbody, hoverSensors, groundLayer);
                
                hoverSensor_HitCount = 0;
            }
            
            private void GravityGenerator(int i, Rigidbody shipRigidbody, Transform[] hoverSensors, LayerMask groundLayer)
            {
                Vector3 gravitySensor_Pos = Vector3.Lerp(shipRigidbody.position, hoverSensors[i].position, gravitySpread);
                Vector3 gravitySensor_Dir = -hoverSensors[i].up;
                
                if (Physics.Raycast(gravitySensor_Pos, gravitySensor_Dir, out RaycastHit hoverSensor_Hit, hoverRange, groundLayer))
                    gravitySensor_Dir = (hoverDirection == 0) ? -hoverSensor_Hit.normal : gravitySensor_Dir;
                
                gravityForce = Mathf.Lerp(airGravity, groundGravity, (float) hoverSensor_HitCount / hoverSensor_Count);
                
                if (Physics.Raycast(gravitySensor_Pos, gravitySensor_Dir, out RaycastHit hoverHit, hoverRange, groundLayer))
                {
                    float groundAngle = Vector3.Angle(-hoverHit.normal, Vector3.down);
                    float angleFactor = Mathf.Clamp(groundAngle, 0, 90) / 90;
                    
                    Vector3 groundGravity_Dir = lockToGround ? -hoverHit.normal : (gravityMode == 0) ?
                        Vector3.Lerp(-hoverHit.normal, Vector3.down, angleFactor) : Vector3.down;
                    Vector3 airGravity_Dir = Vector3.down;
                    
                    float totalGravityForce = Mathf.Lerp(airGravity, gravityForce, hoverPower) / hoverSensor_Count;
                    Vector3 totalGravityDirection = Vector3.Lerp(airGravity_Dir, groundGravity_Dir, hoverPower);
                    shipRigidbody.AddForceAtPosition(totalGravityForce * totalGravityDirection, gravitySensor_Pos, ForceMode.Acceleration);
                    
                    if (!gravityDebug) return;
                    Debug.Log("Normal: " + hoverHit.normal + " Ground Angle: " + groundAngle);
                    Debug.Log("Ground Gravity Direction: " + Vector3.Lerp(-hoverHit.normal, Vector3.down, angleFactor));
                }
                else
                {
                    float totalGravityForce = Mathf.Lerp(airGravity, gravityForce, hoverPower) / hoverSensor_Count;
                    shipRigidbody.AddForce(totalGravityForce * Vector3.down, ForceMode.Acceleration);
                }
            }
        }
        
        
        [Serializable]
        public class ThrustModule
        {
            [SerializeField] private float thrustForce;
            [SerializeField] private float thrustSpeed;
            [SerializeField] private float thrustStall;
            [Space(5)]
            [SerializeField] [Range(-1, 1)] private float thrustControl;
            [SerializeField] [Range(0, 2)] private float thrustAttack;
            [SerializeField] [Range(0, 2)] private float thrustRelease;
            
            private const float KphFactor = 3.6f;
            private float p_thrustControl;
            
            public ThrustModule()
            {
                thrustForce = 225;
                thrustSpeed = 600;
                thrustStall = 500;
                
                thrustAttack = 1f;
                thrustRelease = 0.5f;
            }
            
            public void UpdateControl(float thrustInput)
            {
                float thrustTime = Mathf.Abs(thrustControl) >= Mathf.Abs(p_thrustControl) ? thrustAttack : thrustRelease;
                p_thrustControl = thrustControl;
                thrustControl = Mathf.MoveTowards(thrustControl, thrustInput, Time.deltaTime / thrustTime);
            }
            
            public void ApplyThrust(Rigidbody shipRigidbody)
            {
                Vector3 thrustDirection = shipRigidbody.transform.forward;
                float currentThrustSpeed = Vector3.Dot(shipRigidbody.linearVelocity * KphFactor, thrustDirection);
                
                float dragFactor = currentThrustSpeed / thrustSpeed;
                float stallFactor = currentThrustSpeed / 1000;
                
                float appliedThrustForce = thrustForce * thrustControl;
                float appliedThrustDrag = thrustForce * dragFactor * Mathf.Abs(thrustControl);
                float appliedThrustStall = thrustStall * stallFactor * (1 - Mathf.Abs(thrustControl));
                
                float totalThrustForce = (appliedThrustForce - appliedThrustDrag - appliedThrustStall) / KphFactor;
                shipRigidbody.AddForce(totalThrustForce * thrustDirection, ForceMode.Acceleration);
            }
        }
        
        
        [Serializable]
        public class SteerModule
        {
            [SerializeField] private float steerForce;
            [SerializeField] private float steerSpeed;
            [SerializeField] private float steerStall;
            [Space(5)]
            [SerializeField] [Range(-1, 1)] private float steerControl;
            [SerializeField] [Range(0, 2)] private float steerAttack;
            [SerializeField] [Range(0, 2)] private float steerRelease;
            
            private float p_steerControl;
            
            public SteerModule()
            {
                steerForce = 360;
                steerSpeed = 90;
                steerStall = 360;
                
                steerAttack = 0.1f;
                steerRelease = 0.1f;
            }
            
            public void UpdateControl(float steerInput)
            {
                float steerTime = Mathf.Abs(steerControl) >= Mathf.Abs(p_steerControl) ? steerAttack : steerRelease;
                p_steerControl = steerControl;
                steerControl = Mathf.MoveTowards(steerControl, steerInput, Time.deltaTime / steerTime);
            }
            
            public void ApplySteer(Rigidbody shipRigidbody)
            {
                Vector3 steerAxis = shipRigidbody.transform.up;
                float currentSteerSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, steerAxis);
                
                float dragFactor = currentSteerSpeed / steerSpeed;
                float stallFactor = currentSteerSpeed / 90;
                
                float appliedSteerForce = steerForce * steerControl;
                float appliedSteerDrag = steerForce * dragFactor * Mathf.Abs(steerControl);
                float appliedSteerStall = steerStall * stallFactor * (1 - Mathf.Abs(steerControl));
                
                float totalSteerForce = (appliedSteerForce - appliedSteerDrag - appliedSteerStall) * Mathf.Deg2Rad;
                shipRigidbody.AddTorque(totalSteerForce * steerAxis, ForceMode.Acceleration);
            }
        }
        
        
        [Serializable]
        public class AirbrakeModule
        {
            [SerializeField] private float airbrakeDrag;
            [SerializeField] private float airbrakeTorque;
            [SerializeField] private float maxDragAt;
            [SerializeField] private float maxTorqueAt;
            [Space(5)]
            [SerializeField] [Range(0, 1)] private float dragControl;
            [SerializeField] [Range(0, 2)] private float dragAttack;
            [SerializeField] [Range(0, 2)] private float dragRelease;
            [SerializeField] private AnimationCurve dragCurve;
            [SerializeField] [Range(-1, 1)] private float torqueControl;
            [SerializeField] [Range(0, 2)] private float torqueAttack;
            [SerializeField] [Range(0, 2)] private float torqueRelease;
            [SerializeField] private AnimationCurve torqueCurve;
            [Space(5)]
            [SerializeField] private bool airbrakeDebug;
            [Range(0, 1)] [SerializeField] private float dragInput;
            [Range(-1, 1)] [SerializeField] private float torqueInput;
            
            private const float KphFactor = 3.6f;
            private float p_dragControl;
            private float p_torqueControl;
            
            public AirbrakeModule()
            {
                airbrakeDrag = 1000;
                airbrakeTorque = 300;
                maxDragAt = 1000;
                maxTorqueAt = 1000;
                
                dragAttack = 0.0625f;
                dragRelease = 0.0625f;
                dragCurve = new AnimationCurve(
                    new Keyframe(0, 0, 0f, 0f),
                    new Keyframe(0.5f, 0, 0f, 2f),
                    new Keyframe(1, 1, 2f, 0f)
                );
                torqueAttack = 0.15f;
                torqueRelease = 0.15f;
                torqueCurve = new AnimationCurve(
                    new Keyframe(0, 0, 0f, 3f),
                    new Keyframe(1, 1, 0f, 0f)
                );
                
                airbrakeDebug = false;
            }
            
            public void UpdateControl(float airbrakeLeftInput, float airbrakeRightInput)
            {
                dragInput = dragCurve.Evaluate((airbrakeLeftInput + airbrakeRightInput) * 0.5f);
                torqueInput = airbrakeRightInput - airbrakeLeftInput;
                
                float dragTime = dragControl >= p_dragControl ? dragAttack : dragRelease;
                p_dragControl = dragControl;
                dragControl = Mathf.MoveTowards(dragControl, dragInput, Time.deltaTime / dragTime);
                
                float torqueTime = torqueControl >= p_torqueControl ? torqueAttack : torqueRelease;
                p_torqueControl = torqueControl;
                torqueControl = Mathf.MoveTowards(torqueControl, torqueInput, Time.deltaTime / torqueTime);
            }
            
            public void ApplyAirbrake(Rigidbody shipRigidbody)
            {
                Vector3 dragDirection = shipRigidbody.transform.forward;
                Vector3 torqueDirection = shipRigidbody.transform.up;
                
                float forwardSpeed = Vector3.Dot(shipRigidbody.linearVelocity * KphFactor, dragDirection);
                float dragFactor = forwardSpeed / maxDragAt;
                float torqueFactor = torqueCurve.Evaluate(forwardSpeed / maxTorqueAt);
                
                float appliedAirbrakeDrag = airbrakeDrag * dragFactor / KphFactor * dragControl;
                float appliedAirbrakeTorque = airbrakeTorque * torqueFactor * Mathf.Deg2Rad * torqueControl;
                
                shipRigidbody.AddForce(appliedAirbrakeDrag * -dragDirection, ForceMode.Acceleration);
                shipRigidbody.AddTorque(appliedAirbrakeTorque * torqueDirection, ForceMode.Acceleration);
                
                if (!airbrakeDebug) return;
                Debug.Log("Applied Airbrake Drag: " + appliedAirbrakeDrag + " Applied Airbrake Torque: " + appliedAirbrakeTorque);
            }
        }
        
        
        [Serializable]
        public class GripModule
        {
            [SerializeField] private float gripForce;
            [Space]
            [SerializeField] bool gripDebug;
            [SerializeField] bool gripGizmos;
            [SerializeField] private float rayOffset;
            [SerializeField] private float rayTimeFrame;
            
            private Vector3 gripPlane = new Vector3(1, 0, 1);
            
            public GripModule()
            {
                gripForce = 10;
                
                gripDebug = false;
                gripGizmos = true;
                rayOffset = 4;
                rayTimeFrame = 0.25f;
            }
            
            public void ApplyGrip(Rigidbody shipRigidbody, bool isGroundHit, bool lockToGround)
            {
                Vector3 gripDirection = isGroundHit && lockToGround ?
                    shipRigidbody.transform.right : Vector3.Scale(shipRigidbody.transform.right, gripPlane);
                float currentGripSpeed = Vector3.Dot(shipRigidbody.linearVelocity, gripDirection);
                
                float appliedGripForce = gripForce * currentGripSpeed;
                float totalGripForce = appliedGripForce;
                shipRigidbody.AddForce(totalGripForce * -gripDirection, ForceMode.Acceleration);
                
                if (!gripDebug) return;
                Debug.Log("Side Direction: " + shipRigidbody.transform.right + " Gated Side Direction: " + gripDirection);
            }
            
            public void DebugGrip(Rigidbody shipRigidbody, bool isGroundHit, bool lockToGround)
            {
                if (!gripGizmos) return;
                
                Vector3 gripDirection = isGroundHit && lockToGround ?
                    shipRigidbody.transform.right : Vector3.Scale(shipRigidbody.transform.right, gripPlane);
                float gripSpeed = Vector3.Dot(shipRigidbody.linearVelocity, gripDirection);
                Vector3 rayPos = shipRigidbody.transform.position;
                Vector3 rayDir = shipRigidbody.transform.right;
                
                if (gripSpeed >= 0)
                    rayPos += rayDir * rayOffset;
                else rayPos -= rayDir * rayOffset;
                
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(rayPos, rayDir * gripSpeed * rayTimeFrame);
            }
        }
        
        
        [Serializable]
        public class PitchModule
        {
            [SerializeField] private float pitchForce;
            [SerializeField] private float pitchSpeed;
            [SerializeField] private float pitchStall;
            [SerializeField] private float airPitchForce;
            [SerializeField] private float airPitchSpeed;
            [SerializeField] private float airPitchStall;
            
            [SerializeField] [Range(-1, 1)] private float pitchControl;
            [SerializeField] [Range(0, 2)] private float pitchAttack;
            [SerializeField] [Range(0, 2)] private float pitchRelease;
            
            [SerializeField] private bool holdPitchUp;
            [SerializeField] private bool holdPitchDown;
            
            private float p_pitchControl;
            
            public PitchModule()
            {
                pitchForce = 154f;
                pitchSpeed = 90;
                pitchStall = 360;
                airPitchForce = 120f;
                airPitchSpeed = 45;
                airPitchStall = 60;
                
                pitchAttack = 0.1f;
                pitchRelease = 0.1f;
            }
            
            public void UpdateControl(float pitchInput)
            {
                float pitchTime = Mathf.Abs(pitchControl) >= Mathf.Abs(p_pitchControl) ? pitchAttack : pitchRelease;
                p_pitchControl = pitchControl;
                pitchControl = Mathf.MoveTowards(pitchControl, pitchInput, Time.deltaTime / pitchTime);
                
                if (holdPitchUp && !holdPitchDown) pitchControl = -1;
                else if (holdPitchDown && !holdPitchUp) pitchControl = 1;
            }
            
            public void ApplyPitch(Rigidbody shipRigidbody, bool isOnGround)
            {
                Vector3 pitchAxis = shipRigidbody.transform.right;
                float currentPitchSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, pitchAxis);
                
                float usedPitchForce = isOnGround ? pitchForce : airPitchForce;
                float usedPitchSpeed = isOnGround ? pitchSpeed : airPitchSpeed;
                float usedPitchStall = isOnGround ? pitchStall : airPitchStall;
                
                float dragFactor = currentPitchSpeed / usedPitchSpeed;
                float stallFactor = currentPitchSpeed / 90;
                
                float appliedPitchForce = usedPitchForce * pitchControl;
                float appliedPitchDrag = usedPitchForce * dragFactor * Mathf.Abs(pitchControl);
                float appliedPitchStall = usedPitchStall * stallFactor * (1 - Mathf.Abs(pitchControl));
                
                float totalPitchForce = (appliedPitchForce - appliedPitchDrag - appliedPitchStall) * Mathf.Deg2Rad;
                shipRigidbody.AddTorque(totalPitchForce * pitchAxis, ForceMode.Acceleration);
            }
        }
        
        
        [Serializable]
        public class RollModule
        {
            [SerializeField] private float rollForce;
            [SerializeField] private float rollSpeed;
            [SerializeField] private float rollStall;
            [SerializeField] private bool groundOrient;
            [SerializeField] private bool airOrient;
            [SerializeField] private float orientForce;
            [SerializeField] private float orientDrag;
            // [SerializeField] private float tiltStall;
            [SerializeField] [Range(-1, 1)] private float rollControl;
            [SerializeField] [Range(0, 2)] private float rollAttack;
            [SerializeField] [Range(0, 2)] private float rollRelease;
            [Space(5)]
            [SerializeField] private bool tiltDebug;
            
            private float p_rollControl;
            
            public RollModule()
            {
                groundOrient = true;
                airOrient = false;
                
                
                orientForce = 45;
                orientDrag = 10;
                
                tiltDebug = false;
            }
            
            public void UpdateControl(float rollInput)
            {
                float rollTime = Mathf.Abs(rollControl) >= Mathf.Abs(p_rollControl) ? rollAttack : rollRelease;
                p_rollControl = rollControl;
                rollControl = Mathf.MoveTowards(rollControl, rollInput, Time.deltaTime / rollTime);
            }
            
            public void ApplyRoll(Rigidbody shipRigidbody, bool isOnGround, Vector3 groundNormal)
            {
                Vector3 rollAxis = shipRigidbody.transform.forward;
                float currentRollSpeed = Vector3.Dot(shipRigidbody.angularVelocity, rollAxis);
                
                Vector3 orientDirection = isOnGround ? groundNormal : Vector3.up;
                float upsideAngle = Vector3.Angle(shipRigidbody.transform.up, orientDirection);
                float rollAngle = Vector3.Angle(shipRigidbody.transform.right, orientDirection) - 90;
                
                float angleSign = (rollAngle == 0) ? 0 : rollAngle / Mathf.Abs(rollAngle);
                float angleFactor = (upsideAngle < 90) ? rollAngle : 90 * angleSign;
                if (upsideAngle == 180) angleFactor = 90;
                
                if (tiltDebug) Debug.Log("Upside Angle: " + upsideAngle + " Roll Angle: " + rollAngle + " Angle Factor: " + angleFactor);
                if ((isOnGround && !groundOrient) || (!isOnGround && !airOrient)) return;
                
                float appliedRollForce = orientForce * (angleFactor / 45);
                float appliedRollDrag = orientDrag * currentRollSpeed;
                
                float totalRollForce = appliedRollForce - appliedRollDrag;
                shipRigidbody.AddTorque(totalRollForce * rollAxis, ForceMode.Acceleration);
            }
        }
        
        
        [Serializable]
        public class JumpModule
        {
            [SerializeField] private float jumpForce;
            [SerializeField] private float jumpCooldown;
            [SerializeField] private bool airJump;
            [SerializeField] private bool jumpControl;
            
            [Space(5)]
            [SerializeField] private bool canJump;
            [SerializeField] private float timer;
            
            public JumpModule()
            {
                jumpForce = 67;
                jumpCooldown = 1.75f;
                airJump = false;
            }
            
            public void UpdateControl(bool jumpInput, bool isOnGround)
            {
                if (jumpInput && canJump) jumpControl = true;
                else canJump = (isOnGround || airJump) && timer <= 0;
                timer -= (timer <= 0) ? timer : Time.deltaTime;
            }
            
            public void ApplyJump(Rigidbody shipRigidbody)
            {
                if (jumpControl)
                {
                    jumpControl = false;
                    canJump = false;
                    timer += jumpCooldown;
                    
                    Vector3 jumpDirection = shipRigidbody.transform.up;
                    float totalJumpForce = jumpForce;
                    shipRigidbody.AddForce(totalJumpForce * jumpDirection, ForceMode.Impulse);
                }
            }
        }
        
        
    }

    
}
