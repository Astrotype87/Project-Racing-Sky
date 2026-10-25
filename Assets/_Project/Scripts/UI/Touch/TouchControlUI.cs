using System;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class TouchControlUI : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [SerializeField] private bool enableEventLogs;
        
        [Header("Touch Control Elements")]
        [SerializeField] private TouchButton thrustButton;
        [SerializeField] private TouchSlider thrustSlider;
        
        [SerializeField] private TouchButton steerLeftButton;
        [SerializeField] private TouchButton steerRightButton;
        [SerializeField] private TouchSlider steerSlider;
        
        [SerializeField] private TouchButton airbrakeButton;
        [SerializeField] private TouchButton airbrakeLeftButton;
        [SerializeField] private TouchButton airbrakeRightButton;
        
        [SerializeField] private TouchButton airbrakeButton2;
        [SerializeField] private TouchButton airbrakeLeftButton2;
        [SerializeField] private TouchButton airbrakeRightButton2;
        
        [SerializeField] private TouchButton pitchDownButton;
        [SerializeField] private TouchButton pitchUpButton;
        [SerializeField] private TouchSlider pitchSlider;
        
        [SerializeField] private TouchButton boostButton;
        [SerializeField] private TouchButton fireButton;
        [SerializeField] private TouchButton lookBackButton;
        [SerializeField] private TouchButton pauseButton;
        
        [SerializeField] private TouchDpad dpad;
        [SerializeField] private TouchJoystick joystick;
        
        
        // EVENTS
        public Action<float> OnThrust;
        public Action<float> OnSteer;
        public Action<float> OnAirbrakeLeft;
        public Action<float> OnAirbrakeRight;
        public Action<float> OnPitch;
        public Action OnBoost;
        public Action OnJump;
        public Action OnFire;
        public Action OnDrain;
        public Action OnLookBehind;
        public Action OnPause;
        
        public Action OnAirbrakeLeftPressed;
        public Action OnAirbrakeRightPressed;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            RegisterEventsToButtons();
            RegisterEventLogs();
        }
        
        
        // PUBLIC METHODS
        public void ClearEventListeners()
        {
            OnThrust = null;
            OnSteer = null;
            OnAirbrakeLeft = null;
            OnAirbrakeRight = null;
            OnPitch = null;
            OnBoost = null;
            OnJump = null;
            OnFire = null;
            OnDrain = null;
            OnLookBehind = null;
            OnPause = null;
        }
        
        
        // PRIVATE METHODS
        private void SetThrust(float value) => OnThrust?.Invoke(value);
        private void SetSteer(float value) => OnSteer?.Invoke(value);
        private void SetAirbrakeLeft(float value) => OnAirbrakeLeft?.Invoke(value);
        private void SetAirbrakeRight(float value) => OnAirbrakeRight?.Invoke(value);
        private void SetPitch(float value) => OnPitch?.Invoke(value);
        private void SetBoost() => OnBoost?.Invoke();
        private void SetJump() => OnJump?.Invoke();
        private void SetFire() => OnFire?.Invoke();
        private void SetDrain() => OnDrain?.Invoke();
        private void SetLookBehind() => OnLookBehind?.Invoke();
        private void SetPause() => OnPause?.Invoke();
        
        private void RegisterEventsToButtons()
        {
            if (thrustButton) thrustButton.OnButtonUpdate += value => SetThrust(value);
            if (thrustSlider) thrustSlider.OnButtonUpdate += value => SetThrust(value);
            
            if (steerLeftButton) steerLeftButton.OnButtonUpdate += value => SetSteer(-value);
            if (steerRightButton) steerRightButton.OnButtonUpdate += value => SetSteer(value);
            if (steerSlider) steerSlider.OnButtonUpdate += value => SetSteer(value);
            
            if (airbrakeButton) airbrakeButton.OnButtonUpdate += value => SetAirbrakeLeft(value);
            if (airbrakeButton) airbrakeButton.OnButtonUpdate += value => SetAirbrakeRight(value);
            if (airbrakeLeftButton) airbrakeLeftButton.OnButtonUpdate += value => SetAirbrakeLeft(value);
            if (airbrakeRightButton) airbrakeRightButton.OnButtonUpdate += value => SetAirbrakeRight(value);
            
            if (airbrakeButton2) airbrakeButton2.OnButtonUpdate += value => SetAirbrakeLeft(value);
            if (airbrakeButton2) airbrakeButton2.OnButtonUpdate += value => SetAirbrakeRight(value);
            if (airbrakeLeftButton2) airbrakeLeftButton2.OnButtonUpdate += value => SetAirbrakeLeft(value);
            if (airbrakeRightButton2) airbrakeRightButton2.OnButtonUpdate += value => SetAirbrakeRight(value);
            
            // For sideshift
            if (airbrakeLeftButton) airbrakeLeftButton.OnButtonPressed += () => OnAirbrakeLeftPressed();        
            if (airbrakeRightButton) airbrakeRightButton.OnButtonPressed += () => OnAirbrakeRightPressed();
            if (airbrakeLeftButton2) airbrakeLeftButton2.OnButtonPressed += () => OnAirbrakeLeftPressed();
            if (airbrakeRightButton2) airbrakeRightButton2.OnButtonPressed += () => OnAirbrakeRightPressed();
            
            if (pitchDownButton) pitchDownButton.OnButtonUpdate += value => SetPitch(value);
            if (pitchUpButton) pitchUpButton.OnButtonUpdate += value => SetPitch(-value);
            if (pitchSlider) pitchSlider.OnButtonUpdate += value => SetPitch(value);
            
            if (boostButton) boostButton.OnButtonUpdate += value => SetBoost();
            if (fireButton) fireButton.OnButtonPressed += () => SetFire();
            if (lookBackButton) lookBackButton.OnButtonPressed += () => SetLookBehind();
            if (pauseButton) pauseButton.OnButtonPressed += () => SetPause();
            
            if (dpad) dpad.OnButtonUpdate += value => SetSteer(value.x);
            if (dpad) dpad.OnButtonUpdate += value => SetPitch(value.y);
            if (joystick) joystick.OnButtonUpdate += value => SetSteer(value.x);
            if (joystick) joystick.OnButtonUpdate += value => SetPitch(value.y);
        }
        
        private void UnregisterEventsToButtons()
        {
            if (thrustButton) thrustButton.OnButtonUpdate = null;
            if (thrustSlider) thrustSlider.OnButtonUpdate = null;
            
            if (steerLeftButton) steerLeftButton.OnButtonUpdate = null;
            if (steerRightButton) steerRightButton.OnButtonUpdate = null;
            if (steerSlider) steerSlider.OnButtonUpdate = null;
            
            if (airbrakeButton) airbrakeButton.OnButtonUpdate = null;
            if (airbrakeButton) airbrakeButton.OnButtonUpdate = null;
            if (airbrakeLeftButton) airbrakeLeftButton.OnButtonUpdate = null;
            if (airbrakeRightButton) airbrakeRightButton.OnButtonUpdate = null;
            
            if (airbrakeButton2) airbrakeButton2.OnButtonUpdate = null;
            if (airbrakeButton2) airbrakeButton2.OnButtonUpdate = null;
            if (airbrakeLeftButton2) airbrakeLeftButton2.OnButtonUpdate = null;
            if (airbrakeRightButton2) airbrakeRightButton2.OnButtonUpdate = null;
            
            if (pitchDownButton) pitchDownButton.OnButtonUpdate = null;
            if (pitchUpButton) pitchUpButton.OnButtonUpdate = null;
            if (pitchSlider) pitchSlider.OnButtonUpdate = null;
            
            if (boostButton) boostButton.OnButtonUpdate = null;
            if (fireButton) fireButton.OnButtonPressed = null;
            if (lookBackButton) lookBackButton.OnButtonPressed = null;
            if (pauseButton) pauseButton.OnButtonPressed = null;
            
            if (dpad) dpad.OnButtonUpdate = null;
            if (dpad) dpad.OnButtonUpdate = null;
            if (joystick) joystick.OnButtonUpdate = null;
            if (joystick) joystick.OnButtonUpdate = null;
        }
        
        private void RegisterEventLogs()
        {
            void Log(object message)
            {
                if (enableEventLogs) Debug.Log(message);
            }
            
            OnThrust +=         value => Log("Thrust: " + value);
            OnSteer +=          value => Log("Steer: " + value);
            OnAirbrakeLeft +=   value => Log("AirbrakeLeft: " + value);
            OnAirbrakeRight +=  value => Log("AirbrakeRight: " + value);
            OnPitch +=          value => Log("Pitch: " + value);
            OnBoost +=          () => Log("Boost!");
            OnJump +=           () => Log("Jump!");
            OnFire +=           () => Log("Fire!");
            OnDrain +=          () => Log("Drain!");
            OnLookBehind +=     () => Log("LookBehind!");
            OnPause +=          () => Log("Pause!");
            
        }
        
        
    }
    
}
