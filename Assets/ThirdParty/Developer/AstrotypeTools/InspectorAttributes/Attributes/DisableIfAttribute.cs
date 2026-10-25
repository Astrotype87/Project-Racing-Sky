using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisableIfAttribute : PropertyAttribute
    {
        public readonly string condition = null;
        
        public DisableIfAttribute(string condition)
        {
            this.condition = condition;
        }
    }
}
