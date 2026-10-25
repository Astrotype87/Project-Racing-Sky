using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectRacingSky.Editor
{
    using Camera;
    
    [CustomEditor(typeof(CameraFollow))]
    public class CameraFollowEditor : UnityEditor.Editor
    {
        public static bool allowCameraProfileLoad = false;
        
        public override void OnInspectorGUI()
        {
            CameraFollow camera = (CameraFollow) target;
            
            if (!GUILayout.Button("Load Camera Profile") || !EditorUtility.DisplayDialog(
                "Load Camera Profile (Warning you for some hassle)",
                "Make sure you save your scene first BEFORE loading."
                    + "\nIt is NOT possible to UNDO changes from loading profiles."
                    + "\n\nPlease DO NOT rush upon saving your scene after loading profile, unless you like to apply changes permanently."
                    + "\n\nAre you sure do you want to load the Camera profile?",
                "Load Camera Profile", "Cancel"))
            {
                base.OnInspectorGUI();
                return;
            }
            
            if (camera.LoadCameraProfileInEditor(allowCameraProfileLoad))
            {
                base.OnInspectorGUI();
                return;
            }
            
            if (EditorUtility.DisplayDialog(
                "Not Allowed To Load Camera Profile",
                "You have been prevented from loading the camera profile."
                + "\n\nThis is just a protective measure to prevent unwanted permanent changes to your scene."
                + "\n\nTo allow camera profile loading, open the CameraFollowEditor script and set allowCameraProfileLoad to true.",
                "Open Script", "Okay"))
            {
                #if UNITY_EDITOR
                
                string[] guids = AssetDatabase.FindAssets(nameof(CameraFollowEditor) + " t:Script");
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Object scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
                AssetDatabase.OpenAsset(scriptAsset, 13);
                
                #endif
            }
            
            base.OnInspectorGUI();
        }
    }
}
