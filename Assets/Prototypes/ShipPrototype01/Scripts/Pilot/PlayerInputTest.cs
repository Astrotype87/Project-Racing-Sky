using UnityEngine;
using ProjectRacingSky.Ship;

namespace Archived
{
    public class PlayerInputTest : MonoBehaviour
    {
        [SerializeField] private InputData shipInput;
        
        public InputData ShipInput {get => shipInput;}
        
        private void Update()
        {
            UpdateInput();
        }
        
        private void UpdateInput()
        {
            shipInput.thrustInput = Input.GetAxisRaw("Vertical");
            shipInput.steerInput = Input.GetAxisRaw("Horizontal");
            shipInput.pitchInput = Input.GetAxisRaw("Vertical 2");
            shipInput.rollInput = Input.GetAxisRaw("Horizontal 3");
            shipInput.airbrakeLeftInput = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
            shipInput.airbrakeRightInput = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            // shipInput.jumpInput = Input.GetKeyDown(KeyCode.Space);
        }
    }
    
}
