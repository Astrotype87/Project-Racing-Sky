using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    public enum Icon
    {
        _,
        GameObject,
        Transform,
        RectTransform
    }
    
    public static class IconDictionary
    {
        public static readonly Dictionary<Icon, string> iconDictionary = new()
        {
            { Icon._, "" },
            { Icon.GameObject, "gameobject icon" },
            { Icon.Transform, "transform icon" },
            { Icon.RectTransform, "recttransform icon"}
        };
        
        public static string IconPath(Icon icon)
        {
            iconDictionary.TryGetValue(icon, out string path);
            return path;
        }
    }
}