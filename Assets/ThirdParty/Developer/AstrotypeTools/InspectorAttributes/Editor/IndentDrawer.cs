using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentDrawer : PropertyDrawer
    {
        public const float IndentWidth = 15f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IndentAttribute attr = attribute as IndentAttribute;
            
            if (attr.Remainder == 0.0f)
            {
                int originalIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = attr.Fixed ? attr.Indent : originalIndent + attr.Indent;
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.indentLevel = originalIndent;
            }
            else
            {
                int originalIndent = EditorGUI.indentLevel;
                float originalLabelWidth = EditorGUIUtility.labelWidth;
                
                Rect rect = position;
                rect.x += attr.Remainder * IndentWidth;
                rect.width -= attr.Remainder * IndentWidth;
                
                EditorGUI.indentLevel = attr.Fixed ? attr.Indent : originalIndent + attr.Indent;
                EditorGUIUtility.labelWidth -= attr.Remainder * IndentWidth;
                
                EditorGUI.PropertyField(rect, property, label, true);
                
                EditorGUI.indentLevel = originalIndent;
                EditorGUIUtility.labelWidth += originalLabelWidth;
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
