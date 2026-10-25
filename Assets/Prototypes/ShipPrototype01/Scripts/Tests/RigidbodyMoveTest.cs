using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archived
{
    public class RigidbodyMoveTest : MonoBehaviour
    {
        public Vector3 moveSpeed;
        public Vector3 velocity;
        Rigidbody cubeRb;
        
        // Start is called before the first frame update
        void Start()
        {
            cubeRb = GetComponent<Rigidbody>();
        }

        // FixedUpdate is called once per fixed delta tine
        void FixedUpdate()
        {
            cubeRb.AddForce(moveSpeed - cubeRb.linearVelocity, ForceMode.VelocityChange);
            velocity = cubeRb.linearVelocity;
        }
    }
    
}
