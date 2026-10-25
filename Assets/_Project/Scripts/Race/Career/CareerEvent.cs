using System;
using NaughtyAttributes;
using Nested = NaughtyAttributes.AllowNestingAttribute;

namespace ProjectRacingSky.Race
{
    [Serializable]
    public class CareerEvent
    {
        public string name;
        [ResizableTextArea] public string description;
        public EventMode mode;
        
        [Nested, ShowIf(nameof(mode), EventMode.Single)] public RaceEventConfig raceConfig;
        [Nested, ShowIf(nameof(mode), EventMode.Tournament)] public TournamentEventConfig tournamentConfig;
        
        public enum EventMode
        {
            Single, Tournament
        }
    }
}
