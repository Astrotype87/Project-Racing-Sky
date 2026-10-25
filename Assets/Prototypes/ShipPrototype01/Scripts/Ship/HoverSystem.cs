using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    // Final Iteration of Hover Physics
    public class HoverSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody shipRigidbdy;
        [SerializeField] private Transform shipTransform;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform mainSensor;
        [SerializeField] private Transform[] hoverSensors;
        
        [Header("Gravity Settings")]
        [SerializeField] private float gravity;
        
        [Header("Hover Settings")]
        [SerializeField] private float hoverRange;
        [SerializeField] private float hoverForce;
        [SerializeField] private float hoverDamp;
        
        [Header("Debug")]
        [SerializeField] private int hoverSensor_Count;
        [SerializeField] private bool[] isHoverHit;
        [SerializeField] private Vector3[] p_HoverSensor_Pos;
        
        
        
        private void Awake()
        {
            int count = hoverSensors.Length;
            isHoverHit = new bool[count];
            p_HoverSensor_Pos = new Vector3[count];
        }
        
        private void FixedUpdate()
        {
            hoverSensor_Count = hoverSensors.Length;
            
            ApplyGravity();
            for (int i = 0; i < hoverSensor_Count; i++)
                ApplyHover(i);
        }
        
        private void ApplyGravity()
        {
            Vector3 totalGravity = gravity * Vector3.down;
            shipRigidbdy.AddForce(totalGravity);
        }
        
        private void ApplyHover(int i)
        {
            Vector3 hoverSensor_Pos = hoverSensors[i].position;
            Vector3 hoverSensor_Dir = -hoverSensors[i].up;
            
            if (Physics.Raycast(hoverSensor_Pos, hoverSensor_Dir, out RaycastHit hoverSensor_Hit, hoverRange, groundLayer))
            {
                float hoverHit_Distance = hoverSensor_Hit.distance;
                float hoverHit_Percent = (hoverRange - hoverHit_Distance) / hoverRange;
                // Debug.Log("Sensor " + i + " Pressure: " + hoverHit_Percent);
                
                if (!isHoverHit[i]) p_HoverSensor_Pos[i] = hoverSensor_Pos;
                float hoverHit_Velocity = Vector3.Dot(hoverSensor_Pos - p_HoverSensor_Pos[i], -hoverSensor_Dir) / Time.fixedDeltaTime;
                // Debug.Log("Sensor " + i + " Velocity: " + hoverHit_Velocity);
                
                float appliedHoverForce = hoverForce * hoverHit_Percent;
                float appliedHoverDamp = hoverDamp * hoverHit_Velocity;
                
                Vector3 totalHover = (appliedHoverForce - appliedHoverDamp) / hoverSensor_Count * -hoverSensor_Dir;
                shipRigidbdy.AddForceAtPosition(totalHover, hoverSensor_Pos, ForceMode.Acceleration);
                
                isHoverHit[i] = true;
                p_HoverSensor_Pos[i] = hoverSensor_Pos;
            }
            else
            {
                isHoverHit[i] = false;
            }
        }
        
        private void OnDrawGizmos()
        {
            for (int i = 0; i < hoverSensors.Length; i++)
                DebugHoverSensor(i);
        }
        
        private void DebugHoverSensor(int i)
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