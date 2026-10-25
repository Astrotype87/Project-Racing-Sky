using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRacingSky.Input
{
    [DefaultExecutionOrder(ScriptOrder.InputHandler)]
    public abstract class InputHandler : MonoBehaviour
    {
        // PROPERTIES
        public abstract InputEvents InputEvents {protected get; set;}
        public abstract InputActionAsset InputActionAsset {protected get; set;}
    }
}
