using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Archived
{
    public class RaceHUDV1 : MonoBehaviour
    {
        public ShipPhysicsV2 shipPhysics;
        public TextMeshProUGUI speedText;
        
        private int speed;
        
        // Update is called once per frame
        void Update()
        {
            speed = (int) Mathf.Abs(shipPhysics.ShipSpeedKPH - 0.5f);
            speedText.text = speed.ToString();
        }
    }
    
}
