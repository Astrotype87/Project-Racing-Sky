using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    /// <summary>
    /// Field must reference a Unity tag.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TagAttribute : PropertyAttribute
    {
        public TagAttribute() { }
    }
}
