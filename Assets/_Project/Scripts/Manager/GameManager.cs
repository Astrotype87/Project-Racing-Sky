using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Manager
{
    public class GameManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Managers")]
        [SerializeField] private RaceManager raceManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private CameraManager cameraManager;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            SetManagerReferences();
            // DontDestroyOnLoad(this);
        }
        
        
        // PRIVATE METHODS
        private void SetManagerReferences()
        {
            if (inputManager && cameraManager)
            inputManager.SetCameraManager(cameraManager);
        }
        
        
        
    }
    
}
