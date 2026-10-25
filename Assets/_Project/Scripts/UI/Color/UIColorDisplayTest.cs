using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI.ColorManagement
{
    public class UIColorDisplayTest : MonoBehaviour
    {
        [SerializeField] private ColorCustom color;
        
        private void OnValidate()
        {
            color.Update();
        }
        
    }
}
