using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Input
{
    public class InputEvents
    {
        // RACE ACTIONS
        public Action<bool> OnRearView;
        public Action OnToggleView;
        public Action OnPause;
        public Action OnResume;
        
        // SHIP ACTIONS
        public Action<float> OnThrust;
        public Action<float> OnReverse;
        public Action<bool> OnAfterburner;
        
        public Action<float> OnAirbrakeLeft;
        public Action<float> OnAirbrakeRight;
        
        public Action<bool> OnSideshift;
        public Action<float> OnStrafe;
        
        public Action<float> OnSteer;
        public Action<float> OnPitch;
        public Action<float> OnRoll;
        
        public Action OnHover;
        public Action OnJump;
        public Action OnBoost;
        
        public Action OnFire;
        public Action OnDrain;
        public Action OnRespawn;
        public Action OnCanopy;
        
        public void ClearAllSubscribers()
        {
            OnRearView = null;
            OnToggleView = null;
            OnPause = null;
            OnResume = null;
            
            OnThrust = null;
            OnReverse = null;
            OnAfterburner = null;
            OnAirbrakeLeft = null;
            OnAirbrakeRight = null;
            OnSideshift = null;
            OnStrafe = null;
            OnSteer = null;
            OnPitch = null;
            OnRoll = null;
            OnHover = null;
            OnJump = null;
            OnBoost = null;
            
            OnFire = null;
            OnDrain = null;
            OnRespawn = null;
            OnCanopy = null;
        }
        
    }
    
}
