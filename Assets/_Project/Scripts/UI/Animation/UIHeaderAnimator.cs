using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PrimeTween;

namespace ProjectRacingSky.UI
{
    using ProjectRacingSky.Utility;
    using Properties = UIHeader.Properties;
    using Components = UIHeader.Components;
    using HeaderData = UIHeader.HeaderData;
    
    public class UIHeaderAnimator : UIAnimator
    {
        // INSPECTOR FIELDS
        [Header("Animation")]
        [SerializeField] private HeaderData headerData;
        [SerializeField] private BarAnimation barAnimation;
        [SerializeField] private TitleAnimation titleAnimation;
        
        // PRIVATE FIELDS
        private Properties properties;
        private Components components;
        private float baseHeaderHeight;
        private Vector2 baseTitlePosition;
        private Vector2 iconBasePosition;
        
        private Sequence animateOpenHeader;
        private Sequence animateCloseHeader;
        private Sequence animateChangeTitle;
        private Sequence animateChangePath;
        
        
        // PUBLIC METHODS
        public void InitializeAnimator(Components components, Properties properties)
        {
            this.components = components;
            this.properties = properties;
            
            baseHeaderHeight = components.rectHeader.sizeDelta.y;
            
            baseTitlePosition = components.textTitle.rectTransform.anchoredPosition;
            iconBasePosition = components.textTitle.rectTransform.anchoredPosition;
        }
        
        
        // ANIMATOR METHODS
        public override IEnumerator AnimateOpen()
        {
            // REFERENCES
            CanvasGroup canvasGroup = components.canvasGroup;
            RectTransform rectHeader = components.rectHeader;
            BarAnimation ba = barAnimation;
            
            // INITIALIZE ANIMATION
            ForceClose();
            
            // PROCESS ANIMATION
            animateOpenHeader = Sequence.Create()
                .ChainCallback(() => canvasGroup.alpha = 1)
                .Group(Tween.Custom(0, 1, ba.moveTime, ease: Ease.Linear,
                    onValueChange: x => rectHeader.SetAnchorMaxX(x)))
                // .Group(Tween.UIOffsetMaxX(
                //     rectHeader, -baseHeaderWidth, 0, ba.moveTime, Ease.Linear))
                .ChainDelay(ba.waitDelay)
                .Chain(Tween.Custom(
                    rectHeader.sizeDelta.y, baseHeaderHeight, ba.expandTime, ease: Ease.Linear,
                    onValueChange: y => rectHeader.SetSizeDeltaY(y)))
                .ChainDelay(ba.waitDelay);
            
            yield return animateOpenHeader.ToYieldInstruction();
        }
        
        public override IEnumerator AnimateClose()
        {
            ForceClose();
            yield break;
        }
        
        public override void ForceOpen()
        {
            animateOpenHeader.Stop();
            animateCloseHeader.Stop();
            animateChangeTitle.Stop();
            animateChangePath.Stop();
            
            components.canvasGroup.alpha = 1;
            components.rectHeader.SetSizeDeltaY(baseHeaderHeight);
        }
        
        public override void ForceClose()
        {
            animateOpenHeader.Stop();
            animateCloseHeader.Stop();
            
            components.canvasGroup.alpha = 0;
            components.rectHeader.SetSizeDeltaY(barAnimation.startHeight);
        }
        
        // CUSTOM ANIMATOR METHODS
        public IEnumerator AnimateChangeTitle(HeaderData headerData, bool isReverse)
        {
            // REFERENCES
            RectTransform rectTitle = components.textTitle.rectTransform;
            TMP_Text textTitle = components.textTitle;
            TMP_Text textPath = components.textPath;
            
            TitleAnimation t = titleAnimation;
            float moveAmount = t.moveAmount * (isReverse ? -1 : 1);
            
            // PROCESS ANIMATION
            animateChangeTitle = Sequence.Create()
                .Chain(Tween.Alpha(
                    textTitle, 0, t.inTime, Ease.Linear))
                .Group(Tween.UIAnchoredPositionX(
                    rectTitle, baseTitlePosition.x - moveAmount, t.inTime, Ease.Linear))
                .ChainCallback(() =>
                    textTitle.text = headerData.titleText)
                .Chain(Tween.Alpha(
                    textTitle, 1, t.outTime, Ease.OutCirc))
                .Group(Tween.UIAnchoredPositionX(
                    rectTitle, baseTitlePosition.x + moveAmount, baseTitlePosition.x, t.outTime, Ease.OutCirc));
            
            float textPathAlpha = textPath.color.a;
            
            animateChangePath = Sequence.Create()
                .Chain(Tween.Alpha(
                    textPath, 0, t.inTime, Ease.Linear))
                .ChainCallback(() =>
                    textPath.text = headerData.pathText)
                .Chain(Tween.Alpha(
                    textPath, textPathAlpha, t.outTime, Ease.OutCirc));
            
            yield return animateChangePath.ToYieldInstruction();
        }
        
        public void ForceChangeTitle(HeaderData headerData)
        {
            this.headerData = headerData;
            
            components.textTitle.text = this.headerData.titleText;
            components.textPath.text = this.headerData.pathText;
        }
        
        
        
        // STRUCTS
        [Serializable] public struct BarAnimation
        {
            public float waitDelay;
            public float startHeight;
            public float moveTime;
            public float expandTime;
        }
        
        [Serializable] public struct TitleAnimation
        {
            public float moveAmount;
            public float inTime;
            public float outTime;
        }
        
        
    }
}
