using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRacingSky.Input
{
    using UI;
    
    [DefaultExecutionOrder(ScriptOrder.InputHandler)]
    public class TouchInputHandler : InputHandler
    {
        // INSPECTOR FIELDS
        [SerializeField] private TouchControlUI touchControlUI;
        [SerializeField] private TouchInputConfig touchConfig;
        
        // PROPERTIES
        public override InputEvents InputEvents { protected get; set; }
        public override InputActionAsset InputActionAsset { protected get; set; }
        private Vector3 acceleration => Accelerometer.current.acceleration.ReadValue(); // TODO: CHECK FOR NULL REFERENCE
        
        // PRIVATE FIELDS : SIDESHIFT
        private bool firstAirbrakeLeftPress;
        private bool firstAirbrakeRightPress;
        private Coroutine OnSideshiftCoroutine;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            // if (!touchControlUI)
            //     Debug.LogWarning("InputManager/TouchInputHandler.TouchControlUI is not set on the inspector.");
            
            RegisterToTouchControls();
        }
        
        private void Start()
        {
            RegisterAccelerometer();
        }
        
        private void Update()
        {
            UpdateAccelerometer();
        }
        
        private void OnDestroy()
        {
            UnregisterFromTouchControls();
            UnregisterAccelerometer();
        }
        
        
        // INPUT ACTIONS
        private void RegisterToTouchControls()
        {
            if (!touchControlUI) return;
            
            touchControlUI.OnThrust +=          InvokeThrust;
            touchControlUI.OnSteer +=           InvokeSteer;
            touchControlUI.OnAirbrakeLeft +=    InvokeAirbrakeLeft;
            touchControlUI.OnAirbrakeRight +=   InvokeAirbrakeRight;
            touchControlUI.OnPitch +=           InvokePitch;
            
            // FIXME: TouchInputHandler BOOST & AFTERBURNER events
            touchControlUI.OnBoost +=           InvokeBoost;
            touchControlUI.OnJump +=            InvokeJump;
            touchControlUI.OnFire +=            InvokeFire;
            touchControlUI.OnDrain +=           InvokeDrain;
            
            touchControlUI.OnAirbrakeLeftPressed +=   InvokeSideshiftLeft;
            touchControlUI.OnAirbrakeRightPressed +=  InvokeSideshiftRight;
        }
        
        private void UnregisterFromTouchControls()
        {
            // if (!touchControlUI) Debug.LogWarning("InputManager/TouchInputHandler.TouchControlUI "
            //     + "tried to unsubscribe but the TouchControlUI component is already destroyed.");
            if (!touchControlUI) return;
            
            touchControlUI.OnThrust -=          InvokeThrust;
            touchControlUI.OnSteer -=           InvokeSteer;
            touchControlUI.OnAirbrakeLeft -=    InvokeAirbrakeLeft;
            touchControlUI.OnAirbrakeRight -=   InvokeAirbrakeRight;
            touchControlUI.OnPitch -=           InvokePitch;
            
            // FIXME: TouchInputHandler BOOST & AFTERBURNER events
            touchControlUI.OnBoost -=           InvokeBoost;
            touchControlUI.OnJump -=            InvokeJump;
            touchControlUI.OnFire -=            InvokeFire;
            touchControlUI.OnDrain -=           InvokeDrain;
            
            touchControlUI.OnAirbrakeLeftPressed -=   InvokeSideshiftLeft;
            touchControlUI.OnAirbrakeRightPressed -=  InvokeSideshiftRight;
        }
        
        // INVOKE METHODS
        #region INVOKE METHODS
        private void InvokeThrust(float value)         => InputEvents.OnThrust?.Invoke(value);
        private void InvokeReverse(float value)        => InputEvents.OnReverse?.Invoke(value);
        private void InvokeAfterburner(bool value)     => InputEvents.OnAfterburner?.Invoke(value);
        private void InvokeAirbrakeLeft(float value)   => InputEvents.OnAirbrakeLeft?.Invoke(value);
        private void InvokeAirbrakeRight(float value)  => InputEvents.OnAirbrakeRight?.Invoke(value);
        private void InvokeSteer(float value)          => InputEvents.OnSteer?.Invoke(value);
        private void InvokePitch(float value)          => InputEvents.OnPitch?.Invoke(value);
        private void InvokeRoll(float value)           => InputEvents.OnRoll?.Invoke(value);
        private void InvokeStrafe(float value)         => InputEvents.OnStrafe?.Invoke(value);
        private void InvokeJump()                      => InputEvents.OnJump?.Invoke();
        private void InvokeBoost()                     => InputEvents.OnBoost?.Invoke();
        private void InvokeSideshiftLeft()             => StartSideshift(false);
        private void InvokeSideshiftRight()            => StartSideshift(true);
        
        private void InvokeFire()           => InputEvents.OnFire?.Invoke();
        private void InvokeDrain()          => InputEvents.OnDrain?.Invoke();
        private void InvokeRespawn()        => InputEvents.OnRespawn?.Invoke();
        private void InvokeCanopy()         => InputEvents.OnCanopy?.Invoke();
        
        private void InvokeRearViewOn()     => InputEvents.OnRearView?.Invoke(true);
        private void InvokeRearViewOff()    => InputEvents.OnRearView?.Invoke(false);
        private void InvokeToggleView()     => InputEvents.OnToggleView?.Invoke();
        #endregion
        
        
        // ACCELEROMETER
        private void RegisterAccelerometer()
        {
            if (Accelerometer.current == null)
            {
                InputSystem.AddDevice<Accelerometer>();
                InputSystem.EnableDevice(Accelerometer.current);
                Accelerometer.current.MakeCurrent();
                Debug.Log("Accelerometer registered");
            }
        }
        
        private void UnregisterAccelerometer()
        {
            InputSystem.RemoveDevice(Accelerometer.current);
        }
        
        private void UpdateAccelerometer()
        {
            // if (Accelerometer.current == null)
            // {
            //     Debug.Log("The Accelerometer Device does not exist");
            // }
            // else if (!Accelerometer.current.enabled)
            // {
            //     Debug.Log("The Device is not enabled");
            //     InputSystem.EnableDevice(Accelerometer.current);
            //     Debug.Log("Linear Acceleration was re-enabled!!");
            // }
            // else
            // {
            //     Debug.Log("Currently Enabled: " + Accelerometer.current.ToString());
            //     Debug.Log(Accelerometer.current.acceleration.ReadValue().ToString());
            // }
            
            if (touchConfig.enableSteerTilt)
            {
                float steerInput = Mathf.Clamp(acceleration.x * touchConfig.steerTiltSensitivity, -1, 1);
                InputEvents.OnSteer?.Invoke(steerInput);
            }
            if (touchConfig.enablePitchTilt)
            {
                float pitchInput = Mathf.Clamp(acceleration.y * touchConfig.pitchTiltSensitivity, -1, 1);
                InputEvents.OnPitch?.Invoke(pitchInput);
            }
        }
        
        
        // SIDESHIFT COROUTINE
        private void StartSideshift(bool direction)
        {
            if (OnSideshiftCoroutine != null) StopCoroutine(OnSideshiftCoroutine);
            OnSideshiftCoroutine = StartCoroutine(OnSideshift(direction));
        }
        
        private IEnumerator OnSideshift(bool direction)
        {
            if (!direction && !firstAirbrakeLeftPress)
            {
                firstAirbrakeLeftPress = true;
                firstAirbrakeRightPress = false;
                
                yield return new WaitForSeconds(touchConfig.sideshiftPressTime);
                firstAirbrakeLeftPress = false;
                yield break;
            }
            
            if (direction && !firstAirbrakeRightPress)
            {
                firstAirbrakeRightPress = true;
                firstAirbrakeLeftPress = false;
                
                yield return new WaitForSeconds(touchConfig.sideshiftPressTime);
                firstAirbrakeRightPress = false;
                yield break;
            }
            
            if (firstAirbrakeLeftPress ^ firstAirbrakeRightPress)
            {
                firstAirbrakeLeftPress = false;
                firstAirbrakeRightPress = false;
                
                InputEvents.OnSideshift?.Invoke(direction);
            }
        }
        
        
        
    }
    
}
