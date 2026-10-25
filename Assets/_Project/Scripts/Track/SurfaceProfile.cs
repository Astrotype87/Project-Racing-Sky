using UnityEngine;

namespace ProjectRacingSky.Track
{
    [CreateAssetMenu(fileName = "SurfaceProfile", menuName = "Project Racing Sky/Track/SurfaceProfile")]
    public class SurfaceProfile : ScriptableObject
    {
        [Range(0.0f, 1.0f)] public float maglevAmount;
        [Space(2)]
        [Range(1.0f, 4.0f)] public float speedMultiplier = 1f;
        [Range(1.0f, 0.25f)] public float stallMultiplier = 1f;
        [Space(2)]
        [Range(1.0f, 2.0f)] public float gripMultiplier = 1f;
        [Range(1.0f, 0.5f)] public float driftMultiplier = 1f;
        [Space(2)]
        [Range(0.5f, 2.0f)] public float cooldownMultiplier = 1f;
        [Range(0.25f, 4.0f)] public float abrasionRate = 1f;
    }
}
