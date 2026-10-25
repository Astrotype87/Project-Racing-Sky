using System;
using UnityEngine;

namespace AstrotypeTools.ClassSelector
{
    /// <summary>
    /// Adds a dropdown generic menu for selecting and assigning a concrete class
    /// to fields marked with <c><see cref="SerializeReference"/></c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ClassSelectorAttribute : PropertyAttribute
    {
        public readonly bool folded = false;
        
        public ClassSelectorAttribute() { }
        
        public ClassSelectorAttribute(bool folded)
        {
            this.folded = folded;
        }
    }
}
