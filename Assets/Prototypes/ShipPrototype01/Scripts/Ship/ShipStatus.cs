using UnityEngine;

namespace Archived
{
    public class ShipStatus : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [field:Header("Ship Data")]
        [field:SerializeField] public InputData InputData {get; private set;}
        [field:SerializeField] public MechanicData MechanicData {get; private set;}
        [field:SerializeField] public CollisionData CollisionData {get; private set;}
        [field:SerializeField] public StateData StateData {get; private set;}
        [field:SerializeField] public RaceData RaceData {get; private set;}
        [field:SerializeField] public VectorData VectorData {get; private set;}
        [field:SerializeField] public PhysicsData PhysicsData {get; private set;}
        
        [Header("References")]
        [SerializeField] Rigidbody shipRigidbody;
        [SerializeField] Transform shipModel;
        
        
        // SETTERS
        public void SetShipModel(ShipModelConfig modelData) => shipModel = modelData.shipModel;
        
        
        // PRIVATE VARIABLES
        private int hitColliderCount;           // For ship with multiple colliders, to make sure it will be only triggered once
        private bool hasCrossedFirstLap;        // This could be changed/removed if you add a new checkpoint system (changed to hit last checkpoint)
        private const float KPHFactor = 3.6f;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            InitializeShipStatus();
        }
        
        private void Update()
        {
            UpdateShipVector();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Start Line"))
            {
                if (hitColliderCount == 0 && hasCrossedFirstLap)
                    CollisionData.OnLapTrigger?.Invoke(RaceData);
                
                if (!hasCrossedFirstLap) 
                    hasCrossedFirstLap = true;
                hitColliderCount ++;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Start Line")) hitColliderCount --;
        }
        
        
        // CLASS METHODS
        private void InitializeShipStatus()
        {
            RaceData.lap = 1;
            RaceData.lapTimes.Add(0);
        }
        
        private void UpdateShipVector()
        {
            if (!shipRigidbody) return;
            
            VectorData.position = shipRigidbody.transform.position;
            VectorData.rotation = shipRigidbody.transform.rotation;
            
            VectorData.speed = shipRigidbody.linearVelocity.magnitude;
            VectorData.speedKPH = VectorData.speed * KPHFactor;
            
            VectorData.localVelocity = shipRigidbody.transform.InverseTransformVector(shipRigidbody.linearVelocity);
            VectorData.localAngularVelocity = shipRigidbody.transform.InverseTransformVector(shipRigidbody.angularVelocity);
            
            VectorData.localVelocityKPH = VectorData.localVelocity * KPHFactor;
            VectorData.localAngularVelocityDPS = VectorData.localAngularVelocity * Mathf.Rad2Deg;
            
            VectorData.modelLocalPosition = shipModel.localPosition;
            VectorData.modelLocalRotation = shipModel.localRotation;
        }
        
    }
    
}
