using System;
using UnityEngine;

namespace AstrotypeLibrary.Oscillator
{
    /// <summary>
    /// A time stateful 3D wave generator, containing settings to configure wave generation for each axes. <br/>
    /// * Note: The same wave type will be applied to all axes, but with different settings.
    /// </summary>
    public class Wave3D
    {
        // Global settings
        public bool mute;
        public float amplitude = 1;
        public float frequency = 1;
        public WaveType type = WaveType.Sine;
        public AnimationCurve customWave = AnimationCurve.Linear(0, 0, 1, 1);
        
        // Wave settings
        public Vector3 amplitude3D = Vector3.one;
        public Vector3 frequency3D = Vector3.one;
        
        // Wave type settings
        public Vector3 phase;
        
        // Noise type settings
        public Vector3Int seed = new(0, 1, 2);
        public Vector3Int range = Vector3Int.one * 100;
        public Vector3Int offset = Vector3Int.zero;
        public int octave = 0;
        
        // Output
        [SerializeField] private Vector3 time;
        [SerializeField, Slider(-1, 1)] private Vector3 output;
        
        // Constants
        /// <summary> Used to prevent floating point precision loss with large time values. </summary>
        private const int TIME_RANGE = 1000;
        
        
        // PUBLIC METHODS
        public Vector3 UpdateOutput(float deltaTime)
        {
            if (mute) return Vector3.zero;
            
            output.x = UpdateValue(0, deltaTime);
            output.y = UpdateValue(1, deltaTime);
            output.z = UpdateValue(2, deltaTime);
            
            return output;
        }
        
        // PRIVATE METHODS
        private float UpdateValue(int index, float deltaTime)
        {
            // Update time with frequency and offset
            float time = this.time[index] + offset[index];
            time += deltaTime * frequency * frequency3D[index];
            
            // Loop time starting from 0 and repeat in opposite sign
            if (time > TIME_RANGE) time -= 2 * TIME_RANGE;
            else if (time < -TIME_RANGE) time += 2 * TIME_RANGE;
            
            // Evaluate wave
            float output = type switch
            {
                WaveType.Sine => WaveUtil.Sine(time, phase[index]),
                WaveType.Triangle => WaveUtil.Triangle(time, phase[index]),
                WaveType.Sawtooth => WaveUtil.Sawtooth(time, phase[index]),
                WaveType.Square => WaveUtil.Square(time, phase[index]),
                WaveType.Perlin => WaveUtil.StandardPerlin(time, seed[index], range[index], octave),
                WaveType.UnityPerlin => WaveUtil.UnityPerlin(time, seed[index], range[index]),
                WaveType.Custom => WaveUtil.Custom(time, customWave, phase[index]),
                _ => WaveUtil.Sine(time, phase[index]),
            };
            
            // Save updated time
            this.time[index] = time;
            
            // Return final output
            return output * amplitude * amplitude3D[index];
        }
    }
    
    public enum WaveType
    {
        Sine = 1,
        Triangle = 2,
        Sawtooth = 3,
        Square = 4,
        Perlin = 5,
        UnityPerlin = 6,
        Custom = 0,
    }
    
    
}
