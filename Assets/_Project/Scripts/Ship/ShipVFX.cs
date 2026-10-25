using UnityEngine;

namespace ProjectRacingSky.Ship
{
    [DefaultExecutionOrder(ScriptOrder.ShipVFX)]
    public class ShipVFX : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Thruster Flame")]
        [SerializeField] [Range(0, 1)] private float baseLength;
        [SerializeField] [Range(0, 5)] private float fullLength;
        [SerializeField] [Range(0, 5)] private float increaseTime;
        [SerializeField] [Range(0, 5)] private float decreaseTime;
        
        
        // PRIVATE GETTERS
        private ParticleSystem[] thrusterFlame => shipModel.ThrusterFlame;
        private ParticleSystem afterburnerTrail => shipModel.AfterburnerTrail;
        
        // PRIVATE FIELDS
        private ShipModel shipModel;
        private InputData inputData;
        private MechanicsData mechanicsData;
        
        private float thrustControl;
        private float p_thrustControl;
        private Color baseAfterburnerTrailColor;
        
        
        // CONSTRUCTOR & FINALIZER
        public ShipVFX()
        {
            baseLength = 0.1f;
            fullLength = 0.25f;
            increaseTime = 2;
            decreaseTime = 2;
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            if (!afterburnerTrail) return;
            baseAfterburnerTrailColor = afterburnerTrail.main.startColor.color;
        }
        
        private void LateUpdate()
        {
            UpdateThrusterFlame();
            UpdateAfterburnerTrail();
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(InputData inputData, MechanicsData mechanicsData, ShipModel shipModel)
        {
            this.shipModel = shipModel;
            this.inputData = inputData;
            this.mechanicsData = mechanicsData;
        }
        
        public void SetShipModel(ShipModel shipModel)
        {
            this.shipModel = shipModel;
        }
        
        public void SetVFXConfig(VFXConfig vFXConfig)
        {
            // TODO: Empty SetVFXConfig
        }
        
        
        // CLASS METHODS
        private void UpdateThrusterFlame()
        {
            float thrustTime = thrustControl >= p_thrustControl ? increaseTime : decreaseTime;
            p_thrustControl = thrustControl;
            thrustControl = Mathf.MoveTowards(thrustControl, Mathf.Clamp01(inputData.thrustInput), Time.deltaTime / thrustTime);
            
            if (thrusterFlame == null) return;
            foreach (ParticleSystem thrusterFlame in thrusterFlame)
            {
                if (!thrusterFlame) return;
                ParticleSystem.MainModule thrusterFlameMain = thrusterFlame.main;
                thrusterFlameMain.startLifetimeMultiplier = baseLength + (thrustControl * fullLength);
            }
        }
        
        private void UpdateAfterburnerTrail()
        {
            if (!afterburnerTrail) return;
            
            float afterburnerInput = inputData.afterburnerInput;
            
            Color trailColor = baseAfterburnerTrailColor;
            trailColor.a *= afterburnerInput;
            
            ParticleSystem.MainModule afterburnerTrailMain = afterburnerTrail.main;
            ParticleSystem.MinMaxGradient afterburnerTrailGradient = afterburnerTrail.main.startColor;
            
            afterburnerTrailGradient.color = trailColor;
            afterburnerTrailMain.startColor = afterburnerTrailGradient;
        }
        
    }
}
