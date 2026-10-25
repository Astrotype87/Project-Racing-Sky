using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideLabelAttribute : PropertyAttribute
    {
        public HideLabelAttribute() { }
    }
}