using UnityEngine;

namespace Archived
{
    public class ShipModelConfig : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Ship Model")]
        [SerializeField] private Transform ShipMesh;
        [SerializeField] private Transform AirbrakeLeft;
        [SerializeField] private Transform AirbrakeRight;
        [SerializeField] private Transform Canopy;
        
        [Header("VFX")]
        [SerializeField] private ParticleSystem[] ThrusterFlame;
        
        
        // GETTERS
        public Transform shipModel {get => transform;}
        public Transform shipMesh {get => ShipMesh;}
        public Transform airbrakeLeft {get => AirbrakeLeft;}
        public Transform airbrakeRight {get => AirbrakeRight;}
        public Transform canopy {get => Canopy;}
        public ParticleSystem[] thrusterFlame {get => ThrusterFlame;}
    }
    
}
