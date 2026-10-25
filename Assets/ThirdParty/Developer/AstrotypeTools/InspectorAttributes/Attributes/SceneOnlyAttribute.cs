using System;
using UnityEngine;

namespace AstrotypeTools.InspectorAttributes
{
    /// <summary>
    /// Field must reference a scene object, not a project asset or prefab.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SceneOnlyAttribute : PropertyAttribute
    {
        public SceneOnlyAttribute() { }
    }
}