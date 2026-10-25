using UnityEngine;
using TMPro;

namespace ProjectRacingSky.UI
{
    public class VersionPresenter : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Properties")]
        [SerializeField] private string versionText;
        [SerializeField] private string platformText;
        [SerializeField] private string dateText;
        
        [Header("References")]
        [SerializeField] private TMP_Text versionComponent;
        [SerializeField] private TMP_Text platformComponent;
        [SerializeField] private TMP_Text dateComponent;
        
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            if (versionComponent) versionComponent.text = versionText;
            if (platformComponent) platformComponent.text = platformText;
            if (dateComponent) dateComponent.text = dateText;
        }
        
    }
}
