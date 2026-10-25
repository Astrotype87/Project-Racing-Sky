using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRacingSky.Input
{
    using CallbackContext = InputAction.CallbackContext;
    
    [DefaultExecutionOrder(ScriptOrder.InputHandler)]
    public class KeyboardInputHandler : InputHandler
    {
        // INSPECTOR FIELDS
        [SerializeField] private KeyboardInputConfig keyboardConfig;
        
        // PROPERTIES
        public override InputEvents InputEvents { protected get; set; }
        public override InputActionAsset InputActionAsset { protected get; set; }
        public KeyboardInputConfig KeyboardConfig { get => keyboardConfig; set => keyboardConfig = value; }
        
        // PRIVATE FIELDS : INPUT ACTIONS
        private InputActionMap shipActionMap;
        private InputAction thrustAction;
        private InputAction reverseAction;
        private InputAction steerAction;
        private InputAction airbrakeLeftAction;
        private InputAction airbrakeRightAction;
        private InputAction pitchAction;
        private InputAction rollAction;
        private InputAction strafeAction;
        private InputAction boostAction;
        private InputAction afterburnerAction;
        private InputAction jumpAction;
        private InputAction fireAction;
        private InputAction drainAction;
        private InputAction respawnAction;
        private InputAction canopyAction;
        private InputAction lookBehindAction;
        private InputAction toggleViewAction;
        
        // PRIVATE FIELDS : SIDESHIFT
        private bool firstAirbrakeLeftPress;
        private bool firstAirbrakeRightPress;
        private Coroutine OnSideshiftCoroutine;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (InputActionAsset == null) return;
            shipActionMap = InputActionAsset.FindActionMap("Ship");
            
            if (shipActionMap == null) return;
            shipActionMap.Enable();
            
            FindAllInputActions();
            SubscribeInputEventsToActions();
        }
        
        private void OnEnable()
        {
            shipActionMap?.Enable();
        }
        
        private void OnDisable()
        {
            shipActionMap?.Disable();
        }
        
        private void OnDestroy()
        {
            if (shipActionMap == null) return;
            
            UnsubscribeInputEventsFromActions();
        }
        
        
        // PUBLIC METHODS
        public void SetKeyboardConfig(KeyboardInputConfig config)
        {
            this.keyboardConfig = config;
        }
        
        
        // INPUT ACTIONS
        private void FindAllInputActions()
        {
            thrustAction = shipActionMap.FindAction("Thrust");
            reverseAction = shipActionMap.FindAction("Reverse");
            steerAction = shipActionMap.FindAction("Steer");
            airbrakeLeftAction = shipActionMap.FindAction("AirbrakeLeft");
            airbrakeRightAction = shipActionMap.FindAction("AirbrakeRight");
            pitchAction = shipActionMap.FindAction("Pitch");
            rollAction = shipActionMap.FindAction("Roll");
            strafeAction = shipActionMap.FindAction("Strafe");
            boostAction = shipActionMap.FindAction("Boost");
            afterburnerAction = shipActionMap.FindAction("Afterburner");
            
            jumpAction = shipActionMap.FindAction("Jump");
            fireAction = shipActionMap.FindAction("Fire");
            drainAction = shipActionMap.FindAction("Drain");
            respawnAction = shipActionMap.FindAction("Respawn");
            canopyAction = shipActionMap.FindAction("Canopy");
            
            lookBehindAction = shipActionMap.FindAction("LookBehind");
            toggleViewAction = shipActionMap.FindAction("ToggleView");
        }
        
        private void SubscribeInputEventsToActions()
        {
            thrustAction.performed += InvokeThrust;               thrustAction.canceled += InvokeThrust;
            reverseAction.performed += InvokeReverse;             reverseAction.canceled += InvokeReverse;
            afterburnerAction.performed += InvokeAfterburner;     afterburnerAction.canceled += InvokeAfterburner;
            airbrakeLeftAction.performed += InvokeAirbrakeLeft;   airbrakeLeftAction.canceled += InvokeAirbrakeLeft;
            airbrakeRightAction.performed += InvokeAirbrakeRight; airbrakeRightAction.canceled += InvokeAirbrakeRight;
            steerAction.performed += InvokeSteer;                 steerAction.canceled += InvokeSteer;
            pitchAction.performed += InvokePitch;                 pitchAction.canceled += InvokePitch;
            rollAction.performed += InvokeRoll;                   rollAction.canceled += InvokeRoll;
            strafeAction.performed += InvokeStrafe;               strafeAction.canceled += InvokeStrafe;
            
            jumpAction.started += InvokeJump;
            boostAction.started += InvokeBoost;
            airbrakeLeftAction.started += InvokeSideshiftLeft;
            airbrakeRightAction.started += InvokeSideshiftRight;
            
            fireAction.started += InvokeFire;
            drainAction.started += InvokeDrain;
            respawnAction.started += InvokeRespawn;
            canopyAction.started += InvokeCanopy;
            
            lookBehindAction.started += InvokeRearViewOn;         lookBehindAction.canceled += InvokeRearViewOff;
            toggleViewAction.started += InvokeToggleView;
        }
        
        private void UnsubscribeInputEventsFromActions()
        {
            thrustAction.performed -= InvokeThrust;               thrustAction.canceled -= InvokeThrust;
            reverseAction.performed -= InvokeReverse;             reverseAction.canceled -= InvokeReverse;
            afterburnerAction.performed -= InvokeAfterburner;     afterburnerAction.canceled -= InvokeAfterburner;
            airbrakeLeftAction.performed -= InvokeAirbrakeLeft;   airbrakeLeftAction.canceled -= InvokeAirbrakeLeft;
            airbrakeRightAction.performed -= InvokeAirbrakeRight; airbrakeRightAction.canceled -= InvokeAirbrakeRight;
            steerAction.performed -= InvokeSteer;                 steerAction.canceled -= InvokeSteer;
            pitchAction.performed -= InvokePitch;                 pitchAction.canceled -= InvokePitch;
            rollAction.performed -= InvokeRoll;                   rollAction.canceled -= InvokeRoll;
            strafeAction.performed -= InvokeStrafe;               strafeAction.canceled -= InvokeStrafe;
            
            jumpAction.started -= InvokeJump;
            boostAction.started -= InvokeBoost;
            airbrakeLeftAction.started -= InvokeSideshiftLeft;
            airbrakeRightAction.started -= InvokeSideshiftRight;
            
            fireAction.started -= InvokeFire;
            drainAction.started -= InvokeDrain;
            respawnAction.started -= InvokeRespawn;
            canopyAction.started -= InvokeCanopy;
            
            lookBehindAction.started -= InvokeRearViewOn;           lookBehindAction.canceled -= InvokeRearViewOff;
            toggleViewAction.started -= InvokeToggleView;
        }
        
        // INVOKE METHODS
        #region INVOKE METHODS
        private void InvokeThrust(CallbackContext ctx)          => InputEvents.OnThrust?.Invoke(ctx.ReadValue<float>());
        private void InvokeReverse(CallbackContext ctx)         => InputEvents.OnReverse?.Invoke(ctx.ReadValue<float>());
        private void InvokeAfterburner(CallbackContext ctx)     => InputEvents.OnAfterburner?.Invoke(ctx.ReadValue<float>() >= 0.5f);
        private void InvokeAirbrakeLeft(CallbackContext ctx)    => InputEvents.OnAirbrakeLeft?.Invoke(ctx.ReadValue<float>());
        private void InvokeAirbrakeRight(CallbackContext ctx)   => InputEvents.OnAirbrakeRight?.Invoke(ctx.ReadValue<float>());
        private void InvokeSteer(CallbackContext ctx)           => InputEvents.OnSteer?.Invoke(ctx.ReadValue<float>());
        private void InvokePitch(CallbackContext ctx)           => InputEvents.OnPitch?.Invoke(ctx.ReadValue<float>());
        private void InvokeRoll(CallbackContext ctx)            => InputEvents.OnRoll?.Invoke(ctx.ReadValue<float>());
        private void InvokeStrafe(CallbackContext ctx)          => InputEvents.OnStrafe?.Invoke(ctx.ReadValue<float>());
        
        private void InvokeJump(CallbackContext ctx)            => InputEvents.OnJump?.Invoke();
        private void InvokeBoost(CallbackContext ctx)           => InputEvents.OnBoost?.Invoke();
        private void InvokeSideshiftLeft(CallbackContext ctx)   => StartSideshift(false);
        private void InvokeSideshiftRight(CallbackContext ctx)  => StartSideshift(true);
        
        private void InvokeFire(CallbackContext ctx)            => InputEvents.OnFire?.Invoke();
        private void InvokeDrain(CallbackContext ctx)           => InputEvents.OnDrain?.Invoke();
        private void InvokeRespawn(CallbackContext ctx)         => InputEvents.OnRespawn?.Invoke();
        private void InvokeCanopy(CallbackContext ctx)          => InputEvents.OnCanopy?.Invoke();
        
        private void InvokeRearViewOn(CallbackContext ctx)      => InputEvents.OnRearView?.Invoke(true);
        private void InvokeRearViewOff(CallbackContext ctx)     => InputEvents.OnRearView?.Invoke(false);
        private void InvokeToggleView(CallbackContext ctx)      => InputEvents.OnToggleView?.Invoke();
        #endregion
        
        
        // KEYBOARD CONFIG
        private void GetKeyboardConfig(KeyboardInputConfig config)
        {
            config.thrustKey = thrustAction.bindings[0].effectivePath;
            config.reverseKey = reverseAction.bindings[0].effectivePath;
            config.steerLeftKey = steerAction.bindings[1].effectivePath;
            config.steerRightKey = steerAction.bindings[2].effectivePath;
            config.airbrakeLeftKey = airbrakeLeftAction.bindings[1].effectivePath;
            config.airbrakeRightKey = airbrakeRightAction.bindings[2].effectivePath;
            config.pitchDownKey = pitchAction.bindings[1].effectivePath;
            config.pitchUpKey = pitchAction.bindings[2].effectivePath;
            config.rollLeftKey = rollAction.bindings[1].effectivePath;
            config.rollRightKey = rollAction.bindings[2].effectivePath;
            config.strafeLeftKey = strafeAction.bindings[1].effectivePath;
            config.strafeRightKey = strafeAction.bindings[2].effectivePath;
            config.boostKey = boostAction.bindings[0].effectivePath;
            
            config.jumpKey = jumpAction.bindings[0].effectivePath;
            config.fireKey = fireAction.bindings[0].effectivePath;
            config.drainKey = drainAction.bindings[0].effectivePath;
            config.respawnKey = respawnAction.bindings[0].effectivePath;
            config.canopyKey = canopyAction.bindings[0].effectivePath;
            
            config.lookBehindKey = lookBehindAction.bindings[0].effectivePath;
            config.toggleViewKey = toggleViewAction.bindings[0].effectivePath;
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
                
                yield return new WaitForSeconds(keyboardConfig.sideshiftPressTime);
                firstAirbrakeLeftPress = false;
                yield break;
            }
            
            if (direction && !firstAirbrakeRightPress)
            {
                firstAirbrakeRightPress = true;
                firstAirbrakeLeftPress = false;
                
                yield return new WaitForSeconds(keyboardConfig.sideshiftPressTime);
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
