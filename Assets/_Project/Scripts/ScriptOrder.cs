using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky
{
    /// <summary> Set up script execution order here with matching class name. </summary>
    public static class ScriptOrder
    {
        public const int Manager        = 500;
        public const int GameManager    = Manager;
        public const int SceneManager   = Manager + 1;
        public const int RaceManager    = Manager + 2;
        public const int InputManager   = Manager + 3;
        
        public const int InputHandler   = 600;
        // public const int KeyboardInputHandler   = InputHandler + 1;
        // public const int GamepadInputHandler    = InputHandler + 2;
        // public const int TouchInputHandler      = InputHandler + 3;
        
        public const int Ship           = 1000;
        public const int ShipInput      = Ship + 1;
        public const int ShipMechanics  = Ship + 2;
        public const int ShipEffects    = Ship + 3;
        public const int ShipStatus     = Ship + 4;
        public const int ShipController = Ship + 5;
        public const int ShipPhysics    = Ship + 6;
        public const int ShipModel      = Ship + 7;
        public const int ShipAnimation  = Ship + 8;
        public const int ShipVFX        = Ship + 9;
        public const int ShipAudio      = Ship + 10;
        public const int ShipDebug      = Ship + 11;
        
        public const int Camera         = 1100;
        public const int CameraDamping  = Camera + 1;
        public const int CameraOffset   = Camera + 2;
        public const int CameraFOV      = Camera + 3;
        public const int CameraShake    = Camera + 4;
        public const int CameraSystem   = Camera + 5;
        
        public const int UI             = 1150;
        
        
        
        
    }
}
