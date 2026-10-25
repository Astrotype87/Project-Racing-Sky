using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Archived
{
    public class SomeButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private InputActionAsset inputActions;
        
        [InputControl(layout = "Button")]
        [SerializeField] private string m_ControlPath;
        
        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
        
        private InputAction screenThrustAction;
        
        
        private void Start()
        {
            screenThrustAction = inputActions.FindAction("Thrust");
            
            screenThrustAction.started += context => Debug.Log("screenThrustAction.started: " + context.ReadValue<float>());
            screenThrustAction.performed += context => Debug.Log("screenThrustAction.performed: " + context.ReadValue<float>());
            screenThrustAction.canceled += context => Debug.Log("screenThrustAction.canceled: " + context.ReadValue<float>());
            
            // ILLEGALLY get reference to Action events started, performed and canceled 
            FieldInfo fieldInfo = typeof(InputAction).GetField("started", BindingFlags.Instance | BindingFlags.NonPublic);
            Action<CallbackContext> action = (Action<CallbackContext>) fieldInfo?.GetValue(screenThrustAction);
            
            // ILLEGALLY invoke Action events started, performed and canceled
            CallbackContext callbackContext = new CallbackContext();
            // callbackContext.
            
            action?.Invoke(callbackContext);
        }
        
        
        public void OnPointerUp(PointerEventData data)
        {
            SendValueToControl(0.0f);
        }
        
        public void OnPointerDown(PointerEventData data)
        {
            SendValueToControl(1.0f);
        }
        
        
        
        
    }
}
