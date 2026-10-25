using System;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class TouchDpad : MonoBehaviour
    {
        // EVENTS
        public Action<Vector2> OnButtonUpdate {get; set;}
    }
    
}
