// using UnityEngine;
// using UnityEditor;

// namespace ProjectATLAS
// {
//     [InitializeOnLoad]
//     public static class CanvasFocusOnDoubleClick
//     {
//         private const float ZoomFactor = 0.6f;
//         private static double lastClickTime = 0;
//         private static int lastClickedID = 0;
//         private const double doubleClickTime = 0.5; // seconds
        
//         static CanvasFocusOnDoubleClick()
//         {
//             EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
//         }
        
//         private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
//         {
//             Event e = Event.current;
//             if (e.type == EventType.MouseDown && e.button == 0 && selectionRect.Contains(e.mousePosition))
//             {
//                 double time = EditorApplication.timeSinceStartup;
                
//                 if (lastClickedID == instanceID && (time - lastClickTime) < doubleClickTime)
//                 {
//                     GameObject go = EditorUtility.EntityIdToObject(instanceID) as GameObject;
//                     if (go != null)
//                     {
//                         if (!TryFocusCanvas(go))
//                         {
//                             // Fallback to default: frame selected object
//                             if (SceneView.lastActiveSceneView)
//                             {
//                                 SceneView.lastActiveSceneView.FrameSelected();
//                             }
//                         }
                        
//                         e.Use(); // Consume the double-click event
//                     }
//                 }
                
//                 lastClickTime = time;
//                 lastClickedID = instanceID;
//             }
//         }
        
//         private static bool TryFocusCanvas(GameObject selected)
//         {
//             Canvas canvas = selected.GetComponent<Canvas>();
//             if (canvas == null || canvas != canvas.rootCanvas)
//                 return false;
            
//             SceneView sceneView = SceneView.lastActiveSceneView;
//             if (sceneView == null) return false;
            
//             sceneView.in2DMode = true;
//             sceneView.orthographic = true;
            
//             Bounds bounds = CalculateBounds(canvas.gameObject);
//             bounds.size *= ZoomFactor;
            
//             sceneView.Frame(bounds, false);
//             return true;
//         }
        
//         private static Bounds CalculateBounds(GameObject root)
//         {
//             var renderers = root.GetComponentsInChildren<Renderer>();
//             var rectTransforms = root.GetComponentsInChildren<RectTransform>();
            
//             if (renderers.Length > 0)
//             {
//                 Bounds bounds = renderers[0].bounds;
//                 for (int i = 1; i < renderers.Length; i++)
//                     bounds.Encapsulate(renderers[i].bounds);
//                 return bounds;
//             }
//             else if (rectTransforms.Length > 0)
//             {
//                 Vector3 min = rectTransforms[0].position;
//                 Vector3 max = rectTransforms[0].position;
                
//                 foreach (var rt in rectTransforms)
//                 {
//                     Vector3[] corners = new Vector3[4];
//                     rt.GetWorldCorners(corners);
//                     foreach (var c in corners)
//                     {
//                         min = Vector3.Min(min, c);
//                         max = Vector3.Max(max, c);
//                     }
//                 }
                
//                 Bounds bounds = new Bounds();
//                 bounds.SetMinMax(min, max);
//                 return bounds;
//             }
            
//             return new Bounds(root.transform.position, Vector3.one);
//         }
//     }
// }
