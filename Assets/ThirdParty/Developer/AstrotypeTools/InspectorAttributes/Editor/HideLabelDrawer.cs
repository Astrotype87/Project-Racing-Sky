using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(HideLabelAttribute))]
    public class HideLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Generic)
            {
                GUIContent content = GUIContent.none;
                EditorGUI.PropertyField(position, property, content, true);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
