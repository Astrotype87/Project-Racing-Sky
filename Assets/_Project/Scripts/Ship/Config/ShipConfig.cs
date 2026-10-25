using System;
using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.Ship
{
    using Utility;
    
    // ENUMS
    public enum ForceType {AddForce, AddForceAtPosition}
    public enum Direction {Ground, Ship, Gravity, World}
    public enum DragFormula
    {
        /// <summary>
        /// Uses a basic linear drag equation.<br/>
        /// <c> density * drag * velocity * referenceArea </c>
        /// </summary>
        Basic,
        /// <summary>
        /// Uses the standard quadratic drag equation.<br/>
        /// <c> 0.5 * density * drag * velocity^2 * referenceArea </c>
        /// </summary>
        Realistic,
        /// <summary>
        /// Uses Unity's drag equation.<br/>
        /// <c> drag / (1 - Time.fixedDeltaTime * drag) * velocity * referenceArea </c>
        /// </summary>
        Unity,
    }
    
    // MAIN CONFIG
    [Serializable] public class GlobalConfig
    {
        
    }
    
    [Serializable] public class ControllerConfig
    {
        public Jump jump;
        public Boost boost;
        public Afterburner afterburner;
        public JumpPad jumpPad;
        
        [Serializable] public struct Jump
        {
            public int initialJumps;        // Starting jumps
            public int maxJumps;            // Maximum loaded jumps
            public float cooldownTime;      // Cooldown time before using another jump
            public float reloadTime;        // Time to load one jump
        }
        
        [Serializable] public struct Boost
        {
            public float initialFuel;       // Starting fuel amount
            public float maxFuel;           // Fuel full/maximum amount
            public float consumeRate;       // Fuel consumed per second
            public float refillRate;        // Fuel refilled per second
            [Space(5)]
            public bool autoRefill;         // Automatically refill instead of using thrust
            public float padRefillAmount;   // Speed pad refill amount
            public float zoneRefillRate;    // Speed zone refill rate
            [Space(5)]
            public float consumeCooldown;   // Cooldown time after consuming
            public float refillCooldown;    // Cooldown time before refilling
        }
        
        [Serializable] public struct Afterburner
        {
            public float initialFuel;       // Starting fuel amount
            public float maxFuel;           // Fuel full/maximum amount
            public float consumeRate;       // Fuel consumed per second
            public float refillRate;        // Fuel refilled per second
            [Space(5)]
            public bool autoRefill;         // Refill by using thrust
            public float minConsumable;     // Minimum current fuel consumable
            public float padRefillAmount;   // Speed pad refill amount
            public float zoneRefillRate;    // Speed zone refill rate
            [Space(5)]
            public float consumeCooldown;   // Cooldown time after consuming
            public float refillCooldown;    // Cooldown time before refilling
        }
        
        [Serializable] public struct Sideshift
        {
            public int initialSideshifts;   // Starting jumps
            public int maxSideshifts;       // Maximum loaded jumps
            public float cooldownTime;      // Cooldown time before using another jump
            public float reloadTime;        // Time to load one jump
        }
        
        [Serializable] public struct JumpPad
        {
            public float cooldownTime;
        }
    }
    
    [Serializable] public class MechanicsConfig
    {
        public float maxHealth;
    }
    
    [Serializable] public class PhysicsConfig
    {
        public World world;
        public Body body;
        [Space(5)]
        public Hover hover;
        public Jump jump;
        [Space(5)]
        public Thrust thrust;
        public Boost boost;
        public Afterburner afterburner;
        public Airbrake airbrake;
        [Space(5)]
        public Grip grip;
        public Strafe strafe;
        public Sideshift sideshift;
        [Space(5)]
        public Steer steer;
        public Pitch pitch;
        public Roll roll;
        public Orient orient;
        [Space(5)]
        public Maglev maglev;
        public JumpPad jumpPad;
        public SpeedPad speedPad;
        
        
        // SHIP COMPONENTS
        [Serializable] public struct World
        {
            public Vector3 gravity;
            public float drag;
            public float angularDrag;
            public float windTime;
            public float turbulenceTime;
            public Noise3D turbulenceNoise;
            
            public float density;
            
            public DragFormula dragFormula;
        }
        
        [Serializable] public struct Body
        {
            public float mass;
            public Vector3 dragScale;
            public Vector3 angularDragScale;
            // public Vector3 tensor;
            // public Vector3 bounciness;
            // public float collisionSlowdown;
        }
        
        [Serializable] public struct Hover
        {
            public Sensor[] sensors;
            public LayerMask groundLayer;
            public float height;
            public float falloff;
            [Space(5)]
            public float force;
            public float pull;
            public float suspension;
            public float damping;
            public AnimationCurve suspensionCurve;
            [Range(0, 1)] public float counterSlip;
            public bool counterDrag;
            [Space(5)]
            public Direction hoverDirection;
            public Direction pullDirection;
            [Range(0, 1)] public float hoverSpread;
            [Range(0, 1)] public float pullSpread;
            [Range(0, 1)] public float pitchEffect;
            [Range(0, 1)] public float rollEffect;
            [Space(5)]
            [Range(0, 2)] public float powerOnTime;
            [Range(0, 2)] public float powerOffTime;
            [CurveRange(0, 0, 1, 1)] public AnimationCurve powerOnCurve;
            [CurveRange(0, 0, 1, 1)] public AnimationCurve powerOffCurve;
        }
        
        [Serializable] public struct Jump
        {
            public float force;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float hold;
            [Range(0, 2)] public float release;
            public bool enableAirJump;
        }
        
        [Serializable] public struct Thrust
        {
            public float speed;
            public float acceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public bool tightAccel;
            public bool softDecel;
        }
        
        [Serializable] public struct Boost
        {
            public float speed;
            public float acceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 10)] public float hold;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct Afterburner
        {
            public float speed;
            public float acceleration;
            [CurveRange(0, 0, 1, 1)] public AnimationCurve accelCurve;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct Airbrake
        {
            [Range(0, 100)] public float dragAmount;
            public float dragDecel;
            [Range(0, 2)] public float dragGain;
            [Range(0, 2)] public float dragRelease;
            
            [Space(5)]
            public float torqueSpeed;
            public float torqueAccel;
            [Range(0, 2)] public float torqueGain;
            [Range(0, 2)] public float torqueRelease;
            public float torqueLimitKph;
            
            [Space(5)]
            [Range(0, 100)] public float driftAmount;
            [Range(0, 2)] public float driftGain;
            [Range(0, 2)] public float driftRelease;
            public float driftLimitKph;
            
            [Space(5)]
            public float slideSpeed;
            public float slideAccel;
            [Range(0, 2)] public float slideGain;
            [Range(0, 2)] public float slideRelease;
            public float slideLimitKph;
            public bool disableSlide;
        }
        
        [Serializable] public struct Grip
        {
            [Range(0, 10)] public float groundGrip;
            [Range(0, 10)] public float airGrip;
            [Range(0, 2)] public float toGroundTime;
            [Range(0, 2)] public float toAirTime;
            public bool localGrip;
            public bool counterDrag;
            
            // [Obsolete, ReadOnly] [NaughtyAttributes.CurveRange(0, 0, 1, 1)] public AnimationCurve gripCurve;
            // [Obsolete, ReadOnly] [Range(0, 10)] public float minimumGrip;
            // [Obsolete, ReadOnly] [Range(0.1f, 1)] public float groundRatio;
        }
        
        [Serializable] public struct Strafe
        {
            public float speed;
            public float acceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public bool localStrafe;
            public bool tightAccel;
            public bool softDecel;  
        }
        
        [Serializable] public struct Sideshift
        {
            // Strafe-based sideshift
            public float speed;
            public float acceleration;
            
            // Impulse-based sideshift
            public float impulse;
            
            // Control configuration
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float hold;
            [Range(0, 2)] public float release;
            public bool isKinematic;
            public bool enableAirSideshift;
        }
        
        [Serializable] public struct Steer
        {
            public float speed;
            public float acceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public bool tightAccel;
            public bool softDecel;        
        }
        
        [Serializable] public struct Pitch
        {
            public float pitchUpHeight;
            public float pitchDownHeight;
            
            public float trackAngle;
            public float trackForce;
            public float trackDamping;
            [Range(0, 1)] public float hoverRatio;
            public bool alignFront;
            public bool alignBack;
            [Space(5)]
            public float flightSpeed;
            public float flightAccel;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            [Space(5)]
            public bool manualMode;
            public float groundSpeed;
            public float groundAccel;
            public float groundDecel;
        }
        
        [Serializable] public struct Roll
        {
            public float trackAngle;
            public float trackForce;
            public float trackDamping;
            public bool rollRecovery;
            public bool disable;
            [Space(5)]
            public float flightSpeed;
            public float flightAccel;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct Orient
        {
            public bool autoRoll;
            public float autoRollForce;
            public float autoRollDamp;
            [Space(5)]
            public bool autoPitch;
            public float autoPitchForce;
            public float autoPitchDamp;
        }
        
        
        // TRACK MECHANICS
        [Serializable] public struct Maglev
        {
            public LayerMask groundLayer;
            public float height;
            public float range;
            [Range(0, 1)] public float force;
            [Range(0, 1)] public float damping;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public bool enableAlign;
            [Range(0, 50)] public float alignForce;
            [Range(0, 50)] public float alignDamping;
        }
        
        [Serializable] public struct JumpPad
        {
            public float force;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float hold;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct SpeedPad
        {
            public float force;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float hold;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct SpeedZone
        {
            public float speed;
            public float acceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct SlowPad
        {
            [Range(0, 1)] public float amount;
            [Range(0, 2)] public float hold;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct StallZone
        {
            [Range(0, 1)] public float amount;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
    }
    
    [Serializable] public class AnimationConfig
    {
        
    }
    
    [Serializable] public class VFXConfig
    {
        
    }
    
    [Serializable] public class AudioConfig
    {
        public Sources source;
        public AudioConstant engine_Speed;
        public AudioConstant engine_Thrust;
        public AudioImpulse engine_SpeedPad;
        
        [Serializable] public struct Sources
        {
            public AudioClip engine;
        }
    }
    
    
    
}
