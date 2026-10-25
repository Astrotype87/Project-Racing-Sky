using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

namespace ProjectRacingSky.Input
{
    using ProjectRacingSky.UI;
    using CallbackContext = InputAction.CallbackContext;
    
    public class UIInputManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Selection")]
        [SerializeField] private GameObject firstSelected;
        
        [Header("Components")]
        [SerializeField] private MenuScreen mainMenuUI;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private InputSystemUIInputModule inputModule;
        
        [Header("Log Selection")]
        [SerializeField] private bool logSelection;
        [SerializeField] private bool logStarted;
        [SerializeField] private bool logUpdated;
        [SerializeField] private bool logLastChanged;
        [SerializeField] private bool logCleared;
        [SerializeField] private bool logCancel;
        
        [Header("Selected")]
        [SerializeField] private GameObject currentSelected;
        [SerializeField] private GameObject lastSelected;
        [SerializeField] private GameObject lastSelectedNotNull;
        
        
        // EVENTS
        public event Action<GameObject> OnSelectionStarted;
        public event Action<GameObject> OnSelectionUpdated;
        public event Action<GameObject> OnLastSelectionChanged;
        public event Action OnSelectionCleared;
        
        // public event Action OnSubmit;
        public event Action OnCancel;
        
        // PRIVATE FIELDS
        private InputAction navigateAction;
        private InputAction cancelAction;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            lastSelected = firstSelected;
            lastSelectedNotNull = firstSelected;
            
            GetInputActionReferences();
            SubscribeEventHandlers();
            
            // DEBUG
            SubscribeSelectionLogs();
        }
        
        private void Update()
        {
            UpdateSelectionEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeEventHandlers();
        }
        
        private void OnValidate()
        {
            eventSystem.firstSelectedGameObject = firstSelected;
        }
        
        
        // PRIVATE METHODS
        private void UpdateSelectionEvents()
        {
            currentSelected = eventSystem.currentSelectedGameObject;
            
            bool isSelectionStarted = currentSelected != null && lastSelected == null;
            bool isSelectionCleared = currentSelected == null && lastSelected != null;
            bool isSelectionUpdated = currentSelected != lastSelected;
            
            if (isSelectionStarted) OnSelectionStarted?.Invoke(currentSelected);
            if (isSelectionCleared) OnSelectionCleared?.Invoke();
            if (isSelectionUpdated)
            {
                OnSelectionUpdated?.Invoke(currentSelected);
                lastSelected = currentSelected;
                
                if (currentSelected != null && currentSelected != lastSelectedNotNull)
                {
                    OnLastSelectionChanged?.Invoke(currentSelected);
                    lastSelectedNotNull = currentSelected;
                }
            }
        }
        
        
        // HANDLE METHODS
        private void GetInputActionReferences()
        {
            navigateAction = inputModule.actionsAsset.FindAction("Navigate");
            cancelAction = inputModule.actionsAsset.FindAction("Cancel");
        }
        
        private void SubscribeEventHandlers()
        {
            if (navigateAction != null) navigateAction.performed += HandleNavigation;
            if (cancelAction != null) cancelAction.performed += HandleCancel;
            
            OnCancel += mainMenuUI.HandleCancel; // CODE SMELL, your'e subscribing to your own event and then invoke the event in the same class.
        }
        
        private void UnsubscribeEventHandlers()
        {
            if (navigateAction != null) navigateAction.performed -= HandleNavigation;
            if (cancelAction != null) cancelAction.performed -= HandleCancel;
        }
        
        private void HandleNavigation(CallbackContext ctx = new())
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(lastSelectedNotNull);
            }
        }
        
        private void HandleCancel(CallbackContext ctx = new())
        {
            OnCancel?.Invoke();
        }
        
        
        // DEBUG METHODS
        private void SubscribeSelectionLogs()
        {
            OnSelectionStarted +=
                go => LogEvent(logStarted,     $"Selection Started: {go.name}");
            OnSelectionUpdated +=
                go => LogEvent(logUpdated,     $"Selection Updated: {(go != null ? go.name : "None")}");
            OnLastSelectionChanged +=
                go => LogEvent(logLastChanged, $"Last Selection Changed: {go.name}");
            OnSelectionCleared +=
                () => LogEvent(logCleared,     $"Selection Cleared");
            OnCancel +=
                () => LogEvent(logCancel,      $"Cancel");
        }
        
        private void LogEvent(bool enableLog, string message)
        {
            if (logSelection && enableLog) Debug.Log("[Navigation Handler] " + message);
        }
        
        
        
        
    }
    
}
