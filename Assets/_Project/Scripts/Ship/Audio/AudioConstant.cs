using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TetraCreations.Attributes;

namespace ProjectRacingSky.Ship
{
    using Utility;
    
    [Serializable] public class AudioConstant : IAudioBehaviour
    {
        public Envelope volume;
        public Envelope pitch;
        public bool disabled;
        
        public float VolumeOutput => volumeOutput;
        public float PitchOutput => pitchOutput;
        
        // PRIVATE FIELDS
        private float volumeControl;
        private float volumeVelocity;
        [SerializeField] [ReadOnly] private float volumeOutput;
        
        private float pitchControl;
        private float pitchVelocity;
        [SerializeField] [ReadOnly] private float pitchOutput;
        
        
        public void Initialize()
        {
            volumeControl = volume.baseUnit;
            pitchControl = pitch.baseUnit;
        }
        
        public void UpdateAudio(float unit, float deltaTime)
        {
            if (disabled)
            {
                volumeOutput = volume.baseUnit;
                pitchOutput = pitch.baseUnit;
                return;
            }
            
            float ampTarget = (Mathf.Min(Mathf.Abs(unit), volume.maxUnit) / volume.amountPerUnit).NaNInfSafe();
            volumeControl = volumeControl.MoveToTarget(volume.baseUnit + ampTarget, volume.gain, volume.release, deltaTime);
            volumeOutput = Mathf.SmoothDamp(volumeOutput, volumeControl, ref volumeVelocity, volume.smooth);
            
            float pitchTarget = (Mathf.Min(Mathf.Abs(unit), pitch.maxUnit) / pitch.amountPerUnit).NaNInfSafe();
            pitchControl = pitchControl.MoveToTarget(pitch.baseUnit + pitchTarget, pitch.gain, pitch.release, deltaTime);
            pitchOutput = Mathf.SmoothDamp(pitchOutput, pitchControl, ref pitchVelocity, pitch.smooth);
        }
        
        [Serializable] public struct Envelope
        {
            public float baseUnit;
            public float maxUnit;
            public float amountPerUnit;
            [Space(5)]
            public float gain;
            public float release;
            public float smooth;
        }
    }
}
