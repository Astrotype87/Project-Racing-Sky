using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectRacingSky.Editor
{
    using Ship;
    
    [CustomEditor(typeof(Ship))]
    public class ShipEditor : UnityEditor.Editor
    {
        public static bool allowShipProfileLoad = true;
        
        public override void OnInspectorGUI()
        {
            Ship ship = (Ship) target;
            
            if (!GUILayout.Button("Load Ship Profile") || !EditorUtility.DisplayDialog(
                "Load Ship Profile (Warning you for some hassle)",
                "Make sure you save your scene first BEFORE loading."
                    + "\nIt is NOT possible to UNDO changes from loading profiles."
                    + "\n\nPlease DO NOT rush upon saving your scene after loading profile, unless you like to apply changes permanently."
                    + "\n\nAre you sure do you want to load the Ship profile?",
                "Load Ship Profile", "Cancel"))
            {
                base.OnInspectorGUI();
                return;
            }
            
            if (ship.LoadShipProfileInEditor(allowShipProfileLoad))
            {
                base.OnInspectorGUI();
                return;
            }
            
            if (EditorUtility.DisplayDialog(
                "Not Allowed To Load Ship Profile",
                "You have been prevented from loading the camera profile."
                + "\n\nThis is just a protective measure to prevent unwanted permanent changes to your scene."
                + "\n\nTo allow ship profile loading, open the ShipEditor script and set allowShipProfileLoad to true.",
                "Open Script", "Okay"))
            {
                #if UNITY_EDITOR
                
                string[] guids = AssetDatabase.FindAssets(nameof(ShipEditor) + " t:Script");
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Object scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
                AssetDatabase.OpenAsset(scriptAsset, 13);
                
                #endif
            }
            
            base.OnInspectorGUI();
        }
        
        
        
    }
}
