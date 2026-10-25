using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace ProjectRacingSky.Ship
{
    using Utility;
    
    /// <summary> Facade class for representation of a racing ship. </summary>
    [DefaultExecutionOrder(ScriptOrder.Ship)] [SelectionBase]
    public class Ship : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Profile")]
        [SerializeField] private ShipProfile shipProfile;
        [SerializeField] private LoadSettings loadSettings;
        [SerializeField] private ShipType shipType;
        [SerializeField] private SpeedClass speedClass;
        
        [Header("Components")]
        [SerializeField] private Rigidbody shipRigidbody;
        [SerializeField] private ShipController shipController;
        [SerializeField] private ShipPhysics shipPhysics;
        [SerializeField] private ShipAnimation shipAnimation;
        [SerializeField] private ShipVFX shipVFX;
        [SerializeField] private ShipAudio shipAudio;
        [SerializeField] private ShipDebug shipDebug;
        
        [Header("Models")]
        [SerializeField] private ShipModel shipModel;
        [SerializeField] private GameObject shipCollider;       // TODO: Create ShipCollider class
        [SerializeField] private Transform colliderContainer;
        
        [Header("Input")]
        [SerializeField] private PilotInput pilotInput;         // raw player input
        [SerializeField] private TrackInput trackInput;         // track mechanics
        [SerializeField] private ZoneInput zoneInput;           // physics modifier zones
        
        [Header("Data")]
        [SerializeField] private InputData inputData;           // processed input
        [SerializeField] private ControllerData controllerData; // controller states
        [SerializeField] private MechanicsData mechanicsData;   // ship status like health, boost, etc.
        [SerializeField] private PhysicsData physicsData;       // physics data
        [SerializeField] private VectorData vectorData;         // ship vector properties
        [SerializeField] private RaceData raceData;             // race data like laps, time
        
        
        // PROPERTIES
        public Transform Transform => transform;
        public ShipModel ShipModel => shipModel;
        public Transform ModelTransform => shipModel.transform;
        
        public Rigidbody Rigidbody => shipRigidbody;
        public ShipController ShipController => shipController;
        public ShipPhysics ShipPhysics => shipPhysics;
        public ShipAnimation ShipAnimation => shipAnimation;
        public ShipVFX ShipVFX => shipVFX;
        public ShipAudio ShipAudio => shipAudio;
        public ShipDebug ShipDebug => shipDebug;
        
        public GameObject ShipCollider => shipCollider;
        
        public PilotInput PilotInput => pilotInput;
        public TrackInput TrackInput => trackInput;
        public ZoneInput ZoneInput => zoneInput;
        
        public InputData InputData => inputData;
        public ControllerData ControllerData => controllerData;
        public MechanicsData MechanicsData => mechanicsData;
        public PhysicsData PhysicsData => physicsData;
        public VectorData VectorData => vectorData;
        public RaceData RaceData => raceData;
        
        
        // PRIVATE FIELDS
        private bool _isHoverOn;
        private bool _isMaglevOn;
        private const float KPHFactor = 3.6f;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            InitializeComponents();
            InitializeData();
        }
        
        private void Start()
        {
            if (loadSettings.loadOnStart) LoadShipProfile();
        }
        
        private void Update()
        {
            SimulateTrackInput();
            UpdateVectorData();
        }
        
        
        // PUBLIC METHODS
        public void SetShipProfile(ShipProfile shipProfile)
        {
            this.shipProfile = shipProfile;
            LoadShipProfile();
        }
        
        public bool LoadShipProfileInEditor(bool isEditor = false)
        {
            if (isEditor) LoadShipProfile(true);
            else Debug.LogWarning("Ship.LoadShipProfileInEditor() is called without permission!");
            return isEditor;
        }
        
        
        // INITIALIZATION
        private void InitializeComponents()
        {
            shipController.SetDependencies(pilotInput, trackInput, zoneInput,
                inputData, controllerData, mechanicsData, physicsData);
            shipPhysics.SetDependencies(shipRigidbody, inputData, controllerData,
                mechanicsData, physicsData);
            shipAnimation.SetDependencies(shipModel, inputData, physicsData, vectorData);
            shipVFX.SetDependencies(inputData, mechanicsData, shipModel);
            shipAudio.SetDependencies(inputData, vectorData);
        }
        
        private void InitializeData()
        {
            raceData.lap = 1;
            raceData.lapTimes = new List<float>() { 0 };
        }
        
        private void ReloadShipModelToComponents()
        {
            shipAnimation.SetShipModel(shipModel);
            shipVFX.SetShipModel(shipModel);
        }
        
        
        // SHIP PROFILE
        private void LoadShipProfile(bool isLoadedOnEditor = false)
        {
            if (!shipProfile) Debug.LogWarning("There is no ship profile set in the inspector!");
            if (!loadSettings.loadProfile) Debug.Log("Ship profile loading is disabled on load settings!");
            if (!loadSettings.loadProfile || !shipProfile) return;
            
            if (loadSettings.loadConfig) LoadShipConfiguration();
            if (loadSettings.loadModel) LoadShipModel(isLoadedOnEditor);
            if (loadSettings.loadCollider) LoadShipCollider(isLoadedOnEditor);
            
            loadSettings.LogLoadingResult(shipProfile.name, isLoadedOnEditor);
        }
        
        private void LoadShipConfiguration()
        {
            if (loadSettings.loadPhysicsConfig) shipPhysics.SetPhysicsConfig(shipProfile.physicsConfig);
            if (loadSettings.loadAnimationConfig) shipAnimation.SetAnimationConfig(shipProfile.animationConfig);
            if (loadSettings.loadVFXConfig) shipVFX.SetVFXConfig(shipProfile.vFXConfig);
            if (loadSettings.loadAudioConfig) shipAudio.SetAudioConfig(shipProfile.audioConfig);
        }
        
        private void LoadShipModel(bool isLoadedOnEditor)
        {
            // NO SHIP MODEL INSIDE SHIP PROFILE
            if (!shipProfile.shipModel)
            {
                Debug.LogWarning($"The ship profile \"{shipProfile.name}\" has no ship model set!");
                return;
            }
            
            // DESTROY EXISTING SHIP MODEL
            GameObject shipModelGameObject = shipModel && shipModel.gameObject ?
                shipModel.gameObject : GetComponentInChildren<ShipModel>().gameObject;
            if (shipModelGameObject)
            {
                if (isLoadedOnEditor) DestroyImmediate(shipModelGameObject);
                else Destroy(shipModelGameObject);
            }
            
            // CREATE NEW SHIP MODEL PREFAB INSTANCE
            shipModelGameObject = Instantiate(shipProfile.shipModel, this.transform);
            shipModelGameObject.name = "Ship Model";
            shipModelGameObject.transform.SetSiblingIndex(0);
            
            // ASSIGN SHIP MODEL COMPONENT TO THIS SHIP AND OTHER COMPONENTS
            ShipModel shipModelComponent = shipModelGameObject.GetComponent<ShipModel>();
            if (!shipModelComponent)
            {
                Debug.LogWarning("The ship model prefab doesn't have the ShipModel component!"
                    + " Automatically creating new ShipModel component.");
                shipModelComponent = shipModelGameObject.AddComponent<ShipModel>();
            }
            shipModel = shipModelComponent;
            ReloadShipModelToComponents();
        }
        
        private void LoadShipCollider(bool isLoadedOnEditor)
        {
            if (!shipProfile.shipCollider)
            {
                Debug.LogWarning($"The ship profile \"{shipProfile.name}\" has no ship collider set!");
                return;
            }
            
            if (shipCollider && shipCollider.IsSceneObject())
            {
                if (isLoadedOnEditor) DestroyImmediate(shipCollider);
                else Destroy(shipCollider);
            }
            
            Transform container = colliderContainer != null ? colliderContainer : this.transform;
            shipCollider = Instantiate(shipProfile.shipCollider, container);
            shipCollider.name = "Ship Collider";
            shipCollider.transform.SetSiblingIndex(1);
        }
        
        
        // UPDATES
        private void UpdateVectorData()
        {
            vectorData.position = shipRigidbody.transform.position;
            vectorData.rotation = shipRigidbody.transform.rotation;
            
            vectorData.speed = shipRigidbody.linearVelocity.magnitude;
            vectorData.speedKPH = vectorData.speed * KPHFactor;
            
            vectorData.localVelocity = shipRigidbody.transform.InverseTransformVector(shipRigidbody.linearVelocity);
            vectorData.localAngularVelocity = shipRigidbody.transform.InverseTransformVector(shipRigidbody.angularVelocity);
            
            vectorData.localVelocityKPH = vectorData.localVelocity * KPHFactor;
            vectorData.localAngularVelocityDPS = vectorData.localAngularVelocity * Mathf.Rad2Deg;
            
            vectorData.modelPosition = shipModel.transform.position;
            vectorData.modelRotation = shipModel.transform.rotation;
            
            vectorData.modelLocalPosition = shipModel.transform.localPosition;
            vectorData.modelLocalRotation = shipModel.transform.localRotation;
        }
        
        private void SimulateTrackInput()
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                _isHoverOn = !_isHoverOn;
                pilotInput.OnHover?.Invoke(_isHoverOn);
            }
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                _isMaglevOn = !_isMaglevOn;
                trackInput.OnMaglev?.Invoke(_isMaglevOn);
            }
            if (Keyboard.current.tKey.wasPressedThisFrame) trackInput.OnJumpPad?.Invoke(Vector3.up);
            if (Keyboard.current.yKey.wasPressedThisFrame) trackInput.OnSpeedPad?.Invoke(transform.forward);
        }
        
        
    }
    
    [Serializable]
    public struct LoadSettings
    {
        public bool loadOnStart;
        public bool loadProfile;
        [Space(5)]
        [AllowNesting, EnableIf("loadProfile")] public bool loadConfig;
        [AllowNesting, EnableIf("loadProfile")] public bool loadModel;
        [AllowNesting, EnableIf("loadProfile")] public bool loadCollider;
        [Space(5)]
        [AllowNesting, EnableIf(EConditionOperator.And, "loadProfile", "loadConfig")] public bool loadPhysicsConfig;
        [AllowNesting, EnableIf(EConditionOperator.And, "loadProfile", "loadConfig")] public bool loadMechanicsConfig;
        [AllowNesting, EnableIf(EConditionOperator.And, "loadProfile", "loadConfig")] public bool loadAnimationConfig;
        [AllowNesting, EnableIf(EConditionOperator.And, "loadProfile", "loadConfig")] public bool loadVFXConfig;
        [AllowNesting, EnableIf(EConditionOperator.And, "loadProfile", "loadConfig")] public bool loadAudioConfig;
        
        public void LogLoadingResult(string profileName, bool isLoadedOnEditor)
        {
            string loadMode = isLoadedOnEditor ? "by inspector" : "in play mode";
            Debug.Log($"The ship profile \"{profileName}\" has been successfully loaded {loadMode}!");
        }
    }
    
}
