using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(SceneOnlyAttribute))]
    public class SceneOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isInvalid = property.propertyType == SerializedPropertyType.ObjectReference
                && property.objectReferenceValue != null
                && EditorUtility.IsPersistent(property.objectReferenceValue);
            
            Color oldColor = GUI.color;
            
            if (isInvalid) GUI.color = Color.yellow;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.color = oldColor;
            
            if (isInvalid)
            {
                Rect helpBoxRect = position;
                helpBoxRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                helpBoxRect.height = EditorGUIUtility.singleLineHeight * 2f;
                
                EditorGUI.HelpBox(helpBoxRect, $"{label.text} must reference a scene object, not an asset!", MessageType.Warning);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            
            if (property.propertyType == SerializedPropertyType.ObjectReference
                && property.objectReferenceValue != null
                && EditorUtility.IsPersistent(property.objectReferenceValue))
            {
                height += EditorGUIUtility.singleLineHeight * 2f;
            }
            
            return height;
        }
    }
}