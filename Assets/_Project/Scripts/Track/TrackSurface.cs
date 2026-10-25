using UnityEngine;
using AstrotypeTools.InspectorAttributes;

namespace ProjectRacingSky.Track
{
    public class TrackSurface : MonoBehaviour, ITrackSurface
    {
        // INSPECTOR FIELDS
        [Header("Surface Profile")]
        [SerializeField, InlineEditor] private SurfaceProfile surfaceProfile;
        
        [Header("Modifiers")]
        public bool groundLock;
        
        
        // PROPERTIES
        float ITrackSurface.MaglevAmount => surfaceProfile ? surfaceProfile.maglevAmount : 0;
        float ITrackSurface.SpeedMultiplier => surfaceProfile ? surfaceProfile.speedMultiplier : 1;
        float ITrackSurface.StallMultiplier => surfaceProfile ? surfaceProfile.stallMultiplier : 1;
        float ITrackSurface.GripMultiplier => surfaceProfile ? surfaceProfile.gripMultiplier : 1;
        float ITrackSurface.DriftMultiplier => surfaceProfile ? surfaceProfile.driftMultiplier : 1;
        float ITrackSurface.CooldownMultiplier => surfaceProfile ? surfaceProfile.cooldownMultiplier : 1;
        float ITrackSurface.AbrasionRate => surfaceProfile ? surfaceProfile.abrasionRate : 1;
        
        bool ITrackSurface.GroundLock => groundLock;
    }
}
