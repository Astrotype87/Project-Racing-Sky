using UnityEngine;

namespace ProjectRacingSky.Ship
{
    public class ShipStatus : MonoBehaviour
    {
        // SHIP STATUS WAS RESPONSIBLE FOR:
        // - Health state
        // - Vector information
        // - Race information
        
        // SHIP STATUS FIXES:
        // - race 
        
        // INSPECTOR FIELDS
        [field:Header("Ship Data")]
        [field:SerializeField] public InputData InputData {get; private set;}
        [field:SerializeField] public MechanicsData MechanicsData {get; private set;}
        [field:SerializeField] public StateData StateData {get; private set;}
        [field:SerializeField] public RaceData RaceData {get; private set;}
        [field:SerializeField] public VectorData VectorData {get; private set;}
        [field:SerializeField] public PhysicsData PhysicsData {get; private set;}
        
        [Header("References")]
        [SerializeField] Rigidbody shipRigidbody;
        [SerializeField] Transform shipModel;
        
        
        // SETTERS
        public void SetShipModel(ShipModel modelData) => shipModel = modelData.ModelTransform;
        
        
        // PRIVATE VARIABLES
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
            
            VectorData.velocity = shipRigidbody.linearVelocity;
            VectorData.angularVelocity = shipRigidbody.angularVelocity;
            
            VectorData.localVelocity = shipRigidbody.transform.InverseTransformVector(shipRigidbody.linearVelocity);
            VectorData.localAngularVelocity = shipRigidbody.transform.InverseTransformVector(shipRigidbody.angularVelocity);
            
            VectorData.localVelocityKPH = VectorData.localVelocity * KPHFactor;
            VectorData.localAngularVelocityDPS = VectorData.localAngularVelocity * Mathf.Rad2Deg;
            
            VectorData.modelLocalPosition = shipModel.localPosition;
            VectorData.modelLocalRotation = shipModel.localRotation;
        }
        
    }
}
