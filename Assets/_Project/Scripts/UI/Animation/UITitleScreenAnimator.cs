using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace ProjectRacingSky.UI
{
    using ProjectRacingSky.Utility;
    using Properties = UITitleScreen.Properties;
    using Components = UITitleScreen.Components;
    
    public class UITitleScreenAnimator : UIAnimator
    {
        // INSPECTOR FIELDS
        [Header("Animation")]
        [SerializeField] private LogoAnimation logoAnimation;
        [SerializeField] private BarAnimation barAnimation;
        [SerializeField] private CloseAnimation closeAnimation;
        
        // PROPERTIES
        public bool IsAnimating { get; private set; }
        
        // PRIVATE FIELDS
        private Properties properties;
        private Components components;
        private Vector2 baseTextPosition;
        private float baseBarHeight;
        
        private Sequence animateOpenBar;
        private Sequence animateOpenLogo;
        
        private Sequence animateCloseBar;
        private Sequence animateCloseLogo;
        
        
        // PUBLIC METHODS
        public void InitializeAnimator(Components components, Properties properties)
        {
            this.components = components;
            this.properties = properties;
            
            if (components.rectBar)
            {
                baseBarHeight = components.rectBar.sizeDelta.y;
            }
            if (components.rectText)
            {
                baseTextPosition = components.rectText.anchoredPosition;
            }
        }
        
        
        // ANIMATOR METHODS
        public override IEnumerator AnimateOpen()
        {
            // REFERENCES
            Components c = components;
            Properties p = properties;
            LogoAnimation la = logoAnimation;
            BarAnimation ba = barAnimation;
            
            // INITIALIZE ANIMATION
            ForceClose();
            
            // PROCESS ANIMATION
            IsAnimating = true;
            animateOpenLogo = Sequence.Create()
                .Chain(Tween.Alpha(
                    c.groupLogo, 0, 1, la.fadeTime, Ease.OutSine, startDelay: la.startDelay));
            
            animateOpenBar = Sequence.Create()
                .ChainDelay(ba.startDelay)
                .ChainCallback(() => c.groupBar.alpha = 1)
                .Group(Tween.Custom(0, 1, ba.moveTime, ease: Ease.Linear,
                    onValueChange: x => c.rectBar.SetAnchorMaxX(x)))
                // .Group(Tween.UIOffsetMaxX(
                //     c.rectBar, -baseBarWidth, 0, ba.moveTime, Ease.Linear))
                .ChainDelay(ba.waitDelay)
                .Chain(Tween.Custom(
                    c.rectBar.sizeDelta.y, baseBarHeight, ba.expandTime, ease: Ease.Linear,
                    onValueChange: y => c.rectBar.SetSizeDeltaY(y)))
                .ChainDelay(ba.waitDelay);
                // .Chain(Tween.UIAnchoredPositionX(
                //     c.rectText, baseTextPosition.x + ba.textOffset, baseTextPosition.x,
                //     ba.textMoveTime, ba.textMoveEase))
                // .Group(Tween.Alpha(c.textLabel, 0, 1, ba.textFadeTime, ba.textFadeEase))
            for (int i = 0; i < ba.flashCount; i++)
            {
                animateOpenBar.ChainCallback(() => c.textLabel.alpha = 0)
                    .ChainDelay(ba.flashTime)
                    .ChainCallback(() => c.textLabel.alpha = 1)
                    .ChainDelay(ba.flashTime);
            }
            animateOpenBar.OnComplete(() => IsAnimating = false);
            
            yield return animateOpenBar.ToYieldInstruction();
        }
        
        public override IEnumerator AnimateClose()
        {
            // REFERENCES
            Components c = components;
            Properties p = properties;
            CloseAnimation ca = closeAnimation;
            
            // INITIALIZE ANIMATION
            ForceOpen();
            
            // PROCESS ANIMATION
            IsAnimating = true;
            Sequence animateExpandBar = Sequence.Create()
                .Chain(Tween.Custom(
                    baseBarHeight, baseBarHeight + ca.expandOffset, ca.expandTime, ease: ca.expandEase,
                    onValueChange: y => c.rectBar.SetSizeDeltaY(y)
                ));
            // animateExpandBar
            //     .Chain(Tween.Custom(
            //         baseBarHeight + ca.expandOffset, baseBarHeight, ca.returnTime, ease: ca.returnEase,
            //         onValueChange: y => c.rectBar.SetSizeDeltaY(y)
            //     ));
            
            animateCloseBar = Sequence.Create();
            for (int i = 0; i < ca.flashCount; i++)
            {
                animateCloseBar.ChainCallback(() => c.textLabel.alpha = 0)
                    .ChainDelay(ca.flashTime)
                    .ChainCallback(() => c.textLabel.alpha = 1)
                    .ChainDelay(ca.flashTime);
            }
            animateCloseBar
                .Chain(Tween.UIAnchoredPositionX(
                    c.rectText, baseTextPosition.x, baseTextPosition.x + ca.moveOffset,
                    ca.moveTime, ca.moveEase))
                .Group(Tween.Alpha(c.textLabel, 1, 0, ca.fadeTime, ca.fadeEase))
                .OnComplete(() => IsAnimating = false);
            
            yield return animateCloseBar.ToYieldInstruction();
        }
        
        public override void ForceOpen()
        {
            StopAllAnimation();
            
            IsAnimating = false;
            components.groupLogo.alpha = 1;
            components.groupBar.alpha = 1;
            components.rectBar.SetSizeDeltaY(baseBarHeight);
            components.rectText.anchoredPosition = baseTextPosition;
            components.textLabel.alpha = 1;
        }
        
        public override void ForceClose()
        {
            StopAllAnimation();
            
            IsAnimating = false;
            components.groupLogo.alpha = 0;
            components.groupBar.alpha = 0;
            components.rectBar.SetSizeDeltaY(barAnimation.startHeight);
            components.rectText.anchoredPosition = baseTextPosition;
            components.textLabel.alpha = 0;
        }
        
        
        // PRIVATE METHODS
        private void StopAllAnimation()
        {
            animateOpenBar.Stop();
            animateOpenLogo.Stop();
            animateCloseBar.Stop();
            animateCloseLogo.Stop();
            IsAnimating = false;
        }
        
        
        // STRUCTS
        [Serializable] public struct LogoAnimation
        {
            public float startDelay;
            public float fadeTime;
        }
        
        [Serializable] public struct BarAnimation
        {
            public float startDelay;
            public float moveTime;
            public float waitDelay;
            public float startHeight;
            public float expandTime;
            [Space]
            public float textOffset;
            public float textMoveTime;
            public Ease textMoveEase;
            public float textFadeTime;
            public Ease textFadeEase;
            [Space]
            public float flashTime;
            public float flashCount;
        }
        
        [Serializable] public struct CloseAnimation
        {
            public float expandOffset;
            public float expandTime;
            public Ease expandEase;
            // public float returnTime;
            // public Ease returnEase;
            [Space]
            public float flashTime;
            public float flashCount;
            public float moveTime;
            public float moveOffset;
            public Ease moveEase;
            public float fadeTime;
            public Ease fadeEase;
        }
        
        
    }
}
