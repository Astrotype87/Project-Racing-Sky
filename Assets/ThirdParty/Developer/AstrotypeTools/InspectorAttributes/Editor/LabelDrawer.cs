using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute attr = attribute as LabelAttribute;
            
            string text = attr.text == null ? label.text : attr.text;
            GUIContent content;
            Texture icon = null;
            
            if (attr.icon != Icon._)
            {
                try {
                    string iconPath = IconDictionary.IconPath(attr.icon);
                    icon = EditorGUIUtility.IconContent(iconPath).image;
                }
                catch { }
            }
            content = icon == null ? new(text) : new(text, icon);
            
            if (property.propertyType != SerializedPropertyType.Generic
                && property.propertyType == SerializedPropertyType.ManagedReference
                    && property.managedReferenceValue == null
                )
            {
                GUIStyle originalStyle = new(EditorStyles.label);
                
                GUIStyle labelStyle = new(EditorStyles.label);
                labelStyle.fontStyle = attr.style;
                labelStyle.alignment = attr.align switch {
                    TextAlign.Center => TextAnchor.MiddleCenter,
                    TextAlign.Right => TextAnchor.MiddleRight,
                    _ => TextAnchor.MiddleLeft
                };
                
                float originalWidth = EditorGUIUtility.labelWidth;
                float contentWidth = labelStyle.CalcSize(content).x;
                float labelWidth = attr.autoWidth ? contentWidth + 2f : (attr.width >= 0) ? attr.width : originalWidth;
                
                EditorStyles.label.fontStyle = labelStyle.fontStyle;
                EditorStyles.label.alignment = labelStyle.alignment;
                EditorGUIUtility.labelWidth = labelWidth;
                
                EditorGUI.PropertyField(position, property, content, true);
                
                EditorStyles.label.fontStyle = originalStyle.fontStyle;
                EditorStyles.label.alignment = originalStyle.alignment;
                EditorGUIUtility.labelWidth = originalWidth;
            }
            else if (!property.isArray)
            {
                GUIStyle originalStyle = new(EditorStyles.foldout);
                
                GUIStyle labelStyle = new(EditorStyles.foldout);
                labelStyle.fontStyle = attr.style;
                labelStyle.alignment = attr.align switch {
                    TextAlign.Center => TextAnchor.MiddleCenter,
                    TextAlign.Right => TextAnchor.MiddleRight,
                    _ => TextAnchor.MiddleLeft
                };
                
                EditorStyles.foldout.fontStyle = labelStyle.fontStyle;
                EditorStyles.foldout.alignment = labelStyle.alignment;
                
                EditorGUI.PropertyField(position, property, content, true);
                
                EditorStyles.foldout.fontStyle = originalStyle.fontStyle;
                EditorStyles.foldout.alignment = originalStyle.alignment;
            }
            else
            {
                EditorGUI.PropertyField(position, property, content, true);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
