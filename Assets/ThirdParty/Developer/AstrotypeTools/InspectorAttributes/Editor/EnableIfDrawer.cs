using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnableIfAttribute enableIfAttribute = (EnableIfAttribute)attribute;
            bool condition = GetConditionResult(property, enableIfAttribute.condition);
            
            bool previousGUIEnabled = GUI.enabled;
            
            GUI.enabled = condition;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = previousGUIEnabled;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        private bool GetConditionResult(SerializedProperty property, string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return true;
            
            // Get the target object that this property belongs to
            object target = property.serializedObject.targetObject;
            Type targetType = target.GetType();
            
            // Search for a field
            FieldInfo field = targetType.GetField(condition, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null && field.FieldType == typeof(bool))
            {
                return (bool)field.GetValue(target);
            }
            
            // Search for a property
            PropertyInfo prop = targetType.GetProperty(condition, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                return (bool)prop.GetValue(target);
            }
            
            // Search for a method
            MethodInfo method = targetType.GetMethod(condition, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
            {
                return (bool)method.Invoke(target, null);
            }
            
            return true;
        }
    }
}
