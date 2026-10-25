using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    /// <summary>
    /// Field must reference an asset from the Project window, not a scene object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AssetOnlyAttribute : PropertyAttribute
    {
        public AssetOnlyAttribute() { }
    }
}