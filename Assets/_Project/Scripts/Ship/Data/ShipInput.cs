using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky
{
    [Serializable] public class PilotInput
    {
        [Range(0, 1)] public float thrustInput;
        [Range(0, 1)] public float reverseInput;
        public bool afterburnerInput;
        [Range(0, 1)] public float airbrakeLeftInput;
        [Range(0, 1)] public float airbrakeRightInput;
        [Range(-1, 1)] public float strafeInput;
        [Range(-1, 1)] public float steerInput;
        [Range(-1, 1)] public float pitchInput;
        [Range(-1, 1)] public float rollInput;
        
        public Action<bool> OnHover;
        public Action OnJump;
        public Action OnBoost;
        public Action<bool> OnSideshift;
        
        public Action OnFire;
        public Action OnDrain;
        public Action OnRespawn;
        public Action OnCanopy;
        
        public void Reset()
        {
            thrustInput = 0;
            reverseInput = 0;
            afterburnerInput = false;
            airbrakeLeftInput = 0;
            airbrakeRightInput = 0;
            strafeInput = 0;
            steerInput = 0;
            pitchInput = 0;
            rollInput = 0;
        }
    }
    
    [Serializable] public class TrackInput
    {
        public Action<bool> OnMaglev;
        public Action<Vector3> OnJumpPad;
        public Action<Vector3> OnSpeedPad;
        public float speedZoneInput;
        
        public Action OnPickupPad;
        public Action<float> OnSlowdown;
    }
    
    [Serializable] public class ZoneInput
    {
        public Vector3 windInput;
        public float turbulenceInput;
    }
    
    
    
}
