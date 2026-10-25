using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    // Basic Version of Ship Physics
    // Enough to make the ship move
    public class ShipPhysicsV1 : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private Transform shipTransform;
        [SerializeField] private Transform mainSensor;
        [SerializeField] private Transform[] hoverSensors;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Components")]
        [SerializeField] private HoverModule hoverModule;
        [SerializeField] private GravityModule gravityModule;
        [SerializeField] private ThrustModule thrustModule;
        [SerializeField] private SteerModule steerModule;
        [SerializeField] private GripModule gripModule;
        
        private void Awake()
        {
            hoverModule.Awake(hoverSensors.Length);
        }
        
        private void Update()
        {
            thrustModule.UpdateControl();
            steerModule.UpdateControl();
        }
        
        private void FixedUpdate()
        {
            hoverModule.ApplyHover(shipRigidbody, hoverSensors, groundLayer);
            gravityModule.ApplyGravity(shipRigidbody);
            thrustModule.ApplyThrust(shipRigidbody, shipTransform);
            steerModule.ApplySteer(shipRigidbody, shipTransform);
            gripModule.ApplyGrip(shipRigidbody, shipTransform);
        }
        
        private void OnDrawGizmos()
        {
            hoverModule.DebugHoverSensor(hoverSensors, groundLayer);
            gripModule.DebugGrip(shipRigidbody, shipTransform);
            
            // Gizmos.color = Color.white;
            // Gizmos.DrawSphere(transform.TransformPoint(shipRigidbody.centerOfMass), 1);
        }
        
        
        
        
        // SHIP PHYSICS COMPONENTS:
        // HOVER, GRAVITY, STABILIZER, THRUST, STEER, GRIP, AIRBRAKE
        
        [Serializable]
        private class HoverModule
        {
            [SerializeField] private float hoverRange;
            [SerializeField] private float hoverForce;
            [SerializeField] private float hoverDamp;
            
            private int hoverSensor_Count;
            private bool[] isHoverHit;
            private Vector3[] p_hoverSensor_Pos;
            
            public HoverModule()
            {
                hoverRange = 8.25f;
                hoverForce = 375;
                hoverDamp = 6;
            }
            
            public void Awake(int count)
            {
                isHoverHit = new bool[count];
                p_hoverSensor_Pos = new Vector3[count];
            }
            
            public void ApplyHover(Rigidbody rb, Transform[] hs, LayerMask gr)
            {
                hoverSensor_Count = hs.Length;
                for (int i = 0; i < hs.Length; i++)
                    HoverGenerator(rb, hs, i, gr);
            }
            
            private void HoverGenerator(Rigidbody shipRigidbody, Transform[] hoverSensors, int i, LayerMask groundLayer)
            {
                Vector3 hoverSensor_Pos = hoverSensors[i].position;
                Vector3 hoverSensor_Dir = -hoverSensors[i].up;
                
                if (Physics.Raycast(hoverSensor_Pos, hoverSensor_Dir, out RaycastHit hoverSensor_Hit, hoverRange, groundLayer))
                {
                    float hoverHit_Distance = hoverSensor_Hit.distance;
                    float hoverHit_Percent = (hoverRange - hoverHit_Distance) / hoverRange;
                    
                    if (!isHoverHit[i]) p_hoverSensor_Pos[i] = hoverSensor_Pos;
                    float hoverHit_Velocity = Vector3.Dot(hoverSensor_Pos - p_hoverSensor_Pos[i], -hoverSensor_Dir) / Time.fixedDeltaTime;
                    
                    float appliedHoverForce = hoverForce * hoverHit_Percent;
                    float appliedHoverDamp = hoverDamp * hoverHit_Velocity;
                    
                    Vector3 totalHoverForce = (appliedHoverForce - appliedHoverDamp) / hoverSensor_Count * -hoverSensor_Dir;
                    shipRigidbody.AddForceAtPosition(totalHoverForce, hoverSensor_Pos, ForceMode.Acceleration);
                    
                    isHoverHit[i] = true;
                    p_hoverSensor_Pos[i] = hoverSensor_Pos;
                }
                else
                {
                    isHoverHit[i] = false;
                }
            }
            
            public void DebugHoverSensor(Transform[] hoverSensors, LayerMask groundLayer)
            {
                for (int i = 0; i < hoverSensors.Length; i++)
                {
                    Vector3 hoverSensor_Pos = hoverSensors[i].position;
                    Vector3 hoverSensor_Dir = -hoverSensors[i].up;
                    
                    if (Physics.Raycast(hoverSensor_Pos, hoverSensor_Dir, out RaycastHit hoverSensor_Hit, hoverRange, groundLayer))
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(hoverSensor_Pos, hoverSensor_Hit.point);
                        Gizmos.DrawSphere(hoverSensor_Hit.point, 0.5f);
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawRay(hoverSensor_Pos, hoverSensor_Dir * hoverRange);
                    }
                }
            }
        }
        
        
        [Serializable]
        private class GravityModule
        {
            [SerializeField] private float gravity;
            
            public GravityModule()
            {
                gravity = 125;
            }
            
            public void ApplyGravity(Rigidbody shipRigidbody)
            {
                Vector3 totalGravity = gravity * Vector3.down;
                shipRigidbody.AddForce(totalGravity, ForceMode.Acceleration);
            }
        }
        
        
        [Serializable]
        private class ThrustModule
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
                thrustForce = 300;
                thrustSpeed = 600;
                thrustStall = 500;
                
                thrustAttack = 0.25f;
                thrustRelease = 0.25f;
            }
            
            public void UpdateControl()
            {
                float thrustTime = Mathf.Abs(thrustControl) >= Mathf.Abs(p_thrustControl) ? thrustAttack : thrustRelease;
                p_thrustControl = thrustControl;
                thrustControl = Mathf.MoveTowards(thrustControl, Input.GetAxisRaw("Vertical"), Time.deltaTime / thrustTime);
            }
            
            public void ApplyThrust(Rigidbody shipRigidbody, Transform shipTransform)
            {
                Vector3 thrustDirection = shipTransform.forward;
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
        private class SteerModule
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
            
            public void UpdateControl()
            {
                float steerTime = Mathf.Abs(steerControl) >= Mathf.Abs(p_steerControl) ? steerAttack : steerRelease;
                p_steerControl = steerControl;
                steerControl = Mathf.MoveTowards(steerControl, Input.GetAxisRaw("Horizontal"), Time.deltaTime / steerTime);
            }
            
            public void ApplySteer(Rigidbody shipRigidbody, Transform shipTransform)
            {
                Vector3 steerAxis = shipTransform.up;
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
        private class GripModule
        {
            [SerializeField] private float gripForce;
            
            private float rayOffset = 4;
            private float rayTimeFrame = 0.25f;
            
            public GripModule()
            {
                gripForce = 10;
            }
            
            public void ApplyGrip(Rigidbody shipRigidbody, Transform shipTransform)
            {
                Vector3 sideDirection = shipTransform.right;
                float sideSpeed = Vector3.Dot(shipRigidbody.linearVelocity, sideDirection);
                
                float appliedGripForce = gripForce * sideSpeed;
                float totalGripForce = appliedGripForce;
                shipRigidbody.AddForce(totalGripForce * -sideDirection, ForceMode.Acceleration);
            }
            
            public void DebugGrip(Rigidbody shipRigidbody, Transform shipTransform)
            {
                Vector3 sideDirection = shipTransform.right;
                float sideSpeed = Vector3.Dot(shipRigidbody.linearVelocity, sideDirection);
                Vector3 rayPos = shipTransform.position;
                Vector3 rayDir = shipTransform.right;
                
                if (sideSpeed >= 0)
                    rayPos += rayDir * rayOffset;
                else rayPos -= rayDir * rayOffset;
                
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(rayPos, rayDir * sideSpeed * rayTimeFrame);
            }
        }
        
        
        
    }
    
}
