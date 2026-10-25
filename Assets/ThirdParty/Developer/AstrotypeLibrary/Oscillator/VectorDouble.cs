using System;

namespace AstrotypeLibrary.Oscillator
{
    /// <summary>
    /// Representation of 2D vectors and points using doubles.
    /// </summary>
    [Serializable]
    public struct Vector2Double
    {
        public double x, y;
        
        /// <summary> Shorthand for writing Vector2Double(0, 0). </summary>
        public static Vector2Double zero => new Vector2Double(0, 0);
        
        public Vector2Double(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        
        public double this[int index]
        {
            get => index switch {
                0 => x, 1 => y, _=> 0
            };
            set { switch (index) {
                case 0: x = value; break;
                case 1: y = value; break;
            } }
        }
    }
    
    /// <summary>
    /// Representation of 3D vectors and points using doubles.
    /// </summary>
    [Serializable]
    public struct Vector3Double
    {
        public double x, y, z;
        
        /// <summary> Shorthand for writing Vector3Double(0, 0, 0). </summary>
        public static Vector3Double zero => new Vector3Double(0, 0, 0);
        
        public Vector3Double(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public double this[int index]
        {
            get => index switch {
                0 => x, 1 => y, 2 => z, _=> 0
            };
            set { switch (index) {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
            } }
        }
    }
    
    
}
