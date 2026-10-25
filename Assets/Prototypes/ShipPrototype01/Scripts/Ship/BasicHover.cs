using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    // First Iteration of Hover Physics
    public class BasicHover : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody objectRb;
        public Transform sensor;
        public LayerMask ground;
        
        [Header("Hover")]
        public float hoverRange;
        public float hoverForce;
        public float hoverDamp;
        public float gravity;
        
        [Header("Debug")]
        public bool isHit;
        public RaycastHit raycastHit;
        [Range(0, 1)] public float pressure;
        [Range(0, 1)] public float pressureFactor;
        public float dampFactor;
        public Vector3 p_sensorPos;
        public float appliedHoverForce;
        public float appliedHoverDamp;
        
        
        void FixedUpdate()
        {
            ApplyGravity();
            ApplyHover();
        }
        
        void ApplyGravity()
        {
            Vector3 appliedGravity = gravity * -sensor.up;
            objectRb.AddForce(appliedGravity, ForceMode.Acceleration);
        }
        
        void ApplyHover()
        {
            if (isHit = Physics.Raycast(sensor.position, Vector3.down, out raycastHit, hoverRange, ground))
            {
                // Calculate hover force
                pressureFactor = (hoverRange - raycastHit.distance) / hoverRange;
                
                // Calculate hover damp
                if (!isHit) p_sensorPos = sensor.position;
                dampFactor = Vector3.Dot(sensor.position - p_sensorPos, sensor.up) / Time.fixedDeltaTime;
                p_sensorPos = sensor.position;
                
                // Apply hover forces
                appliedHoverForce = hoverForce * pressureFactor;
                appliedHoverDamp = hoverDamp * dampFactor;
                
                Vector3 totalHoverForce = (appliedHoverForce - appliedHoverDamp) * sensor.up;
                objectRb.AddForce(totalHoverForce, ForceMode.Acceleration);
            }
            else
            {
                
            }
        }
        
        void OnDrawGizmos()
        {
            if (Physics.Raycast(sensor.position, Vector3.down, out RaycastHit debugHit, hoverRange, ground))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(sensor.position, raycastHit.point);
                Gizmos.DrawSphere(raycastHit.point, 0.125f);
                pressure = (hoverRange - debugHit.distance) / hoverRange;
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(sensor.position, Vector3.down * hoverRange);
                pressure = 0f;
            }
        }
    }
    
}
