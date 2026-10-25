using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    public class UIPageAnimator : UIAnimator
    {
        [Header("Animation")]
        [SerializeField] private float interval;
        
        [Header("Reference")]
        [SerializeField] private CanvasGroup canvasGroup;
        
        // PRIVATE FIELDS
        private List<UIButtonAnimator> buttonAnimators;
        private List<UIAnimator> otherAnimators;
        
        
        // PUBLIC METHODS
        public void InitializeAnimator()
        {
            buttonAnimators = new();
            otherAnimators = new();
            
            UIAnimator[] childAnimators = GetComponentsInChildren<UIAnimator>();
            for (int i = 0; i < childAnimators.Length; i++)
            {
                // Do not include this UIPageAnimator itself, or it'l cause StackOverflow
                if (childAnimators[i] == this)
                    continue;
                else if (childAnimators[i] is UIButtonAnimator animator)
                    buttonAnimators.Add(animator);
                else
                    otherAnimators.Add(childAnimators[i]);
            }
        }
        
        
        // ANIMATE METHODS
        public override IEnumerator AnimateOpen()
        {
            AnimateOpen(false);
            yield break;
        }
        
        public override IEnumerator AnimateClose()
        {
            // wait for clicked button to exit
            
            for (int i = 0; i < buttonAnimators.Count; i++)
            {
                StartCoroutine(buttonAnimators[i].AnimateClose());
            }
            for (int i = 0; i < otherAnimators.Count; i++)
            {
                StartCoroutine(otherAnimators[i].AnimateClose());
            }
            yield break;
        }
        
        public override void ForceOpen()
        {
            canvasGroup.alpha = 1;
            for (int i = 0; i < buttonAnimators.Count; i++)
            {
                buttonAnimators[i].ForceOpen();
            }
            for (int i = 0; i < otherAnimators.Count; i++)
            {
                otherAnimators[i].ForceOpen();
            }
        }
        
        public override void ForceClose()
        {
            for (int i = 0; i < buttonAnimators.Count; i++)
            {
                buttonAnimators[i].ForceClose();
            }
            for (int i = 0; i < otherAnimators.Count; i++)
            {
                otherAnimators[i].ForceClose();
            }
            canvasGroup.alpha = 0;
        }
        
        
        // CUSTOM ANIMATOR METHODS
        public void AnimateOpen(bool isPreviousPage)
        {
            for (int i = 0; i < buttonAnimators.Count; i++)
            {
                buttonAnimators[i].AnimateOpen(isPreviousPage, interval * i);
            }
            for (int i = 0; i < otherAnimators.Count; i++)
            {
                StartCoroutine(otherAnimators[i].AnimateOpen());
            }
        }
        
        
        
    }
}
