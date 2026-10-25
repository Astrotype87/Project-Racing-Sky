using UnityEngine;
using UnityEngine.UIElements;

namespace AstrotypeLibrary.Oscillator
{
    public class OscillatorTest : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] private float float1;
        [SerializeField, Slider(0, 10)] private int int1;
        [SerializeField, Slider(0, 1, 2, 3)] private Vector2 vector2;
        [SerializeField, Slider(0, 1, 2, 3, 4, 5)] private Vector3 vector3;
        [SerializeField, Slider(0, 1, 2, 3, 4, 5, 6, 7)] private Vector4 vector4;
    }
}
