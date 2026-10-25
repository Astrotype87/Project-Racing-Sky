using UnityEngine;

namespace ProjectRacingSky.Input
{
    public interface IShipActionListener
    {
        public void SetThrust(float value);
        public void SetReverse(float value);
        public void SetAfterburner(bool value);
        public void SetAirbrakeLeft(float value);
        public void SetAirbrakeRight(float value);
        public void SetStrafe(float value);
        public void SetSteer(float value);
        public void SetPitch(float value);
        public void SetRoll(float value);
        
        public void InvokeHover(bool isOn);
        public void InvokeJump();
        public void InvokeBoost();
        public void InvokeSideshift(bool direction);
        
        public void InvokeUsePickup();
        public void InvokeDrainPickup();
        public void InvokeRespawn();
        public void InvokeCanopy();
    }
}
