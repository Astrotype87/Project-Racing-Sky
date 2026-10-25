using UnityEngine;

namespace AstrotypeLibrary.Oscillator
{
    /// <summary>
    /// Attribute used to make a float, int, Vector2, Vector3, or Vector4
    /// variable in a script be restricted to a specific range.<br/>
    /// Different range settings can also be applied to different axis of vectors.<br/>
    /// <br/>
    /// The foldout arrow option places all the sliders under a foldout.<br/>
    /// Closing the foldout displays the original vector property view instead.
    /// </summary>
    public class SliderAttribute : PropertyAttribute
    {
        public readonly float MinX;
        public readonly float MaxX;
        public readonly float MinY;
        public readonly float MaxY;
        public readonly float MinZ;
        public readonly float MaxZ;
        public readonly float MinW;
        public readonly float MaxW;
        public readonly bool Foldout;
        
        public SliderAttribute(float min, float max, bool foldout = false)
        {
            MinX = MinY = MinZ = MinW = min;
            MaxX = MaxY = MaxZ = MaxW = max;
            Foldout = foldout;
        }
        
        public SliderAttribute(
            float minX, float maxX,
            float minY, float maxY,
            bool foldout = false)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = MinW = 0;
            MaxZ = MaxW = 1;
            Foldout = foldout;
        }
        
        public SliderAttribute(
            float minX, float maxX,
            float minY, float maxY,
            float minZ, float maxZ,
            bool foldout = false)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
            MinW = 0;
            MaxW = 1;
            Foldout = foldout;
        }
        
        public SliderAttribute(
            float minX, float maxX,
            float minY, float maxY,
            float minZ, float maxZ,
            float minW, float maxW,
            bool foldout = false)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
            MinW = minW;
            MaxW = maxW;
            Foldout = foldout;
        }
    }
}
