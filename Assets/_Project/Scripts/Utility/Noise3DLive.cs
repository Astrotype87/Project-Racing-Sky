using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TetraCreations.Attributes;

namespace ProjectRacingSky.Utility
{
    public enum ShakeMode {SineWave, PerlinNoise}
    
    /// <summary>
    /// A 3D noise generator, containing settings to configure noise generation for each axes.
    /// </summary>
    [Serializable] public class Noise3DLive
    {
        // INSPECTOR FIELDS
        public bool disabled;
        public float ampMix = 1;
        public float freqMix = 1;
        public Vector3 amplitude = Vector3.zero;
        public Vector3 frequency = new Vector3(2, 4, 1);
        public ShakeMode mode = ShakeMode.SineWave;
        [DrawIf("mode", ShakeMode.SineWave)] public Vector3 phase;
        [DrawIf("mode", ShakeMode.PerlinNoise)] public Vector3Int seed = Vector3Int.zero;
        [DrawIf("mode", ShakeMode.PerlinNoise)] public Vector3Int range = Vector3Int.one * 100;
        [DrawIf("mode", ShakeMode.PerlinNoise)] public Vector3Int offset = Vector3Int.zero;
        
        [Space]
        [ReadOnly] [SerializeField] private Vector3Double time;
        [ReadOnly] [SerializeField] [Range(-1, 1)] private float outputX;
        [ReadOnly] [SerializeField] [Range(-1, 1)] private float outputY;
        [ReadOnly] [SerializeField] [Range(-1, 1)] private float outputZ;
        
        
        // METHODS
        public void Reset()
        {
            time = Vector3Double.zero;
            outputX = outputY = outputZ = 0;
        }
        
        public Vector3 UpdateValue(double deltaTime, float ampMod, float freqMod)
        {
            if (disabled) return Vector3.zero;
            
            // UPDATE TIME
            for (int i = 0; i < 3; i++)
            {
                float freq = frequency[i] * freqMix * freqMod;
                time[i] += deltaTime * freq;
            }
            
            // UPDATE OUTPUT
            Vector3 output = Vector3.zero;
            if (mode == ShakeMode.SineWave)
            {
                for (int i = 0; i < 3; i++)
                    output[i] = (float)Math.Sin(Math.PI * (time[i] + phase[i] * 2));
            }
            else if (mode == ShakeMode.PerlinNoise)
            {
                for (int i = 0; i < 3; i++)
                {
                    float value = (float)MathUtil.RepeatSigned(time[i], range[i]) + offset[i];
                    output[i] = MathUtil.PerlinNoise(Mathf.Abs(value) + offset[i], seed[i]) * Mathf.Sign(value);
                }
            }
            
            outputX = output.x; outputY = output.y; outputZ = output.z;
            return Vector3.Scale(output, amplitude) * ampMix * ampMod;
        }
        
    }
}
