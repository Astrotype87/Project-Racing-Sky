using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.Mechanics.Pads
{
    using Ship;
    
    public class JumpPad : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Ship ship = other.transform.GetComponentInParent<Ship>();
            
            if (ship)
                ship.TrackInput.OnJumpPad?.Invoke(transform.up);
        }
    }
}
