using System;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class TouchSlider : MonoBehaviour
    {
        // EVENTS
        public Action<float> OnButtonUpdate {get; set;}
    }
    
}
