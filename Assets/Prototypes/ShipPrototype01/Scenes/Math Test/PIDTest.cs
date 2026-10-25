using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDTest : MonoBehaviour
{
    public Rigidbody rb;
    public Rigidbody tg;
    
    [Range(-5, 5)] public float target;
    [Range(-5, 5)] public float current;
    
    [Range(0, 1)] public float force;
    [Range(0, 1)] public float damping;
    
    
    private void FixedUpdate()
    {
        current = transform.position.x;
        
        float distance = target - current;
        float speed = rb.linearVelocity.x;
        
        float force = distance * Mathf.Pow(Time.fixedDeltaTime, -2) * this.force;
        float damping = speed * (1 / Time.fixedDeltaTime) * this.damping;
        
        rb.AddForce((force - damping) * transform.right, ForceMode.Acceleration);
        tg.position = new Vector3(target, 1, -1.5f);
        
        Debug.Log("Current Pos: " + current
            +  "\tTarget Pos: " + target
            +  "\tSpeed: " + speed
            +  "\tForce: " + (force - damping));
    }
    
}
