using UnityEngine;
using UnityEngine.UI;

namespace ProjectRacingSky.UI
{
    public class UIBackground : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private Color color;
        [SerializeField] [Range(0, 1)] private float opacity;
        
        [Header("References")]
        [SerializeField] private Image image;
        
        private void OnValidate()
        {
            color.a = opacity;
            if (image) image.color = color;
        }
        
    }
}
