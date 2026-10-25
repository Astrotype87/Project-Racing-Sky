using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace ProjectRacingSky.Manager
{
    using Input;
    
    public class DataManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Keyboard Binding Data")]
        [SerializeField] private KeyboardInputHandler keyboard;
        [SerializeField] string bindingFileName = "bindings";
        
        [Header("Save Path")]
        [SerializeField, ReadOnly, TextArea] private string location;
        // [SerializeField] string location = "C:\\Users\\GOOSE\\AppData\\LocalLow\\Astrotype Software\\Mobile Test";
        
        // PRIVATE FIELDS
        private IDataHandler dataHandler;
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            dataHandler = new JsonDataHandler(Application.persistentDataPath, bindingFileName, true);
        }
        
        private void OnValidate()
        {
            location = Application.persistentDataPath;
        }
        
        
        // PUBLIC METHODS
        public void LoadBindingData()
        {
            KeyboardInputConfig dataToLoad = default;
            if (dataHandler.LoadData<KeyboardInputConfig>(ref dataToLoad))
            {
                keyboard.KeyboardConfig = dataToLoad;
                Debug.Log("Data loaded.");
            }
            else
            {
                Debug.Log("No data found.");
            }
        }
        
        public void SaveBindingData()
        {
            object dataToSave = keyboard.KeyboardConfig;
            
            dataHandler.SaveData(dataToSave);
            Debug.Log("Data saved.");
        }
        
        
        
    }
    
}
