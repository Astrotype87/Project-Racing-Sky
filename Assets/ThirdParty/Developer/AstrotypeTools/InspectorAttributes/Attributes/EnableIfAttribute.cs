using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnableIfAttribute : PropertyAttribute
    {
        public readonly string condition = null;
        
        public EnableIfAttribute(string condition)
        {
            this.condition = condition;
        }
    }
}
