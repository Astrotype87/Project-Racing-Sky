using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TetraCreations.Attributes;

namespace ProjectRacingSky.Ship
{
    using Utility;
    
    [Serializable] public class AudioImpulse : IAudioBehaviour
    {
        public Envelope volume;
        public Envelope pitch;
        public bool disabled;
        
        public float VolumeOutput => volumeOutput * volume.amount;
        public float PitchOutput => pitchOutput * pitch.amount;
        
        // PRIVATE FIELDS
        private float volumeInput;
        private float volumeControl;
        private float volumeVelocity;
        [SerializeField] [ReadOnly] private float volumeOutput;
        
        private float pitchInput;
        private float pitchControl;
        private float pitchVelocity;
        [SerializeField] [ReadOnly] private float pitchOutput;
        
        
        public void Initialize()
        {
            volumeControl = 0;
            pitchControl = 0;
        }
        
        public void UpdateAudio(float unit, float deltaTime)
        {
            if (disabled)
            {
                volumeOutput = 0;
                pitchOutput = 0;
                return;
            }
            
            volumeControl = volumeControl.MoveToTarget(volumeInput.Clamp01Mirror(), volume.gain, volume.release, deltaTime);
            volumeOutput = Mathf.SmoothDamp(volumeOutput, volumeControl, ref volumeVelocity, volume.smooth);
            
            pitchControl = pitchControl.MoveToTarget(pitchInput.Clamp01Mirror(), pitch.gain, pitch.release, deltaTime);
            pitchOutput = Mathf.SmoothDamp(pitchOutput, pitchControl, ref pitchVelocity, pitch.smooth);
            
            // BUG: VOLUME RELEASE PROBLEM (ex: setting 1s of volume release, the output release quicker than 1 s)
            // float volumeUnit = volume.amountPerUnit;
            // float volumeTarget = volume.baseUnit + (volumeInput * volumeUnit).ClampMirror(volume.maxUnit);
            // volumeControl = volumeControl.MoveToTarget(volumeTarget, volume.gain, volume.release, deltaTime);
            // volumeOutput = Mathf.SmoothDamp(volumeOutput, volumeControl, ref volumeVelocity, volume.smooth);
            
            // float pitchUnit = pitch.amountPerUnit;
            // float pitchTarget = pitch.baseUnit + (pitchInput * pitchUnit).ClampMirror(pitch.maxUnit);
            // pitchControl = pitchControl.MoveToTarget(pitchTarget, pitch.gain, pitch.release, deltaTime);
            // pitchOutput = Mathf.SmoothDamp(pitchOutput, pitchControl, ref pitchVelocity, pitch.smooth);
        }
        
        public void OnImpulse(MonoBehaviour script, float value)
        {
            script.StartCoroutine(OnVolumeImpulse(value));
            script.StartCoroutine(OnPitchImpulse(value));
        }
        
        public IEnumerator OnVolumeImpulse(float value)
        {
            volumeInput += value;
            float time = Mathf.Max(volume.gain + volume.hold, Time.fixedDeltaTime * 1.1f);
            yield return new WaitForSeconds(time);
            volumeInput -= value;
        }
        
        public IEnumerator OnPitchImpulse(float value)
        {
            pitchInput += value;
            float time = Mathf.Max(pitch.gain + pitch.hold, Time.fixedDeltaTime * 1.1f);
            yield return new WaitForSeconds(time);
            pitchInput -= value;
        }
        
        [Serializable] public struct Envelope
        {
            public float amount;
            [Space(5)]
            public float gain;
            public float hold;
            public float release;
            public float smooth;
        }
    }
}
