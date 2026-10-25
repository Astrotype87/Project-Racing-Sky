using System;
using UnityEngine;
using TetraCreations.Attributes;

namespace ProjectRacingSky.Ship
{
    [CreateAssetMenu(menuName = "Project Racing Sky/Ship/Ship Profile", fileName = "ShipProfile")]
    public class ShipProfile : ScriptableObject
    {
        [Header("Details")]
        public string profileID;
        public Details details;
        
        [Header("Statistics")]
        public ShipType shipType;
        public SpeedClass speedClass;
        public StandardStats standardStats;
        public RawStats rawStats;
        
        [Header("Model")]
        public GameObject shipModel;
        public GameObject shipCollider;
        
        [Header("Configuration")]
        public MechanicsConfig mechanicsConfig;
        public PhysicsConfig physicsConfig;
        public AnimationConfig animationConfig;
        public VFXConfig vFXConfig;
        public AudioConfig audioConfig;
        
        [Header("Branding")]
        public Sprite brandLogo;
        public Sprite textLogo;
        public Sprite banner;
        
    }
    
    public enum ShipType
    {
        Classic, Speed, Agile, Drift, Combat, Flight, Space
    }
    
    public enum SpeedClass
    {
        Stall, Strike, Sabre, Stealth, Sonic
    }
    
    [Serializable] public struct Details
    {
        public string brandName;
        public string shipName;
        [TextArea] public string fullName;
        [TextArea] public string description;
    }
    
    [Serializable] public struct StandardStats
    {
        [SnappedSlider(0.25f, 0, 10)] public float speed;
        [SnappedSlider(0.25f, 0, 10)] public float acceleration;
        [SnappedSlider(0.25f, 0, 10)] public float handling;
        [SnappedSlider(0.25f, 0, 10)] public float grip;
        [SnappedSlider(0.25f, 0, 10)] public float airbrake;
        [SnappedSlider(0.25f, 0, 10)] public float stability;
        [SnappedSlider(0.25f, 0, 10)] public float health;
        [SnappedSlider(0.25f, 0, 10)] public float mass;
        
        [SnappedSlider(0.25f, 0, 10)] public float boost;
        [SnappedSlider(0.25f, 0, 10)] public float afterburner;
        [SnappedSlider(0.25f, 0, 10)] public float firepower;
        [SnappedSlider(0.25f, 0, 10)] public float sideshift;
        [SnappedSlider(0.25f, 0, 10)] public float strafe;
        [SnappedSlider(0.25f, 0, 10)] public float roll;
    }
    
    [Serializable] public struct RawStats
    {
        public string speed;
        public string acceleration;
        public string handling;
        public string grip;
        public string airbrake;
        public string stability;
        public string health;
        public string mass;
        
        public string boost;
        public string afterburner;
        public string firepower;
        public string sideshift;
        public string strafe;
        public string roll;
    }
    
}
