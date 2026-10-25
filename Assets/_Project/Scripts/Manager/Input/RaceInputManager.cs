using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRacingSky.Input
{
    using ProjectRacingSky.Ship;
    using ProjectRacingSky.Manager;
    
    [DefaultExecutionOrder(ScriptOrder.InputManager)]
    public class RaceInputManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Player Ships")]
        [SerializeField] private Ship playerShip;
        [SerializeField] private Ship playerShip2;
        
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private List<InputHandler> inputHandlers;
        
        [Header("References")]
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private RebindManager rebindManager;
        
        // PRIVATE FIELDS
        private InputEvents inputEvents_player1;
        private InputEvents inputEvents_player2;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            inputEvents_player1 = new InputEvents();
            
            InitializeInputHandlers();
            SubscribeShipToInputEvents(inputEvents_player1);
        }
        
        private void Start()
        {
            
        }
        
        private void OnDestroy()
        {
            UnsubscribeShipToInputEvents(inputEvents_player1);
        }
        
        
        // PUBLIC METHODS
        public void SetPlayerShip(Ship playerShip)
        {
            this.playerShip = playerShip;
        }
        
        public void SetCameraManager(CameraManager cameraManager)
        {
            this.cameraManager = cameraManager;
        }
        
        
        // INPUT EVENTS & HANDLERS
        #region INPUT EVENTS
        
        private void SetThrust(float value)         => playerShip.PilotInput.thrustInput = value;
        private void SetReverse(float value)        => playerShip.PilotInput.reverseInput = value;
        private void SetAfterburner(bool value)     => playerShip.PilotInput.afterburnerInput = value;
        private void SetAirbrakeLeft(float value)   => playerShip.PilotInput.airbrakeLeftInput = value;
        private void SetAirbrakeRight(float value)  => playerShip.PilotInput.airbrakeRightInput = value;
        
        private void SetSteer(float value)          => playerShip.PilotInput.steerInput = value;
        private void SetPitch(float value)          => playerShip.PilotInput.pitchInput = value;
        private void SetRoll(float value)           => playerShip.PilotInput.rollInput = value;
        private void SetStrafe(float value)         => playerShip.PilotInput.strafeInput = value;
        
        private void InvokeJump()                   => playerShip.PilotInput.OnJump?.Invoke();
        private void InvokeBoost()                  => playerShip.PilotInput.OnBoost?.Invoke();
        private void InvokeSideshift(bool value)    => playerShip.PilotInput.OnSideshift?.Invoke(value);
        
        private void InvokeUsePickup()              => playerShip.PilotInput.OnFire?.Invoke();
        private void InvokeDrainPickup()            => playerShip.PilotInput.OnDrain?.Invoke();
        private void InvokeRespawn()                => playerShip.PilotInput.OnRespawn?.Invoke();
        private void InvokeCanopy()                 => playerShip.PilotInput.OnCanopy?.Invoke();
        
        private void SetRearView(bool value)        => cameraManager.ActionInput.OnRearView?.Invoke(value);
        private void SetToggleView()                => cameraManager.ActionInput.OnToggleView?.Invoke();
        
        #endregion
        
        private void InitializeInputHandlers()
        {
            foreach (InputHandler inputHandler in inputHandlers)
            {
                inputHandler.InputEvents = this.inputEvents_player1;
                inputHandler.InputActionAsset = this.inputActionAsset;
            }
        }
        
        private void SubscribeShipToInputEvents(InputEvents inputEvents)
        {
            inputEvents.OnThrust += SetThrust;
            inputEvents.OnReverse += SetReverse;
            inputEvents.OnAfterburner += SetAfterburner;
            inputEvents.OnAirbrakeLeft += SetAirbrakeLeft;
            inputEvents.OnAirbrakeRight += SetAirbrakeRight;
            inputEvents.OnSteer += SetSteer;
            inputEvents.OnPitch += SetPitch;
            inputEvents.OnRoll += SetRoll;
            inputEvents.OnStrafe += SetStrafe;
            inputEvents.OnJump += InvokeJump;
            inputEvents.OnBoost += InvokeBoost;
            inputEvents.OnSideshift += InvokeSideshift;
            
            inputEvents.OnFire += InvokeUsePickup;
            inputEvents.OnDrain += InvokeDrainPickup;
            inputEvents.OnRespawn += InvokeRespawn;
            inputEvents.OnCanopy += InvokeCanopy;
            
            inputEvents.OnRearView += SetRearView;
            inputEvents.OnToggleView += SetToggleView;
        }
        
        private void UnsubscribeShipToInputEvents(InputEvents inputEvents)
        {
            inputEvents.OnThrust -= SetThrust;
            inputEvents.OnReverse -= SetReverse;
            inputEvents.OnAfterburner -= SetAfterburner;
            inputEvents.OnAirbrakeLeft -= SetAirbrakeLeft;
            inputEvents.OnAirbrakeRight -= SetAirbrakeRight;
            inputEvents.OnSteer -= SetSteer;
            inputEvents.OnPitch -= SetPitch;
            inputEvents.OnRoll -= SetRoll;
            inputEvents.OnStrafe -= SetStrafe;
            inputEvents.OnJump -= InvokeJump;
            inputEvents.OnBoost -= InvokeBoost;
            inputEvents.OnSideshift -= InvokeSideshift;
            
            inputEvents.OnFire -= InvokeUsePickup;
            inputEvents.OnDrain -= InvokeDrainPickup;
            inputEvents.OnRespawn -= InvokeRespawn;
            inputEvents.OnCanopy -= InvokeCanopy;
            
            inputEvents.OnRearView -= SetRearView;
            inputEvents.OnToggleView -= SetToggleView;
        }
        
        
        
    }

}
