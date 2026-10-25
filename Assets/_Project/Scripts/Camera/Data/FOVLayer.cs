using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Utility;
    
    [Serializable] public class FOVLayer
    {
        public float FOV;
        [Range(0, 4)] public float gain;
        [Range(0, 4)] public float release;
        [Range(0, 0.5f)] public float smooth;
        [Range(0, 4)] public float hold;
        
        private float control;
        private float velocity;
        private float output;
        
        private float eventInput;
        
        public float UpdateFOV(float target, float deltaTime)
        {
            control = control.MoveToTargetSigned(target + eventInput.Clamp01Mirror(), gain, release, deltaTime);
            output = Mathf.SmoothDamp(output, control, ref velocity, smooth);
            return output * FOV;
        }
        
        public IEnumerator OnImpulse(float value)
        {
            control = 0;
            eventInput += value;
            yield return new WaitForSeconds(Mathf.Max(gain + hold, Time.fixedDeltaTime * 1.1f));
            eventInput -= value;
        }
    }
}
