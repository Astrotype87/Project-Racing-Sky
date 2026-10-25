using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    /// <summary>
    /// Draws the inspector editor of a referenced UnityEngine.Object inside a foldout.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InlineEditorAttribute : PropertyAttribute
    {
        public InlineEditorAttribute() { }
    }
}
