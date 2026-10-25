using UnityEngine;

namespace ProjectRacingSky.Ship
{
    using Config = AudioConfig;
    
    [DefaultExecutionOrder(ScriptOrder.ShipAudio)]
    public class ShipAudio : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Clips")]
        [SerializeField] private Config.Sources sources;
        
        [Header("Behaviour (Constant)")]
        [SerializeField] private AudioConstant engine_Speed;
        [SerializeField] private AudioConstant engine_Thrust;
        
        [Header("Behaviour (Impulse)")]
        [SerializeField] private AudioImpulse engine_SpeedPad;
        
        [Header("Sources")]
        [SerializeField] private AudioSource engineSource;
        
        // PRIVATE FIELDS
        private InputData inputData;
        private VectorData vectorData;
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            SetAudioSources();
            PlayAudioSources();
            InitializeAudioBehaviours();
        }
        
        private void LateUpdate()
        {
            UpdateShipAudio();
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(InputData inputData, VectorData vectorData)
        {
            this.inputData = inputData;
            this.vectorData = vectorData;
        }
        
        public void SetAudioConfig(AudioConfig audioConfig)
        {
            // TODO: Empty SetAudioConfig
        }
        
        
        
        // INITIALIZATION
        private void SetAudioSources()
        {
            engineSource.clip = sources.engine;
        }
        
        private void PlayAudioSources()
        {
            engineSource.Play();
            engineSource.loop = true;
            engineSource.spatialBlend = 1;
        }
        
        private void InitializeAudioBehaviours()
        {
            engine_Thrust.Initialize();
            engine_Speed.Initialize();
            engine_SpeedPad.Initialize();
            
            inputData.OnSpeedPad += value => engine_SpeedPad.OnImpulse(this, speedPadFactor(value));
            
            float speedPadFactor(Vector3 direction) => Mathf.Max(Vector3.Dot(transform.forward, direction), -0.5f);
        }
        
        // UPDATE
        private void UpdateAudioSource(AudioSource source, (IAudioBehaviour behaviour, float unit)[] layer)
        {
            float volume = 0, pitch = 0;
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i].behaviour.UpdateAudio(layer[i].unit, Time.deltaTime);
                volume += layer[i].behaviour.VolumeOutput;
                pitch += layer[i].behaviour.PitchOutput;
            }
            source.volume = volume;
            source.pitch = pitch;
        }
        
        private void UpdateShipAudio()
        {
            float thrustInput = inputData.thrustInput;
            float speedKPH = vectorData.speedKPH;
            
            UpdateAudioSource(engineSource, new (IAudioBehaviour, float)[] {
                (engine_Speed, speedKPH),
                (engine_Thrust, thrustInput),
                (engine_SpeedPad, 1)
            });
        }
        
        
    }
    
}
