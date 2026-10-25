using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Utility
{
    /// <summary>
    /// Collection of useful math extension methods for Vector3 calculations.
    /// </summary>
    public static class Vector3Util
    {
        /// <summary>
        /// Replaces NaN and Infinity components of this Vector3 to zero. Useful for avoiding invalid rigidbody input force.
        /// </summary>
        public static Vector3 NaNInfSafe(this Vector3 vector)
        {
            if (float.IsNaN(vector.x) || float.IsInfinity(vector.x)) vector.x = 0;
            if (float.IsNaN(vector.y) || float.IsInfinity(vector.y)) vector.y = 0;
            if (float.IsNaN(vector.z) || float.IsInfinity(vector.z)) vector.z = 0;
            return vector;
        }
        
        /// <summary>
        /// Divide two vectors by dividing separate components. ex: (a.x / b.x, a.y / b.y, a.z / b.z)
        /// </summary>
        public static Vector3 DivideBy(this Vector3 a, Vector3 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            a.z /= b.z;
            return a;
        }
        
        /// <summary>
        /// Make all components of a vector positive.
        /// </summary>
        public static Vector3 Absolute(this Vector3 a)
        {
            a.x = Mathf.Abs(a.x);
            a.y = Mathf.Abs(a.y);
            a.z = Mathf.Abs(a.z);
            return a;
        }
        
        /// <summary>
        /// Clamps all x, y and z values between zero to one.
        /// </summary>
        public static Vector3 Clamp(this Vector3 a, float min, float max)
        {
            a.x = Mathf.Clamp(a.x, min, max);
            a.y = Mathf.Clamp(a.y, min, max);
            a.z = Mathf.Clamp(a.z, min, max);
            return a;
        }
        
        /// <summary>
        /// Clamps all x, y and z values from given vector in between negative and positive of x, y, z values from minMax vector.
        /// </summary>
        public static Vector3 ClampSigned(this Vector3 value, Vector3 minMax)
        {
            value.x = Mathf.Clamp(value.x, -minMax.x, minMax.x);
            value.y = Mathf.Clamp(value.y, -minMax.y, minMax.y);
            value.z = Mathf.Clamp(value.z, -minMax.z, minMax.z);
            return value;
        }
        
        /// <summary>
        /// Clamps all x, y and z values from given vector in between negative and positive of x, y, z values from minMax vector.
        /// </summary>
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            value.z = Mathf.Clamp(value.z, min.z, max.z);
            return value;
        }
        
        /// <summary>
        /// Clamps all x, y and z values between zero to one.
        /// </summary>
        public static Vector3 Clamp01(this Vector3 a)
        {
            a.x = Mathf.Clamp01(a.x);
            a.y = Mathf.Clamp01(a.y);
            a.z = Mathf.Clamp01(a.z);
            return a;
        }
        
        /// <summary>
        /// Replaces any x, y and z component that is zero with a given replace value
        /// </summary>
        public static Vector3 ReplaceZero(this Vector3 a, float replace)
        {
            if (a.x == 0) a.x = replace;
            if (a.y == 0) a.y = replace;
            if (a.z == 0) a.z = replace;
            return a;
        }
        
        /// <summary>
        /// Returns a vector where each component is squared while preserving the original sign.
        /// </summary>
        public static Vector3 Squared(this Vector3 v)
        {
            return new Vector3(v.x * Mathf.Abs(v.x), v.y * Mathf.Abs(v.y), v.z * Mathf.Abs(v.z));
        }
    }
}
