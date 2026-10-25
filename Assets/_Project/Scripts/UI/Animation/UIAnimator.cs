using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.UI
{
    /// <summary> Base class for all UI animators, with auto-start options. </summary>
    public abstract class UIAnimator : MonoBehaviour
    {
        // ENUMS
        public enum AutoStart { None, Start, OnEnable }
        public enum AnimationType { ForceOpen, ForceClose, AnimateOpen, AnimateClose }
        
        // INSPECTOR FIELDS
        [Header("Auto-Start Options")]
        [SerializeField] protected AutoStart autoStart;
        [SerializeField, DisableIf(nameof(autoStart), (AutoStart)0)] protected new AnimationType animation;
        
        // PRIVATE FIELDS
        private bool isAnimationStarted;
        
        
        // MONOBEHAVIOUR METHODS
        protected virtual void Start()
        {
            if (autoStart is not (AutoStart.Start or AutoStart.OnEnable)) return;
            switch (animation)
            {
                case AnimationType.ForceOpen: ForceOpen(); break;
                case AnimationType.ForceClose: ForceClose(); break;
                case AnimationType.AnimateOpen: StartCoroutine(AnimateOpen()); break;
                case AnimationType.AnimateClose: StartCoroutine(AnimateClose()); break;
            }
            isAnimationStarted = true;
        }
        
        protected virtual void OnEnable()
        {
            if (autoStart != AutoStart.OnEnable || !isAnimationStarted) return;
            switch (animation)
            {
                case AnimationType.ForceOpen: ForceOpen(); break;
                case AnimationType.ForceClose: ForceClose(); break;
                case AnimationType.AnimateOpen: StartCoroutine(AnimateOpen()); break;
                case AnimationType.AnimateClose: StartCoroutine(AnimateClose()); break;
            }
        }
        
        
        // ABSTRACT METHODS
        public abstract IEnumerator AnimateOpen();
        
        public abstract IEnumerator AnimateClose();
        
        public abstract void ForceOpen();
        
        public abstract void ForceClose();
        
        
        
    }
}
