using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Nested = NaughtyAttributes.AllowNestingAttribute;

namespace ProjectRacingSky.Race
{
    using Ship;
    
    [Serializable]
    public struct TournamentEventConfig
    {
        public RaceEventConfig[] events;
    }
    
    [Serializable]
    public struct RaceEventConfig
    {
        [Space(-10)] [Header("Event")]
        public RaceMode raceMode;
        public ShipType shipType;
        public SpeedClass speedClass;
        
        [Nested, ShowIf(nameof(raceMode), RaceMode.Elimination)] public EliminationConfig elimination;
        [Nested, ShowIf(nameof(raceMode), RaceMode.Combat)] public CombatConfig combat;
        [Nested, ShowIf(nameof(raceMode), RaceMode.Blitz)] public BlitzConfig blitz;
        [Nested, ShowIf(nameof(raceMode), RaceMode.Speedrun)] public SpeedrunConfig speedrun;
        
        [Header("Racetrack")]
        public string racetrack;
        public string variant;
        public int laps;
        
        [Header("Mechanics")]
        public bool hasSpeedPad;
        public bool hasSlowPad;
        public bool hasSpeedZone;
        public bool hasStallZone;
        
        [Header("Pilot")]
        public int pilots;
        public Difficulty difficulty;
        public List<ShipProfile> playerShips;
        public List<ShipProfile> AIShips;
        
        [Header("Pickups")]
        public PickupLevel pickupLevel;
        public string[] pickupList;
        
        [Header("Conditions")]
        public bool canRespawn;
        public bool canDamage;
        public bool canClip;
        
        // STRUCTS
        [Serializable] public struct EliminationConfig
        {
            public CountdownMode mode;
            public float startTime;
            public float time;
            
            public enum CountdownMode
            {
                Timer, PerLap, LastPlaceTimer
            }
        }
        
        [Serializable] public struct CombatConfig
        {
            public float time;
            public TargetMode mode;
            public int target;
            
            public enum TargetMode
            {
                Kills, Points
            }
        }
        
        [Serializable] public struct BlitzConfig
        {
            public float timeHealth;
        }
        
        [Serializable] public struct SpeedrunConfig
        {
            public float targetSpeed;
        }
        
        
    }
    
}
