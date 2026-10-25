using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Manager
{
    using Camera;
    
    public class CameraManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Cameras")]
        [SerializeField] private CameraFollow shipCamera;
        
        [Header("Data")]
        [SerializeField] private ActionInput actionInput;
        
        
        // GETTERS
        public ActionInput ActionInput => this.actionInput;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            RegisterToCameraControls();
        }
        
        
        // PRIVATE METHODS
        private void RegisterToCameraControls()
        {
            actionInput.OnRearView += value => shipCamera.SetLookBehind(value);
            actionInput.OnToggleView += () => shipCamera.ToggleCameraView();
        }
        
        
        
        
    }
}
