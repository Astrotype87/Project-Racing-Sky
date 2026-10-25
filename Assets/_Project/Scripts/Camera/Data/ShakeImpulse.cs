using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Utility;
    
    /// <summary> Camera impulse shake settings containing multiple shake layers. </summary>
    [Serializable] public class ShakeImpulse
    {
        // INSPECTOR FIELDS
        // TODO: public ShakeImpulsePreset preset; // Scriptable object
        public bool disabled;
        [Range(0, 2)] public float fadeIn;
        [Range(0, 2)] public float hold = 0.5f;
        [Range(0, 2)] public float fadeOut;
        public AnimationCurve fadeInCurve;
        public AnimationCurve fadeOutCurve;
        [Space(5)]
        public float amplitude;
        public float frequency;
        public Noise3DLive[] positionShake;
        public Noise3DLive[] rotationShake;
        
        
        // PROPERTIES
        public Vector3 PositionOutput {get; set;}
        public Vector3 RotationOutput {get; set;}
        
        // PRIVATE FIELDS
        [SerializeField] private float posControl;
        [SerializeField] private float rotControl;
        [SerializeField] private int eventInput;
        
        
        // METHODS
        public void Reset()
        {
            for (int i = 0; i < positionShake.Length; i++) positionShake[i].Reset();
            for (int i = 0; i < rotationShake.Length; i++) rotationShake[i].Reset();
        }
        
        public void Update(float deltaTime)
        {
            if (disabled) return;
            if (eventInput == 0 && posControl == 0) return;
            
            posControl = posControl.MoveToTarget(Mathf.Clamp01(eventInput), fadeIn, fadeOut, deltaTime);
            
            PositionOutput = Vector3.zero;
            for (int i = 0; i < positionShake.Length; i++)
                PositionOutput += positionShake[i].UpdateValue(deltaTime, amplitude * posControl, frequency);
            
            RotationOutput = Vector3.zero;
            for (int i = 0; i < rotationShake.Length; i++)
                RotationOutput += rotationShake[i].UpdateValue(deltaTime, amplitude * posControl, frequency);
        }
        
        public IEnumerator OnImpulse(float amount)
        {
            if (eventInput == 0) Reset();
            
            eventInput += 1;
            yield return new WaitForSeconds(Mathf.Max(fadeIn + hold, Time.fixedDeltaTime * 1.1f));
            eventInput -= 1;
             
            yield return new WaitForSeconds(fadeOut);
            if (eventInput == 0)
            {
                PositionOutput = Vector3.zero;
                RotationOutput = Vector3.zero;
            }
        }
        
    }
} 
