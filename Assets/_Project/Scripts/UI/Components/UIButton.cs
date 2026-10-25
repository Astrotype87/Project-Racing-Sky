using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ProjectRacingSky.UI
{
    public class UIButton : Selectable,
        IInteractableHandler, ISelectableHandler, IConfirmableHandler
    {
        [SerializeField] private Components components;
        [SerializeField] private Properties properties;
        
        [Header("Debug")]
        [SerializeField] private bool enableLog;
        [SerializeField] private bool logInteractable;
        [SerializeField] private bool logSelectable;
        [SerializeField] private bool logConfirmable;
        [SerializeField] private bool logButton;
        
        // PROPERTIES
        public bool IsHold {get; set;}
        public bool IsSelected {get; set;}
        public bool IsDisabled {get; set;}
        public UIButtonAnimator ButtonAnimator => buttonAnimator;
        
        // EVENTS
        public event Action OnClicked;
        
        // PRIVATE VARIABLES
        private UIButtonAnimator buttonAnimator;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            base.Awake();
            GetAnimatorComponent();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (!Application.isPlaying) return;
            HandleButtonNormal();
        }
        
        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateProperties();
        }
        #endif
        
        // POINTER METHODS
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (logInteractable) LogButton("OnPointerDown");
            
            base.OnPointerDown(eventData);
            IsHold = true;
            
            HandleButtonPress();
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (logInteractable) LogButton("OnPointerUp");
            
            base.OnPointerUp(eventData);
            IsHold = false;
            
            if (IsSelected)
                HandleButtonSelect();
            else
                HandleButtonNormal();
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (logInteractable) LogButton("OnPointerEnter");
            
            base.OnPointerEnter(eventData);
            eventData.selectedObject = this.gameObject;
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (logInteractable) LogButton("OnPointerExit");
            
            base.OnPointerExit(eventData);
            if (EventSystem.current)
                EventSystem.current.SetSelectedGameObject(null);
        }
        
        public override void OnSelect(BaseEventData eventData)
        {
            if (logSelectable) LogButton("OnSelect");
            
            base.OnSelect(eventData);
            IsSelected = true;
            
            HandleButtonSelect();
        }
        
        public override void OnDeselect(BaseEventData eventData)
        {
            if (logSelectable) LogButton("OnDeselect");
            
            base.OnDeselect(eventData);
            IsSelected = false;
            
            HandleButtonNormal();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (logConfirmable) LogButton("OnPointerClick");
            
            StartCoroutine(HandleButtonClick());
        }
        
        public void OnSubmit(BaseEventData eventData)
        {
            if (logConfirmable) LogButton("OnSubmit");
            
            StartCoroutine(HandleButtonClick());
        }
        
        
        // HANDLE METHODS
        public void HandleButtonNormal()
        {
            if (logButton) LogButton("OnButtonNormal");
            buttonAnimator.AnimateNormal();
        }
        
        private void HandleButtonSelect()
        {
            if (logButton) LogButton("OnButtonSelect");
            buttonAnimator.AnimateSelect();
        }
        
        private void HandleButtonPress()
        {
            if (logButton) LogButton("OnButtonPress");
            buttonAnimator.AnimatePress();
        }
        
        private void HandleButtonDisable()
        {
            if (logButton) LogButton("OnButtonDisable");
            buttonAnimator.AnimateDisable();
        }
        
        private IEnumerator HandleButtonClick()
        {
            if (logButton) LogButton("> > > OnButtonClick! < < <");
            
            if (buttonAnimator.IsAnimatingClick) yield break;
            
            yield return buttonAnimator.AnimateClick();
            HandleButtonNormal();
            OnClicked?.Invoke();
        }
        
        
        // AWAKE & ON VALIDATE METHODS
        private void GetAnimatorComponent()
        {
            if (TryGetComponent(out buttonAnimator))
            {
                buttonAnimator.InitializeAnimator(components, properties);
            }
        }
        
        private void UpdateProperties()
        {
            Components c = components;
            Properties p = properties;
            
            this.name = $"{p.text} Button";
            
            if (c.imageButton && p.buttonSprite) c.imageButton.sprite = p.buttonSprite;
            if (c.imageIcon && p.iconSprite) c.imageIcon.sprite = p.iconSprite;
            if (c.textLabel) c.textLabel.text = p.text;
            
            if (c.imageButton) c.imageButton.color = p.buttonColor.normal;
            if (c.imageIcon) c.imageIcon.color = p.iconColor.normal;
            if (c.textLabel) c.textLabel.color = p.textColor.normal;
        }
        
        private void LogButton(object message)
        {
            if (enableLog) Debug.Log($"[{name}] {message}");
        }
        
        
        // CLASSES
        [Serializable] public class Components
        {
            public RectTransform rectButton;
            public CanvasGroup groupButton;
            public Image imageButton;
            public Image imageIcon;
            public TMP_Text textLabel;
        }
        
        [Serializable] public class Properties
        {
            public string text;
            public Sprite buttonSprite;
            public Sprite iconSprite;
            public ColorState buttonColor;
            public ColorState iconColor;
            public ColorState textColor;
        }
        
        // STRUCTS
        [Serializable] public struct ColorState
        {
            public Color normal;
            public Color selected;
            public Color pressed;
            public Color disabled;
            
            public ColorState(Color normal, Color selected, Color pressed, Color disabled)
            {
                this.normal = normal;
                this.selected = selected;
                this.pressed = pressed;
                this.disabled = disabled;
            }
        }
        
        
        
    }
    
}
