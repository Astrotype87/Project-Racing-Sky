using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AstrotypeTools.ImprovedEditor
{
    [InitializeOnLoad]
    public class HierarchyToggle
    {
        public static bool followIndent = true;
        
        private static GameObject pressedObject = null;
        private static GameObject hoveredSelectedObject = null;
        
        private const float LeftOffset = 32f;
        private const float FollowOffset = -28f;
        
        static HierarchyToggle()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
        
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            // Get the current game object
            GameObject hoveredObject = EditorUtility.EntityIdToObject(instanceID) as GameObject;
            if (hoveredObject == null) return;
            
            // Get the current rectangle width
            float hierarchyViewWidth = EditorGUIUtility.currentViewWidth - 2f;
            Rect fullRowRect = new(0, selectionRect.y, hierarchyViewWidth, selectionRect.height);
            
            Vector2 mousePosition = Event.current.mousePosition;
            bool isHovered = fullRowRect.Contains(mousePosition);
            bool isSelected = Selection.Contains(hoveredObject);
            
            // Check if hovered over one of multiple selected game objects
            if (isHovered && isSelected)
                hoveredSelectedObject = hoveredObject;
            else if (!isHovered && hoveredObject == hoveredSelectedObject)
                hoveredSelectedObject = null;
            
            bool showCheckbox = isHovered || (hoveredSelectedObject != null && isSelected);
            if (!showCheckbox) return;
            
            // Get checkbox rect
            float checkboxOffset = followIndent ? selectionRect.x + FollowOffset : LeftOffset;
            float checkboxSize = selectionRect.height;
            Rect checkboxRect = new(checkboxOffset, selectionRect.y - 0.5f, checkboxSize, checkboxSize);
            
            // Check mouse click events
            Event evt = Event.current;
            bool isMouseInsideCheckbox = checkboxRect.Contains(Event.current.mousePosition);
            const int LeftClick = 0;
            
            // Handle mouse events
            if (evt.button == LeftClick && isMouseInsideCheckbox)
            {
                if (evt.type == EventType.MouseDown)
                {
                    pressedObject = hoveredObject;
                    evt.Use();
                }
                else if (evt.type == EventType.MouseUp)
                {
                    HandleLeftMouseUp(hoveredObject, evt);
                    pressedObject = null;
                }
                else if (evt.type == EventType.MouseDrag)
                {
                    evt.Use();
                }
            }
            
            // Draw checkbox
            EditorGUI.Toggle(checkboxRect, hoveredObject.activeSelf);
        }
        
        private static void HandleLeftMouseUp(GameObject hoveredObject, Event evt)
        {
            if (hoveredObject == pressedObject)
            {
                bool isOneOrNoneSelected = Selection.gameObjects.Length <= 1;
                bool isSelected = Selection.gameObjects.Contains(hoveredObject);
                bool ctrl = evt.control;
                bool shift = evt.shift;
                bool alt = evt.alt;
                
                if (isOneOrNoneSelected || !isSelected) ToggleSelf(hoveredObject);
                else if (ctrl && !shift && !alt) ToggleSelf(hoveredObject);
                else ToggleAll(hoveredObject, ctrl, shift, alt);
                
                EditorApplication.RepaintHierarchyWindow();
                evt.Use();
            }
        }
        
        private static void ToggleSelf(GameObject hoveredObject)
        {
            if (hoveredObject == null) return;
            
            bool setActive = !hoveredObject.activeSelf;
            Undo.RecordObject(hoveredObject, $"Modified Is Active in {hoveredObject.name}");
            hoveredObject.SetActive(setActive);
        }
        
        private static void ToggleAll(GameObject hoveredObject, bool toggleSelf, bool toggleOther, bool flipState)
        {
            // Get target game objects
            GameObject[] targetObjects = Selection.Contains(hoveredObject)
                ? Selection.gameObjects
                : new[] { hoveredObject };
            
            // Check mixed active states
            int activeObjectCount = 0;
            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] != null && targetObjects[i].activeSelf)
                    activeObjectCount++;
            }
            
            // Update game object active states
            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] == null) continue;
                
                if (!toggleSelf && !toggleOther && !flipState)
                {
                    bool isNotAllActive = activeObjectCount < targetObjects.Length;
                    Undo.RecordObject(targetObjects[i], $"Modified Is Active in {targetObjects.Length} objects");
                    targetObjects[i].SetActive(isNotAllActive);
                }
                else if (targetObjects[i] == hoveredObject)
                {
                    if (!toggleSelf && toggleOther && !flipState) continue;
                    
                    bool isActive;
                    if (flipState) isActive = !targetObjects[i].activeSelf;
                    else if (toggleOther) isActive = !targetObjects[i].activeSelf;
                    else isActive = activeObjectCount < targetObjects.Length - (targetObjects[i].activeSelf ? 1 : 0);
                    
                    Undo.RecordObject(targetObjects[i], $"Modified Is Active in {targetObjects.Length} objects");
                    targetObjects[i].SetActive(isActive);
                }
                else
                {
                    if (!toggleOther && toggleSelf && !flipState) continue;
                    
                    bool isActive;
                    if (flipState) isActive = !targetObjects[i].activeSelf;
                    else if (toggleSelf) isActive = activeObjectCount < targetObjects.Length - 1;
                    else isActive = activeObjectCount < targetObjects.Length - (targetObjects[i].activeSelf ? 1 : 0);
                    
                    Undo.RecordObject(targetObjects[i], $"Modified Is Active in {targetObjects.Length} objects");
                    targetObjects[i].SetActive(isActive);
                }
            }
        }
    }
}