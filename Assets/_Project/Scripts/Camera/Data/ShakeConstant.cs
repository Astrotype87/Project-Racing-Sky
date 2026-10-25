using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Utility;
    
    /// <summary> Camera constant shake settings containing multiple shake layers. </summary>
    [Serializable] public class ShakeConstant
    {
        // INSPECTOR FIELDS
        // TODO: public ShakeConstantPreset preset; // Scriptable object
        public bool disabled;
        public float ampBase = 1;
        public float ampPerUnit;
        public float ampMaxUnit;
        [Space(5)]
        public float freqBase = 1;
        public float freqPerUnit;
        public float freqMaxUnit;
        [Space(5)]
        public float amplitude = 1;
        public float frequency = 1;
        public Noise3DLive[] positionShake;
        public Noise3DLive[] rotationShake;
        
        
        // PROPERTIES
        public Vector3 PositionOutput {get; set;}
        public Vector3 RotationOutput {get; set;}
        
        
        // METHODS
        public void Reset()
        {
            for (int i = 0; i < positionShake.Length; i++) positionShake[i].Reset();
            for (int i = 0; i < rotationShake.Length; i++) rotationShake[i].Reset();
        }
        
        public void Update(float deltaTime, float unit, float ampMod = 1, float freqMod = 1)
        {
            if (disabled) return;
            
            float ampFactor = (ampBase + (Mathf.Min(Mathf.Abs(unit), ampMaxUnit) / ampPerUnit).NaNInfSafe()) * ampMod;
            float freqFactor = (freqBase + (Mathf.Min(Mathf.Abs(unit), freqMaxUnit) / freqPerUnit).NaNInfSafe()) * freqMod;
            
            PositionOutput = Vector3.zero;
            for (int i = 0; i < positionShake.Length; i++)
                PositionOutput += positionShake[i].UpdateValue(deltaTime, amplitude * ampFactor, frequency * freqFactor);
            
            RotationOutput = Vector3.zero;
            for (int i = 0; i < rotationShake.Length; i++)
                RotationOutput += rotationShake[i].UpdateValue(deltaTime, amplitude * ampFactor, frequency * freqFactor);
        }
        
    }
    
}
