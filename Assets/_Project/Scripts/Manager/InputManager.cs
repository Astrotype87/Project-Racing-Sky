using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRacingSky.Manager
{
    using ProjectRacingSky.Input;
    
    [DefaultExecutionOrder(ScriptOrder.InputManager)]
    public class InputManager : MonoBehaviour
    {
        [Header("Input Action Asset")]
        [SerializeField] private InputActionAsset inputActions;
        
        [Header("Input Channels")]
        [SerializeField] private UIInputManager uIInputManager;
        [SerializeField] private RaceInputManager raceInputManager;
        [SerializeField] private EditorInputManager editorInputManager;
        
        [Header("Rebind")]
        [SerializeField] private RebindManager rebindManager;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
        
        
        // PUBLIC METHODS
        public void SetCameraManager(CameraManager cameraManager)
        {
            raceInputManager.SetCameraManager(cameraManager);
        }
        
    }
}
