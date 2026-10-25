using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class UIEventBindings : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private OpenPageBinding[] openPageBindings;
        [SerializeField] private UIButton[] backBindings;
        
        [Header("Triggers (Component-Based)")]
        [SerializeField] private UITriggerOpenPage[] openPageTriggers;
        [SerializeField] private UITriggerBack[] backTriggers;
        
        
        // EVENTS
        public event Action<UIPage> OnOpenPage;
        public event Action OnBack;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            SubscribeToEventTriggers();
            SubscribeToEventBindings();
        }
        
        private void OnDestroy()
        {
            UnsubscribeToEventTriggers();
            UnsubscribeToEventBindings();
        }
        
        
        // PRIVATE METHODS : EventBindings
        private void SubscribeToEventBindings()
        {
            foreach (var binding in openPageBindings)
            {
                binding.button.OnClicked += () => OnOpenPage?.Invoke(binding.page);
            }
            foreach (var binding in backBindings)
            {
                binding.OnClicked += () => OnBack();
            }
        }
        
        private void UnsubscribeToEventBindings()
        {
            
        }
        
        // PRIVATE METHODS : EventTriggers
        private void SubscribeToEventTriggers()
        {
            foreach (var trigger in openPageTriggers)
            {
                trigger.OnTriggerOpenPage += OnOpenPage;
            }
            foreach (var trigger in backTriggers)
            {
                trigger.OnTriggerBack += OnBack;
            }
        }
        
        private void UnsubscribeToEventTriggers()
        {
            foreach (var trigger in openPageTriggers)
            {
                trigger.OnTriggerOpenPage -= OnOpenPage;
            }
            foreach (var trigger in backTriggers)
            {
                trigger.OnTriggerBack -= OnBack;
            }
        }
        
        
        
        
        
    }
}
