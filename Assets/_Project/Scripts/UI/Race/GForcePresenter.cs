using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    using ProjectRacingSky.Ship;
    using TMPro;
    
    [DefaultExecutionOrder(ScriptOrder.UI)]
    public class GForcePresenter : MonoBehaviour
    {
        [Header("Ship")]
        [SerializeField] private Ship ship;
        [SerializeField] private LocalTransform localTransform;
        private enum LocalTransform { Ship, Model, Global }
        
        [Header("Text")]
        [SerializeField] private TMP_Text totalText;
        [SerializeField] private TMP_Text xText;
        [SerializeField] private TMP_Text yText;
        [SerializeField] private TMP_Text zText;
        
        private Transform shipTransform => ship.Transform;
        private Transform modelTransform => ship.ModelTransform;
        
        private Vector3 shipAcceleration;
        private Vector3 lastShipVelocity;
        
        private const float GRAVITY = 9.80665f;
        
        
        private void FixedUpdate()
        {
            // executed after ship fixed update via DefaultExecutionOrder(ScriptOrder.UI)
            shipAcceleration = CalculateShipAcceleration();
        }
        
        private void LateUpdate()
        {
            Vector3 shipGForce = shipAcceleration / GRAVITY;
            Vector3 shipLocalGForce = localTransform switch
            {
                LocalTransform.Ship => shipTransform.InverseTransformVector(shipGForce),
                LocalTransform.Model => modelTransform.InverseTransformVector(shipGForce),
                _ => shipGForce
            };
            
            float totalGForce = shipGForce.magnitude;
            float xGForce = shipLocalGForce.x;
            float yGForce = shipLocalGForce.y;
            float zGForce = shipLocalGForce.z;
            
            totalText.text = $"{totalGForce:F2} G";
            xText.text = $"{xGForce:F2} G";
            yText.text = $"{yGForce:F2} G";
            zText.text = $"{zGForce:F2} G";
        }
        
        private Vector3 CalculateShipAcceleration()
        {
            Vector3 shipVelocity = ship.Rigidbody.linearVelocity;
            Vector3 shipAcceleration = (shipVelocity - lastShipVelocity) / Time.deltaTime;
            lastShipVelocity = shipVelocity;
            
            return shipAcceleration;
        }
        
    }
    
}
