using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDTest2 : MonoBehaviour
{
    public Rigidbody targetRb;
    public Rigidbody currentRb;
    
    [Header("Input Values")]
    [Range(-5, 5)] public float targetValue;
    [Range(-5, 5)] public float currentValue;
    
    public float pCoeff;
    public float iCoeff;
    public float dCoeff;
    
    [Header("PID Computations")]
    public float proportional;
    public float derivative;
    public float integral;
    public float output;
    
    
    private float lastProportional;
    
    private void FixedUpdate()
    {
        float currentValue = transform.position.x;
        float errorValue = targetValue - currentValue;
        
        proportional = pCoeff * errorValue;
        integral += iCoeff * proportional;
        
        float speed = (proportional - lastProportional);
        lastProportional = proportional;
        
        derivative = dCoeff * -speed;
        
        output = proportional - integral - derivative;
        
        currentRb.AddForce(output * transform.right, ForceMode.Acceleration);
        targetRb.position = new Vector3(targetValue, 1, -1.5f);
    }
    
}
