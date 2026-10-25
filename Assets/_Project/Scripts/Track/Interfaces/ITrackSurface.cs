using UnityEngine;

namespace ProjectRacingSky.Track
{
    public interface ITrackSurface
    {
        public float MaglevAmount { get; }
        public float SpeedMultiplier { get; }
        public float StallMultiplier { get; }
        public float GripMultiplier { get; }
        public float DriftMultiplier { get; }
        public float CooldownMultiplier { get; }
        public float AbrasionRate { get; }
        
        public bool GroundLock { get; }
    }
}
