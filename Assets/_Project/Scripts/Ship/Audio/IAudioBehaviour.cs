using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    public interface IAudioBehaviour
    {
        public float VolumeOutput {get;}
        public float PitchOutput {get;}
        
        public void UpdateAudio(float unit, float deltaTime);
    }
}
