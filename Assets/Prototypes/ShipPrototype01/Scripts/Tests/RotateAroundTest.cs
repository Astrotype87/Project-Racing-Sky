using UnityEngine;

namespace Archived
{
    [ExecuteInEditMode]
    public class RotateAroundTest : MonoBehaviour
    {
        public Vector3 origin;
        public float distance;
        public Vector3 orbit;
        
        void Update()
        {
            Vector3 originalVector = Vector3.forward * distance;
            
            // You can rotate the vector's direction by multiplying it after the Quaternion that contains rotation you want
            // The order of operation should be quaternion first before vector
            Vector3 offset = Quaternion.Euler(orbit) * originalVector;
            transform.position = origin + offset;
            
            
            
            // STEP 1: Get target position. Add the camera offset.
            // STEP 2: Create a new pivot offset from the camera offset: target forward * pivot distance
            // STEP 3: Get the camera offset back: target forward * -pivot distance
            
            /*
            
            Transform ship;
            Transform camera;
            
            Vector3Global targetPosition = ship.position
            Vector3Local cameraOffset = new Vector3(0, 3, -8)
            float pivotDistance;
            Vector3Rotation orbit;
            
            Vector3Local pivotPosition = cameraOffset + (ship.forward * pivotDistance)
            Vector3Local pivotOffset = QuaternionEuler(orbit) * (ship.forward * -pivotDistance)
            
            camera.position = ship.GlobalVector(pivotOffset)
            */
            
            
            
            // Vector has two properties:
            // 1. direction - is where the vector is facing/heading towards
            // 2. magnitude - is the length/distance/range of the vector
            
            // Vector is used to describe:
            // 1. velocity
            // 2. offset
            // 3. 
            
            // You can get a vector by subtracting two points
            
            // Point represents a position consisting of three variables:
            // 1. X - left and right
            // 2. Y - up and down
            // 3. Z - front and back
        }
    }
    
}
