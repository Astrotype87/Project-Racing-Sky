using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectRacingSky.UI
{
    using System.Collections;

    public class UIHeader : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [SerializeField] private Components components;
        [SerializeField] private Properties properties;
        [SerializeField] private HeaderData headerData;
        
        // PRIVATE FIELDS
        private UIHeaderAnimator headerAnimator;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            GetAnimatorComponent();
        }
        
        private void OnValidate()
        {
            UpdateProperties();
        }
        
        
        // PUBLIC METHODS
        public IEnumerator OpenHeader()
        {
            gameObject.SetActive(true);
            
            headerAnimator.ForceChangeTitle(new());
            yield return headerAnimator.AnimateOpen();
            
            // headerAnimator.ForceChangeTitle(new());
            // HeaderData newHeaderData = new(properties.titleText, properties.pathText);
            // Coroutine animateHeader = StartCoroutine(newHeaderData);
            // Coroutine animateTitle = StartCoroutine(headerAnimator.AnimateChangeTitle(newHeaderData, false));
            // yield return animateHeader;
            // // yield return animateTitle;
        }
        
        public IEnumerator CloseHeader()
        {
            yield return headerAnimator.AnimateClose();
            
            Coroutine animateHeader = StartCoroutine(headerAnimator.AnimateClose());
            Coroutine animateTitle = StartCoroutine(headerAnimator.AnimateChangeTitle(new HeaderData(), false));
            
            yield return animateHeader;
            // yield return animateTitle;
            
            gameObject.SetActive(false);
        }
        
        public IEnumerator ChangeTitle(HeaderData headerData, bool isReverse)
        {
            yield return headerAnimator.AnimateChangeTitle(headerData, isReverse);
        }
        
        public void ForceOpenHeader()
        {
            components.canvasGroup.alpha = 1;
            ForceChangeTitle(new(properties.titleText, properties.pathText));
        }
        
        public void ForceCloseHeader()
        {
            components.canvasGroup.alpha = 0;
            ForceChangeTitle(new());
        }
        
        public void ForceChangeTitle(HeaderData headerData)
        {
            headerAnimator.ForceChangeTitle(headerData);
        }
        
        
        // AWAKE & ON VALIDATE METHODS
        private void GetAnimatorComponent()
        {
            if (TryGetComponent(out headerAnimator))
            {
                headerAnimator.InitializeAnimator(components, properties);
            }
        }
        
        private void UpdateProperties()
        {
            Components c = components;
            Properties p = properties;
            
            if (c.imageFill) c.imageFill.sprite = p.fillSprite;
            if (c.imageShadow) c.imageShadow.sprite = p.shadowSprite;
            if (c.imageIcon) c.imageIcon.sprite = p.iconSprite;
            if (c.textTitle) c.textTitle.text = p.titleText;
            if (c.textPath) c.textPath.text = p.pathText;
            
            if (c.imageFill) c.imageFill.color = p.fillColor;
            if (c.imageShadow) c.imageShadow.color = p.shadowColor;
            if (c.imageIcon) c.imageIcon.color = p.iconColor;
            if (c.textTitle) c.textTitle.color = p.titleColor;
            if (c.textPath) c.textPath.color = p.pathColor;
        }
        
        
        // STRUCTS
        [Serializable] public struct HeaderData
        {
            public string titleText;
            public string pathText;
            public Sprite iconSprite;
            
            public HeaderData(string titleText = "", string pathText = "", Sprite iconSprite = null)
            {
                this.titleText = titleText;
                this.pathText = pathText;
                this.iconSprite = iconSprite;
            }
        }
        
        // CLASSES
        [Serializable] public class Properties
        {
            public Color fillColor;
            public Color shadowColor;
            public Color titleColor;
            public Color pathColor;
            public Color iconColor;
            [Space]
            public Sprite fillSprite;
            public Sprite shadowSprite;
            public string titleText;
            public string pathText;
            public Sprite iconSprite;
        }
        
        [Serializable] public class Components
        {
            public RectTransform rectHeader;
            public RectTransform rectTitle;
            public CanvasGroup canvasGroup;
            public Image imageFill;
            public Image imageShadow;
            public Image imageIcon;
            public TMP_Text textTitle;
            public TMP_Text textPath;
        }
        
        
        
    }
    
}
