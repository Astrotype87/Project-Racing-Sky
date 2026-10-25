namespace ProjectRacingSky.Race
{
    public enum RaceMode
    {
        SingleRace, TimeAttack, LapAttack,
        Versus, Elimination, Combat,
        Speedrun, Blitz, GateRun,
        Sector, SectorRace,
        FreePlay
    }
    
    public enum ShipType
    {
        Normal, Speed, Agile, Drift, Combat, Flight, Space, Any
    }
    
    public enum SpeedClass
    {
        Stall, Strike, Sabre, Stealth,
        Spectrum, Supersonic
    }
    
    public enum Difficulty
    {
        Casual, Beginner, Intermediate, Advanced, Expert
    }
    
    public enum PickupLevel
    {
        Pure = 0,
        Safe = 1,
        Standard = 2,
        Advanced = 3,
        Destructive = 4,
        Custom = 5
    }
}
