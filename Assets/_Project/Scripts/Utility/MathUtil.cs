using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In-project tools and helper methods for specific calculation cases.
/// </summary>
namespace ProjectRacingSky.Utility
{
    /// <summary>
    /// Collection of useful math extension methods for float calculations.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// Replaces NaN float value to a specified value. Useful for avoiding invalid rigidbody input force. 
        /// </summary>
        public static float NaNSafe(this float value, float replace = 0.0f)
            => float.IsNaN(value) ? replace : value;
        
        /// <summary>
        /// Replaces Infinity float value to a specified value. Useful for avoiding invalid rigidbody input force. 
        /// </summary>
        public static float InfSafe(this float value, float replace = 0.0f)
            => float.IsInfinity(value) ? replace : value;
        
        /// <summary>
        /// Replaces NaN or Infinity float value to a specified value. Useful for avoiding invalid rigidbody input force. 
        /// </summary>
        public static float NaNInfSafe(this float value, float replace = 0.0f)
            => float.IsNaN(value) || float.IsInfinity(value) ? replace : value;
        
        /// <summary>
        /// Replaces NaN or Infinity float value to a specified value. Useful for avoiding invalid rigidbody input force. 
        /// </summary>
        public static double NaNInfSafe(this double value, double replace = 0.0)
            => double.IsNaN(value) || double.IsInfinity(value) ? replace : value;
        
        
        /// <summary>
        /// <para> Mathf.Clamp method as extension method. </para>
        /// <para> Clamps and returns the given value between the given minimum float and maximum float values. </para>
        /// </summary>
        public static float Clamp(this float value, float min, float max)
            => Mathf.Clamp(value, min, max);
        
        /// <summary>
        /// <para> Mathf.Clamp01 method as extension method. </para>
        /// <para> Clamps value between 0 and 1 and returns value. </para>
        /// </summary>
        public static float Clamp01(this float value)
            => Mathf.Clamp01(value);
        
        /// <summary>
        /// <para> Mathf.Clamp01 method as extension method. </para>
        /// <para> Clamps value between -1 and 1 and returns value. </para>
        /// </summary>
        public static float Clamp01Mirror(this float value)
            => Mathf.Clamp(value, -1, 1);
        
        /// <summary>
        /// <para> Clamps and returns the given value between negative limit value and positive limit value. </para>
        /// </summary>
        public static float ClampMirror(this float value, float limit)
            => Mathf.Clamp(value, -limit, limit);
        
        /// <summary>
        /// Converts a bool value into integer. Returns 1 if true and 0 if false.
        /// </summary>
        public static float ToInt(this bool value)
            => value ? 1 : 0;
        
        /// <summary>
        /// Converts a bool value into a positive or negative integer. Returns 1 if true and -1 if false.
        /// </summary>
        public static float ToSign(this bool value)
            => value ? 1 : -1;
        
        /// <summary>
        /// Linearly interpolates between a and b by t.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation value between the two floats.</param>
        /// <returns>The interpolated float result between the two float values.</returns>
        public static double Lerp(double a, double b, double t)
        {
            t = Math.Clamp(t, 0.0, 1.0);
            return a + (b - a) * t;
        }
        
        /// <summary>
        /// Returns true if values are not zero and have opposite signs.
        /// </summary>
        public static bool IsOppositeSign(float a, float b)
        {
            // return a == 0 || b == 0 || Mathf.Sign(a) == Mathf.Sign(b) ? false : true;
            return a != 0 && b != 0 && Mathf.Sign(a) != Mathf.Sign(b);
        }
        
        
        /// <summary>
        /// Returns the nth term from the triangular sequence (0, 1, 3, 6, 10, 15, 21, etc.) 
        /// </summary>
        public static float TriangularSequence(float n)
            => (n * n + n) * 0.5f;
        
        /// <summary>
        /// Returns smooth value between -1 and 1 using Unity's Mathf.PerlinNoise.
        /// </summary>
        public static float PerlinNoise(float pos, float seed)
        {
            float xSeed = seed > 0 ? seed : 0;
            float ySeed = seed < 0 ? -seed : 0;
            return (Mathf.PerlinNoise(pos + xSeed, pos + ySeed) - Mathf.PerlinNoise(0, 0)) * Mathf.Sign(pos) * 2;
        }
        
        /// <summary>
        /// Double version of Mathf.Repeat().
        /// </summary>
        public static double Repeat(double t, double length)
            => t - Math.Floor(t / length) * length;
        
        /// <summary>
        /// <para> Double version of Mathf.Repeat(). </para>
        /// <para> Starts value at 0 and returns value between negative and positive length. </para>
        /// </summary>
        public static double RepeatSigned(double t, double length)
            => (t - Math.Floor(t / length / 2 + 0.5) * length * 2).NaNInfSafe();
        
        /// <summary>
        /// Double version of Mathf.PingPong().
        /// </summary>
        public static double PingPong(double t, double length)
            => Math.Abs(Repeat(t + length, length * 2) - length);
        
        
        /// <summary>
        ///     <para> Moves a value current towards target based on increase and decrease time. </para>
        ///     <para> If the current value is less than target value, gain time is used, else release time is used. </para>
        /// </summary>
        /// <param name="current"> The current value. </param>
        /// <param name="target"> The value to move towards. </param>
        /// <param name="gain"> The increase-only time to reach target value. </param>
        /// <param name="release"> The decrease-only time reach target value. </param>
        /// <param name="deltaTime"> The amount of time passed since last updated. </param>
        /// <param name="stepBased"> If true, gain/release times are used as step amounts instead of time-based. </param>
        public static float MoveToTarget(this float current, float target, float gain, float release, float deltaTime, bool stepBased = false)
        {
            float moveTime = current < target ? gain : release;
            float time = stepBased ? moveTime : deltaTime / moveTime;
            return Mathf.MoveTowards(current, target, time);
        }
        
        /// <summary>
        ///     <para> Moves a value current towards target, with a special case of choosing between increase or decrease time. </para>
        ///     <para> If the current absolute value is less than target absolute value, gain time is used, otherwise release time is used. </para>
        ///     <para> If the current sign is opposite to target sign, release time is used. </para>
        /// </summary>
        /// <param name="current"> The current value. </param>
        /// <param name="target"> The value to move towards. </param>
        /// <param name="gain"> The increase-only time to reach target value. </param>
        /// <param name="release"> The decrease-only time reach target value. </param>
        /// <param name="deltaTime"> The amount of time passed since last updated. </param>
        public static float MoveToTargetSigned(this float current, float target, float gain, float release, float deltaTime)
        {
            bool isNonZero = current != 0 && target != 0;
            bool isOppositeTarget = Mathf.Sign(current) != Mathf.Sign(target) && isNonZero;
            bool isGreaterTarget = Mathf.Abs(current) < Mathf.Abs(target) && !isOppositeTarget;
            
            float time = isGreaterTarget ? gain : Mathf.Max(release, deltaTime / Mathf.Abs(current));
            return Mathf.MoveTowards(current, target, deltaTime / time);
        }
        
        /// <summary>
        /// Check if a is greater than b or difference between a and b is smaller than gap
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="gap"></param>
        /// <returns>true if a > b or difference between a and b is smaller than gap</returns>
        public static bool IsGreaterOrEqualTo(this float a, float b, float gap = 0.001f)
            => a > b || Mathf.Abs(b - a) < gap;
        
        
    }
    
}
