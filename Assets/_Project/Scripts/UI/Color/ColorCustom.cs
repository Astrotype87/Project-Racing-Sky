using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.UI.ColorManagement
{
    using Nested = AllowNestingAttribute;
    
    [Serializable]
    public struct ColorCustom
    {
        public enum ColorMode { Basic, Mod, Shift, Custom }
        
        public ColorMode mode;
        [Nested, ShowIf(nameof(isNotCustom))] public ColorProfile profile;
        [Nested, ShowIf(nameof(isNotCustom))] public Color original;
        [Nested, ShowIf(nameof(isModOrShift))] [Range(-180, 180)] public float hueShift;
        [Nested, ShowIf(nameof(isMod))] [Label("Shade / Tint")] [Range(-1, 1)] public float shadeTint;
        [Nested, ShowIf(nameof(isMod))] [Label("Tone / Hue")] [Range(-1, 1)] public float toneHue;
        [Nested, ShowIf(nameof(isShift))] [Range(-1, 1)] public float saturation;
        [Nested, ShowIf(nameof(isShift))] [Range(-1, 1)] public float value;
        public Color color;
        [Range(0, 1)] public float alpha;
        
        
        // PRIVATE FIELDS
        private bool isModOrShift => mode is ColorMode.Mod or ColorMode.Shift;
        private bool isMod => mode is ColorMode.Mod;
        private bool isShift => mode is ColorMode.Shift;
        private bool isNotCustom => mode is not ColorMode.Custom;
        
        
        
        public void Update()
        {
            original = profile == null ? Color.white : profile.color;
            color = original;
        }
        
        
        private void ApplyAlpha()
        {
            color.a = alpha;
        }
        
        private static Color ProcessColor(
            Color color, float hueShift, float saturationShift, float valueShift, float alpha)
        {
            return color;
        }
        
        
    }
}
