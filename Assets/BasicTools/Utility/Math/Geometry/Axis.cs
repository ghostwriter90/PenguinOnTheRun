
using UnityEngine;

namespace BasicTools.Utility
{
    public enum Axis : byte { Z, Y, X }

    public static class AxisExpanded
    {
        public static Vector3 ToPositiveVector(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return Vector3.right;
                case Axis.Y: return Vector3.up;
                case Axis.Z: return Vector3.forward;
            }
            return new Vector3();
        }

        public static Direction6 ToPositiveDirection(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return Direction6.Right;
                case Axis.Y: return Direction6.Up;
                case Axis.Z: return Direction6.Forward;
            }
            return Direction6.Right;
        }

        public static Vector3 ToNegativeVector(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return Vector3.left;
                case Axis.Y: return Vector3.down;
                case Axis.Z: return Vector3.back;
            }
            return new Vector3();
        }

        public static Direction6 ToNegativeDirection(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return Direction6.Left;
                case Axis.Y: return Direction6.Down;
                case Axis.Z: return Direction6.Back;
            }
            return Direction6.Right;
        }
    }
}
