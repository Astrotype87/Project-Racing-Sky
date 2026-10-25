using System;
using UnityEngine;

public class TouchControlButtons : MonoBehaviour
{
    public ButtonEvent ThrustButton;
    public ButtonEvent SteerLeftButton;
    public ButtonEvent SteerRightButton;
    public ButtonEvent AirbrakeFullButton;
    public ButtonEvent AirbrakeLeftButton;
    public ButtonEvent AirbrakeRightButton;
    public ButtonEvent PitchDownButton;
    public ButtonEvent PitchUpButton;
    
    public Action OnTouchControlUpdate;
    
    
    // MONOBEHAVIOUR METHODS
    private void OnEnable()
    {
        ThrustButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        SteerLeftButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        SteerRightButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        AirbrakeFullButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        AirbrakeLeftButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        AirbrakeRightButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        PitchDownButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
        PitchUpButton.OnButtonUpdate += InvokeOnTouchControlUpdate;
    }
    
    private void OnDisable()
    {
        ThrustButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        SteerLeftButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        SteerRightButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        AirbrakeFullButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        AirbrakeLeftButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        AirbrakeRightButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        PitchDownButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
        PitchUpButton.OnButtonUpdate -= InvokeOnTouchControlUpdate;
    }
    
    private void InvokeOnTouchControlUpdate()
    {
        OnTouchControlUpdate?.Invoke();
    }
}
