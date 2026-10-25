using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.ClassSelector
{
    /// <summary>
    /// Custom property drawer for <c><see cref="ClassSelectorAttribute"/></c> that draws
    /// a dropdown generic menu without interfering with other property drawers.
    /// </summary>
    [CustomPropertyDrawer(typeof(ClassSelectorAttribute))]
    public class ClassSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                DrawDropdownSelector(position, property);
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                DrawWarning(position, property, label);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
        
        
        private void DrawDropdownSelector(Rect position, SerializedProperty property)
        {
            // Returns formatted information about a SerializeReference marked field
            static string GetClassInfo(SerializedProperty property)
            {
                Type fieldType = GetManagedReferenceFieldType(property);
                Type instanceType = GetManagedReferenceInstanceType(property);
                Type[] inheritanceChain = TypeUtility.GetInheritanceChain(instanceType, fieldType);
                
                string info = "";
                if (property.managedReferenceValue != null)
                {
                    string instanceTypeName = instanceType.PrettyName(TypeNaming.Scope.Namespace);
                    info += $"Full Name:\n{instanceTypeName}\n\n";
                    
                    var classNames = inheritanceChain.Select(t => t.PrettyName());
                    info += $"Inheritance:\n{string.Join(" -> ", classNames)}\n\n";
                }
                string fieldTypeName = fieldType.PrettyName(TypeNaming.Scope.Namespace);
                info += $"Base Type:\n{fieldTypeName}";
                
                return info;
            }
            
            // Get class name
            bool isInstanceNull = property.managedReferenceValue == null;
            string className = isInstanceNull ? "(null)" : property.managedReferenceValue.GetType().PrettyName();
            string classInfoTooltip = GetClassInfo(property);
            
            // Try to get icon
            Texture icon = null;
            float iconSpace = 0f;
            try {
                icon = EditorGUIUtility.IconContent("gameobject icon").image;
                if (icon != null) iconSpace = 14f;
            } catch { }
            
            // Configure content and style
            GUIContent content = new(className, icon, classInfoTooltip);
            GUIStyle style = new("DropDownButton");
            style.padding.left = 4;
            style.padding.right = 16;
            style.fontSize = 11; // 12
            style.alignment = TextAnchor.MiddleLeft;
            style.fixedHeight = EditorGUIUtility.singleLineHeight - 2f;
            
            
            // Calculate widths
            float contentWidth = style.CalcSize(new(className)).x + iconSpace;
            float propertyWidth = position.width - EditorGUIUtility.labelWidth;
            
            // Calculate rect
            float dropdownWidth = Mathf.Min(contentWidth + 2f, propertyWidth);
            float dropdownX = position.x + position.width - dropdownWidth;
            Rect dropdownRect = new(dropdownX, position.y + 1f, dropdownWidth, EditorGUIUtility.singleLineHeight);
            
            // Draw dropdown
            if (GUI.Button(dropdownRect, content, style))
            {
                DisplayClassSelectorGenericMenu(property);
            }
        }
        
        private void DrawWarning(Rect position, SerializedProperty property, GUIContent label)
        {
            const string MissingTooltip = "Add [SerializeReference] attribute to this field. [ClassSelector] attribute does not work without this attribute.";
            const string ObjectTooltip = "[ClassSelector] attribute does not allow primitive types (ex: int, string) and Unity structs (ex: Vector2, Color).";
            const string PrimitiveTooltip = "[ClassSelector] attribute does not allow UnityEngine.Object-derived types ex: (Component, MonoBehaviour, ScriptableObject).";
            
            GUIContent content = property.propertyType switch
            {
                SerializedPropertyType.Generic => new("Missing SerializeReference", MissingTooltip),
                SerializedPropertyType.ObjectReference => new("Unity objects not allowed", ObjectTooltip),
                _ => new("Primitive types not allowed", PrimitiveTooltip)
            };
            
            EditorGUI.LabelField(position, label, new GUIContent(content));
        }
        
        private void DisplayClassSelectorGenericMenu(SerializedProperty property)
        {
            // Get field type and instantiable assignable types
            Type fieldType = GetManagedReferenceFieldType(property);
            Type[] assignableTypes = TypeUtility.GetInstantiableTypesAssignableTo(fieldType);
            Type instanceType = GetManagedReferenceInstanceType(property);
            
            // Create generic menu with title and None option
            GenericMenu menu = new();
            bool isNull = property.managedReferenceValue == null;
            menu.AddDisabledItem(new GUIContent($"Select {fieldType.PrettyName()} Type"));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("None (null)"), isNull, () => InstantiateAndAssignToProperty(property, null));
            menu.AddSeparator("");
            
            // Add more options
            foreach (Type type in assignableTypes)
            {
                var classSelectorAttribute = attribute as ClassSelectorAttribute;
                string itemName = classSelectorAttribute.folded
                    ? type.PrettyName(TypeNaming.Scope.Namespace, '/')
                    : type.Name;
                
                bool isInstanceType = type == instanceType;
                GenericMenu.MenuFunction function = isInstanceType
                    ? () => { }
                    : () => InstantiateAndAssignToProperty(property, type);
                
                menu.AddItem(new GUIContent(itemName), isInstanceType, function);
            }
            if (assignableTypes.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No valid types found."));
            }
            
            // Display generic menu
            menu.ShowAsContext();
        }
        
        
        private static void InstantiateAndAssignToProperty(SerializedProperty property, Type type)
        {
            object instance = null;
            try
            {
                if (type != null) instance = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                const string Message = "[ClassSelector] Custom generic arguments are not supported at the moment.";
                const string Message2 = "This will be later supported with a generic argument selector window.";
                Debug.LogWarning($"{Message} {Message2}\n{e.Message}");
                return;
            }
            
            property.serializedObject.Update();
            property.managedReferenceValue = instance;
            property.serializedObject.ApplyModifiedProperties();
        }
        
        
        private static Type GetManagedReferenceFieldType(SerializedProperty property)
        {
            if (property == null) return null;
            if (property.propertyType != SerializedPropertyType.ManagedReference) return null;
            
            string fieldTypeName = property.managedReferenceFieldTypename;
            return GetManagedReferenceType(fieldTypeName);
        }
        
        private static Type GetManagedReferenceInstanceType(SerializedProperty property)
        {
            if (property == null) return null;
            if (property.propertyType != SerializedPropertyType.ManagedReference) return null;
            if (property.managedReferenceValue == null) return null;
            
            string instanceTypeName = property.managedReferenceFullTypename;
            return GetManagedReferenceType(instanceTypeName);
        }
        
        private static Type GetManagedReferenceType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            
            int firstSpace = typeName.IndexOf(' ');
            if (firstSpace < 0) return null;
            
            string outerAssembly = typeName[..firstSpace];
            string outerAndInner = typeName[(firstSpace + 1)..];
            
            // Replace '/' with '+' for nested classes
            outerAndInner = outerAndInner.Replace("/", "+");
            
            return TypeUtility.GetType(outerAssembly, outerAndInner);
        }
    }
}
