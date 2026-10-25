using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectRacingSky.Editor
{
    using Utility;
    
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            var enabledProperty = property.FindPropertyRelative("enabled");
            
            position.width -= EditorGUI.indentLevel > 0 ? 0 : 20;
            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();
            
            // get checkbox position
            position.x += position.width + 2 + (EditorGUI.indentLevel > 0 ? 5 : 20);
            // width and height is set to checkbox height
            position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            // x position is back
            position.x -= position.width;
            // checkbox is drawn
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
        }
        
    }
    
}

