using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class UITriggerBack : UIEventTrigger
    {
        // EVENTS
        public event Action OnTriggerBack;
        
        // OVERRIDE METHODS
        protected override void OnTriggerEvent()
        {
            OnTriggerBack?.Invoke();
        }
    }
}
