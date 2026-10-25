using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.Camera
{
    using Ship;
    using Utility;
    
    [DefaultExecutionOrder(ScriptOrder.CameraSystem)]
    public class CameraFollow : MonoBehaviour
    {
        [Header("Profile")]
        [SerializeField] private Ship targetShip;
        [SerializeField] private CameraProfile cameraProfile;
        [SerializeField] private LoadSettings loadSettings;
        
        [Header("Camera View")]
        [SerializeField] private ViewMode viewMode;
        [SerializeField] private bool lookBehind;
        [SerializeField] private bool debugCamera;
        
        [Header("View Settings")]
        [SerializeField] private CameraView closeView;
        [SerializeField] private CameraView farView;
        [SerializeField] private CameraView topView;
        [SerializeField] private CameraView frontView;
        [SerializeField] private CameraView cockpitView;
        [SerializeField] private CameraView rearView;
        
        [Header("Extra Options")]
        [SerializeField] private Vector3 addPivot;
        [SerializeField] private Vector3 addOrbit;
        [SerializeField] private Vector3 addOffset;
        [SerializeField] private Vector3 addRotation;
        
        [Header("Components")]
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private CameraDamping cameraDamping;
        [SerializeField] private CameraOffset cameraOffset;
        [SerializeField] private CameraFOV cameraFOV;
        [SerializeField] private CameraShake cameraShake;
        
        [Header("Data")]
        [SerializeField] private CameraData cameraData;
        
        
        // PUBLIC GETTERS
        public bool IsFollowModel => cameraData.cameraView.followModel;
        
        
        // CONSTRUCTOR
        public CameraFollow()
        {
            closeView.SetCameraView(    new(0, 3.25f, -9),     new(10, 0, 0), new(0, 0, 50), 78, false, false);
            farView.SetCameraView(      new(0, 4.25f, -10.5f), new(10, 0, 0), new(0, 0, 50), 78, false, false);
            topView.SetCameraView(      new(0, 1.75f, -5),     new(0, 0, 0),  new(0, 0, 50), 78, true,  false);
            frontView.SetCameraView(    new(0, 0, 5),          new(0, 0, 0),  new(0, 0, 50), 78, true,  false);
            cockpitView.SetCameraView(  new(0, 0.5f, -2),      new(0, 0, 0),  new(0, 0, 50), 78, true,  false);
            
            closeView.SetModifiers(     new(new(-5, -5, 0),       new(5, 2.5f, 0),    new(0, 0, 0), new(0, 0, 0), 1, 1, 1, 1, 1, 1, 1));
            farView.SetModifiers(       new(new(-7.5f, -6, 0),    new(7.5f, 3, 0),    new(0, 0, 0), new(0, 0, 0), 1, 1, 1, 1, 1, 1, 1));
            topView.SetModifiers(       new(new(-1, -0.75f, 0),   new(1, 0.75f, 0),   new(0, 0, 0), new(0, 0, 0), 0.05f, 0.025f, 0, 0, 1, 1, 0.75f));
            frontView.SetModifiers(     new(new(0, 0, 0),         new(0, 0, 0),       new(0, 0, 0), new(0, 0, 0), 0, 0, 0, 0, 0, 0, 0.75f));
            cockpitView.SetModifiers(   new(new(-0.1f, -0.1f, 0), new(0.1f, 0.1f, 0), new(0, 0, 0), new(0, 0, 0), 0.01f, 0.01f, 0.1f, 0.1f, 0.2f, 0.4f, 0.25f));
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            InitializeComponents();
            InitializeData();
            if (loadSettings.loadOnStart) LoadCameraProfile(cameraProfile);
        }
        
        private void Update()
        {
            UpdateView();
        }
        
        private void LateUpdate()
        {
            UpdateCamera();
        }
        
        private void OnValidate()
        {
            if (!Application.isPlaying && targetShip)
            {
                UpdateView();
                UpdateCamera();
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!debugCamera) return;
            DrawCameraGizmos(cameraData.cameraView);
            
            void DrawCameraGizmos(CameraView view)
            {
                Transform target = view.followModel ? targetShip.ModelTransform : targetShip.Transform;
                Vector3 cameraPosition = mainCamera.transform.position;
                Vector3 offsetPosition = target.TransformPoint(view.offset);
                Vector3 pivotPosition = target.TransformPoint(view.offset + view.pivot);
                
                DrawLineSphere(target.position, offsetPosition, new Color(1, 1, 1, 0.25f), new Color(1, 1, 1, 0.25f), 0.5f);
                DrawLineSphere(offsetPosition, pivotPosition, new Color(1, 1, 1, 0.5f), Color.white, 0.5f);
                DrawLineSphere(pivotPosition, cameraPosition, new Color(1, 1, 0, 0.5f), Color.blue, 0.25f);
            }
            
            void DrawLineSphere(Vector3 start, Vector3 end, Color c1, Color c2, float radius)
            {
                Gizmos.color = c1; Gizmos.DrawLine(start, end);
                Gizmos.color = c2; Gizmos.DrawSphere(end, radius);
            }
        }
        
        
        // PUBLIC METHODS
        public void ToggleCameraView()
        {
            int viewCount = Enum.GetNames(typeof(ViewMode)).Length - 1;
            viewMode = (ViewMode) ((int)viewMode == viewCount ? 0 : (int) viewMode + 1);
            
            if (viewMode == 0) cameraDamping.SetDampingReset();
            // if (viewMode == 0 || (int)viewMode == 2) cameraDamping.SetDampingReset();
        }
        
        public void SetLookBehind(bool lookBehind)
        {
            this.lookBehind = lookBehind;
            cameraDamping.SetDampingReset();
        }
        
        public void SetTargetShip(Ship targetShip)
        {
            this.targetShip = targetShip;
        }
        
        public void SetCameraProfile(CameraProfile cameraProfile)
        {
            this.cameraProfile = cameraProfile;
            LoadCameraProfile();
        }
        
        public bool LoadCameraProfileInEditor(bool isEditor = false)
        {
            if (isEditor) LoadCameraProfile();
            else Debug.LogWarning("CameraFollow.LoadCameraProfileInEditor() is called without permission!");
            return isEditor;
        }
        
        
        // INITIALIZATION
        private void InitializeComponents()
        {
            mainCamera = UnityEngine.Camera.main;
            
            cameraDamping.SetDependencies(targetShip, cameraData);
            cameraOffset.SetDependencies(targetShip, cameraData);
            cameraFOV.SetDependencies(targetShip, cameraData);
            cameraShake.SetDependencies(targetShip, cameraData);
        }
        
        private void InitializeData()
        {
            cameraData.ResetOffset();
        }
        
        
        // CAMERA PROFILE
        private void LoadCameraProfile(bool isLoadedOnEditor = false)
        {
            if (!loadSettings.loadProfile) Debug.Log("Camera profile loading is disabled on load settings!");
            if (!cameraProfile)  Debug.LogWarning("There is no camera profile set in the inspector!");
            if (!loadSettings.loadProfile || !cameraProfile) return;
            
            LoadCameraConfiguration();
            
            loadSettings.LogLoadingResult(cameraProfile.name, isLoadedOnEditor);
        }
        
        private void LoadCameraConfiguration()
        {
            if (loadSettings.loadViewConfig)
            {
                this.closeView = cameraProfile.viewConfig.closeView;
                this.farView = cameraProfile.viewConfig.farView;
                this.topView = cameraProfile.viewConfig.topView;
                this.frontView = cameraProfile.viewConfig.frontView;
                this.cockpitView = cameraProfile.viewConfig.cockpitView;
                this.rearView = cameraProfile.viewConfig.rearView;
            }
            
            if (loadSettings.loadDampingConfig) this.cameraDamping.SetDampingConfig(cameraProfile.dampingConfig);
            if (loadSettings.loadOffsetConfig) this.cameraOffset.SetOffsetConfig(cameraProfile.offsetConfig);
            if (loadSettings.loadFOVConfig) this.cameraFOV.SetFOVConfig(cameraProfile.fOVConfig);
            if (loadSettings.loadShakeConfig) this.cameraShake.SetShakeConfig(cameraProfile.shakeConfig);
        }
        
        
        // CAMERA VIEW
        private CameraView GetCameraView(ViewMode viewMode)
        {
            return viewMode switch {
                ViewMode.Close => closeView,
                ViewMode.Far => farView,
                ViewMode.Top => topView,
                ViewMode.Front => frontView,
                ViewMode.Cockpit => cockpitView,
                _ => closeView
            };
        }
        
        private Vector3 GetShipViewOffset(ViewMode viewMode)
        {
            if (lookBehind) return targetShip.ShipModel.ViewOffset.rearView;
            
            return viewMode switch {
                ViewMode.Close => targetShip.ShipModel.ViewOffset.closeView,
                ViewMode.Far => targetShip.ShipModel.ViewOffset.farView,
                ViewMode.Top => targetShip.ShipModel.ViewOffset.topView,
                ViewMode.Front => targetShip.ShipModel.ViewOffset.frontView,
                ViewMode.Cockpit => targetShip.ShipModel.ViewOffset.cockpitView,
                _ => Vector3.zero
            };
        }
        
        private float GetFOVOverride(ViewMode viewMode)
        {
            if (lookBehind) return targetShip.ShipModel.FOVOverride.rearFOV;
            
            return viewMode switch {
                ViewMode.Close => targetShip.ShipModel.FOVOverride.closeFOV,
                ViewMode.Far => targetShip.ShipModel.FOVOverride.farFOV,
                ViewMode.Top => targetShip.ShipModel.FOVOverride.topFOV,
                ViewMode.Front => targetShip.ShipModel.FOVOverride.frontFOV,
                ViewMode.Cockpit => targetShip.ShipModel.FOVOverride.cockpitFOV,
                _ => 0
            };
        }
        
        
        // CAMERA UPDATE
        private void UpdateView()
        {
            cameraData.viewMode = viewMode;
            cameraData.cameraView = lookBehind ? rearView : GetCameraView(viewMode);
        }
        
        private void UpdateCamera()
        {
            CameraData data = cameraData;
            CameraView view = cameraData.cameraView;
            ViewModifiers mod = view.modifiers;
            Transform target = view.followModel ? targetShip.ModelTransform : targetShip.Transform;
            
            // Calculate pivot offset and look rotation
            Vector3 pivotPoint = view.pivot + this.addPivot;
            Vector3 pivotOffset = pivotPoint +
                Quaternion.Euler(data.orbitDampOffset * mod.orbitDampMix + this.addOrbit) * -pivotPoint;
            Quaternion lookRotation = !view.lookAtPivot ? target.rotation :
                Quaternion.LookRotation(target.TransformDirection(pivotPoint.normalized), target.up);
            
            // Calculate custom offset
            Vector3 extraPosition = mod.ApplyFollowClamp(data.followDampOffset * mod.followDampMix)
                + data.offsetPosition * mod.offsetPosMix + data.shakePosition * mod.shakePosMix;
            Vector3 extraRotation = mod.ApplyOrbitClamp(data.orbitDampOffset * mod.orbitDampMix)
                + data.offsetRotation * mod.offsetRotMix + data.shakeRotation * mod.shakeRotMix;
            
            // Calculate total offset
            Vector3 positionOffset = view.offset + GetShipViewOffset(this.viewMode) + pivotOffset + this.addOffset;
            Vector3 rotationOffset = view.rotation + this.addRotation + this.addOrbit;
            
            // Calculate and apply camera position and rotation
            Vector3 cameraPosition = target.TransformPoint(positionOffset + extraPosition);
            Quaternion cameraRotation = lookRotation * Quaternion.Euler(rotationOffset + extraRotation);
            mainCamera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
            
            // Calculate camera FOV
            float shipFOVOverride = GetFOVOverride(viewMode);
            float baseFOV = shipFOVOverride == 0 ? view.baseFOV : shipFOVOverride;
            
            // Calculate and apply camera FOV
            mainCamera.fieldOfView = baseFOV + cameraData.fOVOffset * mod.fOVOffsetMix;
            
            // Reset camera offset
            cameraData.ResetOffset();
        }
        
        
        
    }
    
    [Serializable]
    public struct LoadSettings
    {
        public bool loadOnStart;
        public bool loadProfile;
        [Space(5)]
        [AllowNesting, EnableIf("loadProfile")] public bool loadViewConfig;
        [AllowNesting, EnableIf("loadProfile")] public bool loadDampingConfig;
        [AllowNesting, EnableIf("loadProfile")] public bool loadOffsetConfig;
        [AllowNesting, EnableIf("loadProfile")] public bool loadFOVConfig;
        [AllowNesting, EnableIf("loadProfile")] public bool loadShakeConfig;
        
        public void LogLoadingResult(string profileName, bool isLoadedOnEditor)
        {
            string loadMode = isLoadedOnEditor ? "by inspector" : "in playmode";
            Debug.Log($"The camera profile \"{profileName}\" has been successfully loaded {loadMode}!");
        }
        
    }
    
}
