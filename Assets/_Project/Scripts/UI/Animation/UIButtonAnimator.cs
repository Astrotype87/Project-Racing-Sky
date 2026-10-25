using System;
using System.Collections;
using System.Collections.Generic;
using ProjectRacingSky.UI;
using UnityEngine;
using PrimeTween;

namespace ProjectRacingSky
{
    using ProjectRacingSky.Utility;
    using Properties = UIButton.Properties;
    using Components = UIButton.Components;
    
    public class UIButtonAnimator : UIAnimator
    {
        // INSPECTOR FIELDS
        [Header("Animation")]
        [SerializeField] private ButtonAnimation buttonAnimation;
        
        // PROPERTIES
        public bool IsAnimatingClick { get; private set; }
        
        // PRIVATE FIELDS
        private Properties properties;
        private Components components;
        private Vector2 basePosition;
        private float baseWidth;
        
        private Sequence animateOpen;
        private Sequence animateNormal;
        private Sequence animateSelect;
        private Sequence animateClick;
        
        
        // PUBLIC METHODS
        public void InitializeAnimator(Components components, Properties properties)
        {
            this.components = components;
            this.properties = properties;
            
            if (components.rectButton)
            {
                basePosition = components.rectButton.anchoredPosition;
                baseWidth = components.rectButton.sizeDelta.x;
            }
        }
        
        
        // ANIMATE METHODS
        public override IEnumerator AnimateOpen()
        {
            AnimateNormal();
            AnimateOpen(false, 0);
            yield break;
        }
        
        public override IEnumerator AnimateClose()
        {
            ForceClose();
            yield break;
        }
        
        public override void ForceOpen()
        {
            AnimateNormal();
            components.groupButton.alpha = 1;
            components.rectButton.SetAnchoredPositionX(basePosition.x);
        }
        
        public override void ForceClose()
        {
            components.groupButton.alpha = 0;
            components.rectButton.SetAnchoredPositionX(basePosition.x);
        }
        
        // CUSTOM ANIMATOR METHODS
        public void AnimateOpen(bool isPreviousPage, float delay)
        {
            // REFERENCES
            Components c = components;
            Properties p = properties;
            ButtonAnimation ba = buttonAnimation;
            float openOffset = ba.openOffset * (isPreviousPage ? -1 : 1);
            
            // INITIALIZE ANIMATION
            ForceClose();
            
            // PROCESS ANIMATION
            animateOpen = Sequence.Create()
                .Group(Tween.UIAnchoredPositionX(
                    c.rectButton, basePosition.x + openOffset, basePosition.x, ba.openTime, Ease.Linear,
                    startDelay: delay))
                .Group(Tween.Alpha(
                    c.groupButton, 0, 1, ba.openTime, Ease.Linear, startDelay: delay));
        }
         
        public void AnimateNormal()
        {
            if (IsAnimatingClick) return;
            animateNormal.Stop();
            animateSelect.Stop();
            
            Components c = components;
            Properties p = properties;
            ButtonAnimation ba = buttonAnimation;
            
            SetButtonColor(p.buttonColor.normal, p.iconColor.normal, p.textColor.normal);
            
            float currentWidth = c.rectButton.sizeDelta.x;
            animateNormal = Sequence.Create()
                .Chain(Tween.Custom(currentWidth, baseWidth, ba.resetTime, ease: ba.resetEase,
                    onValueChange: x => c.rectButton.SetSizeDeltaX(x)));
        }
        
        public void AnimateSelect()
        {
            if (IsAnimatingClick) return;
            animateNormal.Stop();
            animateSelect.Stop();
            
            Components c = components;
            Properties p = properties;
            ButtonAnimation ba = buttonAnimation;
            
            SetButtonColor(p.buttonColor.selected, p.iconColor.selected, p.textColor.selected);
            
            float currentWidth = c.rectButton.sizeDelta.x;
            animateSelect = Sequence.Create()
                .Chain(Tween.Custom(currentWidth, baseWidth + ba.expandWidth, ba.expandTime, ease: ba.expandEase,
                    onValueChange: x => c.rectButton.SetSizeDeltaX(x)));
        }
        
        public void AnimatePress()
        {
            
        }
        
        public void AnimateDisable()
        {
            Properties p = properties;
            SetButtonColor(p.buttonColor.disabled, p.iconColor.disabled, p.textColor.disabled);
        }
        
        public IEnumerator AnimateClick()
        {
            // REFERENCES
            Components c = components;
            Properties p = properties;
            ButtonAnimation ba = buttonAnimation;
            
            
            // PROCESS ANIMATION
            AnimateSelect();
            IsAnimatingClick = true;
            
            animateClick = Sequence.Create();
            for (int i = 0; i < ba.clickFlashCount; i++)
            {
                animateClick
                    .ChainCallback(() => {
                        c.textLabel.alpha = 0; c.imageIcon.SetAlpha(0);
                    })
                    .ChainDelay(ba.clickFlashTime)
                    .ChainCallback(() => {
                        c.textLabel.alpha = 1; c.imageIcon.SetAlpha(1);
                    })
                    .ChainDelay(ba.clickFlashTime);
            }
            animateClick
                .ChainDelay(ba.clickEndDelay)
                .OnComplete(() => IsAnimatingClick = false);
            
            yield return animateClick.ToYieldInstruction();
        }
        
        
        // PRIVATE METHODS
        private void SetButtonColor(Color buttonColor, Color iconColor, Color textColor)
        {
            Components c = components;
            if (c.imageButton) c.imageButton.color = buttonColor;
            if (c.imageIcon) c.imageIcon.color = iconColor;
            if (c.textLabel) c.textLabel.color = textColor;
        }
        
        
        // STRUCTS
        [Serializable] public struct ButtonAnimation
        {
            public float openOffset;
            public float openTime;
            [Space]
            public float expandTime;
            public float expandWidth;
            public Ease expandEase;
            [Space]
            public float resetTime;
            public Ease resetEase;
            [Space]
            public int clickFlashCount;
            public float clickFlashTime;
            public float clickEndDelay;
        }
        
        
        
    }
}
