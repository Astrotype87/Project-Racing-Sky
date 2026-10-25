using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Manager
{
    using Camera;
    using KBCore.Refs;
    using ProjectRacingSky.Track;
    using Ship;
    using UI;
    
    public class RaceManager : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Sub-Managers")]
        [SerializeField] private ShipManager shipManager;
        
        [Header("Track")]
        [SerializeField] [Scene] private RaceLine raceLine;
        
        [Header("Player")]                          // References for single player games
        [SerializeField] Ship playerShip;           // If this ship completes all the laps, the race is complete
        [SerializeField] RaceHUD raceHUD;           // Reference to HUD
        [SerializeField] CameraDamping followCamera; // Reference to follow camera
        
        [Header("Race")]
        [SerializeField] float totalRaceTime;       // Refers to race event time, but not related to ship's total time.
        [SerializeField] bool isRacing;             // If the last ship completes the race, the whole race event is finished.
        
        [Header("Ship")]
        [SerializeField] List<Ship> ships;
        [SerializeField] List<RaceData> shipRaceData;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (raceLine)
            {
                raceLine.OnRaceLineCrossed += HandleRaceLineCrossed;
            }
        }
        
        private void Start()
        {
            SpawnShips();
            InitializeRace();
            
            StartRace();
        }
        
        private void Update()
        {
            if (isRacing)
            UpdateRaceTime();
        }
        
        
        // PRIVATE METHODS
        private void SpawnShips()
        {
            // TODO: Spawn pilots & ships here using Instantiate() (based on Race Options pilot count and chosen ships)
            // Spawn ships using Instantiate() and set their respective ship profiles
            // Assign to pilots their respective ships
            
            RaceData raceData;
            foreach (Ship ship in ships)
            {
                // Cache ship race data for all ships
                raceData = ship.RaceData;
                
                if (raceData.lap != 1) raceData.lap = 1;
                if (raceData.lapTimes == null) raceData.lapTimes = new List<float>();
                if (raceData.lapTimes.Count == 0) raceData.lapTimes.Add(0.0f);
                
                shipRaceData.Add(raceData);
                
                // Race manager listens to every ship's race data related events.(ex: OnShipCompleteLap, OnShipCompleteRace)
                // Only race manager should manage ship's race data (ex: total time, lap time, current lap)
                // ship.CollisionData.OnLapTrigger += IncreaseLap;
            }
        }
        
        private void InitializeRace()
        {
            // ALT: Can be initialized in the ShipStatus instead;
            // foreach (RaceData raceData in shipRaceData)
            // {
            //     // Initialize lap
            //     raceData.lap = 1;
            //     raceData.lapTimes.Add(0);
            // }
        }
        
        private void StartRace()
        {
            isRacing = true;
        }
        
        private void UpdateRaceTime()
        {
            // Calculate total race time
            float deltaTime = Time.deltaTime * Time.timeScale;
            totalRaceTime += deltaTime;
            
            // Update every ship's race data
            foreach (RaceData raceData in shipRaceData)
            {
                // If the ship has completed the race, simply don't update it's race data times
                // if (raceData.isRaceComplete) continue;
                
                // Update ship's total time & current lap time
                raceData.totalTime += deltaTime;
                raceData.lapTimes[raceData.lap - 1] += deltaTime;
            }
        }
        
        
        // EVENT SUBSCRIBED METHODS
        private void HandleRaceLineCrossed(Ship ship)
        {
            RaceData raceData = ship.RaceData;
            
            if (raceData.hasCrossedFirstLap)
            {
                // If the ship completes a lap, increase the current lap
                raceData.lap ++;
                // Add a new lap time slot to te lap times list (if not, it will throw an IndexOutOfRangeException)
                raceData.lapTimes.Add(0);
            }
            else
            {
                raceData.hasCrossedFirstLap = true;
            }
        }
        
    }
}
