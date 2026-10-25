using UnityEngine;
using NaughtyAttributes;

namespace ProjectRacingSky.Ship
{
    using Utility;
    using Config = ControllerConfig;
    
    [DefaultExecutionOrder(ScriptOrder.ShipController)]
    public class ShipController : MonoBehaviour
    {
        #region INSPECTOR
        
        [SerializeField] private bool showStatusBars;
        [SerializeField] private bool enableLog;
        
        [Header("Jump")]
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Jump", nameof(jumpCapacity), EColor.Yellow)]
        private float jumpFuel;
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Cooldown", nameof(jumpCoolTime), EColor.Gray)]
        private float jumpCooldown;
        
        [Header("Boost")]
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Boost", nameof(boostCapacity), EColor.Green)]
        private float boostFuel;
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Consume Cooldown", nameof(boostConCoolTime), EColor.Gray)]
        private float boostConCool;
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Refill Cooldown", nameof(boostRefCoolTime), EColor.Gray)]
        private float boostRefCool;
        
        [Header("Afterburner")]
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Afterburner", nameof(aftburCapacity), EColor.Blue)]
        private float aftburFuel;
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Consume Cooldown", nameof(aftburConCoolTime), EColor.Gray)]
        private float aftburConCool;
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Refill Cooldown", nameof(aftburRefCoolTime), EColor.Gray)]
        private float aftburRefCool;
        
        [Header("Sideshift")]
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Sideshift", nameof(sideshiftCapacity), EColor.White)]
        private float sideshiftFuel;
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Cooldown", nameof(sideshiftCoolTime), EColor.Gray)]
        private float sideshiftCooldown;
        
        [Header("Jump Pad")]
        [SerializeField, ShowIf(nameof(showStatusBars)), ProgressBar("Cooldown", nameof(jumpPadCoolTime), EColor.Gray)]
        private float jumpPadCooldown;
        
        [Header("Settings")]
        [SerializeField] private Config.Jump jump;
        [SerializeField] private Config.Boost boost;
        [SerializeField] private Config.Afterburner afterburner;
        [SerializeField] private Config.Sideshift sideshift;
        [SerializeField] private Config.JumpPad jumpPad;
        
        #endregion
        
        #region FIELDS
        
        // PRIVATE PROPERTIES
        private float jumpCapacity => jump.maxJumps;
        private float jumpCoolTime => jump.cooldownTime;
        private float boostCapacity => boost.maxFuel;
        private float boostConCoolTime => boost.consumeCooldown;
        private float boostRefCoolTime => boost.refillCooldown;
        private float aftburCapacity => afterburner.maxFuel;
        private float aftburConCoolTime => afterburner.consumeCooldown;
        private float aftburRefCoolTime => afterburner.refillCooldown;
        private float sideshiftCapacity => sideshift.maxSideshifts;
        private float sideshiftCoolTime => sideshift.cooldownTime;
        private float jumpPadCoolTime => jumpPad.cooldownTime;
        
        // PRIVATE FIELDS
        private PilotInput pilotInput;
        private TrackInput trackInput;
        private ZoneInput zoneInput;
        
        private InputData inputData;
        private ControllerData controllerData;
        private MechanicsData mechanicsData;
        private PhysicsData physicsData;
        
        private bool isAftburHoldingEmpty;         // If you keep holding afterburner after emptying fuel
        private bool isAftburHoldingCooldown;      // If you hold afterburner while its cooling down
        private bool hasAftburConsumedLastFrame;   // If you consumed afterburner last frame
        
        #endregion
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            SubscribeInputDataToRawInput();
            InitializeControllerData();
            
            SubscribeLogToInputData();
        }
        
        private void Update()
        {
            UpdateInputData();
            UpdateInspectorDisplay();
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            jumpFuel = jump.initialJumps;
            boostFuel = boost.initialFuel;
            aftburFuel = afterburner.initialFuel;
            sideshiftFuel = sideshift.initialSideshifts;
        }
        
        
        // PUBLIC METHODS
        public void SetDependencies(PilotInput pilotInput, TrackInput trackInput, ZoneInput zoneInput,
            InputData inputData, ControllerData controllerData, MechanicsData mechanicsData, PhysicsData physicsData)
        {
            this.pilotInput = pilotInput;
            this.trackInput = trackInput;
            this.zoneInput = zoneInput;
            this.inputData = inputData;
            this.controllerData = controllerData;
            this.mechanicsData = mechanicsData;
            this.physicsData = physicsData;
        }
        
        
        // INITIALIZATION
        private void SubscribeInputDataToRawInput()
        {
            // PILOT INPUT EVENTS
            pilotInput.OnHover += value => inputData.OnHover?.Invoke(value);
            pilotInput.OnJump += InvokeJump;
            pilotInput.OnBoost += InvokeBoost;
            pilotInput.OnSideshift += InvokeSideshift;
            pilotInput.OnCanopy += () => inputData.OnCanopy?.Invoke();
            
            // TRACK INPUT EVENTS
            trackInput.OnMaglev += value => inputData.OnMaglev?.Invoke(value);
            trackInput.OnJumpPad += InvokeJumpPad;
            trackInput.OnSpeedPad += InvokeSpeedPad;
            
            trackInput.OnPickupPad += () => inputData.OnPickupPad?.Invoke();
            trackInput.OnSlowdown += value => inputData.OnSlowdown?.Invoke(value);
        }
        
        private void InitializeControllerData()
        {
            controllerData.jumpFuel = jump.initialJumps;
            controllerData.boostFuel = boost.initialFuel;
            controllerData.aftburFuel = afterburner.initialFuel;
            controllerData.sideshiftFuel = sideshift.initialSideshifts;
        }
        
        
        // UPDATE
        private void UpdateInputData()
        {
            // PILOT INPUT CONSTANT
            inputData.thrustInput = pilotInput.thrustInput;
            inputData.reverseInput = pilotInput.reverseInput;
            inputData.airbrakeLeftInput = pilotInput.airbrakeLeftInput;
            inputData.airbrakeRightInput = pilotInput.airbrakeRightInput;
            inputData.strafeInput = pilotInput.strafeInput;
            inputData.steerInput = pilotInput.steerInput;
            inputData.pitchInput = pilotInput.pitchInput;
            inputData.rollInput = pilotInput.rollInput;
            
            // TRACK INPUT CONSTANT
            inputData.speedZoneInput = trackInput.speedZoneInput;
            
            // ZONE INPUT CONSTANT
            inputData.windInput = zoneInput.windInput;
            inputData.turbulenceInput = zoneInput.turbulenceInput;
            
            // INPUT COOLDOWN
            UpdateJump(Time.deltaTime);
            UpdateBoost(Time.deltaTime);
            UpdateAfterburner(Time.deltaTime);
            UpdateSideshift(Time.deltaTime);
            UpdateJumpPad(Time.deltaTime);
        }
        
        private void UpdateJump(float deltaTime)
        {
            // UPDATE COOLDOWN
            if (controllerData.jumpCooldown > 0) controllerData.jumpCooldown -= deltaTime;
            
            // RELOAD JUMP FUEL
            controllerData.isJumpRefilling = controllerData.jumpFuel < jump.maxJumps;
            if (controllerData.isJumpRefilling) controllerData.jumpFuel += deltaTime / jump.reloadTime;
            
            // CLAMP VALUES
            controllerData.jumpFuel = Mathf.Clamp(controllerData.jumpFuel.NaNSafe(controllerData.jumpFuel), 0, jump.maxJumps);
            controllerData.jumpCooldown = Mathf.Clamp(controllerData.jumpCooldown, 0, jump.cooldownTime);
        }
        
        private void UpdateBoost(float deltaTime)
        {
            // INPUT
            float thrustInput = boost.autoRefill ? 1 : pilotInput.thrustInput;
            
            // UPDATE COOLDOWN
            if (controllerData.boostConsumeCooldown > 0) controllerData.boostConsumeCooldown -= deltaTime;
            if (controllerData.boostRefillCooldown > 0) controllerData.boostRefillCooldown -= deltaTime;
            
            // REFILL BOOST FUEL
            bool isFull = controllerData.boostFuel >= boost.maxFuel;
            controllerData.isBoostRefilling = !isFull && controllerData.boostRefillCooldown <= 0;
            if (controllerData.isBoostRefilling) controllerData.boostFuel += boost.refillRate * deltaTime * thrustInput;
            
            // CLAMP VALUES
            controllerData.boostFuel = Mathf.Clamp(controllerData.boostFuel, 0, boost.maxFuel);
            controllerData.boostConsumeCooldown = Mathf.Clamp(controllerData.boostConsumeCooldown, 0, boost.consumeCooldown);
            controllerData.boostRefillCooldown = Mathf.Clamp(controllerData.boostRefillCooldown, 0, boost.refillCooldown);
        }
        
        private void UpdateAfterburner(float deltaTime)
        {
            // INPUT
            float thrustInput = afterburner.autoRefill ? 1 : pilotInput.thrustInput;
            bool afterburnerInput = pilotInput.afterburnerInput;
            
            // FUEL STATES
            bool isEmpty = controllerData.aftburFuel <= 0;
            bool isFull = controllerData.aftburFuel >= afterburner.maxFuel;
            bool isBelowMinConsumable = controllerData.aftburFuel < afterburner.minConsumable;
            
            // HOLDING STATES
            if (afterburnerInput && isEmpty) isAftburHoldingEmpty = true;
            if (afterburnerInput && controllerData.aftburConsumeCooldown > 0) isAftburHoldingCooldown = true;
            if (!afterburnerInput) isAftburHoldingEmpty = isBelowMinConsumable;
            if (!afterburnerInput) isAftburHoldingCooldown = false;
            
            // START COOLDOWN
            bool cannotConsume = !afterburnerInput || isEmpty || isAftburHoldingEmpty || isAftburHoldingCooldown;
            if (hasAftburConsumedLastFrame && cannotConsume) controllerData.aftburConsumeCooldown = afterburner.consumeCooldown;
            if (hasAftburConsumedLastFrame && cannotConsume) controllerData.aftburRefillCooldown = afterburner.refillCooldown;
            
            // UPDATE COOLDOWN
            if (controllerData.aftburConsumeCooldown > 0) controllerData.aftburConsumeCooldown -= deltaTime;
            if (controllerData.aftburRefillCooldown > 0) controllerData.aftburRefillCooldown -= deltaTime;
            
            // CONSUME & REFILL STATES
            bool isHolding = isAftburHoldingEmpty || isAftburHoldingCooldown;
            bool isConsuming = afterburnerInput && !isEmpty && !isHolding && controllerData.aftburConsumeCooldown <= 0;
            bool isRefilling = (!afterburnerInput && !isFull || isHolding) && controllerData.aftburRefillCooldown <= 0;
            hasAftburConsumedLastFrame = isConsuming;
            
            // CONSUME & REFILL AFTERBURNER FUEL
            if (isConsuming) controllerData.aftburFuel -= afterburner.consumeRate * deltaTime;
            if (isRefilling) controllerData.aftburFuel += afterburner.refillRate * deltaTime * thrustInput;
            inputData.afterburnerInput = isConsuming ? 1 : 0;
            
            // CLAMP VALUES
            controllerData.aftburFuel = Mathf.Clamp(controllerData.aftburFuel, 0, afterburner.maxFuel);
            controllerData.aftburConsumeCooldown = Mathf.Clamp(controllerData.aftburConsumeCooldown, 0, afterburner.consumeCooldown);
            controllerData.aftburRefillCooldown = Mathf.Clamp(controllerData.aftburRefillCooldown, 0, afterburner.refillCooldown);
        }
        
        private void UpdateSideshift(float deltaTime)
        {
            // UPDATE COOLDOWN
            if (controllerData.sideshiftCooldown > 0) controllerData.sideshiftCooldown -= deltaTime;
            
            // RELOAD SIDESHIFT FUEL
            controllerData.isSideshiftRefilling = controllerData.sideshiftFuel < sideshift.maxSideshifts;
            if (controllerData.isSideshiftRefilling) controllerData.sideshiftFuel += deltaTime / sideshift.reloadTime;
            
            // CLAMP VALUES
            controllerData.sideshiftFuel = Mathf.Clamp(controllerData.sideshiftFuel.NaNSafe(controllerData.sideshiftFuel), 0, sideshift.maxSideshifts);
            controllerData.sideshiftCooldown = Mathf.Clamp(controllerData.sideshiftCooldown, 0, sideshift.cooldownTime);
        }
        
        private void UpdateJumpPad(float deltaTime)
        {
            // UPDATE COOLDOWN
            if (controllerData.jumpPadCooldown > 0) controllerData.jumpPadCooldown -= deltaTime;
            controllerData.jumpPadCooldown = Mathf.Clamp(controllerData.jumpPadCooldown, 0, jumpPad.cooldownTime);
        }
        
        
        // INVOKE
        private void InvokeJump()
        {
            // CONSUME STATES
            bool isNotCurrentlyJumping = !physicsData.isJumping;
            bool isAboveConsumable = controllerData.jumpFuel.IsGreaterOrEqualTo(1); 
            bool canConsume = isNotCurrentlyJumping && isAboveConsumable && controllerData.jumpCooldown <= 0;
            
            if (canConsume)
            {
                // CONSUME JUMP FUEL
                controllerData.jumpFuel -= 1;
                inputData.OnJump?.Invoke();
                
                // START COOLDOWN
                controllerData.jumpCooldown = jump.cooldownTime;
            }
        }
        
        private void InvokeBoost()
        {
            // CONSUME STATES
            bool isNotCurrentlyBoosting = !physicsData.isBoosting;
            bool isAboveConsumable = controllerData.boostFuel.IsGreaterOrEqualTo(boost.consumeRate); 
            bool canConsume = isNotCurrentlyBoosting && isAboveConsumable && controllerData.boostConsumeCooldown <= 0;
            
            if (canConsume)
            {
                // CONSUME BOOST FUEL
                controllerData.boostFuel -= boost.consumeRate;
                inputData.OnBoost?.Invoke();
                
                // START COOLDOWN
                controllerData.boostConsumeCooldown = boost.consumeCooldown;
                controllerData.boostRefillCooldown = boost.refillCooldown;
            }
        }
        
        private void InvokeSideshift(bool direction)
        {
            // CONSUME STATES
            bool isNotCurrentlySideshifting = !physicsData.isSideshifting;
            bool isAboveConsumable = controllerData.sideshiftFuel.IsGreaterOrEqualTo(1); 
            bool canConsume = isNotCurrentlySideshifting && isAboveConsumable && controllerData.sideshiftCooldown <= 0;
            
            if (canConsume)
            {
                // CONSUME SIDESHIFT FUEL
                controllerData.sideshiftFuel -= 1;
                inputData.OnSideshift?.Invoke(direction);
                
                // START COOLDOWN
                controllerData.sideshiftCooldown = sideshift.cooldownTime;
            }
        }
        
        // INVOKE : IJumpPadListener, ISpeedPadListener
        public void InvokeJumpPad(Vector3 direction)
        {
            if (!physicsData.isJumpPadPerforming && controllerData.jumpPadCooldown <= 0)
            {
                // INVOKE JUMP PAD AND START COOLDOWN
                inputData.OnJumpPad?.Invoke(direction);
                controllerData.jumpPadCooldown = jumpPad.cooldownTime;
            }
        }
        
        public void InvokeSpeedPad(Vector3 direction)
        {
            // INVOKE SPEED PAD
            inputData.OnSpeedPad?.Invoke(direction);
            
            // SPEED PAD REFILL
            controllerData.boostFuel += boost.padRefillAmount;
            controllerData.aftburFuel += afterburner.padRefillAmount;
            
            // CLAMP VALUES
            controllerData.boostFuel = Mathf.Clamp(controllerData.boostFuel, 0, boost.maxFuel);
            controllerData.aftburFuel = Mathf.Clamp(controllerData.aftburFuel, 0, afterburner.maxFuel);
        }
        
        
        // DEBUG
        private void UpdateInspectorDisplay()
        {
            jumpFuel = controllerData.jumpFuel;
            jumpCooldown = controllerData.jumpCooldown;
            
            boostFuel = controllerData.boostFuel;
            boostConCool = controllerData.boostConsumeCooldown;
            boostRefCool = controllerData.boostRefillCooldown;
            
            aftburFuel = controllerData.aftburFuel;
            aftburConCool = controllerData.aftburConsumeCooldown;
            aftburRefCool = controllerData.aftburRefillCooldown;
            
            sideshiftFuel = controllerData.sideshiftFuel;
            sideshiftCooldown = controllerData.sideshiftCooldown;
            
            jumpPadCooldown = controllerData.jumpPadCooldown;
        }
        
        private void SubscribeLogToInputData()
        {
            // PILOT INPUT EVENTS
            inputData.OnHover += value => LogController($"HOVER switch {(value ? "ON" : "OFF")}!");
            // inputData.OnJump += () => LogController($"");
            // inputData.OnBoost += () => LogController($"");
            // pilotInput.OnSideshift += value => LogController($"");
            // pilotInput.OnCanopy += () => LogController($"CANOPY toggle!");
            
            // TRACK INPUT EVENTS
            inputData.OnMaglev += value => LogController($"MAGLEV {(value ? "ON" : "OFF")}!");
            inputData.OnJumpPad += value => LogController("JUMP PAD HIT!");
            inputData.OnSpeedPad += value => LogController("SPEED PAD HIT!");
        }
        
        private void LogController(object message)
        {
            if (enableLog) Debug.Log(message);
        }
        
        
    }
    
}
