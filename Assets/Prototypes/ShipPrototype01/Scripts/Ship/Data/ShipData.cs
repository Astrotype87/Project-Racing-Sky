using System;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    [Serializable]
    public class InputData
    {
        [Range(0, 1)] public float hoverInput = 1;
        [Range(0, 1)] public float thrustInput;
        [Range(0, 1)] public float reverseInput;
        [Range(0, 1)] public float afterburnerInput;
        [Range(0, 1)] public float airbrakeLeftInput;
        [Range(0, 1)] public float airbrakeRightInput;
        [Range(-1, 1)] public float strafeInput;
        [Range(-1, 1)] public float steerInput;
        [Range(-1, 1)] public float pitchInput;
        [Range(-1, 1)] public float rollInput;
        public bool boostInput; // FIXME: Remove Later
        
        public Action OnJump;
        public Action OnBoost;
        public Action<bool> OnSideshift;
        
        public Action OnFire;
        public Action OnDrain;
        public Action OnRespawn;
        
        public Action OnCanopy;
        
        public void Reset()
        {
            hoverInput = 1;
            thrustInput = 0;
            reverseInput = 0;
            afterburnerInput = 0;
            airbrakeLeftInput = 0;
            airbrakeRightInput = 0;
            strafeInput = 0;
            steerInput = 0;
            pitchInput = 0;
            rollInput = 0;
            boostInput = false;
        }
        
    }

    [Serializable]
    public class MechanicData
    {
        [Range(0, 1)] public float magnetLockInput;
        [Range(0, 1)] public float speedZoneInput;
        [Range(0, 1)] public float stallZoneInput;
        
        public Action<float> OnSpeedPad;
        public Action<float> OnSlowPad;
        public Action<float> OnSlowdown;
        
        public Action OnPickupPad;
    }

    [Serializable]
    public class CollisionData
    {
        public int collisionCount;  // Some fancy statistics
        public int triggerCount;    // Some fancy statistics
        
        public Action OnWallHit;
        public Action OnFloorHit;
        public Action OnStaticHit;
        public Action OnDynamicHit;
        
        public Action OnShipHit;
        public Action OnProjectileHit;
        
        public Action<RaceData> OnCheckpointTrigger;
        public Action<RaceData> OnLapTrigger;
        
    }

    [Serializable]
    public class StateData
    {
        public float boost;
        public float health;
        public string pickup;           // Change data type to Pickup
        
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

    [Serializable]
    public class RaceData
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
        
    }

    [Serializable]
    public class VectorData
    {
        public Vector3 position;
        public Quaternion rotation;
        
        public float speed;
        public float speedKPH;
        
        public Vector3 localVelocity;
        public Vector3 localAngularVelocity;
        
        public Vector3 localVelocityKPH;
        public Vector3 localAngularVelocityDPS;
        
        public Vector3 modelLocalPosition;
        public Quaternion modelLocalRotation;
        
    }

    [Serializable]
    public class PhysicsData
    {
        
    }
    
}
