using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Utility;
    
    [Serializable] public class OffsetLayer
    {
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        [Space(5)]
        [Range(0, 4)] public float gain;
        [Range(0, 4)] public float release;
        [Range(0, 1)] public float smooth;
        [Range(0, 4)] public float hold;
        public bool correctYOffset;
        
        
        public Vector3 posOffsetOutput;
        public Vector3 rotOffsetOutput;
        
        public Vector3 PosOffsetOutput => posOffsetOutput;
        public Vector3 RotOffsetOutput => rotOffsetOutput;
        
        private float control;
        private float velocity;
        private float output;
        private float eventInput;
        
        
        public void UpdateOffset(float target, float deltaTime)
        {
            control = control.MoveToTargetSigned(target + eventInput.Clamp01Mirror(), gain, release, deltaTime);
            output = Mathf.SmoothDamp(output, control, ref velocity, smooth);
            
            posOffsetOutput = positionOffset * output;
            rotOffsetOutput = rotationOffset * output;
            
            // posOffsetOutput.y = positionOffset.y * (Mathf.Cos((output + 1) * Mathf.PI) + 1);
            
            // if (!mirrorPosition) posOffsetOutput = positionOffset * output;
            // else for (int i = 0; i < 3; i++)
            //     posOffsetOutput[i] = Mathf.Abs(positionOffset[i] * output) * Mathf.Sign(positionOffset[i]);
            
            // if (!mirrorRotation) rotOffsetOutput = rotationOffset * output;
            // else for (int i = 0; i < 3; i++)
            //     rotOffsetOutput[i] = Mathf.Abs(rotationOffset[i] * output) * Mathf.Sign(rotationOffset[i]);
        }
        
        public void UpdateYCorrection(float mod)
        {
            if (correctYOffset)
            posOffsetOutput.y = positionOffset.y * Mathf.Abs(output * output) * mod;
        }
        
        public IEnumerator OnImpulse(float value)
        {
            eventInput += value;
            yield return new WaitForSeconds(Mathf.Max(gain + hold, Time.fixedDeltaTime * 1.1f));
            eventInput -= value;
        }
        
    }
    
}
