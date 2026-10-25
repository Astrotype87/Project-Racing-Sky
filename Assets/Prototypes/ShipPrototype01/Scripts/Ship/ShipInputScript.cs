using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    public class ShipInputScript : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] [Range(-1, 1)] private float thrustInput;
        [SerializeField] [Range(-1, 1)] private float steerInput;
        [SerializeField] [Range(0, 1)] private float airbrakeLeftInput;
        [SerializeField] [Range(0, 1)] private float airbrakeRightInput;
        [SerializeField] [Range(-1, 1)] private float pitchInput;
        [SerializeField] [Range(-1, 1)] private float rollInput;
        [SerializeField] private bool jumpInput;
        
        // PROPERTIES
        public float ThrustInput {get {return thrustInput;}}
        public float SteerInput {get {return steerInput;}}
        public float PitchInput {get {return pitchInput;}}
        public float RollInput {get {return rollInput;}}
        public float AirbrakeLeftInput {get {return airbrakeLeftInput;}}
        public float AirbrakeRightInput {get {return airbrakeRightInput;}}
        public bool JumpInput {get {return jumpInput;}}
        
        private void Update()
        {
            UpdateInput();
        }
        
        private void UpdateInput()
        {
            thrustInput = Input.GetAxisRaw("Vertical");
            steerInput = Input.GetAxisRaw("Horizontal");
            pitchInput = Input.GetAxisRaw("Vertical 2");
            rollInput = Input.GetAxisRaw("Horizontal 3");
            airbrakeLeftInput = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
            airbrakeRightInput = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            jumpInput = Input.GetKeyDown(KeyCode.Space);
        }
    }
    
}
