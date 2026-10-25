using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    [DisallowMultipleComponent]
    public abstract class UIEventTrigger : MonoBehaviour
    {
        [SerializeField] private UIButton button; // IUIClickable
        
        // MONOBEHAVIOUR METHODS
        private void Reset() => button = GetComponent<UIButton>();
        
        private void Awake() => button.OnClicked += OnTriggerEvent;
        
        private void OnDisable() => button.OnClicked -= OnTriggerEvent;
        
        // ABSTRACT METHODS
        protected abstract void OnTriggerEvent();
    }
}
