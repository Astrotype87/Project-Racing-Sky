using System.Collections;
using UnityEngine;

namespace Archived
{
    // A clean, complete ship physics code
    // One class for physics and one class with nested structs for physics settings
    public class ShipPhysics : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private HoverSensor[] hoverSensors = new HoverSensor[]{
            new HoverSensor(0, 0, 6,   0, 0, 0),
            new HoverSensor(0, 0, -6,  0, 0, 0),
            new HoverSensor(0, 0, 0,   0, 0, 0),
            new HoverSensor(0, 0, 3,   0, 0, 0),
            new HoverSensor(0, 0, -3,  0, 0, 0)
        };
        
        // CONFIGURATION FIELDS
        [Header("Ship Physics")]
        [SerializeField] private PhysicsConfig.Hover hover;
        [SerializeField] private PhysicsConfig.Gravity gravity;
        [SerializeField] private PhysicsConfig.Jump jump;
        [Space]
        [SerializeField] private PhysicsConfig.Thrust thrust;
        [SerializeField] private PhysicsConfig.Boost boost;
        [SerializeField] private PhysicsConfig.Afterburner afterburner;
        [SerializeField] private PhysicsConfig.Airbrake airbrake; 
        [Space]
        [SerializeField] private PhysicsConfig.Grip grip;
        [SerializeField] private PhysicsConfig.Strafe strafe;
        [SerializeField] private PhysicsConfig.Sideshift sideshift;
        [Space]
        [SerializeField] private PhysicsConfig.Steer steer;
        [SerializeField] private PhysicsConfig.Pitch pitch;
        [SerializeField] private PhysicsConfig.Roll roll;
        [SerializeField] private PhysicsConfig.Orient orient;
        
        [Header("Mechanic Physics")]
        [SerializeField] private bool canLockToGround; // FIXME: magnetLock switch, references
        [SerializeField] private MechanicConfig.MagnetLock magnetLock;
        [SerializeField] private MechanicConfig.SpeedPad speedPad;
        [SerializeField] private MechanicConfig.SpeedZone speedZone;
        [SerializeField] private MechanicConfig.SlowPad slowPad;
        [SerializeField] private MechanicConfig.StallZone stallZone;
        
        // CONTROL FIELDS
        [Header("Ship Control")]
        [SerializeField] [Range(0, 1)] private float hoverControl = 1;
        [SerializeField] [Range(0, 1)] private float jumpControl;
        [Space]
        [SerializeField] [Range(0, 1)] private float thrustControl;
        [SerializeField] [Range(0, 1)] private float reverseControl;
        [SerializeField] [Range(0, 1)] private float boostControl;
        [SerializeField] [Range(0, 1)] private float afterburnerControl;
        [Space]
        [SerializeField] [Range(0, 1)] private float abDragControl;
        [SerializeField] [Range(-1, 1)] private float abTorqueControl;
        [SerializeField] [Range(0, 1)] private float abDriftControl;
        [Space]
        [SerializeField] [Range(0, 100)] private float gripControl;
        [SerializeField] [Range(-1, 1)] private float strafeControl;
        [SerializeField] [Range(-1, 1)] private float sideshiftControl;
        [Space]
        [SerializeField] [Range(-1, 1)] private float steerControl;
        [SerializeField] [Range(-1, 1)] private float pitchControl;
        [SerializeField] [Range(-1, 1)] private float rollControl;
        
        [Header("Mechanic Control")]
        [SerializeField] [Range(0, 1)] private float magnetLockControl;
        [SerializeField] [Range(0, 1)] private float speedPadControl;
        [SerializeField] [Range(0, 1)] private float speedZoneControl;
        [SerializeField] [Range(0, 1)] private float slowPadControl;
        [SerializeField] [Range(0, 1)] private float stallZoneControl;
        
        
        // PROPERTIES - Other scripts can only control this ShipPhysics script through these properties
        public InputData InputData          {get; set;} = new InputData();
        public MechanicData MechanicData    {get; set;} = new MechanicData();
        public CollisionData CollisionData  {get; set;} = new CollisionData();
        public StateData StateData          {get; set;} = new StateData();
        public PhysicsData PhysicsData      {get; set;} = new PhysicsData();
        
        
        // FIXME: GETTERS >>>> ONLY ShipDebug script uses ALL THESE 7 GETTERS
        public Vector3 ShipPosition => shipRigidbody.position;  // ShipDebug should refer to VectorData for position
        public HoverSensor[] HoverSensors => hoverSensors;      // Consider moving them to new class PhysicsData INSTEAD
        public LayerMask GroundLayer => groundLayer;            // **
        public PhysicsConfig.Hover HoverConfig => hover;        // **
        public float HoverRange => _hoverRange;                 // **
        
        public bool IsOnGround => _isOnGround;              // TO BE REMOVED // Belongs to StateData
        public bool IsLockToGround => canLockToGround;      // TO BE REMOVED // MagnetLock, belongs to MechnicData
        
        
        // PRIVATE GETTERS
        private float _hoverRange => hover.height + hover.falloff;
        private bool _isOnGround => StateData.isOnGround;
        
        // PRIVATE FIELDS: VELOCITY
        private Vector3 _kinematicVelocity;
        private Vector3 _kinematicAngularVelocity;
        
        private float _boostVelocity;
        private float _sideshiftVelocity;
        
        private float _speedPadVelocity;
        private float _speedZoneVelocity;
        
        // PRIVATE FIELDS: INPUT
        private float _jumpInput;
        private float _boostInput;
        private float _sideshiftInput;
        
        private float _speedPadInput;
        private float _speedZoneInput;
        private float _slowPadInput;
        private float _stallZoneInput;
        private float _slowdownInput;
        
        // PRIVATE FIELDS : HOVER PHYSICS
        private int _hoverCount;
        private int _hoverHitCount;
        private float _hoverHitRatio;
        private Vector3[] _lastHoverPosition;
        private Vector3[] _hoverSensorTip;
        private Vector3[] _hoverNormal;
        
        // FIXME: move to PhysicsData
        // private bool[] _isHoverHit;
        // private float _hoverHitPressure;
        // [SerializeField] [Range(0, 1)] private float[] hoverPressure;
        
        // PRIVATE FIELDS : MAGNET LOCK
        private Vector3 _lastMagnetLockPosition;
        
        // PRIVATE FIELDS : BOOLEAN STATES
        [SerializeField] private bool _isSideshifting;
        [SerializeField] private bool _canJump;
        
        // PRIVATE CONSTANTS
        private const float KPHFactor = 3.6f;
        private const float DecelBaseKPH = 1000;
        private const float DecelBaseMPS = 10;
        private const float DecelBaseDPS = 90;
        
        
        
        // CONSTRUCTOR & FINALIZER
        public ShipPhysics()
        {
            hoverSensors = new HoverSensor[]{
                new HoverSensor(0, 0, 6,   0, 0, 0),
                new HoverSensor(0, 0, -6,  0, 0, 0),
                new HoverSensor(0, 0, 0,   0, 0, 0),
                new HoverSensor(0, 0, 3,   0, 0, 0),
                new HoverSensor(0, 0, -3,  0, 0, 0)
            };
            
            LoadPhysicsConfig(PhysicsConfig.GetDefaultSetings());
            LoadMechanicConfig(MechanicConfig.GetDefaultSettings());
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            InitializeShipPhysics();
        }
        
        private void Start()
        {
            RegisterToEvents();
            
            InputData.hoverInput = 1;
        }
        
        private void Update()
        {
            // TEMPORARY CODE
            if (Input.GetKeyDown(KeyCode.T))
            {
                MechanicData.magnetLockInput = MechanicData.magnetLockInput == 0 ? 1 : 0;
                Debug.Log("MAGNET LOCK " + (MechanicData.magnetLockInput == 0 ? "OFF" : "ON"));
            }
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                MechanicData.OnSpeedPad?.Invoke(1);
                Debug.Log("SPEED PAD HIT");
            }
            
            if (Input.GetKeyDown(KeyCode.U))
            {
                MechanicData.OnSlowPad?.Invoke(1);
                Debug.Log("SLOW PAD HIT");
            }
            
            MechanicData.speedZoneInput = Input.GetKey(KeyCode.I) ? 1 : 0;
            if (Input.GetKeyDown(KeyCode.I)) Debug.Log("SPEED ZONE ENTER");
            if (Input.GetKeyUp(KeyCode.I)) Debug.Log("SPEED ZONE EXIT");
            
            MechanicData.stallZoneInput = Input.GetKey(KeyCode.O) ? 1 : 0;
            if (Input.GetKeyDown(KeyCode.O)) Debug.Log("STALL ZONE ENTER");
            if (Input.GetKeyUp(KeyCode.O)) Debug.Log("STALL ZONE EXIT");
        }
        
        private void FixedUpdate()
        {
            UpdateShipPhysics();
            UpdateMechanicPhysics();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            // _kinematicVelocity += other.impulse;
            // _sideshiftVelocity += Vector3.Dot(other.impulse, transform.right);
        }
        
        private void OnCollisionStay(Collision other)
        {
            // _kinematicVelocity += other.impulse;
            // _sideshiftVelocity += Vector3.Dot(other.impulse, transform.right);
        }
        
        
        // PUBLIC METHODS
        public void LoadPhysicsConfig(PhysicsConfig config)
        {
            hover = config.hover;
            gravity = config.gravity;
            jump = config.jump;
            
            thrust = config.thrust;
            boost = config.boost;
            afterburner = config.afterburner;
            airbrake = config.airbrake;
            
            grip = config.grip;
            strafe = config.strafe;
            sideshift = config.sideshift;
            
            steer = config.steer;
            pitch = config.pitch;
            roll = config.roll;
            orient = config.orient;
        }
        
        public void LoadMechanicConfig(MechanicConfig config)
        {
            magnetLock = config.magnetLock;
            
            speedPad = config.speedPad;
            speedZone = config.speedZone;
            slowPad = config.slowPad;
            stallZone = config.stallZone;
        }
        
        public void InitializeShipPhysics()
        {
            _hoverCount = hoverSensors.Length;
            
            _lastHoverPosition = new Vector3[_hoverCount];
            for (int i = 0; i < _lastHoverPosition.Length; i++)
                _lastHoverPosition[i] = shipRigidbody.position + shipRigidbody.transform.TransformVector(hoverSensors[i].offset);
            
            _hoverSensorTip = new Vector3[_hoverCount];
            _hoverNormal = new Vector3[_hoverCount];
            
            // _isHoverHit = new bool[_hoverCount];
            // hoverPressure = new float[_hoverCount];
            
            _canJump = true;
        }
        
        public void ResetShipPhysics()
        {
            
        }
        
        public void RegisterToEvents()
        {
            InputData.OnJump += () => StartCoroutine(OnJump());
            InputData.OnBoost += () => StartCoroutine(OnBoost());
            InputData.OnSideshift += (value) => StartCoroutine(OnSideshift(value));
            InputData.OnRespawn += () => StartCoroutine(OnRespawn());
            
            MechanicData.OnSpeedPad += (value) => StartCoroutine(OnSpeedPad(value));
            MechanicData.OnSlowPad += (value) => StartCoroutine(OnSlowPad(value));
            MechanicData.OnSlowdown += (value) => StartCoroutine(OnSlowdown(value));
            
            CollisionData.OnWallHit += () => OnWallHit();
            CollisionData.OnFloorHit += () => OnFloorHit();
            CollisionData.OnStaticHit += () => OnStaticHit();
            CollisionData.OnDynamicHit += () => OnDynamicHit();
            CollisionData.OnShipHit += () => OnShipHit();
            CollisionData.OnProjectileHit += () => OnProjectileHit();
            
            StateData.OnShipDamage += (value) => OnShipDamage(value);
            StateData.OnShipDestroy += () => OnShipDestroy();
        }
        
        
        // SHIP PHYSICS METHODS
        private void UpdateShipPhysics()
        {
            ApplyHover(InputData.hoverInput);
            ApplyGravity();
            ApplyJump(_jumpInput);
            
            ApplyAirbrake(InputData.airbrakeLeftInput, InputData.airbrakeRightInput);
            ApplyThrust(InputData.thrustInput, InputData.reverseInput, InputData.boostInput);
            ApplyBoost(_boostInput);
            ApplyAfterburner(InputData.afterburnerInput);
            
            ApplyGrip();
            ApplyStrafe(InputData.strafeInput);
            ApplySideshift(_sideshiftInput);
            
            ApplySteer(InputData.steerInput);
            ApplyPitch(InputData.pitchInput);
            ApplyRoll(InputData.rollInput);
            ApplyOrient();
            
            // Debug.Log("Kinematic Velocity: " + _kinematicVelocity
            //     + "\tSideshift Velocity: " + _sideshiftVelocity);
        }
        
        // - ELEVATION
        private void ApplyHover(float hoverInput) // UPDATED
        {
            float hoverTime = hoverInput > hoverControl ? hover.gain : hover.release;
            hoverControl = Mathf.MoveTowards(hoverControl, hoverInput, Time.fixedDeltaTime / hoverTime);
            
            _hoverHitCount = 0;
            _hoverHitRatio = 0;
            
            for (int i = 0; i < _hoverCount; i++)
            {
                Vector3 hoverPosition = shipRigidbody.position + shipRigidbody.transform.TransformVector(hoverSensors[i].offset);
                Vector3 hoverDirection = hover.direction == PhysicsConfig.Direction.World ? Vector3.down :
                    Quaternion.Euler(hoverSensors[i].rotation) * -shipRigidbody.transform.up;
                
                if (hover.direction == PhysicsConfig.Direction.Ground)
                    if (Physics.Raycast(hoverPosition, hoverDirection, out RaycastHit sensorHit, _hoverRange, groundLayer))
                        hoverDirection = -sensorHit.normal;
                
                float hoverVelocity = Vector3.Dot(hoverPosition - _lastHoverPosition[i], -hoverDirection) / Time.fixedDeltaTime;
                
                if (Physics.Raycast(hoverPosition, hoverDirection, out RaycastHit hoverHit, _hoverRange, groundLayer))
                {
                    float ratio = Mathf.Clamp01((_hoverRange - hoverHit.distance) / hover.falloff); // hoverFalloff = hoverRange - hoverHeight
                    float amount = _hoverRange > hover.height ? ratio : 1;
                    float pressure = Mathf.Max(hover.height - hoverHit.distance, 0) / hover.height;
                    
                    float force = hover.force * amount;
                    float suspension = hover.suspension * hover.suspensionCurve.Evaluate(pressure) * hover.force;
                    float damping = hover.damping * hoverVelocity;
                    
                    float appliedHover = (force + suspension - damping) / _hoverCount * hoverControl;
                    
                    if (!hover.disable)
                    if (hover.forceType == 0)
                        shipRigidbody.AddForce(appliedHover * -hoverDirection, ForceMode.Acceleration);
                    else shipRigidbody.AddForceAtPosition(appliedHover * -hoverDirection, hoverPosition, ForceMode.Acceleration);
                    
                    _hoverHitCount ++;
                    _hoverHitRatio += amount / _hoverCount;
                    
                    _hoverSensorTip[i] = hoverHit.point;
                    _hoverNormal[i] = hoverHit.normal;
                    
                    // FIXME: Move to PhysicsData
                    // _isHoverHit[i] = true;
                    // hoverPressure[i] = pressure;
                }
                else
                {
                    _hoverSensorTip[i] = hoverPosition + hoverDirection * _hoverRange;
                    _hoverNormal[i] = -hoverDirection;
                    
                    // FIXME: Move to PhysicsData
                    // _isHoverHit[i] = false;
                    // hoverPressure[i] = 0;
                }
                
                _lastHoverPosition[i] = hoverPosition;
            }
            
            StateData.isOnGround = _hoverHitCount > 0;
            StateData.canLockToGround = canLockToGround;
        }
        
        private void ApplyGravity() // UPDATED
        {
            float gravityAmount = Mathf.Lerp(gravity.airGravity, gravity.groundGravity, _hoverHitRatio * hoverControl);
            
            for (int i = 0; i < _hoverCount; i++)
            {
                Vector3 hoverPosition = shipRigidbody.position + shipRigidbody.transform.TransformVector(hoverSensors[i].offset);
                Vector3 hoverDirection = hover.direction == PhysicsConfig.Direction.World ? Vector3.down :
                    Quaternion.Euler(hoverSensors[i].rotation) * -shipRigidbody.transform.up;
                
                Vector3 gravityPosition = Vector3.Lerp(shipRigidbody.position, hoverPosition, gravity.gravitySpread);
                Vector3 gravityDirection = gravity.direction == PhysicsConfig.Direction.World ? Vector3.down : hoverDirection;
                
                if (gravity.direction == PhysicsConfig.Direction.Ground)
                    if (Physics.Raycast(gravityPosition, gravityDirection, out RaycastHit sensorHit, _hoverRange, groundLayer))
                        gravityDirection = -sensorHit.normal;
                
                if (Physics.Raycast(gravityPosition, gravityDirection, out RaycastHit hoverHit, _hoverRange, groundLayer))
                {
                    float groundAngle = Vector3.Angle(-hoverHit.normal, Vector3.down);
                    float angleFactor = Mathf.Clamp(groundAngle, 0, 90) / 90;
                    
                    Vector3 groundGravDirection = canLockToGround ? -hoverHit.normal : gravity.slipMode == PhysicsConfig.SlipMode.Ground ?
                        Vector3.Lerp(-hoverHit.normal, Vector3.down, angleFactor) : Vector3.down;
                    
                    float appliedGravity = gravityAmount / _hoverCount;
                    if (!gravity.disable)
                    shipRigidbody.AddForceAtPosition(appliedGravity * groundGravDirection, gravityPosition, ForceMode.Acceleration);
                }
                else
                {
                    float appliedGravity = gravityAmount / _hoverCount;
                    if (!gravity.disable)
                    shipRigidbody.AddForce(appliedGravity * Vector3.down, ForceMode.Acceleration);
                }
            }
        }
        
        private void ApplyJump(float jumpInput)
        {
            
        }
        
        // - PROPULSION
        private void ApplyThrust(float thrustInput, float reverseInput, bool boostInput) // CODE CHANGED - TESTED
        {
            // TODO: Update thrust physics code: remove boost
            
            float thrustTime = thrustInput > thrustControl ? thrust.gain : thrust.release;
            thrustControl = Mathf.MoveTowards(thrustControl, thrustInput, Time.fixedDeltaTime / thrustTime);
            float reverseTime = reverseInput > reverseControl ? thrust.gain : thrust.release;
            reverseControl = Mathf.MoveTowards(reverseControl, reverseInput, Time.fixedDeltaTime / reverseTime);
            // boostControl = boostInput ? 1 : 0;
            
            float extraControlMod = (boostControl + reverseControl - boostControl * reverseControl);
            float thrustControlMod = 1 - ((1 - thrustControl) * extraControlMod);
            
            float extraSpeed = boost.speed * boostControl - thrust.reverseSpeed * reverseControl;
            float extraAccel = Mathf.Abs(boost.acceleration * boostControl - thrust.reverseAccel * reverseControl);
            float abDragAmount = 1 - (airbrake.dragAmount / 100 * abDragControl);
            float abDragDecel = airbrake.dragDecel * abDragControl;
            
            float currentSpeed = Vector3.Dot((shipRigidbody.linearVelocity - _kinematicVelocity) * KPHFactor, transform.forward);
            float targetSpeed = (thrust.speed * thrustControl + extraSpeed) * abDragAmount;
            
            bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
            bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(currentSpeed) <= Mathf.Abs(targetSpeed);
            
            float accelRatio = !isOppositeTarget ? isGreaterTarget ?
                (targetSpeed - currentSpeed) / thrust.speed : 0 : thrust.accelMode == PhysicsConfig.AccelMode.Normal ?
                targetSpeed / thrust.speed : (targetSpeed - currentSpeed) / thrust.speed;
            float decelRatio = !isOppositeTarget ? isGreaterTarget ?
                0 : (currentSpeed - targetSpeed) / DecelBaseKPH : thrust.decelMode == PhysicsConfig.DecelMode.Normal ?
                currentSpeed / DecelBaseKPH : 0;
            
            float appliedAccel = (thrust.acceleration * thrustControlMod + extraAccel) * accelRatio;
            float appliedDecel = (thrust.deceleration + abDragDecel) * decelRatio;
            float appliedThrust = appliedAccel - appliedDecel;
            shipRigidbody.AddForce(appliedThrust / KPHFactor * shipRigidbody.transform.forward, ForceMode.Acceleration);
        }
        
        private void ApplyBoost(float boostInput)
        {
            // TODO: Make boost kinematic. This method simulates Boost Mechanic
            float boostTime = boostInput > boostControl ? boost.gain : boost.release;
            boostControl = Mathf.MoveTowards(boostControl, boostInput, Time.fixedDeltaTime / boostTime);
            
            
            
        }
        
        private void ApplyAfterburner(float afterburnerInput)
        {
            float afterburnerTime = afterburnerInput > afterburnerControl ? afterburner.gain : afterburner.release;
            afterburnerControl = Mathf.MoveTowards(afterburnerControl, afterburnerInput, Time.fixedDeltaTime / afterburnerTime);
            
            
            
            
            
            
        }
        
        private void ApplyAirbrake(float abLeftInput, float abRightInput) // SAME
        {
            float abDragInput = Mathf.Clamp01(abLeftInput + abRightInput - 1);
            float abTorqueInput = abRightInput - abLeftInput;
            float abDriftInput = Mathf.Abs(abLeftInput - abRightInput);
            
            float dragTime = abDragControl < abDragInput ? airbrake.dragGain : airbrake.dragRelease;
            abDragControl = Mathf.MoveTowards(abDragControl, abDragInput, Time.fixedDeltaTime / dragTime);
            
            bool isBothNonZero = abTorqueControl != 0 && abTorqueInput != 0;
            bool isOppositeInput = Mathf.Sign(abTorqueControl) != Mathf.Sign(abTorqueInput) && isBothNonZero;
            bool canGainTorque = Mathf.Abs(abTorqueControl) < Mathf.Abs(abTorqueInput) && !isOppositeInput;
            
            float releaseTime = Mathf.Max(airbrake.torqueRelease, Time.fixedDeltaTime / Mathf.Abs(abTorqueControl));
            float torqueTime = canGainTorque ? airbrake.torqueGain : releaseTime;
            abTorqueControl = Mathf.MoveTowards(abTorqueControl, abTorqueInput, Time.fixedDeltaTime / torqueTime);
            
            float driftTime = abDriftControl < abDriftInput ? airbrake.driftGain : airbrake.driftRelease;
            abDriftControl = Mathf.MoveTowards(abDriftControl, abDriftInput, Time.fixedDeltaTime / driftTime);
        }
        
        // - TRACTION
        private void ApplyGrip() // SAME - TEST VERSION CHANGED
        {
            Vector3 groundNormal = (_hoverNormal[0] + _hoverNormal[1]).normalized;
            // FIXME: change the logic of canLockToGround && _isOnGround for magnet lock
            Vector3 gripPlane = canLockToGround && _isOnGround ? groundNormal : Vector3.up;
            Vector3 gripDirection = Vector3.ProjectOnPlane(shipRigidbody.transform.right, gripPlane);
            
            float forwardSpeed = Vector3.Dot((shipRigidbody.linearVelocity - _kinematicVelocity) * KPHFactor,
                shipRigidbody.transform.forward);
            float forwardFactor = Mathf.Clamp(forwardSpeed / airbrake.driftLimitKph, -1, 1);
            float airbrakeDrift = airbrake.driftAmount * abDriftControl * forwardFactor;
            
            float currentSpeed = Vector3.Dot(shipRigidbody.linearVelocity - _kinematicVelocity, gripDirection);
            gripControl = _hoverHitRatio >= grip.groundRatio ?
                Mathf.SmoothStep(gripControl, grip.groundGrip, 1 / (grip.toGroundTime * 10)):
                Mathf.SmoothStep(gripControl, grip.airGrip, 1 / (grip.toAirTime * 10));
            
            float targetGrip = Mathf.Clamp(gripControl * (1 - airbrakeDrift / 100), grip.minimum, 100);
            float appliedGrip = currentSpeed * grip.gripCurve.Evaluate(targetGrip / 100);
            shipRigidbody.AddForce(appliedGrip * -gripDirection, ForceMode.VelocityChange);
        }
        
        private void ApplyStrafe(float strafeInput) // UPDATED TO PHYSICS-BASED KINEMATIC FORCES
        {
            bool isStrafeNonZero = strafeControl != 0 && strafeInput != 0;
            bool isOppositeInput = Mathf.Sign(strafeControl) != Mathf.Sign(strafeInput) && isStrafeNonZero;
            bool canGainStrafe = Mathf.Abs(strafeControl) < Mathf.Abs(strafeInput) && !isOppositeInput;
            
            float releaseTime = Mathf.Max(strafe.release, Time.fixedDeltaTime / Mathf.Abs(strafeControl));
            float strafeTime = canGainStrafe ? strafe.gain : releaseTime;
            strafeControl = Mathf.MoveTowards(strafeControl, strafeInput, Time.fixedDeltaTime / strafeTime);
            
            float currentSpeed = Vector3.Dot(_kinematicVelocity, transform.right) - _sideshiftVelocity;
            float targetSpeed = strafe.speed * strafeControl;
            
            bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
            bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(currentSpeed) <= Mathf.Abs(targetSpeed);
            
            float accelRatio = !isOppositeTarget ? isGreaterTarget ?
                (targetSpeed - currentSpeed) / strafe.speed : 0 : strafe.accelMode == PhysicsConfig.AccelMode.Normal ?
                targetSpeed / strafe.speed : (targetSpeed - currentSpeed) / strafe.speed;
            float decelRatio = !isOppositeTarget ? isGreaterTarget ?
                0 : (currentSpeed - targetSpeed) / DecelBaseMPS : strafe.decelMode == PhysicsConfig.DecelMode.Normal ?
                currentSpeed / DecelBaseMPS : 0;
            
            float appliedAccel = strafe.acceleration * accelRatio;
            float appliedDecel = strafe.deceleration * decelRatio;
            float appliedStrafe = appliedAccel - appliedDecel;
            
            shipRigidbody.AddForce(appliedStrafe * shipRigidbody.transform.right, ForceMode.Acceleration);
            _kinematicVelocity += appliedStrafe * shipRigidbody.transform.right * Time.fixedDeltaTime;
        }
        
        private void ApplySideshift(float sideshiftInput) // NEW PHYSICS-BASED KINEMATIC FORCES
        {
            bool isSideshiftNonZero = sideshiftControl != 0 && _sideshiftInput != 0;
            bool isOppositeInput = Mathf.Sign(sideshiftControl) != Mathf.Sign(_sideshiftInput) && isSideshiftNonZero;
            bool canGainSideshift = Mathf.Abs(sideshiftControl) < Mathf.Abs(_sideshiftInput) && !isOppositeInput;
            
            float releaseTime = Mathf.Max(sideshift.release, Time.fixedDeltaTime / Mathf.Abs(sideshiftControl));
            float sideshiftTime = canGainSideshift ? sideshift.gain : releaseTime;
            sideshiftControl = Mathf.MoveTowards(sideshiftControl, _sideshiftInput, Time.fixedDeltaTime / sideshiftTime);
            
            float currentSpeed = _sideshiftVelocity;
            float appliedSideshift = (sideshift.speed * sideshiftControl - currentSpeed) / Time.fixedDeltaTime;
            
            shipRigidbody.AddForce(appliedSideshift * shipRigidbody.transform.right, ForceMode.Acceleration);
            _kinematicVelocity += appliedSideshift * shipRigidbody.transform.right * Time.fixedDeltaTime;
            _sideshiftVelocity += appliedSideshift * Time.fixedDeltaTime;
        }
        
        // - ROTATION
        private void ApplySteer(float steerInput) // CODE CHANGED - TESTED
        {
            bool isSteerNonZero = steerControl != 0 && steerInput != 0;
            bool isOppositeInput = Mathf.Sign(steerControl) != Mathf.Sign(steerInput) && isSteerNonZero;
            bool canGainSteer = Mathf.Abs(steerControl) < Mathf.Abs(steerInput) && !isOppositeInput;
            
            float releaseTime = Mathf.Max(steer.release, Time.fixedDeltaTime / Mathf.Abs(steerControl));
            float steerTime = canGainSteer ? steer.gain : releaseTime;
            steerControl = Mathf.MoveTowards(steerControl, steerInput, Time.fixedDeltaTime / steerTime);
            
            float steerControlMod = 1 - ((1 - Mathf.Abs(steerControl)) * Mathf.Abs(abTorqueControl));
            float forwardSpeed = Vector3.Dot((shipRigidbody.linearVelocity - _kinematicVelocity) * KPHFactor, transform.forward);
            float forwardFactor = Mathf.Clamp(forwardSpeed / airbrake.torqueLimitKph, -1, 1);
            
            float abTorqueSpeed = airbrake.torqueSpeed * abTorqueControl * forwardFactor;
            float abTorqueAccel = Mathf.Abs(airbrake.torqueAccel * abTorqueControl * forwardFactor);
            
            float currentSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, transform.up);
            float targetSpeed = steer.speed * steerControl + abTorqueSpeed;
            
            bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
            bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(currentSpeed) <= Mathf.Abs(targetSpeed);
            
            float accelRatio = !isOppositeTarget ? isGreaterTarget ?
                (targetSpeed - currentSpeed) / steer.speed : 0 : steer.accelMode == PhysicsConfig.AccelMode.Normal ?
                targetSpeed / steer.speed : (targetSpeed - currentSpeed) / steer.speed;
            float decelRatio = !isOppositeTarget ? isGreaterTarget ?
                0 : (currentSpeed - targetSpeed) / DecelBaseDPS : steer.decelMode == PhysicsConfig.DecelMode.Normal ?
                currentSpeed / DecelBaseDPS : 0;
            
            float appliedAccel = (steer.acceleration * steerControlMod + abTorqueAccel) * accelRatio;
            float appliedDecel = steer.deceleration * decelRatio;
            float appliedSteer = appliedAccel - appliedDecel;
            shipRigidbody.AddTorque(appliedSteer * Mathf.Deg2Rad * shipRigidbody.transform.up, ForceMode.Acceleration);
        }
        
        private void ApplyPitch(float pitchInput) // SAME - TEST VERSION CHANGED
        {
            bool isOppositeInput = Mathf.Sign(pitchControl) != Mathf.Sign(pitchInput) && pitchControl != 0 && pitchInput != 0;
            float releaseTime = Mathf.Max(pitch.release, Time.fixedDeltaTime / Mathf.Abs(pitchControl));
            float pitchTime = Mathf.Abs(pitchControl) < Mathf.Abs(pitchInput) && !isOppositeInput ? pitch.gain : releaseTime;
            pitchControl = Mathf.MoveTowards(pitchControl, pitchInput, Time.fixedDeltaTime / pitchTime);
            
            Vector3 pitchAxis = shipRigidbody.transform.right;
            Vector3 groundForward = Vector3.ProjectOnPlane(_hoverSensorTip[0] - _hoverSensorTip[1], pitchAxis).normalized;
            
            float currentAngle = Vector3.SignedAngle(groundForward, shipRigidbody.transform.forward, pitchAxis);
            float targetAngle = pitch.trackAngle * pitchControl;
            float currentSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, pitchAxis);
            
            if (_isOnGround && !pitch.manualMode)
            {
                float amount = pitch.trackForce * (targetAngle - currentAngle);
                float damping = pitch.trackDamping * currentSpeed;
                float appliedPitch = (amount - damping) * _hoverHitRatio;
                shipRigidbody.AddTorque(appliedPitch * Mathf.Deg2Rad * pitchAxis, ForceMode.Acceleration);
            }
            else
            {
                if (!_isOnGround && orient.autoPitch) return;
                float pitchAccel = _isOnGround ? pitch.groundAccel : pitch.accel;
                float pitchSpeed = _isOnGround ? pitch.groundSpeed : pitch.speed;
                float pitchDecel = _isOnGround ? pitch.groundDecel : pitch.decel;
                
                float targetSpeed = pitchSpeed * pitchControl;
                bool isBothNonZero = currentSpeed != 0 && targetSpeed != 0;
                bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isBothNonZero;
                bool canAccelerate = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed) || isOppositeTarget;
                
                float appliedAccel = canAccelerate ? pitchAccel : pitchDecel * pitchSpeed / 90;
                float appliedPitch = appliedAccel * (targetSpeed - currentSpeed) / pitchSpeed;
                shipRigidbody.AddTorque(appliedPitch * Mathf.Deg2Rad * pitchAxis, ForceMode.Acceleration);
            }
        }
        
        private void ApplyRoll(float rollInput) // SAME - TEST VERSION CHANGED
        {
            bool isOppositeInput = Mathf.Sign(rollControl) != Mathf.Sign(rollInput) && rollControl != 0 && rollInput != 0;
            float releaseTime = Mathf.Max(roll.release, Time.fixedDeltaTime / Mathf.Abs(rollControl));
            float rollTime = Mathf.Abs(rollControl) < Mathf.Abs(rollInput) && !isOppositeInput ? roll.gain : releaseTime;
            rollControl = Mathf.MoveTowards(rollControl, rollInput, Time.fixedDeltaTime / rollTime);
            
            Vector3 groundNormal = (_hoverNormal[0] + _hoverNormal[1]).normalized;
            Vector3 groundRight = Vector3.ProjectOnPlane(shipRigidbody.transform.right, groundNormal).normalized;
            Vector3 rollAxis = shipRigidbody.transform.forward;
            
            float currentAngle = Vector3.SignedAngle(groundRight, shipRigidbody.transform.right, rollAxis);
            float targetAngle = roll.trackAngle * -rollControl;
            float currentSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, rollAxis);
            float targetSpeed = roll.speed * -rollControl;
            
            if (_isOnGround)
            {
                float amount = roll.trackForce * (targetAngle - currentAngle);
                float damping = roll.trackDamping * currentSpeed;
                float appliedRoll = (amount - damping) * _hoverHitRatio;
                if (!roll.disable) shipRigidbody.AddTorque(appliedRoll * Mathf.Deg2Rad * rollAxis, ForceMode.Acceleration);
            }
            else if (!orient.autoRoll)
            {
                bool isBothNonZero = currentSpeed != 0 && targetSpeed != 0;
                bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isBothNonZero;
                bool canAccelerate = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed) || isOppositeTarget;
                
                float appliedAccel = canAccelerate ? roll.accel : roll.decel * roll.speed / 90;
                float appliedRoll = appliedAccel * (targetSpeed - currentSpeed) / roll.speed;
                shipRigidbody.AddTorque(appliedRoll * Mathf.Deg2Rad * rollAxis, ForceMode.Acceleration);
            }
        }
        
        private void ApplyOrient() // SAME
        {
            if (_isOnGround) return;
            
            if (orient.autoRoll)
            {
                float rollAngle = shipRigidbody.transform.eulerAngles.z;
                if (rollAngle > 180) rollAngle -= 360;
                
                Vector3 rollAxis = shipRigidbody.transform.forward;
                float currentSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, rollAxis);
                
                float amount = orient.autoRollForce * -rollAngle;
                float damping = orient.autoRollDamp * currentSpeed;
                float appliedAutoRoll = amount - damping;
                shipRigidbody.AddTorque(appliedAutoRoll * Mathf.Deg2Rad * rollAxis, ForceMode.Acceleration);
            }
            
            if (orient.autoPitch)
            {
                float pitchAngle = shipRigidbody.transform.eulerAngles.x;
                if (pitchAngle > 180) pitchAngle -= 360;
                
                Vector3 pitchAxis = shipRigidbody.transform.right;
                float currentSpeed = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, pitchAxis);
                
                float amount = orient.autoPitchForce * -pitchAngle;
                float damping = orient.autoPitchDamp * currentSpeed;
                float appliedAutoPitch = amount - damping;
                shipRigidbody.AddTorque(appliedAutoPitch * Mathf.Deg2Rad * pitchAxis, ForceMode.Acceleration);
            }
        }
        
        
        // MECHANIC PHYSICS METHODS
        private void UpdateMechanicPhysics()
        {
            ApplyMagnetLock(MechanicData.magnetLockInput);
            ApplySpeedPad(_speedPadInput);
            ApplySpeedZone(MechanicData.speedZoneInput);
            ApplySlowPad(_speedZoneInput);
            ApplyStallZone(MechanicData.stallZoneInput);
            ApplySlowdown();
        }
        
        private void ApplyMagnetLock(float magnetLockInput)
        {
            float magnetLockTime = magnetLockInput > magnetLockControl ? magnetLock.gain : magnetLock.release;
            magnetLockControl = Mathf.MoveTowards(magnetLockControl, magnetLockInput, Time.fixedDeltaTime / magnetLockTime);
            
            Vector3 sensorPosition = transform.position;
            Vector3 sensorDirection = -transform.up;
            
            if (Physics.Raycast(sensorPosition, sensorDirection, out RaycastHit sensorHit, magnetLock.range, groundLayer))
                sensorDirection = -sensorHit.normal;
            
            float sensorSpeed = Vector3.Dot(sensorPosition - _lastMagnetLockPosition, -sensorDirection) / Time.fixedDeltaTime;
            _lastMagnetLockPosition = sensorPosition;
            
            if (Physics.Raycast(sensorPosition, sensorDirection, out sensorHit, magnetLock.range, groundLayer))
            {
                float distance = magnetLock.height - sensorHit.distance;
                
                float force = distance * Mathf.Pow(Time.fixedDeltaTime, -2) * magnetLock.force;
                float damping = sensorSpeed * (1 / Time.fixedDeltaTime) * magnetLock.damping;
                
                float appliedMagnetLock = (force - damping) * magnetLockControl;
                shipRigidbody.AddForce(appliedMagnetLock * -sensorDirection, ForceMode.Acceleration);
                
                // DEBUG
                // Debug.Log("Current Pos: " + sensorHit2.distance
                // +  "\tTarget Pos: " + hover.height
                // +  "\tSpeed: " + sensorSpeed
                // +  "\tForce: " + (force - damping));
                
                // Debug.Log($"sensorHit2.distance: {sensorHit2.distance}\tupwardSpeed: {upwardSpeed} {Vector3.Dot(shipRigidbody.velocity, -sensorDirection)}");
                // Debug.Log($"errorHeight: {errorHeight}\tappliedMagnetLock: {appliedMagnetLock}\tappliedForce: {appliedMagnetLock * sensorDirection}");
            }
        }
        
        // TODO: For Speed/Slow Pads, and Speed/Slow Zones, they are controlled by Vector3
        // They may have another separate kinematicVelocity layers
        // Heh, a new made-up concept: VELOCITY LAYERING, which is what physics-based kinematic forces use.
        
        private void ApplySpeedPad(float speedPadInput)
        {
            float speedPadTime = speedPadInput > speedPadControl ? speedPad.gain : speedPad.release;
            speedPadControl = Mathf.MoveTowards(speedPadControl, speedPadInput, Time.fixedDeltaTime / speedPadTime);
            
            // update speedPadControl based on speedPadInput
            // apply kinematic forward force based on speedPadControl
        }
        
        private void ApplySpeedZone(float speedZoneInput)
        {
            float speedZoneTime = speedZoneInput > speedZoneControl ? speedZone.gain : speedZone.release;
            speedZoneControl = Mathf.MoveTowards(speedZoneControl, speedZoneInput, Time.fixedDeltaTime / speedZoneTime);
            
            // update speedZoneControl based on speedZoneInput
            // apply kinematic forward force based on speedZoneControl
        }
        
        private void ApplySlowPad(float slowPadInput)
        {
            float slowPadTime = slowPadInput > slowPadControl ? slowPad.gain : slowPad.release;
            slowPadControl = Mathf.MoveTowards(slowPadControl, slowPadInput, Time.fixedDeltaTime / slowPadTime);
            
            // update slowPadControl based on slowPadInput
        }
        
        private void ApplyStallZone(float stallZoneInput)
        {
            float stallZoneTime = stallZoneInput > stallZoneControl ? stallZone.gain : stallZone.release;
            stallZoneControl = Mathf.MoveTowards(stallZoneControl, stallZoneInput, Time.fixedDeltaTime / stallZoneTime);
            
            // update stallZoneControl based on stallZoneInput
        }
        
        private void ApplySlowdown()
        {
            // hijack thrust and steer controls based on slowdownControl
        }
        
        
        // SHIP EVENT METHODS
        private IEnumerator OnJump()
        {
            if (_canJump)
            {
                _canJump = false;
                
                Vector3 jumpDirection = shipRigidbody.transform.up;
                float appliedJump = jump.force;
                shipRigidbody.AddForce(appliedJump * jumpDirection, ForceMode.Impulse);
                
                WaitForSeconds waitForjumpCooldown = new WaitForSeconds(jump.cooldown);
                
                yield return waitForjumpCooldown;
                _canJump = true;
            }
        }
        
        private IEnumerator OnBoost()
        {
            yield break;
        }
        
        private IEnumerator OnSideshift(bool direction)
        {
            if (_isSideshifting) yield break;
            
            _sideshiftInput = direction ? 1 : -1;
            sideshiftControl = 0;
            _isSideshifting = true;
            
            yield return new WaitForSeconds(Mathf.Max(Time.fixedDeltaTime, sideshift.duration));
            _sideshiftInput = 0;
            
            yield return new WaitForSeconds(sideshift.cooldown);
            _isSideshifting = false;
            yield break;
        }
        
        private IEnumerator OnRespawn()
        {
            transform.SetPositionAndRotation(new Vector3(25, 4, -375), Quaternion.Euler(Vector3.zero));
            yield break;
        }
        
        
        // MECHANIC EVENT METHODS
        private IEnumerator OnSpeedPad(float speedPadInput)
        {
            Debug.Log($"The ship hit a Speed Pad at value {speedPadInput}!");
            yield break;
        }
        
        private IEnumerator OnSlowPad(float speedPadInput)
        {
            Debug.Log($"The ship hit a Slow Pad at value {speedPadInput}!");
            yield break;
        }
        
        private IEnumerator OnSlowdown(float slowdownInput)
        {
            Debug.Log($"The ship Slowdown at value {slowdownInput}!");
            yield break;
        }
        
        
        // COLLISION EVENT METHODS
        private void OnWallHit()
        {
            Debug.Log($"The ship hit the Wall!");
        }
        
        private void OnFloorHit()
        {
            Debug.Log($"The ship hit the Floor!");
        }
        
        private void OnStaticHit()
        {
            Debug.Log($"The ship hit a Static Object!");
        }
        
        private void OnDynamicHit()
        {
            Debug.Log($"The ship hit a Moving Object!");
        }
        
        private void OnShipHit()
        {
            Debug.Log($"The ship hit the Ship!");
        }
        
        private void OnProjectileHit()
        {
            Debug.Log($"The ship is hit by Projectile!");
        }
        
        
        // STATE EVENT METHODS
        private IEnumerator OnShipDamage(float slowdownAmount)
        {
            Debug.Log("The ship taken damage.");
            yield break;
        }
        
        private IEnumerator OnShipDestroy()
        {
            Debug.Log("The ship is destroyed. Hover generator shutdown. All physics systems shutdown.");
            yield break;
        }
        
        private IEnumerator OnShipRespawn()
        {
            transform.SetPositionAndRotation(new Vector3(25, 4, -375), Quaternion.Euler(Vector3.zero));
            yield break;
        }
        
        
        
        // CLASSES
        [System.Serializable]
        public struct HoverSensor
        {
            public Vector3 offset;
            public Vector3 rotation;
            
            public HoverSensor(Vector3 offset, Vector3 rotation)
            {
                this.offset = offset;
                this.rotation = rotation;
            }
            
            public HoverSensor(float offsetX, float offsetY, float offsetZ,
                float rotationX, float rotationY, float rotationZ)
            {
                this.offset = new Vector3(offsetX, offsetY, offsetZ);
                this.rotation = new Vector3(rotationX, rotationY, rotationZ);
            }
        }
        
        
    }
    
}
