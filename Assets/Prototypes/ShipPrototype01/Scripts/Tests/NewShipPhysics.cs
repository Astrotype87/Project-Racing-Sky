using System;
using UnityEngine;
using ProjectRacingSky;

namespace Archived
{
    public class NewShipPhysics : MonoBehaviour
    {
        // INSPECTOR
        [Header("References")]
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private Transform[] hoverSensors;
        
        [Header("Physics")]
        [SerializeField] private HoverPhysics hoverPhysics;
        [SerializeField] private GravityPhysics gravityPhysics;
        [SerializeField] private ThrustPhysics thrustPhysics;
        [SerializeField] private SteerPhysics steerPhysics;
        [SerializeField] private AirbrakePhysics airbrakePhysics;
        [SerializeField] private GripPhysics gripPhysics;
        [SerializeField] private PitchPhysics pitchPhysics;
        [SerializeField] private RollPhysics rollPhysics;
        [SerializeField] private BoostPhysics boostPhysics;
        
        // PROPERTIES
        public InputData ShipInput {private get; set;}
        
        // EVENTS
        public Action OnSpeedPadHit;
        
        // PRIVATE VARIABLES
        private Vector3 _groundForward;
        private Vector3 _groundSideward;
        private float _airbrakeDrag;
        private float _airbrakeTorque;
        private float _airbrakeDrift;
        private float _boostAmount;
        
        // ENUMS
        public enum HoverDirection {Track, Sensor, Global}
        public enum GravityDirection {Track, Sensor, Global}
        public enum GripDirection {Ship, Track, Global}
        public enum VectorGate {None, Track}                    // Clips/removes a local axis
        public enum HoverPitchAxis {Track, Global}
        public enum HoverRollAxis {Track, Global}
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            Initialize();
        }
        
        private void FixedUpdate()
        {
            UpdateShipPhysics();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            
        }
        
        
        // CLASS METHODS
        private void Initialize()
        {
            hoverPhysics.rigidbody = shipRigidbody;
            gravityPhysics.rigidbody = shipRigidbody;
            thrustPhysics.rigidbody = shipRigidbody;
            steerPhysics.rigidbody = shipRigidbody;
            airbrakePhysics.rigidbody = shipRigidbody;
            gripPhysics.rigidbody = shipRigidbody;
            pitchPhysics.rigidbody = shipRigidbody;
            rollPhysics.rigidbody = shipRigidbody;
            
            hoverPhysics.hoverSensors = hoverSensors;
            gravityPhysics.hoverSensors = hoverSensors;
        }
        
        private void UpdateShipPhysics()
        {
            hoverPhysics.ApplyHover(out _groundForward, out _groundSideward);
            gravityPhysics.ApplyGravity();
            
            pitchPhysics.ApplyPitch(ShipInput.pitchInput, _groundForward);
            rollPhysics.ApplyRoll(ShipInput.rollInput, _groundSideward);
            
            airbrakePhysics.ApplyAirbrake(
                ShipInput.airbrakeLeftInput, ShipInput.airbrakeRightInput,
                out _airbrakeDrag, out _airbrakeTorque, out _airbrakeDrift);
            boostPhysics.ApplyBoost(10, out _boostAmount);
            
            thrustPhysics.ApplyThrust(ShipInput.thrustInput, _airbrakeDrag);
            steerPhysics.ApplySteer(ShipInput.steerInput, _airbrakeDrag);
            gripPhysics.ApplyGrip(_airbrakeDrift);
        }
        
        
        
        // SHIP PHYSICS SUBCLASSES
        [Serializable]
        public class HoverPhysics
        {
            public float hoverHeight;
            public float hoverFalloff;
            public float hoverForce;
            public float hoverSuspension;
            public float hoverDamping;
            public HoverDirection hoverDirection;
            
            public Rigidbody rigidbody {private get; set;}
            public Transform[] hoverSensors {private get; set;}
            
            public void ApplyHover(out Vector3 groundForward, out Vector3 groundSideward)
            {
                // Insert code for appliyng hover forces
                
                // Calculated ground forward will pe passed here
                groundForward = Vector3.forward;
                groundSideward = Vector3.right;
            }
        }
        
        [Serializable]
        public class GravityPhysics
        {
            public float airGravity;
            public float groundGravity;
            public float groundGravityHeight;
            public Vector3 airGravityDirection;
            public GravityDirection groundGravityDirection;
            public float fallingTerminalVelocity;
            
            public Rigidbody rigidbody {private get; set;}
            public Transform[] hoverSensors {private get; set;}
            
            public void ApplyGravity()
            {
                // Insert code for appliyng gravity
            }
        }
        
        [Serializable]
        public class ThrustPhysics
        {
            public float thrustSpeed;
            public float thrustAccel;
            public float thrustDecell;
            public float thrustGain;
            public float thrustRelease;
            public VectorGate thrustVectorClip;
            
            public Rigidbody rigidbody {private get; set;}
            
            public void ApplyThrust(float thrustInput, float airbrakeDrag)
            {
                
            }
        }
        
        [Serializable]
        public class SteerPhysics
        {
            public float steerSpeed;
            public float steerAccel;
            public float steerDecell;
            public float steerGain;
            public float steerRelease;
            
            public Rigidbody rigidbody {private get; set;}
            
            public void ApplySteer(float steerInput, float _airbrakeTorque)
            {
                
            }
        }
        
        [Serializable]
        public class AirbrakePhysics
        {
            public float dragForce;         // Force amount
            public float dragGain;
            public float dragRelease;
            public float torqueSpeed;       // Added degrees per second
            public float torqueGain;
            public float torqueRelease;
            public float driftAmount;       // Percentage of grip force
            public float driftGain;
            public float driftRelease;
            
            public Rigidbody rigidbody {private get; set;}
            
            public void ApplyAirbrake(float airbrakeLeftInput, float airbrakeRightInput,
                out float appliedDrag, out float appliedTorque, out float appliedDrift)
            {
                appliedDrag = 0;
                appliedTorque = 0;
                appliedDrift = 0;
            }
        }
        
        [Serializable]
        public class GripPhysics
        {
            public float groundGrip;
            public float airGrip;
            public float gripGain;
            public float gripRelease;
            public GripDirection gripDirection;
            public VectorGate gripGate;
            
            public Rigidbody rigidbody {private get; set;}
            
            public void ApplyGrip(float airbrakeDrift)
            {
                
            }
        }
        
        [Serializable]
        public class PitchPhysics
        {
            public float hoverPitchAngle;
            public float hoverPitchForce;
            public float hoverPitchDamping;
            public HoverPitchAxis hoverPitchAxis;
            
            public float airPitchSpeed;
            public float airPitchAccel;
            public float airPitchDeccel;
            public float airPitchGain;
            public float airPitchRelease;
            
            public Rigidbody rigidbody {private get; set;}
            private float pitchControl;
            private float prevPitchControl;
            
            public void ApplyPitch(float pitchInput, Vector3 groundForward)
            {
                
            }
        }
        
        [Serializable]
        public class RollPhysics
        {
            public float hoverRollForce;
            public float hoverRollDamping;
            public HoverRollAxis hoverRollAxis;
            
            public float airRollSpeed;
            public float airRollAccel;
            public float airRollDeccel;
            public float airRollGain;
            public float airRollRelease;
            
            public Rigidbody rigidbody {private get; set;}
            private float rollControl;
            private float prevRollControl;
            
            public void ApplyRoll(float rollInput, Vector3 groundSideward)
            {
                
            }
        }
        
        [Serializable]
        public class BoostPhysics
        {
            public float boostSpeed;    // Added speed in KPH
            public float speedPad;      // Added speed or force in KPH
            
            public void ApplyBoost(float boostInput, out float boostAmount)
            {
                boostAmount = 100;
            }
            
            public void ApplySpeedPad()
            {
                
            }
        }
        
        [Serializable]
        public class SideshiftPhysics
        {
            public float sideshiftSpeed;
            public float sideshiftTime;
            public float cooldownTime;
            
            public void ApplyBoost()
            {
                
            }
        }
        
        
    }
    
}
