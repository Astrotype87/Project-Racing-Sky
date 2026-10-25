using UnityEngine;

namespace Archived
{
    // REFACTOR: Consider changing to new class name: Pilot class
    // This is to make pilot listing in race manager include both player and ai pilots
    public class Player : MonoBehaviour
    {
        // public Ship shipSystem;     // The ship which the pilot will control. (You can switch the ship it controls during race)
        
        // REFACTOR: Consider inheriting from creating new class: PilotInput class, where PlayerInput & AIInput will both inherit ShipInput struct.
        // If a PilotInput component is disabled, it should send ship input with all zero values
        // public PlayerInputTest inputTest;   // Reference to input
        
        
        // MONOBEHAVIOUR
        private void Update()
        {
            UpdateShipInput();
        }
        
        
        // CLASS METHODS
        // public void ChangeShip(Ship ship)
        // {
        //     // INFO: Unused method
        //     shipSystem = ship;
        // }
        
        private void UpdateShipInput()
        {
            // if (shipSystem)
            // REFACTOR: Use the new input system to send input to ship. Also the note above.
            // shipSystem.ShipInputData = inputTest.ShipInput;
        }
    }
    
}
