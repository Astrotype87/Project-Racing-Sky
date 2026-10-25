using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    using Utility;
    
    [CreateAssetMenu(menuName = "Project Racing Sky/Ship/Ship Profile Variant", fileName = "ShipProfileVariant")]
    public class ShipProfileVariant : ScriptableObject
    {
        [Header("Base")]
        public ShipProfileVariant baseProfile;
        
        [Header("Details")]
        public Optional<string> profileID;
        public Optional<Details> details;
        
        [Header("Statistics")]
        public Optional<ShipType> shipType;
        public Optional<SpeedClass> speedClass;
        public Optional<StandardStats> standardStats;
        public Optional<RawStats> rawStats;
        
        [Header("Model")]
        public Optional<GameObject> shipModel;
        public Optional<GameObject> shipCollider;
        
        [Header("Configuration")]
        public Optional<MechanicsConfig> mechanicsConfig;
        public Optional<PhysicsConfigVar> physicsConfig;
        public Optional<AnimationConfig> animationConfig;
        public Optional<VFXConfig> vFXConfig;
        public Optional<AudioConfig> audioConfig;
        
        [Header("Branding")]
        public Optional<Sprite> brandLogo;
        public Optional<Sprite> textLogo;
        public Optional<Sprite> banner;
        
        private void OnValidate()
        {
            if (!baseProfile) return;
            
            if (baseProfile == this)
            {
                Debug.LogWarning("[Ship Profile Variant] You cannot override this profile by itself.");
                baseProfile = null;
                return;
            }
            
            if (!profileID.enabled)         profileID.value = baseProfile.profileID;
            if (!details.enabled)           details.value = baseProfile.details;
            if (!shipType.enabled)          shipType.value = baseProfile.shipType;
            if (!speedClass.enabled)        speedClass.value = baseProfile.speedClass;
            if (!standardStats.enabled)     standardStats.value = baseProfile.standardStats;
            if (!rawStats.enabled)          rawStats.value = baseProfile.rawStats;
            if (!shipModel.enabled)         shipModel.value = baseProfile.shipModel;
            if (!shipCollider.enabled)      shipCollider.value = baseProfile.shipCollider;
            if (!mechanicsConfig.enabled)   mechanicsConfig.value = baseProfile.mechanicsConfig;
            if (!physicsConfig.enabled)     physicsConfig.value = baseProfile.physicsConfig;
            if (!animationConfig.enabled)   animationConfig.value = baseProfile.animationConfig;
            if (!vFXConfig.enabled)         vFXConfig.value = baseProfile.vFXConfig;
            if (!audioConfig.enabled)       audioConfig.value = baseProfile.audioConfig;
            if (!brandLogo.enabled)         brandLogo.value = baseProfile.brandLogo;
            if (!textLogo.enabled)          textLogo.value = baseProfile.textLogo;
            if (!banner.enabled)            banner.value = baseProfile.banner;
            
        }
        
        [Serializable] public class PhysicsConfigVar
        {
            public Optional<PhysicsConfig.World> world;
            public Optional<PhysicsConfig.Body> body;
            [Space(5)]
            public Optional<PhysicsConfig.Hover> hover;
            public Optional<PhysicsConfig.Jump> jump;
            [Space(5)]
            public Optional<PhysicsConfig.Thrust> thrust;
            public Optional<PhysicsConfig.Boost> boost;
            public Optional<PhysicsConfig.Afterburner> afterburner;
            public Optional<PhysicsConfig.Airbrake> airbrake;
            [Space(5)]
            public Optional<PhysicsConfig.Grip> grip;
            public Optional<PhysicsConfig.Strafe> strafe;
            public Optional<PhysicsConfig.Sideshift> sideshift;
            [Space(5)]
            public Optional<PhysicsConfig.Steer> steer;
            public Optional<PhysicsConfig.Pitch> pitch;
            public Optional<PhysicsConfig.Roll> roll;
            public Optional<PhysicsConfig.Orient> orient;
            [Space(5)]
            public Optional<PhysicsConfig.Maglev> maglev;
            public Optional<PhysicsConfig.JumpPad> jumpPad;
            public Optional<PhysicsConfig.SpeedPad> speedPad;
        }
        
    }
    
}
