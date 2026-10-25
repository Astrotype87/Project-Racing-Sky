using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Race
{
    [CreateAssetMenu(menuName = "Project Racing Sky/Race/Season Profile", fileName = "SeasonProfile")]
    public class SeasonProfile : ScriptableObject
    {
        public string seasonName;
        public string description;
        public ChapterProfile[] chapters;
    }
}
