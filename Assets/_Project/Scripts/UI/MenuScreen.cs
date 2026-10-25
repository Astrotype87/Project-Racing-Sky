using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections;

namespace ProjectRacingSky.UI
{
    /// <summary>
    /// <para> Manages the whole main menu user interface. </para>
    /// <para> This component relies on game object activation state. </para>
    /// </summary>
    public class MenuScreen : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Start")]
        [SerializeField] private bool startOnPlay;
        
        [Header("UI Elements")]
        [SerializeField] private UIBackground background;
        [SerializeField] private UIPagesContainer pageManager;
        [SerializeField] private UIHeader header;
        [SerializeField] private UITitleScreen titleScreen;
        
        [Header("Exit")]
        [SerializeField] private UIButton exitButton;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            SubscribeEventHandlers();
        }
        
        private void Start()
        {
            if (startOnPlay) StartCoroutine(OpenTitleScreen());
        }
        
        private void OnDestroy()
        {
            UnsubscribeEventHandlers();
        }
        
        
        // PUBLIC METHODS
        /// <summary> Called when you click the title screen. </summary>
        public void HandleTitleScreenClicked()
        {
            StartCoroutine(OpenMainMenuScreen());
        }
        
        /// <summary> Called by UIInputManager when you click any cancel key. </summary>
        public void HandleCancel()
        {
            // HACK: Upon OpenMainMenuScreen(), the cancel will not work until the pageManager game object is set active
            // The cancel is available after header open animation is finished
            
            if (titleScreen.gameObject.activeInHierarchy)
            {
                HandleExitGame();
            }
            else if (pageManager.gameObject.activeInHierarchy)
            {
                if (pageManager.IsFirstPage)
                {
                    StartCoroutine(ReturnToTitleScreen());
                }
                else
                {
                    StartCoroutine(ReturnToPreviousPage());
                }
            }
        }
        
        /// <summary> Called when the page changes. </summary>
        public void HandlePageChanged(UIHeader.HeaderData headerData, bool isReverse)
        {
            StartCoroutine(UpdateHeaderTitle(headerData, isReverse));
        }
        
        /// <summary> Called when player wants to exit. </summary>
        public void HandleExitGame()
        {
            // TODO: Show game exit confirmation
            
            #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
            
            Debug.Log("[Menu Manager] EXIT event!");
        }
        
        
        // SUBSCRIPTION METHODS
        private void SubscribeEventHandlers()
        {
            titleScreen.OnClicked += HandleTitleScreenClicked;
            pageManager.OnPageChange += HandlePageChanged;
            exitButton.OnClicked += HandleExitGame;
        }
        
        private void UnsubscribeEventHandlers()
        {
            titleScreen.OnClicked -= HandleTitleScreenClicked;
            pageManager.OnPageChange += HandlePageChanged;
            exitButton.OnClicked -= HandleExitGame;
        }
        
        
        // MAIN MENU UI STATE METHODS
        private IEnumerator OpenTitleScreen()
        {
            header.gameObject.SetActive(false);
            pageManager.gameObject.SetActive(false);
            
            EventSystem.current.SetSelectedGameObject(titleScreen.gameObject);
            yield return titleScreen.OpenTitleScreen();
        }
        
        private IEnumerator OpenMainMenuScreen()
        {
            yield return titleScreen.CloseTitleScreen();
            
            yield return header.OpenHeader();
            yield return pageManager.OpenPageManager();
        }
        
        private IEnumerator UpdateHeaderTitle(UIHeader.HeaderData headerData, bool isReverse)
        {
            yield return header.ChangeTitle(headerData, isReverse);
        }
        
        private IEnumerator ReturnToTitleScreen()
        {
            // CODE VERSION 1
            /* yield return */ pageManager.ClosePageManager();  // TODO: Make this coroutine
            pageManager.gameObject.SetActive(false);
            
            yield return header.CloseHeader();
            
            // CODE VERSION 2
            // Coroutine closePageManager = StartCoroutine(pageManager.ClosePageManager());
            // Coroutine closeHeader = StartCoroutine(header.CloseHeader());
            // yield return closeHeader; // wait for header to close
            
            
            
            EventSystem.current.SetSelectedGameObject(titleScreen.gameObject);
            titleScreen.ForceOpenTitleScreen();
            
            // Use this if you want to animate title again
            // titleScreen.OpenTitleScreen();
        }
        
        private IEnumerator ReturnToPreviousPage()
        {
            /* yield return */ pageManager.HandleCancel();      // TODO: Make this coroutine
            yield break;
        }
        
        
        
        
        
    }
    
}
