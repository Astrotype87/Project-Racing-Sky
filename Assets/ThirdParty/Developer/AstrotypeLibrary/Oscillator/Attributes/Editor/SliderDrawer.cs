using UnityEngine;
using UnityEditor;

namespace AstrotypeLibrary.Oscillator
{
    [CustomPropertyDrawer(typeof(SliderAttribute))]
    public class SliderDrawer : PropertyDrawer
    {
        // DRAWER METHODS
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get slider settings from attribute
            SliderAttribute sliderAttr = (SliderAttribute)attribute;
            
            // Begin property
            EditorGUI.BeginProperty(position, label, property);
            
            // Standard values
            float labelWidth = EditorGUIUtility.labelWidth;
            float labelPadding = 2f;
            float labelClickBuffer = 4f;
            float fieldWidth = position.width - labelWidth - labelPadding;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            
            
            // Label/foldout rect buffer
            Rect labelRect = position;
            labelRect.width = labelWidth + labelClickBuffer;
            labelRect.height = lineHeight;
            
            // Draw label/foldout based on vector type
            if (property.propertyType
                is SerializedPropertyType.Vector2
                or SerializedPropertyType.Vector3
                or SerializedPropertyType.Vector4)
            {
                // Use full label width for Vector4
                if (property.propertyType is SerializedPropertyType.Vector4 && property.isExpanded && sliderAttr.Foldout)
                    labelRect.width = position.width;
                
                // Draw label or new foldout
                if (sliderAttr.Foldout)
                    property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
                else
                {
                    int id = GUIUtility.GetControlID(FocusType.Keyboard, labelRect);
                    EditorGUI.PrefixLabel(labelRect, id, label);
                }
            }
            else if (property.propertyType
                is not SerializedPropertyType.Float
                and not SerializedPropertyType.Integer)
            {
                labelRect.width = labelWidth + labelClickBuffer;
                EditorGUI.LabelField(labelRect, label, new GUIContent("Use Slider with float, int, or Vector2/3/4."));
            }
            
            
            // If float or integer, draw single slider
            if (property.propertyType is SerializedPropertyType.Float or SerializedPropertyType.Integer)
            {
                // Slider rect buffer
                Rect sliderRect = position;
                sliderRect.height = lineHeight;
                
                // Draw sliders based on vector type and update value
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    float value = property.floatValue;
                    value = EditorGUI.Slider(sliderRect, label, value, sliderAttr.MinX, sliderAttr.MaxX);
                    property.floatValue = value;
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    float value = property.intValue;
                    value = EditorGUI.Slider(sliderRect, label, value, sliderAttr.MinX, sliderAttr.MaxX);
                    property.intValue = (int)Snapping.Snap(value, 1f);
                }
            }
            // If Vector2/3/4, check foldout status
            else if (!property.isExpanded && sliderAttr.Foldout)
            {
                // If not expanded in foldout mode, draw original property field
                if (property.propertyType is SerializedPropertyType.Vector2 or SerializedPropertyType.Vector3)
                {
                    // Draw default vector field when closed
                    Rect fieldRect = position;
                    fieldRect.x += labelWidth + labelPadding;
                    fieldRect.width = fieldWidth;
                    fieldRect.height = lineHeight;
                    
                    // Draw original property field and clamp value when changed
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Clamp(property, sliderAttr);
                    }
                }
            }
            else if (property.propertyType
                is SerializedPropertyType.Vector2
                or SerializedPropertyType.Vector3
                or SerializedPropertyType.Vector4)
            {
                // Slider rect buffer
                Rect sliderRect = position;
                sliderRect.x += labelWidth + labelPadding;
                sliderRect.width = fieldWidth;
                sliderRect.height = lineHeight;
                
                // Override label width for slider labels
                float originalLabelWidth = EditorGUIUtility.labelWidth;
                const float sliderLabelWidth = 15f;
                EditorGUIUtility.labelWidth = sliderLabelWidth;
                
                // Draw sliders based on vector type and update value
                if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    Vector2 value = property.vector2Value;
                    
                    sliderRect.y = position.y;
                    value.x = EditorGUI.Slider(sliderRect, "X", value.x, sliderAttr.MinX, sliderAttr.MaxX);
                    sliderRect.y += lineHeight + verticalSpacing;
                    value.y = EditorGUI.Slider(sliderRect, "Y", value.y, sliderAttr.MinY, sliderAttr.MaxY);
                    
                    property.vector2Value = value;
                }
                else if (property.propertyType == SerializedPropertyType.Vector3)
                {
                    Vector3 value = property.vector3Value;
                    
                    sliderRect.y = position.y;
                    value.x = EditorGUI.Slider(sliderRect, "X", value.x, sliderAttr.MinX, sliderAttr.MaxX);
                    sliderRect.y += lineHeight + verticalSpacing;
                    value.y = EditorGUI.Slider(sliderRect, "Y", value.y, sliderAttr.MinY, sliderAttr.MaxY);
                    sliderRect.y += lineHeight + verticalSpacing;
                    value.z = EditorGUI.Slider(sliderRect, "Z", value.z, sliderAttr.MinZ, sliderAttr.MaxZ);
                    
                    property.vector3Value = value;
                }
                else if (property.propertyType == SerializedPropertyType.Vector4)
                {
                    Vector4 value = property.vector4Value;
                    
                    sliderRect.y = position.y;
                    value.x = EditorGUI.Slider(sliderRect, "X", value.x, sliderAttr.MinX, sliderAttr.MaxX);
                    sliderRect.y += lineHeight + verticalSpacing;
                    value.y = EditorGUI.Slider(sliderRect, "Y", value.y, sliderAttr.MinY, sliderAttr.MaxY);
                    sliderRect.y += lineHeight + verticalSpacing;
                    value.z = EditorGUI.Slider(sliderRect, "Z", value.z, sliderAttr.MinZ, sliderAttr.MaxZ);
                    sliderRect.y += lineHeight + verticalSpacing;
                    value.w = EditorGUI.Slider(sliderRect, "W", value.w, sliderAttr.MinW, sliderAttr.MaxW);
                    
                    property.vector4Value = value;
                }
                
                // Restore label width
                EditorGUIUtility.labelWidth = originalLabelWidth;
            }
            
            
            // End property
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Get slider settings from attribute
            SliderAttribute sliderAttribute = (SliderAttribute)attribute;
            
            // Standard values
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            
            // Height buffer
            float totalHeight = lineHeight;
            
            // If foldout is closed, only draw one line
            if (!property.isExpanded && sliderAttribute.Foldout)
                return totalHeight;
            
            // If foldout is open, draw sliders based on vector type
            if (property.propertyType == SerializedPropertyType.Vector2)
                totalHeight += verticalSpacing + lineHeight;
            else if (property.propertyType == SerializedPropertyType.Vector3)
                totalHeight += (verticalSpacing + lineHeight) * 2;
            else if (property.propertyType == SerializedPropertyType.Vector4)
                totalHeight += (verticalSpacing + lineHeight) * 3;
            
            // Return total height
            return totalHeight;
        }
        
        // PRIVATE METHODS
        private void Clamp(SerializedProperty property, SliderAttribute range)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                {
                    Vector2 v = property.vector2Value;
                    v.x = Mathf.Clamp(v.x, range.MinX, range.MaxX);
                    v.y = Mathf.Clamp(v.y, range.MinY, range.MaxY);
                    property.vector2Value = v;
                    break;
                }
                case SerializedPropertyType.Vector3:
                {
                    Vector3 v = property.vector3Value;
                    v.x = Mathf.Clamp(v.x, range.MinX, range.MaxX);
                    v.y = Mathf.Clamp(v.y, range.MinY, range.MaxY);
                    v.z = Mathf.Clamp(v.z, range.MinZ, range.MaxZ);
                    property.vector3Value = v;
                    break;
                }
                case SerializedPropertyType.Vector4:
                {
                    Vector4 v = property.vector4Value;
                    v.x = Mathf.Clamp(v.x, range.MinX, range.MaxX);
                    v.y = Mathf.Clamp(v.y, range.MinY, range.MaxY);
                    v.z = Mathf.Clamp(v.z, range.MinZ, range.MaxZ);
                    v.w = Mathf.Clamp(v.w, range.MinW, range.MaxW);
                    property.vector4Value = v;
                    break;
                }
            }
        }
    }
}
