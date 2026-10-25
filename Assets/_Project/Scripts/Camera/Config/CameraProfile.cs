using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    [CreateAssetMenu(menuName = "Project Racing Sky/Camera/Camera Profile", fileName = "CameraProfile")]
    public class CameraProfile : ScriptableObject
    {
        [Header("Details")]
        public string profileName;
        public ViewConfig viewConfig;
        public DampingConfig dampingConfig;
        public OffsetConfig offsetConfig;
        public FOVConfig fOVConfig;
        public ShakeConfig shakeConfig;
    }
}
