using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectRacingSky.UI
{
    using HeaderData = UIHeader.HeaderData;
    
    public class UIPagesContainer : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [SerializeField] private UIPage firstPage;
        [SerializeField] private UIPage[] pages;
        [SerializeField] private UIEventBindings[] eventBindings;
        
        [Header("Debug")]
        [SerializeField] private bool logPageStack;
        
        
        // PROPERTIES & EVENTS
        public bool IsFirstPage => currentPage == firstPage;
        
        // EVENTS
        public event Action<HeaderData, bool> OnPageChange;
        
        // PRIVATE FIELDS
        private UIPage currentPage;
        private Stack<UIPage> pageHistory;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            GetEventBindings();
            SubscribeToEventBindings();
            
            ResetPageManagerState();
        }
        
        
        // PUBLIC METHODS
        public IEnumerator OpenPageManager()
        {
            gameObject.SetActive(true);
            
            ResetPageManagerState();
            OpenPage(firstPage, false);
            
            yield break;
        }
        
        public IEnumerator ClosePageManager()
        {
            if (currentPage) yield return StartCoroutine(currentPage.ClosePage());
            
            gameObject.SetActive(false);
        }
        
        public void ForceOpenPageManager()
        {
            gameObject.SetActive(true);
            
            ResetPageManagerState();
            ForceOpenPage(firstPage, false);
        }
        
        public void ForceClosePageManager()
        {
            if (currentPage) currentPage.ForceClosePage();
        }
        
        public void HandleCancel()
        {
            OpenLastPage();
        }
        
        
        // PAGE METHODS
        private IEnumerator OpenNewPage(UIPage nextPage)
        {
            if (nextPage.IsPageOpened) yield break;
            pageHistory.Push(currentPage);
            
            // Give time for closing animation before opening next page
            // yield return new WaitForSeconds(openPageDelay);
            OpenPage(nextPage, false);
        }
        
        private void OpenLastPage()
        {
            if (pageHistory.Count == 0) return;
            UIPage lastPage = pageHistory.Pop();
            
            if (lastPage.IsPageOpened) return;
            OpenPage(lastPage, true);
        }
        
        private void OpenPage(UIPage page, bool isPreviousPage)
        {
            if (currentPage) StartCoroutine(currentPage.ClosePage());
            currentPage = page;
            
            OnPageChange?.Invoke(page.HeaderData, isPreviousPage);
            page.OpenPage(isPreviousPage);
            
            LogPageHistoryStack();
        }
        
        private void ForceOpenPage(UIPage page, bool isPreviousPage)
        {
            if (currentPage) currentPage.ForceClosePage();
            currentPage = page;
            
            OnPageChange?.Invoke(page.HeaderData, isPreviousPage);
            page.ForceOpenPage();
            
            LogPageHistoryStack();
        }
        
        
        // AWAKE METHODS
        private void GetEventBindings()
        {
            eventBindings = GetComponentsInChildren<UIEventBindings>(true);
        }
        
        private void SubscribeToEventBindings()
        {
            foreach (var channel in eventBindings)
            {
                channel.OnOpenPage += nextPage => StartCoroutine(OpenNewPage(nextPage));
                channel.OnBack += OpenLastPage;
            }
        }
        
        private void ResetPageManagerState()
        {
            currentPage = null;
            pageHistory = new();
            
            if (pages == null) return;
            foreach (UIPage page in pages)
            {
                page.ForceClosePage();
            }
        }
        
        
        // DEBUGGING METHODS
        private void LogPageHistoryStack()
        {
            if (logPageStack)
            Debug.Log("Page stack: " + string.Join(" -> ", pageHistory.Reverse()));
        }
        
        
        
    }
    
}
