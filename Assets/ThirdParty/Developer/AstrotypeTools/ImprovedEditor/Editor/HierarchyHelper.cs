// using System;
// using UnityEditor;
// using UnityEngine;

// namespace AstrotypeTools.ImprovedEditor
// {
//     public class HierarchyHelper : MonoBehaviour
//     {
//         /// <summary>
//         /// CTRL + E to enable / disable a gameobject
//         /// </summary>
//         // [MenuItem ("Tools/Hierarchy/Activate-deactivate objects %e")]
//         // public static void DoDeactivate()
//         // {
//         //     foreach (GameObject go in Selection.gameObjects)
//         //     {
//         //         go.SetActive(!go.activeSelf);
//         //     }
//         // }

//         /// <summary>
//         /// ALT + left to move a gameobject to its parent level
//         /// </summary>
//         [MenuItem ("Tools/Hierarchy/Move to parent level &LEFT")]
//         public static void MoveAsParent()
//         {
//             var selection = Selection.activeGameObject;
//             if(selection == null)
//                 return;

//             if (selection.transform.parent == null)
//                 return;

//             var parentIndex = selection.transform.parent.GetSiblingIndex();

//             selection.transform.parent = selection.transform.parent.parent;
//             selection.transform.SetSiblingIndex(parentIndex + 1);
//         }

//         /// <summary>
//         /// ALT + right to move a gameobject in its direct neighbor scope (as child)
//         /// </summary>
//         [MenuItem ("Tools/Hierarchy/Move to child level &RIGHT")]
//         public static void MoveAsChild()
//         {
//             var selection = Selection.activeGameObject;
//             if(selection == null)
//                 return;

//             var index = selection.transform.GetSiblingIndex();
//             if(index == 0)
//                 return;

//             if(selection.transform.parent == null)
//                 return;

//             var previous = selection.transform.parent.GetChild(index - 1);
//             if(previous == null)
//                 return;

//             selection.transform.parent = previous.transform;
//         }

//         /// <summary>
//         /// ALT + up to move a gameobject in up in the hierarchy (stay on the same level, only the order changes)
//         /// Disclaimer: still some issues with gameobjects at the root level
//         /// </summary>
//         [MenuItem ("Tools/Hierarchy/Move up &UP")]
//         public static void MoveUp()
//         {
//             var selection = Selection.activeGameObject;
//             if(selection == null)
//                 return;

//             var index = selection.transform.GetSiblingIndex();
//             if(index == 0)
//                 return;

//             try
//             {
//                 selection.transform.SetSiblingIndex(index - 1);
//             }
//             catch (Exception)
//             {
//             }
//         }


//         /// <summary>
//         /// ALT + down to move a gameobject in down in the hierarchy (stay on the same level, only the order changes)
//         /// Disclaimer: still some issues with gameobjects at the root level
//         /// </summary>
//         [MenuItem ("Tools/Hierarchy/Move down &DOWN")]
//         public static void MoveDown()
//         {
//             var selection = Selection.activeGameObject;
//             if(selection == null)
//                 return;

//             var index = selection.transform.GetSiblingIndex();
//             try
//             {
//                 selection.transform.SetSiblingIndex(index + 1);
//             }
//             catch (Exception)
//             {
//             }
//         }
//     }
// }
