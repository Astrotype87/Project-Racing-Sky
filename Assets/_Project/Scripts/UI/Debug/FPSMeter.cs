using UnityEngine;
using TMPro;

namespace ProjectRacingSky.UI
{
    public class FPSMeter : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Properties")]
        [SerializeField] private Color color;
        [SerializeField] private string initialText;
        
        [Header("References")]
        [SerializeField] private TMP_Text textFPS;
        
        
        // PRIVATE FIELDS
        private double deltaTime = 0.0f;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            InvokeRepeating(nameof(UpdateFPS), 0, 0.25f);
        }
        
        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
        
        private void OnValidate()
        {
            if (!textFPS) return;
            
            textFPS.color = color;
            if (!Application.isPlaying)
            {
                textFPS.text = initialText;
            }
        }
        
        
        // PRIVATE METHODS
        private void UpdateFPS()
        {
            if (!textFPS) return;
            
            textFPS.text = $"{Mathf.Ceil(1.0f / (float)deltaTime)} FPS";
        }
        
        
    }
    
}
