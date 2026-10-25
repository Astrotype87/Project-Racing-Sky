using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using NaughtyAttributes;

namespace ProjectRacingSky.UI
{
    using ProjectRacingSky.Utility;
    
    public enum RebindOperation
    {
        Rebind, Clear, Reset
    }
    
    public class UIRebindInput : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Action")]
        [SerializeField] private InputActionReference inputActionReference;
        [SerializeField] [Range(0, 2)] int selectedBinding;
        
        [Header("Binding Info")]
        [SerializeField, ReadOnly] private string actionName;
        [SerializeField, ReadOnly] private int bindingIndex;
        [SerializeField, ReadOnly] private InputBinding inputBinding;
        
        // TODO: Use UIButton
        [Header("UI References")]
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private TMP_Text rebindText;
        [SerializeField] private Button rebindButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button resetButton;
        
        // PROPERTIES
        public InputAction Action => inputActionReference.action;
        public int BindingIndex => bindingIndex;
        
        // EVENTS
        public event Action<RebindOperation, UIRebindInput> OnPerformRebind;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            rebindButton.onClick.AddListener(PerformRebind);
            clearButton.onClick.AddListener(ClearBinding);
            resetButton.onClick.AddListener(ResetBinding);
            
            if (inputActionReference != null)
            {
                inputBinding = inputActionReference.action.bindings[selectedBinding];
                bindingIndex = selectedBinding;
            }
        }
        
        private void OnValidate()
        {
            GetBindingInfo();
            RefreshUI();
            RenameGameObject();
        }
        
        
        // PUBLIC METHODS
        public void SetRebindText(string text)
        {
            rebindText.text = text;
        }
        
        public void RefreshUI()
        {
            if (inputActionReference == null || inputActionReference.action == null)
            {
                if (actionText != null) actionText.text = "None";
                if (rebindText != null) rebindText.text = "";
                return;
            }
            
            if (actionText != null) actionText.text = actionName.InsertSpaceBeforeUpperCase();
            if (rebindText != null) rebindText.text = inputActionReference.action.GetBindingDisplayString();
        }
        
        
        // PRIVATE METHODS
        private void GetBindingInfo()
        {
            if (inputActionReference == null || inputActionReference.action == null)
            {
                this.actionName = "";
                this.bindingIndex = 0;
                this.inputBinding = default;
                return;
            }
            
            this.actionName = inputActionReference.action.name;
            
            if (inputActionReference.action.bindings.Count > selectedBinding)
            {
                this.bindingIndex = selectedBinding;
                this.inputBinding = inputActionReference.action.bindings[selectedBinding];
            }
        }
        
        private void RenameGameObject()
        {
            this.name = "Action";
            if (inputActionReference != null)
            {
                this.name += " " + actionName.InsertSpaceBeforeUpperCase();
            }
        }
        
        
        // REBIND METHODS
        private void PerformRebind() => OnPerformRebind?.Invoke(RebindOperation.Rebind, this);
        
        private void ClearBinding() => OnPerformRebind?.Invoke(RebindOperation.Clear, this);
        
        private void ResetBinding() => OnPerformRebind?.Invoke(RebindOperation.Reset, this);
        
        
        
    }
    
}
