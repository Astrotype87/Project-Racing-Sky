using UnityEngine;

namespace ProjectRacingSky.Utility
{
    /// <summary>
    /// Utility extension methods for determining whether a GameObject
    /// is a scene instance or a prefab asset.
    /// </summary>
    /// <remarks>
    /// These checks are primarily intended for editor usage.
    /// At runtime, prefab assets do not exist and all GameObjects
    /// belong to a valid scene.
    /// </remarks>
    public static class GameObjectUtil
    {
        /// <summary>
        /// Determines whether the GameObject exists in a valid scene.
        /// </summary>
        /// <param name="go">The GameObject to check.</param>
        /// <returns>
        /// True if the GameObject is part of a scene; false if it is a prefab asset
        /// or otherwise not associated with a valid scene.
        /// </returns>
        public static bool IsSceneObject(this GameObject go)
        {
            return go.scene.IsValid();
        }
        
        /// <summary>
        /// Determines whether the GameObject is a prefab asset.
        /// </summary>
        /// <param name="go">The GameObject to check.</param>
        /// <returns>
        /// True if the GameObject represents a prefab asset; false if it is a scene instance.
        /// </returns>
        /// <remarks>
        /// This method relies on <see cref="PrefabUtility"/> and is only valid in the editor.
        /// Prefab assets are not associated with a valid scene.
        /// </remarks>
        public static bool IsPrefabAsset(this GameObject go)
        {
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.GetPrefabAssetType(go) != UnityEditor.PrefabAssetType.NotAPrefab
                && !go.scene.IsValid();
#else
            return true;
#endif
        }
    }
}
