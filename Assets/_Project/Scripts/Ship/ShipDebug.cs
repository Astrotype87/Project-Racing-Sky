using UnityEngine;

namespace ProjectRacingSky.Ship
{
    [ExecuteAlways]
    [DefaultExecutionOrder(ScriptOrder.ShipDebug)]
    public class ShipDebug : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("References")]
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private ShipPhysics shipPhysics;
        
        [Header("Debug")]
        public bool runtimeMode;
        public bool debugMainSensor;
        public bool debugHoverSensor;
        public bool debugPitch;
        public bool debugGrip;
        
        [Header("Log")]
        public bool logSpeed;
        public bool logAngleToGround;
        public bool logPitchAngle;
        public bool logMainSensor;
        
        [Header("Sensor Gizmos")]
        public float hitRadius;
        public float forceRadius;
        public float heightRoundUp;
        public float normalLength;
        
        [Header("Grip Gizmos")]
        public float rayOffset;
        public float rayTimeFrame;
        
        
        // PRIVATE GETTERS
        private MainSensors mainSensors         => shipPhysics.DebugLiveData.mainSensors;
        private Sensor[] hoverSensors           => shipPhysics.DebugLiveData.hoverSensors;
        
        private LayerMask groundLayer           => shipPhysics.DebugLiveData.hoverGroundLayer;
        private bool lockToGround               => shipPhysics.DebugLiveData.isLockToGround;
        
        private float hoverHeight               => shipPhysics.DebugLiveData.hoverHeight;
        private float hoverRange                => shipPhysics.DebugLiveData.hoverRange;
        private float hoverSpread               => shipPhysics.DebugLiveData.hoverSpread;
        private float hoverPitchEffect          => shipPhysics.DebugLiveData.hoverPitchEffect;
        private float hoverRollEffect           => shipPhysics.DebugLiveData.hoverRollEffect;
        private Direction direction             => shipPhysics.DebugLiveData.direction;
        
        // PRIVATE FIELDS
        private bool _isOnGround;
        private SensorInfo _frontInfo;
        private SensorInfo _backInfo;
        private SensorInfo _leftInfo;
        private SensorInfo _rightInfo;
        private Vector3 _prevVelocity;
        
        // PRIVATE CONSTANTS
        private const float KPHFactor = 3.6f;
        private readonly Vector3 Gravity = new Vector3(0, -9.81f, 0);
        
        
        // CONSTRUCTOR & FINALIZER
        public ShipDebug()
        {
            hitRadius = 0.25f;
            heightRoundUp = 0.0009f;
            normalLength = 2.5f;
            
            rayOffset = 4;
            rayTimeFrame = 0.25f;
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void OnDrawGizmos()
        {
            if (runtimeMode) return;
            UpdateDebug();
        }
        
        private void FixedUpdate()
        {
            if (!Application.isPlaying) return;
            UpdateLog();
        }
        
        private void LateUpdate()
        {
            if (!runtimeMode) return;
            UpdateDebug();
        }
        
        
        // LOG METHODS
        private void UpdateLog()
        {
            if (logSpeed) LogSpeed();
            if (logAngleToGround) LogAngleToGround();
        }
        
        private void LogSpeed()
        {
            // CALCULATE ACCELERATION
            Vector3 acceleration = (shipRigidbody.linearVelocity - _prevVelocity) / Time.fixedDeltaTime;
            _prevVelocity = shipRigidbody.linearVelocity;
            
            // GET ALL SPEED
            float speedMPS = shipRigidbody.linearVelocity.magnitude;
            float speedKPH = speedMPS * KPHFactor;
            float turningDPS = Vector3.Dot(shipRigidbody.angularVelocity * Mathf.Rad2Deg, transform.up);
            Vector3 gForce = shipRigidbody.transform.InverseTransformVector((-Gravity - acceleration) / 9.81f);
            
            Debug.Log($"Speed: {speedMPS.ToString("0.00")} MPS / {speedKPH.ToString("0.00")} KPH"
                + $"    Turning: {turningDPS.ToString("0.000")} DPS"
                + $"    G-Force: {gForce.ToString("0.000")}");
        }
        
        private void LogAngleToGround()
        {
            Transform ship = transform;
            Vector3 pitchAxis = transform.right;
            Vector3 rollAxis = transform.up;

            Vector3 groundUp = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, hoverRange, groundLayer) ?
                hit.normal : Vector3.up;
            
            float pitchAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(ship.forward, groundUp), ship.forward, pitchAxis);
            float rollAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(ship.right, groundUp), ship.right, rollAxis);
            
            Debug.Log("Pitch Angle: " + pitchAngle + " degrees" + "   Roll Angle: " + rollAngle + " degrees");
        }
        
        
        // GIZMOS METHODS
        private void UpdateDebug()
        {
            if (!shipPhysics || !shipRigidbody || !this.enabled) return;
            
            if (debugMainSensor) DebugMainSensor();
            if (debugHoverSensor) DebugHoverSensor();
            if (debugPitch) DebugPitch();
            if (debugGrip) DebugGrip();
        }
        
        private void DebugMainSensor()
        {
            Vector3 sensorPosition = shipRigidbody.transform.position;
            Vector3 sensorDirection = -shipRigidbody.transform.up;
            
            if (Physics.Raycast(sensorPosition, sensorDirection, out RaycastHit hitInfo, hoverRange, groundLayer))
            {
                DrawRay(sensorPosition, sensorDirection * hitInfo.distance, new Color(1, 1, 1, 0.25f));
                DrawSphere(hitInfo.point, hitRadius, new Color(1, 1, 1, 0.25f));

                Vector3 groundForward = Vector3.ProjectOnPlane(shipRigidbody.transform.forward, hitInfo.normal).normalized;
                Vector3 groundRight = Vector3.ProjectOnPlane(shipRigidbody.transform.right, hitInfo.normal).normalized;

                DrawRay(hitInfo.point, groundForward * normalLength, new Color(0, 1, 0, 0.5f));
                DrawRay(hitInfo.point, groundRight * normalLength, new Color(1, 0, 0, 0.5f));
                if (logMainSensor) Debug.Log("Ground Forward: " + groundForward + "  Ground Right: " + groundRight);
            }
            else
            {
                DrawRay(sensorPosition, sensorDirection * hoverRange, new Color(1, 1, 1, 0.125f));
            }
        }
        
        private void DebugHoverSensor()
        {
            for (int i = 0; i < hoverSensors.Length; i++)
            {
                Vector3 forceOffset = shipRigidbody.transform.TransformVector(
                    Vector3.Scale(hoverSensors[i].offset, new Vector3(hoverRollEffect, 0, hoverPitchEffect)));
                DrawSphere(transform.position + forceOffset, forceRadius, Color.white);
                
                Vector3 sensorOffset = shipRigidbody.transform.TransformVector(hoverSensors[i].offset * hoverSpread);
                Vector3 hoverPosition = transform.position + sensorOffset;
                Vector3 hoverDirection = direction == Direction.World ? Vector3.down :
                    Quaternion.Euler(hoverSensors[i].rotation) * -transform.up;
                
                if (direction == Direction.Ground)
                {
                    DrawSensor(hoverPosition, hoverDirection, hoverRange, new Color(1, 1, 1, 0.33f), Color.clear);
                    if (Physics.Raycast(hoverPosition, hoverDirection, out RaycastHit sensorHit, hoverRange, groundLayer))
                        hoverDirection = -sensorHit.normal;
                    DrawRay(sensorHit.point, sensorHit.normal * normalLength, new Color(1, 1, 0, 0.67f));
                }
                
                DrawSensor(hoverPosition, hoverDirection, hoverRange, Color.blue, Color.red);
                DrawSensor(hoverPosition, hoverDirection, hoverHeight + heightRoundUp, Color.blue, Color.green);
            }
            
            void DrawSensor(Vector3 position, Vector3 direction, float range, Color hitColor, Color notHitColor)
            {
                if (Physics.Raycast(position, direction, out RaycastHit hitInfo, range, groundLayer))
                {
                    DrawRay(position, direction * hitInfo.distance, hitColor);
                    DrawSphere(hitInfo.point, hitRadius, hitColor);
                }
                else
                {
                    DrawRay(position, direction * range, notHitColor);
                }
            }
        }
        
        private void DebugPitch()
        {
            // Group settings into tuple
            (Transform, Direction, float, LayerMask) settings
                = (shipRigidbody.transform, direction, hoverRange, mainSensors.groundLayer);
            
            // Update sensors
            _frontInfo.UpdateSensor(mainSensors.front, settings);
            _backInfo.UpdateSensor(mainSensors.back, settings);
            _leftInfo.UpdateSensor(mainSensors.left, settings);
            _rightInfo.UpdateSensor(mainSensors.right, settings);
            
            Vector3 pitchAxis = shipRigidbody.transform.right;
            Vector3 endpointForward = Vector3.ProjectOnPlane(_frontInfo.endpoint - _backInfo.endpoint, pitchAxis).normalized;
            float pitchAngle = Vector3.SignedAngle(endpointForward, shipRigidbody.transform.forward, pitchAxis);
            
            DrawSphere(_frontInfo.endpoint, hitRadius / 2, new Color(1, 1, 1, 0.5f));
            DrawSphere(_backInfo.endpoint, hitRadius / 2, new Color(1, 1, 1, 0.5f));
            DrawRay(_backInfo.endpoint, _frontInfo.endpoint - _backInfo.endpoint, new Color(1, 1, 0, 0.25f));
            DrawRay(shipRigidbody.transform.position, endpointForward * 3, Color.green);
            
            if (logPitchAngle) Debug.Log("Pitch Angle: " + pitchAngle);
            
            // string grdFwdTxt = "USE BOTH FRONT & BACK SENSOR";
            // if (_frontSensorInfo.isHit ^ _backSensorInfo.isHit)
            //     if (_frontSensorInfo.isHit && pitch.frontRecovery)
            //         grdFwdTxt = "USE FRONT SENSOR";
            //     else if (_backSensorInfo.isHit && pitch.backRecovery)
            //         grdFwdTxt = "USE BACK SENSOR";
            
            // Debug.Log($"LOG PITCH GROUND: {grdFwdTxt}");
        }
        
        private void DebugRoll()
        {
            
        }
        
        private void DebugGrip()
        {
            Vector3 sideDirection = shipRigidbody.transform.right;
            float sideSpeed = Vector3.Dot(shipRigidbody.linearVelocity, sideDirection);
            Vector3 rayPos = shipRigidbody.transform.position;
            Vector3 rayDir = shipRigidbody.transform.right;
            
            if (sideSpeed >= 0)
                rayPos += rayDir * rayOffset;
            else rayPos -= rayDir * rayOffset;
            
            DrawRay(rayPos, rayDir * sideSpeed * rayTimeFrame, Color.blue);
        }
        
        
        // GIZMOS METHODS
        private void DrawRay(Vector3 from, Vector3 direction, Color color)
        {
            if (runtimeMode)
                Popcron.Gizmos.Line(from, from + direction, color);
            else
            {
                Gizmos.color = color;
                Gizmos.DrawRay(from, direction);
            }
        }
        
        private void DrawSphere(Vector3 center, float radius, Color color)
        {
            if (runtimeMode)
                Popcron.Gizmos.Sphere(center, radius, color);
            else
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(center, radius);
            }
        }
        
        
    }
}
