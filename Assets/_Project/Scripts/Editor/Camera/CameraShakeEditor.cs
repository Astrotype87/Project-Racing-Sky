using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectRacingSky.Editor
{
    using Camera;
    
    [CustomEditor(typeof(CameraShake))]
    public class CameraShakeEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            EditorApplication.update += UpdateInspector;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= UpdateInspector;
        }
        
        private void UpdateInspector()
        {
            CameraShake cameraShake = (CameraShake)target;
            if (cameraShake.alwaysRefreshInspector)
                Repaint();
        }
    }
}
