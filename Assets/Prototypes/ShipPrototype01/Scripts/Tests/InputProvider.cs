using UnityEngine;
using ProjectRacingSky;

namespace Archived
{
    public class InputProvider : MonoBehaviour
    {
        public ShipPhysics shipPhysics;
        public InputData shipInput;
        [Range(-1, 1)] public float holdThrust;
        [Range(-1, 1)] public float holdSteer;
        [Range(0, 1)] public float holdAirbrakeLeft;
        [Range(0, 1)] public float holdAirbrakeRight;
        [Range(-1, 1)] public float holdStrafe;
        [Range(-1, 1)] public float holdPitch;
        [Range(-1, 1)] public float holdRoll;
        public bool holdBoost;
        public bool holdJump;
        
        public float _jumpInputDelay;
        
        
        private void Update()
        {
            shipInput.thrustInput = 
                Input.GetKey(KeyCode.W) ? 1 :
                Input.GetKey(KeyCode.S) ? -1 : 0;
            shipInput.steerInput = 
                Input.GetKey(KeyCode.A) ? -1 :
                Input.GetKey(KeyCode.D) ? 1 : 0;
            shipInput.airbrakeLeftInput =
                Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
            shipInput.airbrakeRightInput =
                Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            shipInput.strafeInput =
                Input.GetKey(KeyCode.J) ? -1 :
                Input.GetKey(KeyCode.L) ? 1 : 0;
            shipInput.pitchInput =
                Input.GetKey(KeyCode.UpArrow) ? 1 :
                Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
            shipInput.rollInput =
                Input.GetKey(KeyCode.Q) ? -1 :
                Input.GetKey(KeyCode.E) ? 1 : 0;
            shipInput.boostInput =
                Input.GetKey(KeyCode.LeftShift);
                
            _jumpInputDelay = Input.GetKeyDown(KeyCode.Space) ? Time.fixedDeltaTime :
                _jumpInputDelay > 0 ? _jumpInputDelay - Time.deltaTime : 0;
            // shipInput.jumpInput = _jumpInputDelay > 0;
            
            shipInput.thrustInput = Mathf.Clamp(shipInput.thrustInput + holdThrust, -1, 1);
            shipInput.steerInput = Mathf.Clamp(shipInput.steerInput + holdSteer, -1, 1);
            shipInput.airbrakeLeftInput = Mathf.Clamp(shipInput.airbrakeLeftInput + holdAirbrakeLeft, 0, 1);
            shipInput.airbrakeRightInput = Mathf.Clamp(shipInput.airbrakeRightInput + holdAirbrakeRight, 0, 1);
            shipInput.strafeInput = Mathf.Clamp(shipInput.strafeInput + holdStrafe, -1, 1);
            shipInput.pitchInput = Mathf.Clamp(shipInput.pitchInput + holdPitch, -1, 1);
            shipInput.rollInput = Mathf.Clamp(shipInput.rollInput + holdRoll, -1, 1);
            shipInput.boostInput = shipInput.boostInput || holdBoost;
            // shipInput.jumpInput = shipInput.jumpInput || holdJump;
            
            // shipPhysics.SetInputData(shipInput);
        }
    }
    
}
