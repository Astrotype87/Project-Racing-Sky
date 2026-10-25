using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using ProjectRacingSky.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectRacingSky
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] [Scene] private string RDEAScene;
        [SerializeField] [Scene] private string OvalScene;
        [SerializeField] [Scene] private string CityScene;
        
        [Header("Buttons")]
        [SerializeField] private UIButton RDEAButton;
        [SerializeField] private UIButton OvalButton;
        [SerializeField] private UIButton CityButton;
        
        private void Awake()
        {
            RDEAButton.OnClicked += () => LoadScene(RDEAScene);
            OvalButton.OnClicked += () => LoadScene(OvalScene);
            CityButton.OnClicked += () => LoadScene(CityScene);
        }
        
        private void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        
    }
}
