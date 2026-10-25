using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    [Serializable]
    public class ActionInput
    {
        public Action<bool> OnRearView;   // false = RETURN TO SHIP VIEW, true = LOOK BEHIND
        public Action OnToggleView;
    }
    
    [Serializable]
    public class CameraData
    {
        public ViewMode viewMode;
        public CameraView cameraView;
        
        public Vector3 followDampOffset;
        public Vector3 orbitDampOffset;
        
        public Vector3 offsetPosition;
        public Vector3 offsetRotation;
        
        public Vector3 shakePosition;
        public Vector3 shakeRotation;
        
        public float fOVOffset;
        
        public void ResetOffset()
        {
            followDampOffset = Vector3.zero;
            orbitDampOffset = Vector3.zero;
            
            offsetPosition = Vector3.zero;
            offsetRotation = Vector3.zero;
            
            shakePosition = Vector3.zero;
            shakeRotation = Vector3.zero;
            
            fOVOffset = 0;
        }
        
    }
    
}
