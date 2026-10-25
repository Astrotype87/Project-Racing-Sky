using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyAttribute() { }
    }
}
