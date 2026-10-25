using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

namespace ProjectRacingSky.Utility
{
    public static class UIUtil
    {
        public static void SetAlpha(this Image image, float a)
        {
            Color color = image.color;
            color.a = a;
            image.color = color;
        }
        
        public static double GetCanvasScaleFactor(CanvasScaler canvasScaler, bool logResolution = false)
        {
            Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
            Vector2 referenceResolution = canvasScaler.referenceResolution;
            
            if (logResolution) Debug.Log("screenResolution: " + screenResolution);
            if (logResolution) Debug.Log("referenceResolution: " + referenceResolution);
            
            if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                double widthScale = screenResolution.x / referenceResolution.x;
                double heightScale = screenResolution.y / referenceResolution.y;
                if (logResolution) Debug.Log("widthScale: " + widthScale + "   heightScale: " + heightScale);
                
                return MathUtil.Lerp(widthScale, heightScale, canvasScaler.matchWidthOrHeight);
            }
            else if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
            {
                return Math.Min(screenResolution.x / referenceResolution.x, screenResolution.y / referenceResolution.y);
            }
            else if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
            {
                return Math.Max(screenResolution.x / referenceResolution.x, screenResolution.y / referenceResolution.y);
            }
            return 1f;
        }
        
    }
    
    public static class RectUtil
    {
        /// <summary> Sets the X position of the RectTransform. </summary>
        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.x = x;
            rectTransform.anchoredPosition = anchoredPosition;
        }
        
        /// <summary> Sets the Y position of the RectTransform. </summary>
        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.y = y;
            rectTransform.anchoredPosition = anchoredPosition;
        }
        
        
        /// <summary> Sets the width of the RectTransform. </summary>
        public static void SetSizeDeltaX(this RectTransform rectTransform, float x)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.x = x;
            rectTransform.sizeDelta = sizeDelta;
        }
        
        /// <summary> Sets the height of the RectTransform. </summary>
        public static void SetSizeDeltaY(this RectTransform rectTransform, float y)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = y;
            rectTransform.sizeDelta = sizeDelta;
        }
        
        
        /// <summary> Sets the left side of the anchor of the RectTransform. </summary>
        public static void SetAnchorMinX(this RectTransform rectTransform, float x)
        {
            Vector2 anchorMin = rectTransform.anchorMin;
            anchorMin.x = x;
            rectTransform.anchorMin = anchorMin;
        }
        
        /// <summary> Sets the bottom side of the anchor of the RectTransform. </summary>
        public static void SetAnchorMinY(this RectTransform rectTransform, float y)
        {
            Vector2 anchorMin = rectTransform.anchorMin;
            anchorMin.y = y;
            rectTransform.anchorMin = anchorMin;
        }
        
        /// <summary> Sets the right side of the anchor of the RectTransform. </summary>
        public static void SetAnchorMaxX(this RectTransform rectTransform, float x)
        {
            Vector2 anchorMax = rectTransform.anchorMax;
            anchorMax.x = x;
            rectTransform.anchorMax = anchorMax;
        }
        
        /// <summary> Sets the top side of the anchor of the RectTransform. </summary>
        public static void SetAnchorMaxY(this RectTransform rectTransform, float y)
        {
            Vector2 anchorMax = rectTransform.anchorMax;
            anchorMax.y = y;
            rectTransform.anchorMax = anchorMax;
        }
        
        
    }
    
    
    
}
