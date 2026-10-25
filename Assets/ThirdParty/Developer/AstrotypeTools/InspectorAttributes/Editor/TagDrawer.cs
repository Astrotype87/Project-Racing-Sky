using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AstrotypeTools.InspectorAttributes.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
                return EditorGUIUtility.singleLineHeight;
            
            if (property.isArray && property.arrayElementType == "string")
            {
                if (!property.isExpanded)
                    return EditorGUIUtility.singleLineHeight;
                
                return EditorGUIUtility.singleLineHeight * (property.arraySize + 2);
            }
            
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            if (property.propertyType == SerializedPropertyType.String)
                DrawSingleTag(position, property, label);
            // else if (property.isArray && property.arrayElementType == "string")
            //     DrawTagCollection(position, property, label);
            else
                EditorGUI.LabelField(position, label.text, "Use [Tag] with string, string[] or List<string>.");
            
            EditorGUI.EndProperty();
        }
        
        
        // PRIVATE METHODS
        private void DrawSingleTag(Rect position, SerializedProperty property, GUIContent label)
        {
            // Rect fieldRect = position;
            // fieldRect.width -= 70;
            
            // Rect buttonRect = position;
            // buttonRect.x += fieldRect.width + 5;
            // buttonRect.width = 65;
            
            EditorGUI.PrefixLabel(position, label);
            
            Rect dropdownRect = position;
            dropdownRect.x += EditorGUIUtility.labelWidth;
            dropdownRect.width -= EditorGUIUtility.labelWidth;
            
            string currentTag = property.stringValue;
            string display = string.IsNullOrEmpty(currentTag) ? "(None)" : currentTag;
            
            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(display), FocusType.Keyboard))
            {
                ShowTagMenu(currentTag, tag =>
                {
                    property.stringValue = tag;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            
            // if (!string.IsNullOrEmpty(currentTag) && !TagExists(currentTag))
            // {
            //     if (GUI.Button(position, "Add"))
            //         AddTag(currentTag);
            // }
        }
        
        
        
        private static void ShowTagMenu(string currentTag, Action<string> onSelect)
        {
            GenericMenu menu = new GenericMenu();
            
            foreach (string tag in InternalEditorUtility.tags)
            {
                if (tag == "Untagged")
                {
                    bool isTagEmpty = string.IsNullOrWhiteSpace(currentTag);
                    menu.AddItem(new GUIContent("(None)"), isTagEmpty, () => onSelect(""));
                }
                else
                {
                    bool isSelected = tag == currentTag;
                    menu.AddItem(new GUIContent(tag), isSelected, () => onSelect(tag));
                }
            }
            
            if (InternalEditorUtility.tags.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No tags defined"));
            }
            
            menu.ShowAsContext();
        }
        
        
        
        
        private static bool TagExists(string tag)
        {
            foreach (string t in InternalEditorUtility.tags)
            {
                if (t == tag)
                    return true;
            }
            return false;
        }
        
        private static void AddTag(string tag)
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                    return;
            }
            
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }
    }
}
