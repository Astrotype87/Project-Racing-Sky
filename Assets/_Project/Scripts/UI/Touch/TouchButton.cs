using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectRacingSky.UI
{
    public class TouchButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        // INSPECTOR FIELDS
        [Header("Behaviour")]
        [SerializeField] private TouchMode touchMode = TouchMode.EnterAndHold;
        [SerializeField] private TouchButton holdOverride;
        [SerializeField] private bool holdAfterOverride = true;
        
        [Header("Appearance")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color pressedColor = new(0.875f, 0.875f, 0.875f, 1);
        
        [Header("Debug")]
        [SerializeField] private bool pointerLog;
        [SerializeField] private bool buttonLog;
        [SerializeField] private bool overrideLog;
        
        
        // PROPERTIES
        public bool isHold { get; set; }
        public bool isHover { get; set; }
        public bool isOverridingHold { get; set; }
        public bool isPressed { get; set; }
        
        // EVENTS
        public Action<float> OnButtonUpdate { get; set; }
        public Action OnButtonPressed { get; set; }
        
        
        // PRIVATE FIELDS
        private Image image;
        private TouchButton holdOverridingButton;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (!image) image = GetComponent<Image>();
            ChangeColor(normalColor);
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            if (!image) image = GetComponent<Image>();
            ChangeColor(normalColor);
        }
        
        
        // PUBLIC METHODS
        public void SetHoldOverridingButton(TouchButton button)
        {
            holdOverridingButton = button;
        }
        
        
        // BUTTON METHODS
        public void OnButtonPress()
        {
            ButtonLog("Button PRESSED");
            isPressed = true;
            
            ChangeColor(pressedColor);
            OnButtonUpdate?.Invoke(1);
            OnButtonPressed?.Invoke();
        }
        
        public void OnButtonRelease()
        {
            ButtonLog("Button RELEASED");
            isPressed = false;
            
            ChangeColor(normalColor);
            OnButtonUpdate?.Invoke(0);
        }
        
        public void OnButtonOverrideHold()
        {
            OverrideLog("OVERRIDING hold from " + holdOverride.gameObject.name);
            isOverridingHold = true;
            isHold = true;
            
            holdOverride.OnButtonRelease();
            holdOverride.SetHoldOverridingButton(this);
        }
        
        public void OnButtonReturnHold()
        {
            OverrideLog("RETURNING hold to " + holdOverride.gameObject.name);
            isOverridingHold = false;
            isHold = false;
            
            holdOverride.SetHoldOverridingButton(null);
            OnButtonRelease();
        }
        
        
        // POINTER METHODS
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerLog("OnPointerEnter()");
            if (isHover) return;
            isHover = true;
            
            if (holdOverride && eventData.pointerPress)
                if (holdOverride.gameObject == eventData.pointerPress)
                    if (holdOverride.isPressed)
                        OnButtonOverrideHold();
            
            if (holdOverridingButton)
                holdOverridingButton.OnButtonReturnHold();
            
            if (touchMode == TouchMode.Enter)
                OnButtonPress();
            else if (touchMode == TouchMode.EnterAndHold)
                if (!isHold || !isPressed)
                    OnButtonPress();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerLog("OnPointerDown()");
            if (isHold) return;
            isHold = true;
            
            if (!isPressed)
                OnButtonPress();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            PointerLog("OnPointerUp()");
            if (!isHold) return;
            isHold = false;
            
            if (holdOverridingButton)
                holdOverridingButton.OnButtonReturnHold();
            
            if (isPressed)
                OnButtonRelease();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            PointerLog("OnPointerExit()");
            if (!isHover) return;
            isHover = false;
            
            if (!(isOverridingHold && holdAfterOverride) && isPressed)
                if (touchMode != TouchMode.Hold)
                    if (touchMode != TouchMode.EnterAndHold)
                        OnButtonRelease();
                    else if (!isHold)
                        OnButtonRelease();
        }
        
        
        // CLASS METHODS
        public void ChangeColor(Color color)
        {
            image.color = color;
        }
        
        public void PointerLog(object message)
        {
            if (pointerLog) Debug.Log(gameObject.name + ": " + message);
        }
        
        public void ButtonLog(object message)
        {
            if (buttonLog) Debug.Log(gameObject.name + ": " + message);
        }
        
        public void OverrideLog(object message)
        {
            if (overrideLog) Debug.Log(gameObject.name + ": " + message);
        }
        
        
        
        // ENUMS, STRUCTS & CLASSES
        public enum TouchMode
        {
            Single, Enter, Hold, EnterAndHold
        }
        
    }
    
}
