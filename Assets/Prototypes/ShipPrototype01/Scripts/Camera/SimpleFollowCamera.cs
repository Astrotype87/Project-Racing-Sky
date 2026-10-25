using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    // Minimal version of ship follow camera
    public class SimpleFollowCamera : MonoBehaviour
    {
        // REFERENCES
        public Transform cameraTransform;
        public ShipPhysicsV2 shipPhysics;
        
        [Header("Camera View")]
        public Transform target;
        public Vector3 offset;
        public Vector3 rotation;
        [Range(1e-05f, 179)] public float baseFOV;
        
        [Header("Camera Effects")]
        [Range(0, 100)] public float KPHPerFOV;
        
        // PRIVATE
        Camera mainCamera;
        
        public SimpleFollowCamera()
        {
            offset = new Vector3(0, 3.25f, -10);
            rotation = new Vector3(10, 0, 0);
            baseFOV = 78;
            
            KPHPerFOV = 100;
        }
        
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        
        private void LateUpdate()
        {
            // Apply camera offset, pivot and rotation
            Vector3 cameraPosition = target.TransformPoint(offset);
            Quaternion cameraRotation = target.rotation * Quaternion.Euler(rotation);
            cameraTransform.SetPositionAndRotation(cameraPosition, cameraRotation);
            
            // Increase FOV depending on ship forward speed
            mainCamera.fieldOfView = baseFOV + (shipPhysics.ForwardSpeedKPH / KPHPerFOV * 0.78f);
        }
        
    }
    
}
