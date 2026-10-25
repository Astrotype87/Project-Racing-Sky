using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LabelAttribute : PropertyAttribute
    {
        public readonly string text = null;
        public readonly Icon icon = default;
        public readonly FontStyle style = FontStyle.Normal;
        public readonly TextAlign align = TextAlign.Left;
        public readonly float width = -1f;
        public readonly bool autoWidth = false;
        
        public LabelAttribute(string text,
            Icon icon = default,
            FontStyle style = FontStyle.Normal,
            TextAlign align = TextAlign.Left,
            float width = -1f,
            bool autoWidth = false,
            bool applyToCollection = false) : base(applyToCollection)
        {
            this.text = text;
            this.icon = icon;
            this.style = style;
            this.align = align;
            this.width = width;
            this.autoWidth = autoWidth;
        }
        
        public LabelAttribute(Icon icon,
            FontStyle style = FontStyle.Normal,
            TextAlign align = TextAlign.Left,
            float width = -1f,
            bool autoWidth = false,
            bool applyToCollection = false) : base(applyToCollection)
        {
            this.icon = icon;
            this.style = style;
            this.align = align;
            this.width = width;
            this.autoWidth = autoWidth;
        }
        
        public LabelAttribute(FontStyle style,
            TextAlign align = TextAlign.Left,
            float width = -1f,
            bool autoWidth = false,
            bool applyToCollection = false) : base(applyToCollection)
        {
            this.style = style;
            this.align = align;
            this.width = width;
            this.autoWidth = autoWidth;
        }
        
        public LabelAttribute(TextAlign align = TextAlign.Left,
            float width = -1f,
            bool autoWidth = false,
            bool applyToCollection = false) : base(applyToCollection)
        {
            this.align = align;
            this.width = width;
            this.autoWidth = autoWidth;
        }
        
        public LabelAttribute(float width = -1f,
            bool autoWidth = false,
            bool applyToCollection = false) : base(applyToCollection)
        {
            this.width = width;
            this.autoWidth = autoWidth;
        }
        
        public LabelAttribute(bool autoWidth = false,
            bool applyToCollection = false) : base(applyToCollection)
        {
            this.autoWidth = autoWidth;
        }
        
        public LabelAttribute(bool applyToCollection = false) : base(applyToCollection) { }
    }
}