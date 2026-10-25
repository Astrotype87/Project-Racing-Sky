using System;
using UnityEngine;

namespace ProjectRacingSky.Track
{
    using Ship;
    
    public class RaceLine : MonoBehaviour
    {
        public event Action<Ship> OnRaceLineCrossed;
        
        private void OnTriggerEnter(Collider other)
        {
            ShipTrigger shipTrigger = other.transform.GetComponentInParent<ShipTrigger>();
            
            if (shipTrigger)
                OnRaceLineCrossed?.Invoke(shipTrigger.Ship);
        }
    }
}
