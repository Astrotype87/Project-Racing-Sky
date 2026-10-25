using System.Collections;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    using Utility;
    using Config = PhysicsConfig;
    
    /// <summary> Simulation of ship's arcade physics. </summary>
    [DefaultExecutionOrder(ScriptOrder.ShipPhysics)]
    public class ShipPhysics : MonoBehaviour
    {
        // FIELDS & PROPERTIES
        #region INSPECTOR 
        
        [Header("Sensor Settings")]
        [SerializeField] private MainSensors mainSensors;
        
        [Header("Physical Settings")]
        [SerializeField] private Config.World world;
        [SerializeField] private Config.Body body;
        
        [Header("Ship Settings")]
        [SerializeField] private Config.Hover hover;
        [SerializeField] private Config.Jump jump;
        [Space(5)]
        [SerializeField] private Config.Thrust thrust;
        [SerializeField] private Config.Boost boost;
        [SerializeField] private Config.Afterburner afterburner;
        [SerializeField] private Config.Airbrake airbrake;
        [Space(5)]
        [SerializeField] private Config.Grip grip;
        [SerializeField] private Config.Strafe strafe;
        [SerializeField] private Config.Sideshift sideshift;
        [Space(5)]
        [SerializeField] private Config.Steer steer;
        [SerializeField] private Config.Pitch pitch;
        [SerializeField] private Config.Roll roll;
        [SerializeField] private Config.Orient orient;
        
        [Header("Track Settings")]
        [SerializeField] private Config.Maglev maglev;
        [SerializeField] private Config.JumpPad jumpPad;
        [SerializeField] private Config.SpeedPad speedPad;
        
        [Header("Debug")]
        [SerializeField] private PhysicsData.Control control;
        
        #endregion
        
        #region FIELDS 
        
        // PUBLIC PROPERTIES
        public DebugLiveData DebugLiveData {get; set;} = new DebugLiveData();
        
        // PRIVATE GETTERS
        private Vector3 _shipVelocity => shipRigidbody.linearVelocity - _kinematicVelocity;
        private Vector3 _shipAngularVelocity => shipRigidbody.angularVelocity - _kinematicAngularVelocity;
        private float _hoverRange => hover.height + hover.falloff;
        private float _hoverHitRatio => ((float) _hoverHitCount / _hoverCount).NaNInfSafe();
        
        // PRIVATE FIELDS: REFERENCES
        private Rigidbody shipRigidbody;
        private InputData inputData;
        private ControllerData controllerData;
        private MechanicsData mechanicsData;
        private PhysicsData physicsData;
        
        // PRIVATE FIELDS: EVENT INPUT
        private bool _hoverInput;
        private bool _boostInput;
        private bool _jumpInput;
        private float _sideshiftInput;
        private bool _maglevInput;
        private bool _jumpPadInput;
        private Vector3 _jumpPadDirection;
        private float _speedPadInput;
        private Vector3 _speedPadDirection;
        
        // PRIVATE FIELDS: SENSORS
        private SensorInfo _frontSensorInfo;
        private SensorInfo _backSensorInfo;
        private SensorInfo _leftSensorInfo;
        private SensorInfo _rightSensorInfo;
        
        // PRIVATE FIELDS: GLOBAL
        private Vector3 _counterDrag;           // contributors: hover, thrust, strafe, grip --- users: linear drag, airbrake drag
        private Vector3 _counterAngularDrag;
        
        // PRIVATE FIELDS: KINEMATICS
        private Vector3 _kinematicVelocity;
        private Vector3 _kinematicAngularVelocity;
        private Vector3 _targetKinematicVelocity;
        private Vector3 _targetKinematicAngularVelocity;
        
        // PRIVATE FIELDS: HOVER PHYSICS
        private int _hoverCount;
        private int _hoverHitCount;
        private Vector3[] _lastHoverPosition;
        
        // PRIVATE FIELDS: GRIP PHYSICS
        private float _counterGrip;
        
        // PRIVATE FIELDS: MAGLEV PHYSICS
        private Vector3 _lastMaglevPosition;
        
        // PRIVATE CONSTANTS
        private const float KPHFactor = 3.6f;
        
        #endregion
        
        
        // METHODS
        #region MONOBEHAVIOUR METHODS 
        
        private void Start()
        {
            InitializeShipPhysics();
            RegisterInputEvents();
        }
        
        private void FixedUpdate()
        {
            UpdateShipPhysics();
        }
        
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                DebugLiveData ??= new DebugLiveData();
                inputData ??= new InputData();
            }
            
            DebugLiveData.hoverSensors = hover.sensors;
            DebugLiveData.mainSensors = mainSensors;
            DebugLiveData.hoverGroundLayer = hover.groundLayer;
            if (inputData != null)
                DebugLiveData.isLockToGround = inputData.lockToGround;
            
            DebugLiveData.hoverHeight = hover.height;
            DebugLiveData.hoverRange = _hoverRange;
            DebugLiveData.hoverSpread = hover.hoverSpread;
            DebugLiveData.hoverPitchEffect = hover.pitchEffect;
            DebugLiveData.hoverRollEffect = hover.rollEffect;
            DebugLiveData.direction = hover.hoverDirection;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            ApplyKinematicCounterCollision(other, false);
        }
        
        private void OnCollisionStay(Collision other)
        {
            ApplyKinematicCounterCollision(other, false);
        }
        
        private void OnCollisionExit(Collision other)
        {
            ApplyKinematicCounterCollision(other, true);
        }
        
        #endregion
        
        #region PUBLIC METHODS 
        
        public void SetDependencies(Rigidbody shipRigidbody, InputData inputData,
            ControllerData controllerData, MechanicsData mechanicsData, PhysicsData physicsData)
        {
            this.shipRigidbody = shipRigidbody;
            this.inputData = inputData;
            this.controllerData = controllerData;
            this.mechanicsData = mechanicsData;
            this.physicsData = physicsData;
        }
        
        public void SetPhysicsConfig(Config physicsConfig)
        {
            this.world = physicsConfig.world;
            this.body = physicsConfig.body;
            
            this.hover = physicsConfig.hover;
            this.jump = physicsConfig.jump;
            
            this.thrust = physicsConfig.thrust;
            this.boost = physicsConfig.boost;
            this.afterburner = physicsConfig.afterburner;
            this.airbrake = physicsConfig.airbrake;
            
            this.grip = physicsConfig.grip;
            this.strafe = physicsConfig.strafe;
            this.sideshift = physicsConfig.sideshift;
            
            this.steer = physicsConfig.steer;
            this.pitch = physicsConfig.pitch;
            this.roll = physicsConfig.roll;
            this.orient = physicsConfig.orient;
            
            this.maglev = physicsConfig.maglev;
            this.jumpPad = physicsConfig.jumpPad;
            this.speedPad = physicsConfig.speedPad;
        }
        
        #endregion
        
        
        // INITIALIZATION
        public void InitializeShipPhysics()
        {
            _hoverInput = true;
            control.hover = 1;
            
            _hoverCount = hover.sensors.Length;
            
            _lastHoverPosition = new Vector3[_hoverCount];
            for (int i = 0; i < _lastHoverPosition.Length; i++)
                _lastHoverPosition[i] = shipRigidbody.position + shipRigidbody.transform.TransformVector(hover.sensors[i].offset);
        }
        
        public void RegisterInputEvents()
        {
            inputData.OnHover += InvokeHover;
            inputData.OnJump += InvokeJump;
            inputData.OnBoost += InvokeBoost;
            inputData.OnSideshift += InvokeSideshift;
            
            inputData.OnMaglev += InvokeMaglev;
            inputData.OnJumpPad += InvokeJumpPad;
            inputData.OnSpeedPad += InvokeSpeedPad;
        }
        
        public void UnregisterInputEvents()
        {
            inputData.OnHover += InvokeHover;
            inputData.OnJump += InvokeJump;
            inputData.OnBoost += InvokeBoost;
            inputData.OnSideshift += InvokeSideshift;
            
            inputData.OnMaglev += InvokeMaglev;
            inputData.OnJumpPad += InvokeJumpPad;
            inputData.OnSpeedPad += InvokeSpeedPad;
        }
        
        
        // SHIP PHYSICS
        private void UpdateShipPhysics()
        {
            // - PRE-CALCULATIONS -
            UpdateShipSensor(); UpdateShipControls();
            
            // - CORE PHYSICS -
            ApplyHover(); ApplyPull(); ApplyJump();
            ApplyThrust(); ApplyAirbrake();
            ApplyStrafe(); ApplyGrip();
            ApplySteer(); ApplyPitch(); ApplyRoll(); ApplyOrient();
            
            // - TRACK PHYSICS -
            ApplyMaglev(); ApplyJumpPad();
            ApplySpeedPad();
            
            // - POST PHYSICS -
            ApplyGravity();
            ApplyLinearDrag(); ApplyAngularDrag();
            ApplyKinematics();
        }
        
        private void UpdateShipSensor()
        {
            // Group settings into tuple
            (Transform, Direction, float, LayerMask) settings
                = (shipRigidbody.transform, hover.hoverDirection, _hoverRange, mainSensors.groundLayer);
            
            // Update sensors
            _frontSensorInfo.UpdateSensor(mainSensors.front, settings);
            _backSensorInfo.UpdateSensor(mainSensors.back, settings);
            _leftSensorInfo.UpdateSensor(mainSensors.left, settings);
            _rightSensorInfo.UpdateSensor(mainSensors.right, settings);
            
            // Decide when is on ground
            physicsData.isOnGround = _frontSensorInfo.isHit || _backSensorInfo.isHit;
        }
        
        private void UpdateShipControls()
        {
            void MoveToTarget(ref float current, float target, float gain, float release)
                => current = current.MoveToTarget(target, gain, release, Time.fixedDeltaTime);
            void MoveToTargetSigned(ref float current, float target, float gain, float release)
                => current = current.MoveToTargetSigned(target, gain, release, Time.fixedDeltaTime);
            
            // ELEVATION CONTROL
            MoveToTarget(ref control.hover, _hoverInput.ToInt(), hover.powerOnTime, hover.powerOffTime);
            MoveToTarget(ref control.jump, _jumpInput.ToInt(), jump.gain, jump.release);
            
            // PROPULSION CONTROL
            float thrustInput = inputData.thrustInput - inputData.reverseInput;
            MoveToTarget(ref control.thrust, thrustInput, thrust.gain, thrust.release);
            
            float boostInput = _boostInput.ToInt();
            MoveToTarget(ref control.boost, boostInput, boost.gain, boost.release);
            
            float afterburnerInput = inputData.afterburnerInput;
            MoveToTarget(ref control.afterburner, afterburnerInput, afterburner.gain, afterburner.release);
            
            // AIRBRAKE CONTROL
            float abDragInput = Mathf.Clamp01(inputData.airbrakeLeftInput + inputData.airbrakeRightInput - 1);
            MoveToTarget(ref control.abDrag, abDragInput, airbrake.dragGain, airbrake.dragRelease);
            
            float abTorqueInput = inputData.airbrakeRightInput - inputData.airbrakeLeftInput;
            MoveToTargetSigned(ref control.abTorque, abTorqueInput, airbrake.torqueGain, airbrake.torqueRelease);
            
            float abDriftInput = !MathUtil.IsOppositeSign(abTorqueInput, inputData.steerInput) ? Mathf.Abs(abTorqueInput)
                : Mathf.Clamp01(Mathf.Abs(abTorqueInput) - Mathf.Abs(inputData.steerInput));
            MoveToTarget(ref control.abDrift, abDriftInput, airbrake.driftGain, airbrake.driftRelease);
            
            float abSlideInput = inputData.airbrakeRightInput - inputData.airbrakeLeftInput;
            MoveToTargetSigned(ref control.abSlide, abSlideInput, airbrake.slideGain, airbrake.slideRelease);
            
            // TRACTION CONTROL
            MoveToTargetSigned(ref control.strafe, inputData.strafeInput, strafe.gain, strafe.release);
            MoveToTargetSigned(ref control.sideshift, _sideshiftInput, sideshift.gain, sideshift.release);
            
            // ROTATION CONTROL
            MoveToTargetSigned(ref control.steer, inputData.steerInput, steer.gain, steer.release);
            MoveToTargetSigned(ref control.pitch, inputData.pitchInput, pitch.gain, pitch.release);
            MoveToTargetSigned(ref control.roll, inputData.rollInput, roll.gain, roll.release);
            
            // MISC CONTROL
            MoveToTarget(ref control.maglev, _maglevInput.ToInt(), maglev.gain, maglev.release);
            MoveToTarget(ref control.jumpPad, _jumpPadInput.ToInt(), jumpPad.gain, jumpPad.release);
            MoveToTarget(ref control.speedPad, Mathf.Clamp01(_speedPadInput), speedPad.gain, speedPad.release);
            
            // GLOBAL CONTROL
            float fdt = Time.fixedDeltaTime;
            control.wind = Vector3.MoveTowards(control.wind, inputData.windInput, fdt / world.windTime);
            control.turbulence = Mathf.MoveTowards(control.turbulence, inputData.turbulenceInput, fdt / world.turbulenceTime);
            
            // PHYSICS DATA
            physicsData.control = this.control;
        }
        
        private void ApplyKinematicCounterCollision(Collision other, bool isExit)
        {
            Vector3 kinematicCounterCollision = _targetKinematicVelocity.normalized * Vector3.Dot(other.impulse, _targetKinematicVelocity.normalized);
            _kinematicVelocity += kinematicCounterCollision * isExit.ToSign();
        }
        
        
        // GLOBAL & PHYSICAL
        private void ApplyGravity()
        {
            // APPLY GRAVITY
            shipRigidbody.AddForce(world.gravity, ForceMode.Acceleration);
        }
        
        private void ApplyLinearDrag()
        {
            // Get linear velocity
            Vector3 currentVelocity = shipRigidbody.linearVelocity - _kinematicVelocity;
            Vector3 localVelocity = transform.InverseTransformVector(currentVelocity);
            
            // Select and calculate drag formula
            Vector3 localDragForce = world.dragFormula switch
            {
                DragFormula.Basic =>
                    body.mass * world.density * world.drag * Vector3.Scale(localVelocity, body.dragScale),
                DragFormula.Realistic =>
                    0.5f * world.density * world.drag * Vector3.Scale(localVelocity.Squared(), body.dragScale),
                DragFormula.Unity =>
                    world.density * world.drag / (1 - Time.fixedDeltaTime * world.drag)
                        * Vector3.Scale(localVelocity, body.dragScale),
                _ => Vector3.zero
            };
            
            
            // Get local drag filter
            Vector3 localCounterDrag = transform.InverseTransformVector(_counterDrag);
            Vector3 localDragFilter = Vector3Util.Clamp01(Vector3.one - localCounterDrag);
            
            // Apply local drag filter and get world drag force
            Vector3 filteredDragForce = Vector3.Scale(localDragForce, localDragFilter);
            Vector3 dragForce = -transform.TransformVector(filteredDragForce);
            
            // // Get drag impulse and required impulse to get zero velocity
            // Vector3 dragImpulse = dragForce * Time.fixedDeltaTime;
            // Vector2 requiredImpulse = -body.mass * currentVelocity;
            
            // // Avoid reverse velocity impulse (caused by extremely low values) and stop the object instead
            // if (dragImpulse.magnitude >= requiredImpulse.magnitude)
            //     dragForce = requiredImpulse / Time.fixedDeltaTime;
            
            
            // Apply linear drag force
            shipRigidbody.AddForce(dragForce / body.mass, ForceMode.Acceleration);
            _counterDrag = Vector3.zero;
        }
        
        private void ApplyAngularDrag()
        {
            
            
            
            
            
            
            Vector3 localAngularVelocity = transform.InverseTransformVector(shipRigidbody.angularVelocity);
            Vector3 localKinematicAngularVelocity = transform.InverseTransformVector(_kinematicAngularVelocity);
            Vector3 localCounterAngularDrag = transform.InverseTransformVector(_counterAngularDrag);
            
            Vector3 localScaledAngularDrag = Vector3.Scale(localAngularVelocity - localKinematicAngularVelocity, body.angularDragScale);
            Vector3 localCounteredAngularDrag = Vector3.Scale(localScaledAngularDrag, Vector3Util.Clamp01(Vector3.one - localCounterAngularDrag));
            
            Vector3 appliedAngularDrag = -transform.TransformVector(localCounteredAngularDrag) * world.angularDrag;
            shipRigidbody.AddTorque(appliedAngularDrag, ForceMode.Acceleration);
            _counterAngularDrag = Vector3.zero;
        }
        
        
        private void ApplyKinematics()
        {
            // TARGET KINEMATIC
            Vector3 windVelocity = control.wind;
            Vector3 sideshiftVelocity = shipRigidbody.transform.right * sideshift.speed * control.sideshift * sideshift.isKinematic.ToInt();
            _targetKinematicVelocity = windVelocity + sideshiftVelocity;
            
            // APPLY KINEMATIC
            Vector3 appliedKinematics = _targetKinematicVelocity - _kinematicVelocity;
            shipRigidbody.AddForce(appliedKinematics, ForceMode.VelocityChange);
            _kinematicVelocity += appliedKinematics;
            
            // TARGET ANGULAR KINEMATIC
            Vector3 turbulenceNoiseOutput = world.turbulenceNoise.GetValue(Time.fixedTimeAsDouble, inputData.turbulenceInput, 1);
            Vector3 turbulenceAngularVelocity = shipRigidbody.transform.TransformVector(turbulenceNoiseOutput * Mathf.Deg2Rad);
            _targetKinematicAngularVelocity = turbulenceAngularVelocity;
            
            // APPLY ANGULAR KINEMATIC
            Vector3 appliedAngularKinematics = _targetKinematicAngularVelocity - _kinematicAngularVelocity;
            shipRigidbody.AddTorque(appliedAngularKinematics, ForceMode.VelocityChange);
            _kinematicAngularVelocity += appliedAngularKinematics;
        }
        
        // ELEVATION
        private void ApplyHover()
        {
            _hoverHitCount = 0;
            for (int i = 0; i < hover.sensors.Length; i++)
            {
                // Get hover and force offset
                Vector3 hoverOffset = shipRigidbody.transform.TransformVector(hover.sensors[i].offset * hover.hoverSpread);
                Vector3 forceOffset = shipRigidbody.transform.TransformVector(
                    Vector3.Scale(hover.sensors[i].offset, new Vector3(hover.rollEffect, 0, hover.pitchEffect)));
                
                // Get hover position and direction
                Vector3 hoverPosition = shipRigidbody.position + hoverOffset;
                Vector3 hoverDirection = hover.hoverDirection switch {
                    Direction.World => Vector3.down,
                    Direction.Gravity => world.gravity.normalized,
                    _ => Quaternion.Euler(hover.sensors[i].rotation) * -shipRigidbody.transform.up };
                
                // Recalculate hover direction based on detected ground normal
                if (hover.hoverDirection == Direction.Ground)
                    if (Physics.Raycast(hoverPosition, hoverDirection, out RaycastHit groundHit, _hoverRange, hover.groundLayer))
                        hoverDirection = -groundHit.normal;
                
                // Calculate hover velocity in hover direction axis
                float hoverVelocity = Vector3.Dot(hoverPosition - _lastHoverPosition[i], -hoverDirection) / Time.fixedDeltaTime;
                
                // Evaluate hover power control
                float hoverControl = _hoverInput ? hover.powerOnCurve.Evaluate(control.hover) : hover.powerOffCurve.Evaluate(control.hover);
                // TODO: Move this logic to ShipController in the future
                bool isPowerOff = control.hover == 0 || _jumpInput;
                
                if (Physics.Raycast(hoverPosition, hoverDirection, out RaycastHit hoverHit, _hoverRange, hover.groundLayer) && !isPowerOff)
                {
                    // APPLY HOVER
                    float ratio = Mathf.Clamp01(1 + (hover.height - hoverHit.distance) / hover.height);
                    float amount = _hoverRange > hover.height ? ratio : 1;
                    float pressure = Mathf.Max(hover.height - hoverHit.distance, 0) / hover.height;
                    
                    float force = hover.force * amount;
                    float suspension = hover.suspension * hover.suspensionCurve.Evaluate(pressure) * hover.force;
                    float damping = hover.damping * hoverVelocity;
                    
                    float appliedHover = (force + suspension - damping) / _hoverCount * hoverControl;
                    Vector3 appliedPosition = shipRigidbody.position + forceOffset;
                    shipRigidbody.AddForceAtPosition(appliedHover * -hoverDirection, appliedPosition, ForceMode.Acceleration);
                    
                    _hoverHitCount++;
                    if (hover.counterDrag) _counterDrag += hoverDirection / _hoverCount;
                }
                
                _lastHoverPosition[i] = hoverPosition;
            }
        }
        
        private void ApplyPull()
        {
            for (int i = 0; i < hover.sensors.Length; i++)
            {
                // Get pull and force offset
                Vector3 pullOffset = shipRigidbody.transform.TransformVector(hover.sensors[i].offset * hover.pullSpread);
                Vector3 forceOffset = shipRigidbody.transform.TransformVector(
                    Vector3.Scale(hover.sensors[i].offset, new Vector3(hover.rollEffect, 0, hover.pitchEffect)));
                
                // Get pull position and direction
                Vector3 pullPosition = shipRigidbody.position + pullOffset;
                Vector3 pullDirection = hover.pullDirection switch {
                    Direction.World => Vector3.down,
                    Direction.Gravity => world.gravity.normalized,
                    _ => Quaternion.Euler(hover.sensors[i].rotation) * -shipRigidbody.transform.up};
                
                // Recalculate pull direction based on detected ground normal
                if (hover.pullDirection == Direction.Ground)
                    if (Physics.Raycast(pullPosition, pullDirection, out RaycastHit sensorHit, _hoverRange, hover.groundLayer))
                        pullDirection = -sensorHit.normal;
                
                // Evaluate hover power control
                float hoverControl = _hoverInput ? hover.powerOnCurve.Evaluate(control.hover) : hover.powerOffCurve.Evaluate(control.hover);
                // TODO: Move this logic to ShipController in the future
                bool isPowerOff = control.hover == 0 || _jumpInput;
                
                if (Physics.Raycast(pullPosition, pullDirection, out RaycastHit pullHit, _hoverRange, hover.groundLayer) && !isPowerOff)
                {
                    // APPLY PULL
                    Vector3 gravityDirection = world.gravity.magnitude > -0.0001 ? Vector3.down : world.gravity.normalized;
                    float dotFactor = inputData.lockToGround ? 1 : Mathf.Clamp01(Vector3.Dot(gravityDirection, pullDirection));
                    float appliedPull = hover.pull * dotFactor / _hoverCount * hoverControl;
                    Vector3 appliedPosition = shipRigidbody.position + forceOffset;
                    shipRigidbody.AddForceAtPosition(appliedPull * pullDirection, appliedPosition, ForceMode.Acceleration);
                    
                    // APPLY COUNTER GRAVITY
                    Vector3 counterGravityVector = Vector3.Project(-world.gravity.normalized, -pullDirection);
                    float appliedCounterGravity = world.gravity.magnitude / _hoverCount * hoverControl;
                    shipRigidbody.AddForceAtPosition(appliedCounterGravity * counterGravityVector, appliedPosition, ForceMode.Acceleration);
                    
                    // APPLY COUNTER SLIP
                    Vector3 counterSlipVector = Vector3.ProjectOnPlane(-world.gravity.normalized, -pullDirection);
                    float appliedCounterSlip = world.gravity.magnitude * hover.counterSlip / _hoverCount * hoverControl;
                    shipRigidbody.AddForce(appliedCounterSlip * counterSlipVector, ForceMode.Acceleration);
                }
            }
        }
        
        private void ApplyJump()
        {
            Vector3 jumpDirection = shipRigidbody.transform.up;
            float applyJump = jump.force * control.jump;
            shipRigidbody.AddForce(applyJump * jumpDirection, ForceMode.Acceleration);
        }
        
        // PROPULSION
        private void ApplyThrust()
        {
            Vector3 thrustDirection = shipRigidbody.transform.forward;
            float abDragFactor = 1 - (control.abDrag * airbrake.dragAmount / 100);
            
            float currentSpeed = Vector3.Dot(_shipVelocity * KPHFactor, thrustDirection);
            float targetSpeed = (thrust.speed * control.thrust + boost.speed * control.boost
                + afterburner.speed * control.afterburner) * abDragFactor;
            float boostSpeedCap = thrust.speed + boost.speed + afterburner.speed * control.afterburner;
            
            bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
            bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed);
            float accelRatio = !isOppositeTarget ? isGreaterTarget ? 1 - currentSpeed / targetSpeed : 0
                : thrust.tightAccel ? 1 - currentSpeed / targetSpeed : 1;
            
            // APPLY THRUST
            float appliedThrust = thrust.acceleration * control.thrust * accelRatio.NaNInfSafe();
            float appliedBoost = boost.acceleration * control.boost * (currentSpeed < boostSpeedCap).ToInt();
            float appliedAfterburner = afterburner.acceleration * control.afterburner
                * afterburner.accelCurve.Evaluate(accelRatio.NaNInfSafe());
            
            float totalThrust = appliedThrust + appliedBoost + appliedAfterburner;
            shipRigidbody.AddForce(totalThrust / KPHFactor * thrustDirection, ForceMode.Acceleration);
            
            // COUNTER FORWARD DRAG
            _counterDrag += !isOppositeTarget ? isGreaterTarget ? thrustDirection : Vector3.zero
                : thrust.softDecel ? thrustDirection : Vector3.zero;
        }
        
        private void ApplyAirbrake()
        {
            // Get forward direction and speed
            Vector3 forwardDirection = shipRigidbody.transform.forward;
            float forwardSpeed = Vector3.Dot(_shipVelocity, forwardDirection);
            
            // Filter counter drag at forward direction
            float forwardCounterDrag = Vector3.Dot(_counterDrag.Absolute(), forwardDirection.Absolute());
            
            // When thrust is active, the airbrake directly modifies the top speed of thrust by a percentage
            // This value cancels out airbrake drag when thrust is active
            float counterDrag = 1 - Mathf.Clamp01(Mathf.Abs(forwardCounterDrag));
            
            // Calculate and apply airbrake drag
            float appliedDrag = forwardSpeed * airbrake.dragDecel * control.abDrag * counterDrag;
            shipRigidbody.AddForce(appliedDrag * -forwardDirection, ForceMode.Acceleration);
        }
        
        // TRACTION
        private void ApplyStrafe()
        {
            // APPLY STRAFE
            // STABILIZING GRIP so when the ship tilts, it wont cause too much bouncing
            Vector3 groundNormal = (_frontSensorInfo.normal + _backSensorInfo.normal).normalized;
            Vector3 gripPlane = inputData.lockToGround && physicsData.isOnGround ? groundNormal : world.gravity.normalized;
            Vector3 strafeDirection = Vector3.ProjectOnPlane(shipRigidbody.transform.right, gripPlane);
            if (strafe.localStrafe) strafeDirection = shipRigidbody.transform.right;
            
            float canSideshift = (!sideshift.isKinematic).ToInt();
            float canSlide = (!airbrake.disableSlide).ToInt();
            float forwardSpeed = Vector3.Dot(_shipVelocity * KPHFactor, shipRigidbody.transform.forward);
            float forwardFactor = Mathf.Clamp(forwardSpeed / airbrake.slideLimitKph, -1, 1).NaNInfSafe(1) * canSlide;
            
            float currentSpeed = Vector3.Dot(_shipVelocity, strafeDirection);
            float targetSpeed = strafe.speed * control.strafe + sideshift.speed * control.sideshift * canSideshift
                + airbrake.slideSpeed * -control.abSlide * Mathf.Abs(forwardFactor);
            
            bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
            bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed);
            float accelRatio = !isOppositeTarget ? isGreaterTarget ? 1 - currentSpeed / targetSpeed : 0
                : strafe.tightAccel ? 1 - currentSpeed / targetSpeed : 1;
            
            float appliedStrafe = strafe.acceleration * control.strafe * accelRatio.NaNInfSafe();
            float appliedSideshift = sideshift.acceleration * control.sideshift * accelRatio.NaNInfSafe() * canSideshift;
            float appliedAirbrakeSlide = airbrake.slideAccel * -control.abSlide * Mathf.Abs(forwardFactor) * accelRatio.NaNInfSafe();
            float totalStrafe = appliedStrafe + appliedSideshift + appliedAirbrakeSlide;
            shipRigidbody.AddForce(totalStrafe * strafeDirection, ForceMode.Acceleration);
            
            // COUNTER GRIP
            // cancel out grip : allow grip to slow the ship down
            // allow grip to slow the ship down : cancel out grip & let strafe.accel deal with it
            _counterGrip = !isOppositeTarget ? isGreaterTarget ? 1 : 0 : strafe.softDecel ? 1 : 0;
            
            // COUNTER DRAG
            _counterDrag += !isOppositeTarget ? isGreaterTarget ? strafeDirection : Vector3.zero
                : strafe.softDecel ? strafeDirection : Vector3.zero;
        }
        
        private void ApplyGrip()
        {
            Vector3 groundNormal = (_frontSensorInfo.normal + _backSensorInfo.normal).normalized;
            Vector3 gripPlane = inputData.lockToGround && physicsData.isOnGround ? groundNormal : world.gravity.normalized;
            Vector3 gripDirection = Vector3.ProjectOnPlane(shipRigidbody.transform.right, gripPlane);
            
            if (grip.localGrip)
            {
                float gripFactor = Vector3.Dot(gripDirection, shipRigidbody.transform.right);
                gripDirection = shipRigidbody.transform.right * gripFactor;
            }
            
            float forwardSpeed = Vector3.Dot(_shipVelocity * KPHFactor, shipRigidbody.transform.forward);
            float forwardFactor = Mathf.Clamp(forwardSpeed / airbrake.driftLimitKph, -1, 1).NaNInfSafe();
            float abDriftFactor = 1 - (airbrake.driftAmount / 100 * control.abDrift * Mathf.Abs(forwardFactor));
            
            Vector3 counterVelocity = control.strafe * strafe.speed * shipRigidbody.transform.right;
            float currentSpeed = Vector3.Dot(_shipVelocity - counterVelocity, gripDirection);
            control.grip = physicsData.isOnGround ?
                Mathf.SmoothStep(control.grip, grip.groundGrip, 1 / (grip.toGroundTime * 10)):
                Mathf.SmoothStep(control.grip, grip.airGrip, 1 / (grip.toAirTime * 10));
            
            float gripAmount = MathUtil.TriangularSequence(control.grip / 2);
            float appliedGrip = gripAmount * currentSpeed * abDriftFactor * (1 - _counterGrip);
            shipRigidbody.AddForce(appliedGrip * -gripDirection, ForceMode.Acceleration);
            
            // COUNTER SIDE DRAG
            _counterDrag += grip.counterDrag && physicsData.isOnGround ? gripDirection : Vector3.zero;
        }
        
        // ROTATION
        private void ApplySteer()
        {
            Vector3 steerAxis = shipRigidbody.transform.up;
            float forwardSpeed = Vector3.Dot(_shipVelocity * KPHFactor, shipRigidbody.transform.forward);
            float forwardFactor = Mathf.Clamp(forwardSpeed / airbrake.torqueLimitKph, -1, 1).NaNInfSafe();
            
            float currentSpeed = Vector3.Dot(_shipAngularVelocity * Mathf.Rad2Deg, steerAxis);
            float targetSpeed = steer.speed * control.steer + airbrake.torqueSpeed * control.abTorque * forwardFactor;
            
            bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
            bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed);
            float accelRatio = !isOppositeTarget ? isGreaterTarget ? 1 - currentSpeed / targetSpeed : 0
                : steer.tightAccel ? 1 - currentSpeed / targetSpeed : 1;
            
            // APPLY ALL STEER FORCES
            float appliedSteer = steer.acceleration * control.steer * accelRatio.NaNInfSafe();
            float appliedAirbrakeTorque = airbrake.torqueAccel * control.abTorque * forwardFactor * accelRatio.NaNInfSafe();
            float totalSteer = appliedSteer + appliedAirbrakeTorque;
            shipRigidbody.AddTorque(totalSteer * Mathf.Deg2Rad * shipRigidbody.transform.up, ForceMode.Acceleration);
            
            // COUNTER TURN DRAG
            _counterAngularDrag += !isOppositeTarget ? isGreaterTarget ?
                steerAxis : steerAxis * (targetSpeed / currentSpeed).NaNInfSafe() : steer.softDecel ? steerAxis : Vector3.zero;
        }
        
        private void ApplyPitch()
        {
            // APPLY PITCH
            Vector3 pitchAxis = shipRigidbody.transform.right;
            Vector3 groundForward = Vector3.ProjectOnPlane(_frontSensorInfo.endpoint - _backSensorInfo.endpoint, pitchAxis).normalized;
            if (_frontSensorInfo.isHit ^ _backSensorInfo.isHit)
                if (_frontSensorInfo.isHit && pitch.alignFront)
                    groundForward = Vector3.ProjectOnPlane(shipRigidbody.transform.forward, _frontSensorInfo.normal).normalized;
                else if (_backSensorInfo.isHit && pitch.alignBack)
                    groundForward = Vector3.ProjectOnPlane(shipRigidbody.transform.forward, _backSensorInfo.normal).normalized;
            
            float currentAngle = Vector3.SignedAngle(groundForward, shipRigidbody.transform.forward, pitchAxis);
            float targetAngle = pitch.trackAngle * control.pitch * (1 - control.maglev * maglev.enableAlign.ToInt());
            float currentSpeed = Vector3.Dot(_shipAngularVelocity * Mathf.Rad2Deg, pitchAxis);
            
            if (physicsData.isOnGround && !pitch.manualMode)
            {
                float amount = pitch.trackForce * (targetAngle - currentAngle);
                float damping = pitch.trackDamping * currentSpeed;
                float appliedPitch = (amount - damping) * Mathf.Lerp(1, _hoverHitRatio, pitch.hoverRatio);
                shipRigidbody.AddTorque(appliedPitch * Mathf.Deg2Rad * pitchAxis , ForceMode.Acceleration);
                
                _counterAngularDrag += pitchAxis;
            }
            else
            {
                if (!physicsData.isOnGround && orient.autoPitch) return;
                float pitchAccel = physicsData.isOnGround ? pitch.groundAccel : pitch.flightAccel;
                float pitchSpeed = physicsData.isOnGround ? pitch.groundSpeed : pitch.flightSpeed;
                
                float targetSpeed = pitchSpeed * control.pitch;
                bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
                bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
                bool canAccelerate = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed) || isOppositeTarget;
                
                float appliedAccel = canAccelerate ? 1 - currentSpeed / targetSpeed : 0;
                float appliedPitch = pitchAccel * control.pitch * appliedAccel.NaNInfSafe();
                shipRigidbody.AddTorque(appliedPitch * Mathf.Deg2Rad * pitchAxis, ForceMode.Acceleration);
                
                _counterAngularDrag += canAccelerate ? pitchAxis : Vector3.zero;
            }
        }
        
        private void ApplyRoll()
        {
            // APPLY ROLL
            Vector3 groundNormal = (_frontSensorInfo.normal + _backSensorInfo.normal).normalized;
            if (!_frontSensorInfo.isHit && !_backSensorInfo.isHit && roll.rollRecovery)
                if (_leftSensorInfo.isHit) groundNormal = _leftSensorInfo.normal;
                else if (_rightSensorInfo.isHit) groundNormal = _rightSensorInfo.normal;
            
            Vector3 groundRight = Vector3.ProjectOnPlane(shipRigidbody.transform.right, groundNormal).normalized;
            Vector3 rollAxis = shipRigidbody.transform.forward;
            
            float currentAngle = Vector3.SignedAngle(groundRight, shipRigidbody.transform.right, rollAxis);
            float targetAngle = roll.trackAngle * -control.roll * (1 - control.maglev * maglev.enableAlign.ToInt());
            float currentSpeed = Vector3.Dot(_shipAngularVelocity * Mathf.Rad2Deg, rollAxis);
            float targetSpeed = roll.flightSpeed * -control.roll;
            
            if (physicsData.isOnGround || _leftSensorInfo.isHit || _rightSensorInfo.isHit)
            {
                float amount = roll.trackForce * (targetAngle - currentAngle);
                float damping = roll.trackDamping * currentSpeed;
                float appliedRoll = (amount - damping) * Mathf.Clamp(_hoverHitRatio, 0.5f, 1);
                if (!roll.disable) shipRigidbody.AddTorque(appliedRoll * Mathf.Deg2Rad * rollAxis, ForceMode.Acceleration);
                
                _counterAngularDrag += rollAxis;
            }
            else
            {
                if (!physicsData.isOnGround && orient.autoRoll) return;
                
                bool isNonZero = currentSpeed != 0 && targetSpeed != 0;
                bool isOppositeTarget = Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed) && isNonZero;
                bool canAccelerate = Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed) || isOppositeTarget;
                
                float appliedAccel = canAccelerate ? 1 - currentSpeed / targetSpeed : 0;
                float appliedRoll = roll.flightAccel * -control.roll * appliedAccel.NaNInfSafe();
                shipRigidbody.AddTorque(appliedRoll * Mathf.Deg2Rad * rollAxis, ForceMode.Acceleration);
                
                _counterAngularDrag += canAccelerate ? rollAxis : Vector3.zero;
            }
        }
        
        private void ApplyOrient()
        {
            if (physicsData.isOnGround) return;
            
            if (orient.autoRoll && !_leftSensorInfo.isHit && !_rightSensorInfo.isHit)
            {
                float rollAngle = shipRigidbody.transform.eulerAngles.z;
                if (rollAngle > 180) rollAngle -= 360;
                
                Vector3 rollAxis = shipRigidbody.transform.forward;
                float currentSpeed = Vector3.Dot(_shipAngularVelocity * Mathf.Rad2Deg, rollAxis);
                
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
                float currentSpeed = Vector3.Dot(_shipAngularVelocity * Mathf.Rad2Deg, pitchAxis);
                
                float amount = orient.autoPitchForce * -pitchAngle;
                float damping = orient.autoPitchDamp * currentSpeed;
                float appliedAutoPitch = amount - damping;
                shipRigidbody.AddTorque(appliedAutoPitch * Mathf.Deg2Rad * pitchAxis, ForceMode.Acceleration);
            }
        }
        
        // TRACK
        private void ApplyMaglev()
        {
            Vector3 maglevPosition = shipRigidbody.position;
            Vector3 maglevDirection = -shipRigidbody.transform.up;
            
            if (Physics.Raycast(maglevPosition, maglevDirection, out RaycastHit sensorHit, maglev.range, maglev.groundLayer))
                maglevDirection = -sensorHit.normal;
            
            float maglevSpeed = Vector3.Dot(maglevPosition - _lastMaglevPosition, -maglevDirection) / Time.fixedDeltaTime;
            float alignSpeed = Vector3.ProjectOnPlane(_shipAngularVelocity * Mathf.Rad2Deg, shipRigidbody.transform.up).magnitude;
            _lastMaglevPosition = maglevPosition;
            
            if (Physics.Raycast(maglevPosition, maglevDirection, out RaycastHit maglevHit, maglev.range, maglev.groundLayer))
            {
                // MAGLEV
                float distance = maglev.height - maglevHit.distance;
                float force = maglev.force * distance * Mathf.Pow(Time.fixedDeltaTime, -2);
                float damping = maglevSpeed * (1 / Time.fixedDeltaTime) * maglev.damping;
                
                float appliedMaglev = (force - damping) * control.maglev;
                shipRigidbody.AddForce(appliedMaglev * -maglevDirection, ForceMode.Acceleration);
                
                // ALIGN
                Vector3 alignAxis = Vector3.Cross(shipRigidbody.transform.up, maglevHit.normal).normalized;
                float alignAngle = Vector3.Angle(shipRigidbody.transform.up, maglevHit.normal);
                
                float alignForce = maglev.alignForce * alignAngle * 50;
                float alignDamping = alignSpeed * maglev.alignDamping;
                
                float appliedAlign = (alignForce - alignDamping) * control.maglev * maglev.enableAlign.ToInt();
                shipRigidbody.AddTorque(appliedAlign * Mathf.Deg2Rad * alignAxis, ForceMode.Acceleration);
            }
        }
        
        private void ApplyJumpPad()
        {
            Vector3 jumpPadDirection = _jumpPadDirection;
            float applyJumpPad = jumpPad.force * control.jumpPad;
            shipRigidbody.AddForce(applyJumpPad * jumpPadDirection, ForceMode.Acceleration);
        }
        
        private void ApplySpeedPad()
        {
            Vector3 speedPadDirection = _speedPadDirection.normalized;
            float applySpeedPad = speedPad.force * control.speedPad;
            shipRigidbody.AddForce(applySpeedPad / KPHFactor * speedPadDirection, ForceMode.Acceleration);
        }
        
        
        // INPUT EVENTS
        private void InvokeHover(bool isOn)
        {
            _hoverInput = isOn;
        }
        
        private void InvokeJump()
        {
            StartCoroutine(OnJump());
            IEnumerator OnJump()
            {
                if (physicsData.isJumping && !physicsData.isOnGround && !jump.enableAirJump) yield break;
                
                physicsData.isJumping = true;
                _jumpInput = true;
                // control.jump = 0;
                
                float jumpTime = Mathf.Max(jump.gain + jump.hold, Time.fixedDeltaTime * 1.1f);
                yield return new WaitForSeconds(jumpTime);
                
                _jumpInput = false;
                physicsData.isJumping = false;
            }
        }
        
        private void InvokeBoost()
        {
            StartCoroutine(OnBoost());
            IEnumerator OnBoost()
            {
                if (physicsData.isBoosting) yield break;
                
                physicsData.isBoosting = true;
                _boostInput = true;
                // control.boost = 0;
                
                float boostTime = Mathf.Max(boost.gain + boost.hold, Time.fixedDeltaTime * 1.1f);
                yield return new WaitForSeconds(boostTime);
                
                _boostInput = false;
                physicsData.isBoosting = false;
            }
        }
        
        private void InvokeSideshift(bool direction)
        {
            StartCoroutine(OnSideshift());
            IEnumerator OnSideshift()
            {
                if (physicsData.isSideshifting) yield break;
                
                physicsData.isSideshifting = true;
                _sideshiftInput = direction ? 1 : -1;
                control.sideshift = 0;
                
                float sideshiftTime = Mathf.Max(sideshift.gain + sideshift.hold, Time.fixedDeltaTime);
                yield return new WaitForSeconds(sideshiftTime);
                
                _sideshiftInput = 0;
                physicsData.isSideshifting = false;
                
                yield break;
            }
        }
        
        private void InvokeMaglev(bool isOn)
        {
            _maglevInput = isOn;
        }
        
        private void InvokeJumpPad(Vector3 direction)
        {
            StartCoroutine(OnJumpPad());
            IEnumerator OnJumpPad()
            {
                if (physicsData.isJumpPadPerforming) yield break;
                
                physicsData.isJumpPadPerforming = true;
                _jumpPadInput = true;
                _jumpPadDirection = direction;
                
                float jumpPadTime = Mathf.Max(jumpPad.gain + jumpPad.hold, Time.fixedDeltaTime * 1.1f);
                yield return new WaitForSeconds(jumpPadTime);
                
                _jumpPadInput = false;
                physicsData.isJumpPadPerforming = false;
            }
        }
        
        private void InvokeSpeedPad(Vector3 direction)
        {
            StartCoroutine(OnSpeedPad());
            IEnumerator OnSpeedPad()
            {
                _speedPadInput += 1;
                _speedPadDirection += direction;
                
                float time = Mathf.Max(speedPad.gain + speedPad.hold, Time.fixedDeltaTime * 1.1f);
                yield return new WaitForSeconds(time);
                
                _speedPadInput -= 1;
                _speedPadDirection -= direction;
            }
        }
        
        
    }
}
