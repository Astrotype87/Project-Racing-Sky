using System;
using UnityEngine;

namespace ProjectRacingSky.Input
{
    [Serializable] public class KeyboardInputConfig
    {
        public bool invertSteer;
        public bool invertAirbrake;
        public bool invertPitch;
        public bool invertRoll;
        public float sideshiftPressTime;  // double press airbrake to sideshift
        
        public string thrustKey;
        public string reverseKey;
        public string steerLeftKey;
        public string steerRightKey;
        public string airbrakeLeftKey;
        public string airbrakeRightKey;
        public string pitchDownKey;
        public string pitchUpKey;
        public string rollLeftKey;
        public string rollRightKey;
        public string strafeLeftKey;
        public string strafeRightKey;
        public string boostKey;
        public string afterburnerKey;
        
        public string jumpKey;
        public string fireKey;
        public string drainKey;
        public string respawnKey;
        public string canopyKey;
        
        public string lookBehindKey;
        public string toggleViewKey;
    }

    [Serializable] public class TouchInputConfig
    {
        public bool enableSteerTilt;
        public bool enablePitchTilt;
        
        public float steerTiltSensitivity;
        public float pitchTiltSensitivity;
        
        public bool invertSteer;
        public bool invertAirbrake;
        public bool invertPitch;
        public float sideshiftPressTime;  // double press airbrake to sideshift
    }
    
}
