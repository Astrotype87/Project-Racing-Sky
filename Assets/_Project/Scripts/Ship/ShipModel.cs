using System;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    [DefaultExecutionOrder(ScriptOrder.ShipModel)]
    public class ShipModel : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Ship Model")]
        [SerializeField] private Transform modelMesh;
        [SerializeField] private Transform[] airbrakeLeft;
        [SerializeField] private Transform[] airbrakeRight;
        [SerializeField] private Transform[] airbrakeFull;
        [SerializeField] private Transform canopy;
        
        [Header("VFX")]
        [SerializeField] private ParticleSystem[] thrusterFlame;
        [SerializeField] private ParticleSystem afterburnerTrail;
        
        [Header("Camera View")]
        [SerializeField] private ViewOffset viewOffset;
        [SerializeField] private FOVOverride fOVOverride;
        
        
        // PUBLIC PROPERTIES
        public Transform ModelTransform => transform;
        public Transform ModelMesh => modelMesh;
        public Transform[] AirbrakeLeft => airbrakeLeft;
        public Transform[] AirbrakeRight => airbrakeRight;
        public Transform[] AirbrakeFull => airbrakeFull;
        public Transform Canopy => canopy;
        public ParticleSystem[] ThrusterFlame => thrusterFlame;
        public ParticleSystem AfterburnerTrail => afterburnerTrail;
        public ViewOffset ViewOffset => viewOffset;
        public FOVOverride FOVOverride => fOVOverride;
    }
    
    [Serializable]
    public struct ViewOffset
    {
        public Vector3 closeView;
        public Vector3 farView;
        public Vector3 topView;
        public Vector3 frontView;
        public Vector3 cockpitView;
        public Vector3 rearView;
    }
    
    [Serializable]
    public struct FOVOverride
    {
        [Range(0, 179)] public float closeFOV;
        [Range(0, 179)] public float farFOV;
        [Range(0, 179)] public float topFOV;
        [Range(0, 179)] public float frontFOV;
        [Range(0, 179)] public float cockpitFOV;
        [Range(0, 179)] public float rearFOV;
    }
    
}
