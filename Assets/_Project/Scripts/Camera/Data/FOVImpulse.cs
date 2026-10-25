using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Utility;
    
    /// <summary> Camera FOV animation settings containing multiple FOV layers. </summary>
    [Serializable] public class FOVImpulse
    {
        public bool disabled;
        public float amplitude;
        public FOVLayer[] fOVLayers;
        
        public float UpdateFOV(float target, float deltaTime)
        {
            if (disabled) return 0;
            
            float output = 0;
            for (int i = 0; i < fOVLayers.Length; i++)
                output += fOVLayers[i].UpdateFOV(target, deltaTime);
            return output * amplitude;
        }
        
        public void OnImpulse(MonoBehaviour script, float amount)
        {
            for (int i = 0; i < fOVLayers.Length; i++)
                script.StartCoroutine(fOVLayers[i].OnImpulse(amount));
        }
    }
}
