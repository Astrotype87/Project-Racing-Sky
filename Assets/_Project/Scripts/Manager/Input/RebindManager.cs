using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRacingSky.Input
{
    using ProjectRacingSky.Manager;
    using ProjectRacingSky.UI;
    
    public class RebindManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [SerializeField] private UIRebindPanel uIRebindPanel;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private DataManager dataManager;
        
        // EVENTS
        public event Action OnRebindComplete;
        public event Action OnRebindCancelled;
        // public event Action<InputAction, int> OnRebindStarted;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (!uIRebindPanel) return;
            
            uIRebindPanel.OnPerformRebind += PerformRebind;
            uIRebindPanel.OnLoadBindings += LoadBindings;
            uIRebindPanel.OnSaveBindings += SaveBindings;
            
            OnRebindComplete += () => uIRebindPanel.RefreshUI();
            OnRebindCancelled += () => uIRebindPanel.RefreshUI();
        }
        
        
        // INVOKE METHODS
        private void PerformRebind(RebindOperation rebindOperation, UIRebindInput rebindControl)
        {
            InputAction action = rebindControl.Action;
            int bindingIndex = rebindControl.BindingIndex;
            
            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Couldn't find action or binding");
                return;
            }
            
            switch (rebindOperation)
            {
                case RebindOperation.Rebind: StartRebind(action, bindingIndex, rebindControl); break;
                case RebindOperation.Clear: ClearBinding(action, bindingIndex); break;
                case RebindOperation.Reset: ResetBinding(action, bindingIndex); break;
            }
        }
        
        private void LoadBindings()
        {
            dataManager.LoadBindingData();
            Debug.Log("Keyboard bindings loaded!");
        }
        
        private void SaveBindings()
        {
            dataManager.SaveBindingData();
            Debug.Log("Keyboard bindings saved!");
        }
        
        
        // REBIND METHODS
        private void StartRebind(InputAction action, int bindingIndex, UIRebindInput rebindControl)
        {
            if (action.bindings[bindingIndex].isComposite)
            {
                int firstIndex = bindingIndex + 1;
                if (firstIndex < action.bindings.Count && action.bindings[firstIndex].isPartOfComposite)
                {
                    DoRebinding(action, firstIndex, rebindControl);
                }
            }
            else { DoRebinding(action, bindingIndex, rebindControl); }
        }
        
        private void DoRebinding(InputAction action, int bindingIndex, UIRebindInput rebindControl)
        {
            if (action == null || bindingIndex < 0) return;
            
            string rebindText = action.bindings[bindingIndex].isPartOfComposite
                ? "Press two keys" : "Press a key";
            rebindControl.SetRebindText(rebindText);
            
            action.Disable();
            var rebind = action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("<Mouse>")
                .WithCancelingThrough("<Keyboard>/escape");
            
            rebind.OnComplete(operation =>
            {
                action.Enable();
                operation.Dispose();
                
                if (action.bindings[bindingIndex].isPartOfComposite)
                {
                    int nextIndex = bindingIndex + 1;
                    if (nextIndex < action.bindings.Count && action.bindings[nextIndex].isPartOfComposite)
                        DoRebinding(action, nextIndex, rebindControl);
                }
                OnRebindComplete?.Invoke();
            });
            
            rebind.OnCancel(operation =>
            {
                action.Enable();
                operation.Dispose();
                
                OnRebindCancelled?.Invoke();
            });
            
            rebind.Start();
        }
        
        private void ClearBinding(InputAction action, int bindingIndex)
        {
            if (action.bindings[bindingIndex].isComposite)
            {
                for (int i = bindingIndex; i < action.bindings.Count; i++)
                {
                    if (action.bindings[i].isComposite) continue;
                    action.ApplyBindingOverride(i, "");
                }
            }
            else { action.ApplyBindingOverride(bindingIndex, ""); }
            
            OnRebindComplete?.Invoke();
        }
        
        private void ResetBinding(InputAction action, int bindingIndex)
        {
            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Couldn't find action or binding");
                return;
            }
            
            if (action.bindings[bindingIndex].isComposite)
            {
                for (int i = bindingIndex; i < action.bindings.Count; i++)
                {
                    if (action.bindings[i].isComposite) continue;
                    action.RemoveBindingOverride(i);
                }
            }
            else { action.RemoveBindingOverride(bindingIndex); }
            
            OnRebindComplete?.Invoke();
        }
        
        
    }
}
