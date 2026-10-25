using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace ProjectRacingSky.UI
{
    using ProjectRacingSky.Ship;
    using ProjectRacingSky.Camera;
    using UnityEngine.InputSystem;

    public class RaceHUD : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Target Ship")]
        public Ship targetShip;
        public CameraFollow cameraFollow;
        // Insert reference to race settings (for total laps)
        // Insert reference for race status (for current pilot count)
        
        [Header("HUD Effects")]
        [SerializeField] private bool dynamicHUD;
        [SerializeField] private Vector2 l1MoveStrength;
        [SerializeField] private Vector2 l2MoveStrength;
        [SerializeField] [Range(0, 1)] private float l1MoveSmooth;
        [SerializeField] [Range(0, 1)] private float l2MoveSmooth;
        
        [Header("Layer 1")]
        [SerializeField] private RectTransform layer1;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI currentLapText;
        [SerializeField] private TextMeshProUGUI totalTimeText;
        [SerializeField] private RectTransform lapTimeGroup;
        
        [Header("Layer 2")]
        [SerializeField] private RectTransform layer2;
        [SerializeField] private RectTransform steerMeterHand;
        [SerializeField] private RectTransform pitchMeterHand;
        [SerializeField] private RectTransform rollMeter;
        [SerializeField] private Image airbrakeLeftMeter;
        [SerializeField] private Image airbrakeRightMeter;
        [SerializeField] [Range(0, 180)] private float steerMeterAngle;
        [SerializeField] [Range(0, 1)] private float steerMeterSmooth;
        [SerializeField] [Range(0, 200)] private float pitchMeterRange;
        [SerializeField] [Range(0, 1)] private float pitchMeterSmooth;
        [SerializeField] [Range(0, 1)] private float rollMeterSmooth;
        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;
        
        // Consider if these variables should be private instead
        [SerializeField] private TextMeshProUGUI currentLapNumberText;
        [SerializeField] private TextMeshProUGUI currentLapTimeText;
        
        
        // PRIVATE GETTERS
        private InputData inputData => targetShip.InputData;
        private VectorData vectorData => targetShip.VectorData;
        private RaceData raceData => targetShip.RaceData;
        
        // PRIVATE FIELDS
        private bool isReset;
        private Vector2 currentLayer1Offset;
        private Vector2 currentLayer2Offset;
        private Vector2 layer1Velocity;
        private Vector2 layer2Velocity;
        
        private Ship lastTargetShip;
        private float steerMeterVelocity;
        private float pitchMeterVelocity;
        private float rollMeterVelocity;
        
        
        
        // CONSTRUCTOR & FINALIZER
        public RaceHUD()
        {
            l1MoveStrength = new Vector2(1, 0.5f);
            l2MoveStrength = new Vector2(1, 0.5f);
            l1MoveSmooth = 0.1f;
            l2MoveSmooth = 0.1f;
            
            steerMeterAngle = 60;
            steerMeterSmooth = 0.8f;
            pitchMeterRange = 96;
            pitchMeterSmooth = 0.8f;
            rollMeterSmooth = 0f;
            onColor = new Color(1, 1, 1, 0.12f);
            offColor = new Color(0, 0, 0, 0.12f);
        }
        
        
        // MONOBEHAVIOUR METHODS
        private void Update()
        {
            // HACK: Move to GameManager/SceneManager
            if (Keyboard.current.escapeKey.wasPressedThisFrame) SceneManager.LoadScene(0);
        }
        
        private void LateUpdate()
        {
            if (!targetShip)
            {
                ResetHUD();
                return;
            }
            else isReset = false;
            
            UpdateLayer1();
            UpdateLayer2();
            UpdateDynamicHUD();
            
        }
        
        
        // LAYER 1 METHODS
        private void UpdateLayer1()
        {
            UpdateLap();
            UpdateSpeed();
            UpdateTotalTime();
            UpdateLapTime();
        }
        
        private void UpdateLap()
        {
            // Update lap display
            if (currentLapText && !currentLapText.text.Equals(raceData.lap.ToString()))
            {
                currentLapText.text = $"{raceData.lap}";
            }
        }
        
        private void UpdateSpeed()
        {
            // Get the ship's speed in rounded integers
            int speed = Mathf.RoundToInt(targetShip.VectorData.speedKPH);
            speedText.text = speed.ToString();
        }
        
        private string GetRaceTimeFormat(float time)
        {
            int minutes = (int) time / 60;
            int seconds = (int) time - 60 * minutes;
            int cents = (int) (100 * (time - minutes * 60 - seconds));
            return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, cents);
        }
        
        private void UpdateTotalTime()
        {
            if (totalTimeText)
            totalTimeText.text = GetRaceTimeFormat(targetShip.RaceData.totalTime);
        }
        
        private void UpdateLapTime()
        {
            if (!lapTimeGroup) return;
            
            // If the ship's lap changes or the target ship changes, refresh the lap time display
            if (raceData.lap != lapTimeGroup.childCount || targetShip != lastTargetShip)
            {
                RefreshLapTimeDisplay();
            }
            
            // Update current lap time
            currentLapTimeText.text = GetRaceTimeFormat(raceData.lapTimes[raceData.lap - 1]);
        }
        
        private void RefreshLapTimeDisplay()
        {
            int currentLap = raceData.lap;
            int lapDisplayCount = lapTimeGroup.childCount;
            
            if (currentLap > lapDisplayCount)
            {
                for (int i = lapDisplayCount; i < currentLap; i++)
                {
                    // Instantiate new lap display inside lap time display group, change the object's name
                    Transform newLapDisplay = Instantiate(lapTimeGroup.GetChild(0), lapTimeGroup);
                    newLapDisplay.name = $"Lap {i + 1}";
                    
                    // Change the new lap time display position
                    RectTransform newLapRectTransform = newLapDisplay.GetComponent<RectTransform>();
                    newLapRectTransform.localPosition = new Vector3(22.5f, -35 - 30 * newLapDisplay.GetSiblingIndex(), 0);
                }
            }
            else if (currentLap < lapDisplayCount)
            {
                // Destroy unused lap time displays
                for (int i = lapDisplayCount; i > currentLap; i--) Destroy(lapTimeGroup.GetChild(i - 1).gameObject);
            }
            
            // Refresh all lap display times
            for (int i = 0; i < raceData.lap; i++)
            {
                // Refresh lap count and time
                TextMeshProUGUI lapNumberText = lapTimeGroup.GetChild(i).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI lapTimeText = lapTimeGroup.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                lapNumberText.text = $"{i + 1}";
                lapTimeText.text = GetRaceTimeFormat(raceData.lapTimes[i]);
                
                // If last display on the list, set current lap display reference
                if (i == raceData.lap - 1)
                {
                    currentLapNumberText = lapNumberText;
                    currentLapTimeText = lapTimeText;
                }
            }
            
            // Set this target ship as previously updated
            lastTargetShip = targetShip;
        }
        
        
        // LAYER 2 METHODS
        private void UpdateLayer2()
        {
            UpdateSteerMeter();
            UpdatePitchMeter();
            UpdateRollMeter();
            UpdateAirbrakeMeter();
        }
        
        private void UpdateSteerMeter()
        {
            steerMeterHand.localRotation = Quaternion.Euler(0, 0,
                Mathf.SmoothDampAngle(steerMeterHand.localEulerAngles.z, inputData.steerInput * -steerMeterAngle,
                ref steerMeterVelocity, steerMeterSmooth));
        }
        
        private void UpdatePitchMeter()
        {
            pitchMeterHand.localPosition = new Vector3(0,
                Mathf.SmoothDamp(pitchMeterHand.localPosition.y, inputData.pitchInput * pitchMeterRange,
                ref pitchMeterVelocity, pitchMeterSmooth), 0);
        }
        
        private void UpdateRollMeter()
        {
            Quaternion targetRotation = cameraFollow.IsFollowModel? vectorData.modelRotation : vectorData.rotation;
            
            rollMeter.localRotation = Quaternion.Euler(0, 0,
                Mathf.SmoothDampAngle(rollMeter.localEulerAngles.z, -targetRotation.eulerAngles.z,
                ref rollMeterVelocity, rollMeterSmooth));
        }
        
        private void UpdateAirbrakeMeter()
        {
            airbrakeLeftMeter.color = Color.Lerp(offColor, onColor, inputData.airbrakeLeftInput);
            airbrakeRightMeter.color = Color.Lerp(offColor, onColor, inputData.airbrakeRightInput);
        }
        
        
        // HUD METHODS
        private void UpdateDynamicHUD()
        {
            if (!dynamicHUD)
            {
                layer1.offsetMin = Vector2.zero;
                layer1.offsetMax = Vector2.zero;
                return;
            }
            
            // Compute and update layer 1 offset
            Vector2 targetLayer1Offset = vectorData.localVelocity * l1MoveStrength;
            currentLayer1Offset = Vector2.SmoothDamp(currentLayer1Offset, targetLayer1Offset, ref layer1Velocity, l1MoveSmooth);
            layer1.offsetMin = currentLayer1Offset;
            layer1.offsetMax = currentLayer1Offset;
            
            // Compute and update layer 2 offset
            Vector2 targetLayer2Offset = vectorData.localVelocity * l2MoveStrength;
            currentLayer2Offset = Vector2.SmoothDamp(currentLayer2Offset, targetLayer2Offset, ref layer2Velocity, l2MoveSmooth);
            layer2.offsetMin = currentLayer2Offset;
            layer2.offsetMax = currentLayer2Offset;
        }
        
        private void ResetHUD()
        {
            if (isReset) return;
            
            layer1.offsetMin = Vector2.zero;
            layer1.offsetMax = Vector2.zero;
            layer2.offsetMin = Vector2.zero;
            layer2.offsetMax = Vector2.zero;
            
            isReset = true;
        }
        
        
        
    }
    
}
