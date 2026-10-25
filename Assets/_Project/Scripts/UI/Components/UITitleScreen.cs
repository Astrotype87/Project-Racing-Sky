using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ProjectRacingSky.UI
{
    public class UITitleScreen : Selectable, IConfirmableHandler
    {
        // INSPECTOR FIELDS
        [SerializeField] private Components components;
        [SerializeField] private Properties properties;
        
        // EVENTS
        public event Action OnClicked;
        
        // PRIVATE FIELDS
        private UITitleScreenAnimator titleScreenAnimator;
        
        
        // MONOBEHAVIOUR METHODS
        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            
            base.Awake();
            GetAnimatorComponent();
        }
        
        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateProperties();
        }
        #endif
        
        // PUBLIC METHODS
        public IEnumerator OpenTitleScreen()
        {
            gameObject.SetActive(true);
            yield return titleScreenAnimator.AnimateOpen();
        }
        
        public IEnumerator CloseTitleScreen()
        {
            yield return titleScreenAnimator.AnimateClose();
            gameObject.SetActive(false);
        }
        
        public void ForceOpenTitleScreen()
        {
            gameObject.SetActive(true);
            titleScreenAnimator.ForceOpen();
        }
        
        public void ForceCloseTitleScreen()
        {
            titleScreenAnimator.ForceClose();
            gameObject.SetActive(false);
        }
        
        
        // POINTER METHODS
        public void OnPointerClick(PointerEventData eventData)
        {
            HandleConfirm();
        }
        
        public void OnSubmit(BaseEventData eventData)
        {
            HandleConfirm();
        }
        
        
        // INVOKE METHODS
        private void HandleConfirm()
        {
            if (titleScreenAnimator.IsAnimating) return;
            OnClicked?.Invoke();
        }
        
        
        // AWAKE & ON VALIDATE METHODS
        private void GetAnimatorComponent()
        {
            if (TryGetComponent(out titleScreenAnimator))
            {
                titleScreenAnimator.InitializeAnimator(components, properties);
            }
        }
        
        private void UpdateProperties()
        {
            Components c = components;
            Properties p = properties;
            
            if (c.imageBackground) c.imageBackground.color = p.backgroundColor;
            if (c.imageLogo) c.imageLogo.color = p.logoColor;
            if (c.imageFill) c.imageFill.color = p.fillColor;
            if (c.imageShadow) c.imageShadow.color = p.shadowColor;
            if (c.textLabel) c.textLabel.color = p.labelColor;
            if (c.textLabel) c.textLabel.text = p.labelText;
        }
        
        
        // CLASSES
        [Serializable] public class Components
        {
            public RectTransform rectBar;
            public RectTransform rectText;
            public CanvasGroup groupLogo;
            public CanvasGroup groupBar;
            public Image imageBackground;
            public Image imageLogo;
            public Image imageFill;
            public Image imageShadow;
            public TMP_Text textLabel;
        }
        
        [Serializable] public class Properties
        {
            public Color backgroundColor;
            public Color logoColor;
            public Color fillColor;
            public Color shadowColor;
            public Color labelColor;
            public string labelText;
        }
        
        
        
    }
}
