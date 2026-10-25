using System;
using UnityEngine;

namespace AstrotypeLibrary.Oscillator
{
    /// <summary>
    /// Utility class for evaluating different wave types.<br/>
    /// * Note: All wave functions are designed to output values in the range of -1 to 1 for inputs in the range of 0 to 1, but this may not be strictly enforced for all wave types.
    /// </summary>
    public static class WaveUtil
    {
        /// <summary> Standard sine wave. </summary>
        public static float Sine(float x, float phase)
        {
            x += phase / 360f;
            return Mathf.Sin(x * 2 * Mathf.PI);
        }
        
        /// <summary> Standard triangle wave. </summary>
        public static float Triangle(float x, float phase)
        {
            x += phase / 360f;
            return 2 * Mathf.Abs(2 * (x - Mathf.Floor(x + 0.75f)) - 1) - 1;
        }
        
        /// <summary> Standard sawtooth wave. </summary>
        public static float Sawtooth(float x, float phase)
        {
            x += phase / 360f;
            return 2 * (x - Mathf.Floor(x + 0.5f));
        }
        
        /// <summary> Standard square wave. </summary>
        public static float Square(float x, float phase)
        {
            x += phase / 360f;
            return Mathf.Sign(Mathf.Sin(x * 2 * Mathf.PI));
        }
        
        /// <summary>  Standard Perlin noise with multiple octaves. </summary>
        public static float StandardPerlin(float x, int seed, float range, int octave)
        {
            x = RepeatSigned(x, range);
            return Perlin.Fbm(Mathf.Abs(x), seed, octave + 1) * Mathf.Sign(x) * 4f;
        }
        
        /// <summary> Unity's built-in Perlin noise function. </summary>
        public static float UnityPerlin(float x, float seed, float range)
        {
            const float OFFSET = 0.4652731f;
            const float SCALE = 2.95f;
            x = RepeatSigned(x, range);
            return (Mathf.PerlinNoise(Mathf.Abs(x), seed) - OFFSET) * SCALE * Mathf.Sign(x);
        }
        
        /// <summary> Custom wave defined by an AnimationCurve. </summary>
        public static float Custom(float x, AnimationCurve curve, float phase)
        {
            x += phase / 360f;
            return curve.Evaluate(x - Mathf.Floor(x));
        }
        
        
        /// <summary> Repeats a value within a range while keeping it centered around zero. </summary>
        private static float RepeatSigned(float value, float length)
        {
            if (length == 0) return 0;
            
            float mod = value % length;
            if (mod < 0) mod += length;
            return mod - length / 2;
        }
    }
}
