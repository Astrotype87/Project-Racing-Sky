using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace ProjectRacingSky.UI
{
    using HeaderData = UIHeader.HeaderData;
    
    public class UIPage : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Properties")]
        [SerializeField] private string pageName;
        [SerializeField] private string pagePath;
        
        [Header("Selection")]
        [SerializeField] private UIButton firstSelected;
        
        
        // PROPERTIES
        public bool IsPageOpened => gameObject.activeSelf;
        public HeaderData HeaderData => new(pageName, pagePath);
        
        // EVENTS
        public event Action<HeaderData> OnPageOpened;
        public event Action OnPageClosed;
        
        // PRIVATE FIELDS
        private UIPageAnimator pageAnimator;
        private bool isOpeningOrClosing;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            GetAnimatorComponent();
        }
        
        private void OnValidate()
        {
            UpdateGameObjectName();
        }
        
        
        // PUBLIC METHODS
        public void OpenPage(bool isPreviousPage)
        {
            // ENTER OPENING PAGE
            if (IsPageOpened && isOpeningOrClosing) return;
            isOpeningOrClosing = true;
            
            // PROCESS OPENING PAGE
            // STEP 1: Activate game object
            // STEP 2: Select first button
            // STEP 3: Process animation
            // STEP 4: Invoke OnPageOpened event and send HeaderData
            gameObject.SetActive(true);
            SelectFirstGameObject();
            pageAnimator.AnimateOpen(isPreviousPage);
            
            // EXIT OPENING PAGE
            isOpeningOrClosing = false;
            OnPageOpened?.Invoke(new HeaderData(pageName, pagePath));
        }
        
        public IEnumerator ClosePage()
        {
            // ENTER CLOSING PAGE
            if (!IsPageOpened && isOpeningOrClosing) yield break;
            isOpeningOrClosing = true;
            
            // PROCESS CLOSING PAGE
            // STEP 1: Process animation
            // STEP 2: Deactivate page game object
            // STEP 3: Invoke OnPageClosed event
            yield return pageAnimator.AnimateClose();
            gameObject.SetActive(false);
            
            // EXIT CLOSING PAGE
            isOpeningOrClosing = false;
            OnPageClosed?.Invoke();
        }
        
        public void ForceOpenPage()
        {
            // IMMEDIATE PROCESS CLOSE PAGE
            gameObject.SetActive(true);
            SelectFirstGameObject();
            OnPageOpened?.Invoke(new HeaderData(pageName, pagePath));
        }
        
        public void ForceClosePage()
        {
            // IMMEDIATE PROCESS CLOSE PAGE
            gameObject.SetActive(false);
            OnPageClosed?.Invoke();
        }
        
        
        // AWAKE & ON VALIDATE METHODS
        private void GetAnimatorComponent()
        {
            if (TryGetComponent(out pageAnimator))
            {
                pageAnimator.InitializeAnimator();
            }
        }
        
        private void UpdateGameObjectName()
        {
            this.name = $"{pageName} Page";
        }
        
        
        // PRIVATE METHODS
        private void SelectFirstGameObject()
        {
            if (firstSelected)
            {
                EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
            }
            // else if (TryGetComponent<Selectable>(out Selectable selectable))
            // {
            //     EventSystem.current.SetSelectedGameObject(selectable.gameObject);
            // }
        }
        
        
        
    }
    
}
