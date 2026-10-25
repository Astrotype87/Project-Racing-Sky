using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    public class ShipTrigger : MonoBehaviour
    {
        [SerializeField] [Parent] private Ship ship;
        
        public Ship Ship => ship;
    }
}
