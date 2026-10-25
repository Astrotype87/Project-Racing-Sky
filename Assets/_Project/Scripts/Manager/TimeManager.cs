using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Manager
{
    /// <summary> Manages and warps time and space. </summary>
    public class TimeManager : MonoBehaviour
    {
        [Header("FPS Settings")]
        [Range(1, 2000)] public int FPSLimit = 60;
        public bool defaultFPS;
        
        [Header("Timescale Settings")]
        [Range(0.01f, 2)] public float timeScale = 1;
        
        
        // MONOBEHAVIOUR METHODS
        private void Update()
        {
            Application.targetFrameRate = defaultFPS ? 0 : FPSLimit;
        }
        
        private void FixedUpdate()
        {
            Time.timeScale = timeScale;
        }
        
        
        
    }
    
}
