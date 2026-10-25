using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class IndentAttribute : PropertyAttribute
    {
        public int Indent { get; private set; } = 0;
        public bool Fixed { get; private set; } = false;
        public float Remainder { get; private set; } = 0.0f;
        
        public IndentAttribute()
        {
            Indent = 1;
        }
        
        public IndentAttribute(int indent, bool applyToCollection = false) : base(applyToCollection)
        {
            Indent = indent;
        }
        
        public IndentAttribute(float indent, bool applyToCollection = false) : base(applyToCollection)
        {
            Indent = (indent >= 0) ? Mathf.FloorToInt(indent) : Mathf.CeilToInt(indent);
            Remainder = indent - Indent;
        }
        
        public IndentAttribute(bool absolute, int indent, bool applyToCollection = false) : base(applyToCollection)
        {
            Indent = indent;
            Fixed = absolute;
        }
        
        public IndentAttribute(bool absolute, float indent, bool applyToCollection = false) : base(applyToCollection)
        {
            Indent = (indent >= 0) ? Mathf.FloorToInt(indent) : Mathf.CeilToInt(indent);
            Remainder = indent - Indent;
            Fixed = absolute;
        }
    }
}
