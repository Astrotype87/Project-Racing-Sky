using System;
using UnityEngine;

namespace Archived
{
    [Serializable]
    public class PhysicsConfig
    {
        public Hover hover;
        public Gravity gravity;
        public Jump jump;
        
        public Thrust thrust;
        public Boost boost;
        public Afterburner afterburner;
        public Airbrake airbrake;
        
        public Grip grip;
        public Strafe strafe;
        public Sideshift sideshift;
        
        public Steer steer;
        public Pitch pitch;
        public Roll roll;
        public Orient orient;
        
        public static PhysicsConfig GetDefaultSetings()
        {
            PhysicsConfig config = new PhysicsConfig();
            
            config.hover.height = 4;
            config.hover.falloff = 4;
            config.hover.force = 125;
            config.hover.suspension = 2;
            config.hover.damping = 5;
            config.hover.direction = Direction.Ground;
            config.hover.suspensionCurve = new AnimationCurve(
                new Keyframe(0, 0, 0, 1),
                new Keyframe(0.5f, 0.5f, 1, 2),
                new Keyframe(1, 1.5f, 2, 0)
            );
            config.hover.forceType = ForceType.AddForce;
            config.hover.gain = 1;
            config.hover.release = 1;
            config.hover.disable = false;
            
            config.gravity.airGravity = 125;
            config.gravity.groundGravity = 125;
            config.gravity.direction = Direction.Ground;
            config.gravity.slipMode = SlipMode.Ground;
            config.gravity.gravitySpread = 0;
            config.gravity.disable = false;
            
            config.jump.force = 67;
            config.jump.cooldown = 1.75f;
            config.jump.enableAirJump = false;
            
            config.thrust.speed = 1000;         // 1200
            config.thrust.acceleration = 375;   // 450
            config.thrust.deceleration = 500;
            config.thrust.gain = 1;
            config.thrust.release = 0.5f;
            config.thrust.reverseSpeed = 200;
            config.thrust.reverseAccel = 200;
            config.thrust.accelMode = AccelMode.Normal;
            config.thrust.decelMode = DecelMode.Normal;
            
            config.boost.speed = 200;
            config.boost.acceleration = 500;        // 200
            
            config.airbrake.dragAmount = 75;
            config.airbrake.dragDecel = 500;
            config.airbrake.dragGain = 0.0625f;
            config.airbrake.dragRelease = 0.0625f;
            config.airbrake.torqueSpeed = 30;       // 45
            config.airbrake.torqueAccel = 180;
            config.airbrake.torqueGain = 0.15f;
            config.airbrake.torqueRelease = 0.15f;
            config.airbrake.torqueLimitKph = 500;
            config.airbrake.driftAmount = 12.5f;    // 25
            config.airbrake.driftGain = 0.5f;
            config.airbrake.driftRelease = 0.5f;
            config.airbrake.driftLimitKph = 500;
            
            config.grip.groundGrip = 75;        // 82.5f
            config.grip.airGrip = 50;
            config.grip.toGroundTime = 0;
            config.grip.toAirTime = 0.5f;
            config.grip.gripCurve = new AnimationCurve(
                new Keyframe(0.0f,      0.0f,       0.0f,       0.0625f),
                new Keyframe(0.5f,      0.03125f,   0.0625f,    0.375f),
                new Keyframe(0.75f,     0.125f,     0.375f,     1.0f),
                new Keyframe(0.875f,    0.25f,      1.0f,       2.6666667f),
                new Keyframe(0.96875f,  0.5f,       2.6666667f, 16f),
                new Keyframe(1.0f,      1.0f,       16.0f,      0.0f)
            );
            config.grip.minimum = 10;
            config.grip.groundRatio = 0.25f;
            
            config.strafe.speed = 40;
            config.strafe.acceleration = 200;
            config.strafe.deceleration = 75;
            config.strafe.gain = 0;
            config.strafe.release = 0;
            config.strafe.accelMode = AccelMode.Normal;
            config.strafe.decelMode = DecelMode.Normal;
            
            config.sideshift.speed = 50;
            config.sideshift.gain = 0;
            config.sideshift.release = 0;
            config.sideshift.duration = 0;
            config.sideshift.cooldown = 0;
            
            config.steer.speed = 75;            // 90
            config.steer.acceleration = 360;
            config.steer.deceleration = 360;
            config.steer.gain = 0.1f;
            config.steer.release = 0.1f;
            config.steer.accelMode = AccelMode.Tight;
            config.steer.decelMode = DecelMode.NoOpposite;
            
            config.pitch.trackAngle = 6;
            config.pitch.trackForce = 60;
            config.pitch.trackDamping = 10;
            config.pitch.trackElevation = 1;
            config.pitch.speed = 45;
            config.pitch.accel = 120;
            config.pitch.decel = 60;
            config.pitch.gain = 0.1f;
            config.pitch.release = 0.1f;
            config.pitch.manualMode = false;
            config.pitch.groundSpeed = 90;
            config.pitch.groundAccel = 153.15f;
            config.pitch.groundDecel = 360;
            
            config.roll.trackAngle = 0;
            config.roll.trackForce = 60;
            config.roll.trackDamping = 10;
            config.roll.disable = false;
            config.roll.speed = 120;
            config.roll.accel = 360;      // 45
            config.roll.decel = 360;      // 60
            config.roll.gain = 0.1f;
            config.roll.release = 0.1f;
            
            config.orient.autoRoll = false;
            config.orient.autoRollForce = 30;
            config.orient.autoRollDamp = 10;
            config.orient.autoPitch = false;
            config.orient.autoPitchForce = 30;
            config.orient.autoPitchDamp = 10;
            
            return config;
        }
        
        
        public enum ForceType {AddForce, AddForceAtPosition}
        public enum Direction {Ground, Ship, World}
        public enum SlipMode {Ground, World}
        public enum AccelMode {Normal, Tight}
        public enum DecelMode {Normal, NoOpposite}
        
        
        [Serializable] public struct Hover
        {
            public float height;
            public float falloff;
            public float force;
            public float suspension;
            public float damping;
            public Direction direction;
            public AnimationCurve suspensionCurve;
            public ForceType forceType;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public bool disable;
        }
        
        [Serializable] public struct Gravity
        {
            public float airGravity;
            public float groundGravity;
            public Direction direction;
            public SlipMode slipMode;
            [Range(0, 1)] public float gravitySpread;
            public bool disable;
        }
        
        [Serializable] public struct Jump
        {
            public float force;
            public float cooldown;
            public bool enableAirJump;
        }
        
        
        [Serializable] public struct Thrust
        {
            public float speed;
            public float acceleration;
            public float deceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public float reverseSpeed;
            public float reverseAccel;
            public AccelMode accelMode;
            public DecelMode decelMode;
        }
        
        [Serializable] public struct Boost
        {
            public float speed;
            public float acceleration;
            public float deceleration;
            public float duration;
            public float gain;
            public float release;
        }
        
        [Serializable] public struct Afterburner
        {
            public float speed;
            public float acceleration;
            public float deceleration;
            public float gain;
            public float release;
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
            public float torqueDecel;
            [Range(0, 2)] public float torqueGain;
            [Range(0, 2)] public float torqueRelease;
            public float torqueLimitKph;
            [Space(5)]
            [Range(0, 100)] public float driftAmount;
            [Range(0, 2)] public float driftGain;
            [Range(0, 2)] public float driftRelease;
            public float driftLimitKph;
        }
        
        
        [Serializable] public struct Grip
        {
            [Range(0, 100)] public float groundGrip;
            [Range(0, 100)] public float airGrip;
            [Range(0, 2)] public float toGroundTime;
            [Range(0, 2)] public float toAirTime;
            public AnimationCurve gripCurve;
            [Range(0, 10)] public float minimum;
            [Range(0.1f, 1)] public float groundRatio;
        }
        
        [Serializable] public struct Strafe
        {
            public float speed;
            public float acceleration;
            public float deceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public AccelMode accelMode;
            public DecelMode decelMode;
        }
        
        [Serializable] public struct Sideshift
        {
            public float speed;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            [Range(0, 2)] public float duration;
            [Range(0, 2)] public float cooldown;
        }
        
        
        [Serializable] public struct Steer
        {
            public float speed;
            public float acceleration;
            public float deceleration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
            public AccelMode accelMode;
            public DecelMode decelMode;
        }
        
        [Serializable] public struct Pitch
        {
            public float trackAngle;
            public float trackForce;
            public float trackDamping;
            public float trackElevation;
            [Space(5)]
            public float speed;
            public float accel;
            public float decel;
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
            public bool disable;
            [Space(5)]
            public float speed;
            public float accel;
            public float decel;
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
        
    }

    [Serializable]
    public class MechanicConfig
    {
        public MagnetLock magnetLock;
        public SpeedPad speedPad;
        public SpeedZone speedZone;
        public SlowPad slowPad;
        public StallZone stallZone;
        
        public static MechanicConfig GetDefaultSettings()
        {
            MechanicConfig config = new MechanicConfig();
            
            config.magnetLock.height = 4;
            config.magnetLock.range = 8;
            config.magnetLock.force = 0.125f;
            config.magnetLock.damping = 0.5f;
            
            config.speedPad.speed = 200;
            config.speedPad.duration = 0.25f;
            config.speedPad.gain = 0.25f;
            config.speedPad.release = 0.5f;
            
            config.speedZone.speed = 200;
            config.speedZone.gain = 0.5f;
            config.speedZone.release = 0.5f;
            
            config.slowPad.amount = 0.75f;
            config.slowPad.duration = 0.25f;
            config.slowPad.gain = 0.25f;
            config.slowPad.release = 0.5f;
            
            config.stallZone.amount = 0.75f;
            config.stallZone.gain = 0.5f;
            config.stallZone.release = 0.5f;
            
            return config;
        }
        
        
        [Serializable] public struct MagnetLock
        {
            public float height;
            public float range;
            [Range(0, 1)] public float force;
            [Range(0, 1)] public float damping;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct SpeedPad
        {
            public float speed;
            [Range(0, 2)] public float duration;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct SpeedZone
        {
            public float speed;
            [Range(0, 2)] public float gain;
            [Range(0, 2)] public float release;
        }
        
        [Serializable] public struct SlowPad
        {
            [Range(0, 1)] public float amount;
            [Range(0, 2)] public float duration;
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

    
}
