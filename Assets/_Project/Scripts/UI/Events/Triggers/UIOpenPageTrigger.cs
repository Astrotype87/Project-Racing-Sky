using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class UITriggerOpenPage : UIEventTrigger
    {
        [SerializeField] private UIPage page;
        
        // EVENTS
        public event Action<UIPage> OnTriggerOpenPage;
        
        // OVERRIDE METHODS
        protected override void OnTriggerEvent()
        {
            OnTriggerOpenPage?.Invoke(page);
        }
    }
}
