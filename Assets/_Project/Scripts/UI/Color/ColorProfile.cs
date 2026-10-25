using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky
{
    [CreateAssetMenu(menuName = "Project Racing Sky/UI/Color Profile", fileName = "Color")]
    public class ColorProfile : ScriptableObject
    {
        public Color color;
        [Range(0, 360)] public int hue;
        [Range(0, 100)] public int saturation;
        [Range(0, 100)] public int value;
        [Range(0, 100)] public int alpha;
        
        
        public string hex;
        
        
        
    }
}
