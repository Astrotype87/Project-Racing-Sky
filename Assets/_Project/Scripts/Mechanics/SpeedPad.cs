using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.Mechanics.Pads
{
    using Ship;
    
    public class SpeedPad : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Ship ship = other.transform.GetComponentInParent<Ship>();
            
            if (ship)
                ship.TrackInput.OnSpeedPad?.Invoke(transform.forward);
        }
    }
}
