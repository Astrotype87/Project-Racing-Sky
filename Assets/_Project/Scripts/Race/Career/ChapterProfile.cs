using UnityEngine;

namespace ProjectRacingSky.Race
{
    [CreateAssetMenu(menuName = "Project Racing Sky/Race/Chapter Profile", fileName = "ChapterProfile")]
    public class ChapterProfile : ScriptableObject
    {
        public string chapterName;
        public string description;
        public CareerEvent[] events;
    }
}
