using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    using Editor = UnityEditor.Editor;
    
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorDrawer: PropertyDrawer
    {
        private const float IndentSize = 15f;
        
        private Editor editor = null;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the property field (object reference)
            position.height = EditorGUI.GetPropertyHeight(property, label, true);
            EditorGUI.PropertyField(position, property, label, true);
            
            if (property.objectReferenceValue == null)
                return;
            
            // Draw foldout toggle
            Rect foldoutRect = position;
            foldoutRect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
            
            if (!property.isExpanded)
                return;
            
            // Create cached editor
            if (editor == null || editor.target != property.objectReferenceValue)
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            
            if (editor == null)
                return;
            
            EditorGUI.indentLevel++;
            
            // Create rect for the inline inspector
            float boxX = position.x + (EditorGUI.indentLevel - 1) * IndentSize;
            float boxY = position.y + position.height + EditorGUIUtility.standardVerticalSpacing;
            Rect boxRect = new(boxX, boxY, position.width, GetEditorHeight(editor));
            GUI.Box(boxRect, GUIContent.none); // Optional background box
            
            // Begin a child GUI area to contain the editor GUI
            Rect groupRect = new(position.x, boxY, position.width, GetEditorHeight(editor));
            GUI.BeginGroup(groupRect);
            
            // Shift content inside box
            var innerRect = new Rect(0, 0, groupRect.width, groupRect.height);
            
            // Draw properties using serialized object
            SerializedObject serializedEditor = editor.serializedObject;
            serializedEditor.Update();
            
            SerializedProperty iterator = serializedEditor.GetIterator();
            iterator.NextVisible(true); // Skip "m_Script"
            
            float y = EditorGUIUtility.standardVerticalSpacing;
            while (iterator.NextVisible(false))
            {
                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                Rect propRect = new(0, y, innerRect.width, propHeight);
                EditorGUI.PropertyField(propRect, iterator, true);
                y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            
            serializedEditor.ApplyModifiedProperties();
            
            GUI.EndGroup();
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUI.GetPropertyHeight(property, label, true);
            
            if (property.isExpanded && property.objectReferenceValue != null)
            {
                if (editor == null || editor.target != property.objectReferenceValue)
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
                
                if (editor != null)
                {
                    totalHeight += EditorGUIUtility.standardVerticalSpacing + GetEditorHeight(editor);
                }
            }
            
            return totalHeight;
        }

        
        private float GetEditorHeight(Editor editor)
        {
            float height = 0f;
            SerializedObject serializedEditor = editor.serializedObject;
            
            SerializedProperty iterator = serializedEditor.GetIterator();
            iterator.NextVisible(true); // Skip "m_Script"
            
            while (iterator.NextVisible(false))
            {
                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            }
            
            return height + EditorGUIUtility.standardVerticalSpacing; // Padding for safety
        }
    }
}
