using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    [Serializable] public class InputData
    {
        // PILOT INPUT
        [Range(0, 1)] public float thrustInput;
        [Range(0, 1)] public float reverseInput;
        [Range(0, 1)] public float afterburnerInput;
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
        
        // TRACK INPUT
        public Action<bool> OnMaglev;
        public Action<Vector3> OnJumpPad;
        public Action<Vector3> OnSpeedPad;
        public float speedZoneInput;
        
        public Action OnPickupPad;
        public Action<float> OnSlowdown;
        
        // ZONE INPUT
        public Vector3 windInput;
        public float turbulenceInput;
        public bool lockToGround;
    }
    
    [Serializable] public class ControllerData
    {
        public float jumpFuel;
        public float jumpCooldown;
        public bool isJumpRefilling;
        
        public float boostFuel;
        public float boostConsumeCooldown;
        public float boostRefillCooldown;
        public bool isBoostRefilling;
        
        public float aftburFuel;
        public float aftburConsumeCooldown;
        public float aftburRefillCooldown;
        public bool isAftburConsuming;
        public bool isAftburRefilling;
        
        public float sideshiftFuel;
        public float sideshiftCooldown;
        public bool isSideshiftRefilling;
        
        public float jumpPadCooldown;
    }
    
    [Serializable] public class MechanicsData
    {
        public float health;
        public string pickup;
    }
    
    [Serializable] public class PhysicsData
    {
        public bool isOnGround;
        
        public bool isJumping;
        public bool isBoosting;
        public bool isSideshifting;
        public bool isJumpPadPerforming;
        
        public Control control;
        
        public struct Control
        {
            [Header("Global Control")]
            public Vector3 wind;
            public float turbulence;
            
            [Header("Ship Control")]
            [Range(0, 1)] public float hover;
            [Range(0, 1)] public float jump;
            [Space]
            [Range(-1, 1)] public float thrust;
            [Range(0, 1)] public float boost;
            [Range(0, 1)] public float afterburner;
            [Space]
            [Range(0, 1)] public float abDrag;
            [Range(-1, 1)] public float abTorque;
            [Range(0, 1)] public float abDrift;
            [Range(-1, 1)] public float abSlide;
            [Space]
            [Range(0, 10)] public float grip;
            [Range(-1, 1)] public float strafe;
            [Range(-1, 1)] public float sideshift;
            [Space]
            [Range(-1, 1)] public float steer;
            [Range(-1, 1)] public float pitch;
            [Range(-1, 1)] public float roll;
            
            [Header("Track Control")]
            [Range(0, 1)] public float maglev;
            [Range(0, 1)] public float jumpPad;
            [Range(0, 1)] public float speedPad;
            [Range(0, 1)] public float speedZone;
            public float slowPad;
            [Range(0, 1)] public float stallZone;
        }
        
    }
    
    [Serializable] public class VectorData
    {
        public Vector3 position;
        public Quaternion rotation;
        
        public float speed;
        public float speedKPH;
        
        public Vector3 velocity;
        public Vector3 angularVelocity;
        
        public Vector3 localVelocity;
        public Vector3 localAngularVelocity;
        
        public Vector3 localVelocityKPH;
        public Vector3 localAngularVelocityDPS;
        
        public Vector3 modelPosition;
        public Quaternion modelRotation;
        
        public Vector3 modelLocalPosition;
        public Quaternion modelLocalRotation;
    }
    
    [Serializable] public class EventData
    {
        // ALL SHIP EVENTS
        
        
    }
    
    [Serializable] public class RaceData
    {
        public int lap;
        public int checkpoint;
        public int position;
        
        public float totalTime;
        public List<float> lapTimes;
        
        public Action<int> OnStartRace;
        public Action<int> OnFinishRace;
        public Action<int> OnLap;
        public Action<int> OnCheckpoint;
        public Action<int> OnPosition;
        
        public bool hasCrossedFirstLap;
    }
    
    [Serializable] public class DebugLiveData
    {
        public MainSensors mainSensors;
        public Sensor[] hoverSensors;
        public LayerMask hoverGroundLayer;
        public bool isLockToGround;
        
        public float hoverHeight;
        public float hoverRange;
        public float hoverSpread;
        public float hoverPitchEffect;
        public float hoverRollEffect;
        public Direction direction;
    }
    
    
    
    [Serializable] public class SwitchData
    {
        public bool hover;
        public bool jump;
        [Space]
        public bool thrust;
        public bool boost;
        public bool afterburner;
        [Space]
        public bool abDrag;
        public bool abTorque;
        public bool abDrift;
        public bool abSlide;
        [Space]
        public bool grip;
        public bool strafe;
        [Space]
        public bool steer;
        public bool pitch;
        public bool roll;
    }
    
    [Serializable] public class StateData
    {
        public bool isOnGround;         // COLLISION
        public bool isOnMagnetTrack;
        
        public bool isOnPickupPad;
        public bool isOnSpeedPad;
        public bool isOnSpeedZone;
        public bool isOnSlowPad;
        public bool isOnStallZone;
        
        public bool isBoosting;         // ACTION
        
        public bool canLockToGround;    // SETTING
        
        public Action<float> OnBoostChange;
        public Action<float> OnBoostFill;
        public Action<float> OnBoostConsume;
        public Action OnBoostEmpty;
        
        public Action<float> OnHealthChange;
        public Action<float> OnShipHeal;
        public Action<float> OnShipDamage;
        
        public Action OnShipDestroy;
        public Action OnShipRespawn;
        
        public Action OnPickupGain;
        public Action OnPickupUse;
    }
    
    
}
