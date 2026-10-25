using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Manager
{
    using ProjectRacingSky.UI;
    
    /// <summary>
    /// <para> Manages the entire user interface state. </para>
    /// <para> Controls main menu, race HUD, pause menu, and debug UI canvasses. </para>
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Canvasses")]
        [SerializeField] private MenuScreen mainMenuUI;
        // [SerializeField] private RaceUI raceUI;
        // [SerializeField] private EditorUI editorUI;
        [SerializeField] private DebugScreen debugUI;
        
        
    }
}
