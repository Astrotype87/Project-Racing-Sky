using UnityEngine;

namespace Archived
{
    public class CalculateAngleDifference : MonoBehaviour
    {
        [Header("Rotation Input")]
        public Vector3 firstRotation;
        public Vector3 secondRotation;
        [Range(0, 1)] public float rotationLerp;
        
        [Header("Rotation Output")]
        public Vector3 rotation1Euler;
        public Vector3 rotation2Euler;
        public Vector3 lerpedRotationEuler;
        
        [Header("Delta Rotation Input")]
        public Vector3 deltaRotation1Euler;
        public Vector3 deltaRotation2Euler;
        [Range(0, 1)] public float deltaRotationLerp;
        
        [Header("Delta Rotation Output")]
        public Vector3 lerpedDeltaRotationEuler;
        
        
        private Quaternion rotation1;
        private Quaternion rotation2;
        private Quaternion lerpedRotation;
        private Quaternion deltaRotation1;
        private Quaternion deltaRotation2;
        private Quaternion lerpedDeltaRotation;
        
        
        [System.Serializable]
        public class QuaternionWithSliders
        {
            [Range(-1, 1)] public float w;
            [Range(-1, 1)] public float x;
            [Range(-1, 1)] public float y;
            [Range(-1, 1)] public float z;
            
            public void SetQuaternionValues(Quaternion quaternion)
            {
                this.w = quaternion.w;
                this.x = quaternion.x;
                this.y = quaternion.y;
                this.z = quaternion.z;
            }
        }
        
        private void OnDrawGizmos()
        {
            rotation1 = Quaternion.Euler(firstRotation);
            rotation2 = Quaternion.Euler(secondRotation);
            lerpedRotation = Quaternion.Lerp(rotation1, rotation2, rotationLerp);
            deltaRotation1 = rotation2 * Quaternion.Inverse(rotation1);
            deltaRotation2 = Quaternion.Euler(deltaRotation2Euler);
            lerpedDeltaRotation = Quaternion.Lerp(deltaRotation1, deltaRotation2, deltaRotationLerp);
            
            rotation1Euler = rotation1.eulerAngles;
            rotation2Euler = rotation2.eulerAngles;
            lerpedRotationEuler = lerpedRotation.eulerAngles;
            deltaRotation1Euler = deltaRotation1.eulerAngles;
            deltaRotation2Euler = deltaRotation2.eulerAngles;
            lerpedDeltaRotationEuler = lerpedDeltaRotation.eulerAngles;
            
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(Vector3.zero, rotation1 * Vector3.forward * 2);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Vector3.zero, rotation2 * Vector3.forward * 1.5f);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(Vector3.zero, lerpedRotation * Vector3.forward * 1);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(Vector3.back * 2, deltaRotation1 * Vector3.forward * 1.5f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Vector3.back * 2, deltaRotation2 * Vector3.forward * 1f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(Vector3.back * 2, lerpedDeltaRotation * Vector3.forward * 0.75f);
            
            
            /* quaternionRotation = new QuaternionWithSliders();
            // quaternionRotation.SetQuaternionValues(Quaternion.Euler(eulerRotation));
            
            // Gizmos.color = Color.green;
            // Gizmos.DrawRay(Vector3.zero, Quaternion.Euler(eulerRotation) * Vector3.forward * 2);
            
            // shipRotationReset = Quaternion.Euler(shipRotation).eulerAngles;
            // currentOrbitReset = Quaternion.Euler(currentOrbit).eulerAngles;
            
            // shipRotationInverse = Quaternion.Inverse(Quaternion.Euler(shipRotation)).eulerAngles;
            
            // shipRotationCalib = shipRotationReset;
            // currentOrbitCalib = currentOrbitReset;
            
            
            // for (int i = 0; i < 3; i++)
            // {
            //     if (shipRotationCalib[i] > 180) shipRotationCalib[i] -=360;
            //     if (currentOrbitCalib[i] > 180) currentOrbitCalib[i] -=360;
            // }
            
            // difference = shipRotationCalib - currentOrbitCalib;
            
            // Gizmos.color = Color.blue;
            // Gizmos.DrawRay(Vector3.zero, Quaternion.Euler(shipRotation) * Vector3.forward * lineLength);
            // Gizmos.color = Color.green;
            // Gizmos.DrawRay(Vector3.zero, Quaternion.Euler(shipRotationInverse) * Vector3.forward * lineLength); */
        }
        
        /* private Vector3 AngleDifference(Vector3 a, Vector3 b)
        // {
        //     return new Vector3(
        //         (a.x - b.x + 540) % 360 - 180,
        //         (a.y - b.y + 540) % 360 - 180,
        //         (a.z - b.z + 540) % 360 - 180
        //     );
        // }
        
        // private float AngleDifference(float a, float b)
        // {
        //     return (a - b + 540) % 360 - 180;
        // }*/
    }
    
}
