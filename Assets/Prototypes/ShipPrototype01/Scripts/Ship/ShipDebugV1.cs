using UnityEngine;

namespace Archived
{
    public class ShipDebugV1 : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private ShipStatus shipStatus;
        [SerializeField] private ShipPhysicsV4 shipPhysics;
        
        [Header("Debug")]
        public bool debugGroundSensor;
        public bool debugHoverSensor;
        public bool debugGrip;
        public bool logAngleToGround;
        
        [Header("Sensor Gizmos")]
        public float hitRadius;
        public float trueHeight;
        public float trueRoundUp;
        public float normalLength;
        
        [Header("Grip Gizmos")]
        public float rayOffset;
        public float rayTimeFrame;
        
        
        // PROPERTIES
        private Vector3 shipPosition        {get => shipPhysics.shipPosition;}
        private LayerMask groundLayer       {get => shipPhysics.shipGroundLayer;}
        private Transform groundSensor      {get => shipPhysics.shipGroundSensor.sensor;}
        private float groundRange           {get => shipPhysics.shipGroundSensor.range;}
        private bool isGroundHit            {get => shipPhysics.shipGroundSensor.isOnGround;}
        private bool lockToGround           {get => shipPhysics.shipLockToGround;}
        private Transform[] hoverSensors    {get => shipPhysics.shipHoverSensors;}
        private float hoverHeight            {get => shipPhysics.shipHoverModule.shipHoverHeight;}
        private float hoverRange            {get => shipPhysics.shipHoverModule.shipHoverRange;}
        private float hoverSpread           {get => shipPhysics.shipHoverModule.shipHoverSpread;}
        private ShipPhysicsV4.HoverDirection hoverDirection {get => shipPhysics.shipHoverModule.shipHoverDirection;}
        private Vector3 gripPlane = new Vector3(1, 0, 1);
        
        public ShipDebugV1()
        {
            hitRadius = 0.25f;
            trueHeight = 5f;
            trueRoundUp = 0.0009f;
            normalLength = 2.5f;
            
            rayOffset = 4;
            rayTimeFrame = 0.25f;
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void OnDrawGizmos()
        {
            if (debugGroundSensor) DebugGroundSensor();
            if (debugHoverSensor) DebugHoverSensor();
            if (debugGrip) DebugGrip();
        }
        
        private void Update()
        {
            Transform ship = transform;
            Vector3 pitchAxis = transform.right;
            Vector3 rollAxis = transform.up;
            
            Vector3 groundUp = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundRange, groundLayer) ?
                hit.normal : Vector3.up;
            
            float pitchAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(ship.forward, groundUp), ship.forward, pitchAxis);
            float rollAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(ship.right, groundUp), ship.right, rollAxis);
            
            if (logAngleToGround)
            Debug.Log("Pitch Angle: " + pitchAngle + " degrees" + "   Roll Angle: " + rollAngle + " degrees");
        }
        
        
        // CLASS METHODS
        private void DebugGroundSensor()
        {
            if (Physics.Raycast(groundSensor.position, -groundSensor.up, out RaycastHit debugHit, groundRange, groundLayer))
            {
                Gizmos.color = Color.white;
                Gizmos.DrawRay(groundSensor.position, -groundSensor.up * debugHit.distance);
                Gizmos.DrawSphere(debugHit.point, hitRadius);
                Gizmos.DrawRay(debugHit.point, debugHit.normal * normalLength);
            }
            else
            {
                Gizmos.DrawRay(groundSensor.position, -groundSensor.up * groundRange);
            }
        }
        
        private void DebugHoverSensor()
        {
            for (int i = 0; i < hoverSensors.Length; i++)
            {
                Vector3 hoverSensor_Pos = Vector3.Lerp(shipPosition, hoverSensors[i].position, hoverSpread);
                Vector3 hoverSensor_Dir = -hoverSensors[i].up;
                
                if (hoverDirection == 0)
                {
                    DrawSensor(hoverSensor_Pos, hoverSensor_Dir, hoverRange, new Color(1, 1, 1, 0.33f), Color.clear, groundLayer);
                    if (Physics.Raycast(hoverSensor_Pos, hoverSensor_Dir, out RaycastHit sensorHit, hoverRange, groundLayer))
                        hoverSensor_Dir = -sensorHit.normal;
                    DrawRay(sensorHit.point, sensorHit.normal, normalLength, new Color(1, 1, 0, 0.67f));
                }
                
                DrawSensor(hoverSensor_Pos, hoverSensor_Dir, hoverRange, Color.blue, Color.red, groundLayer);
                DrawSensor(hoverSensor_Pos, hoverSensor_Dir, hoverHeight + trueRoundUp, Color.blue, Color.green, groundLayer);
            }
            
            void DrawSensor(Vector3 position, Vector3 direction, float range, Color hitColor, Color notHitColor, LayerMask hitLayer)
            {
                if (Physics.Raycast(position, direction, out RaycastHit hitInfo, range, hitLayer))
                {
                    Gizmos.color = hitColor;
                    Gizmos.DrawRay(position, direction * hitInfo.distance);
                    Gizmos.DrawSphere(hitInfo.point, hitRadius);
                }
                else
                {
                    Gizmos.color = notHitColor;
                    Gizmos.DrawRay(position, direction * range);
                }
            }
            
            void DrawRay(Vector3 position, Vector3 direction, float range, Color color)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(position, direction * range);
            }
        }
        
        private void DebugGrip()
        {
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

    
}
