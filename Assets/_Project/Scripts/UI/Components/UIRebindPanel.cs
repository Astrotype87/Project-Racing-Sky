using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectRacingSky.UI
{
    public class UIRebindPanel : MonoBehaviour
    {
        // INSPECTOR FIELDS // TODO: Use UIButton
        [SerializeField] private GameObject rebindContainer;
        [SerializeField] private Button loadButton; 
        [SerializeField] private Button saveButton;
        [SerializeField] private Button closeButton;
        
        // EVENTS
        public event Action<RebindOperation, UIRebindInput> OnPerformRebind;
        public event Action OnLoadBindings;
        public event Action OnSaveBindings;
        
        // PRIVATE FIELDS
        [SerializeField] private UIRebindInput[] rebindControls;
        private Canvas canvas;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            rebindControls = rebindContainer.GetComponentsInChildren<UIRebindInput>();
            canvas = GetComponent<Canvas>();
            
            SubscribeToRebindControls();
            SubscribeButtons();
        }
        
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                canvas.enabled = !canvas.enabled;
            }
        }
        
        
        // PUBLIC METHODS
        public void RefreshUI()
        {
            if (rebindControls == null) return;
            for (int i = 0; i < rebindControls.Length; i++)
            {
                rebindControls[i].RefreshUI();
            }
        }
        
        
        // PRIVATE METHODS
        private void SubscribeToRebindControls()
        {
            if (rebindControls == null) return;
            for (int i = 0; i < rebindControls.Length; i++)
            {
                rebindControls[i].OnPerformRebind += InvokePerformRebind;
            }
        }
        
        private void SubscribeButtons()
        {
            if (loadButton) loadButton.onClick.AddListener(InvokeLoadBindings);
            if (saveButton) saveButton.onClick.AddListener(InvokeSaveBindings);
            if (closeButton) closeButton.onClick.AddListener(InvokeClose);
        }
        
        
        // INVOKE METHODS
        private void InvokePerformRebind(RebindOperation operation, UIRebindInput control)
        {
            OnPerformRebind?.Invoke(operation, control);
        }
        
        private void InvokeLoadBindings() => OnLoadBindings.Invoke();
        
        private void InvokeSaveBindings() => OnSaveBindings.Invoke();
        
        private void InvokeClose() => canvas.enabled = false;
        
        
    }
    
}
