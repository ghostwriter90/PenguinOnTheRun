using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BasicTools.Utility
{

    public enum Sequence { Forward, Backward }
    public enum Winding
    {
        Non = 0, Clockwise = -1, CounterClockwise = 1
    }

    public static partial class MathPlus
    {
        public const float InchToCm = 2.54f;
        public const float CmToInch = 0.3937007874f;

        /// <summary>
        /// Rotates a point around the given pivot. (Dec)
        /// </summary>
        public static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
        {
            angle = angle * (Mathf.PI / 180f);
            var rotatedX = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.y - pivot.y) + pivot.x;
            var rotatedY = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.y - pivot.y) + pivot.y;
            return new Vector2(rotatedX, rotatedY);
        }

        /// <summary>
        /// Rotates a point around the given pivot.
        /// </summary>
        /// <returns>The new point position.</returns>
        /// <param name="point">The point to rotate.</param>
        /// <param name="pivot">The pivot's position.</param>
        /// <param name="angles">The angle as a Vector3.</param>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
        {
            // we get point direction from the point to the pivot
            Vector3 direction = point - pivot;
            // we rotate the direction
            direction = Quaternion.Euler(angle) * direction;
            // we determine the rotated point's position
            point = direction + pivot;
            return point;
        }

        /// <summary>
        /// Rotates a vector2 by the angle (in radian) specified and returns it
        /// </summary>
        public static Vector2 RadianToVector2D(float angle)
        {
            float sinus = Mathf.Sin(angle);
            float cosinus = Mathf.Cos(angle);
            return new Vector2( cosinus, sinus);
            // return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static float Vector2DToRadian(Vector2 vec)
        {
            return Mathf.Atan2(vec.y, vec.x);
        }
        
        /// <summary>
        /// Rotates a vector2 by the angle (in degrees) specified and returns it
        /// </summary>
        public static Vector2 RotateVector2Deg(Vector2 vector, float angle)
        {
            if (angle == 0)
            {
                return vector;
            }
            float sinus = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cosinus = Mathf.Cos(angle * Mathf.Deg2Rad);

            float oldX = vector.x;
            float oldY = vector.y;
            vector.x = (cosinus * oldX) - (sinus * oldY);
            vector.y = (sinus * oldX) + (cosinus * oldY);
            return vector;
        }

        /// <summary>
        /// Computes and returns the angle between two vectors, on a 360° scale
        /// </summary>
        public static float AngleBetweenDeg(Vector2 vectorA, Vector2 vectorB)
        {
            float angle = Vector2.Angle(vectorA, vectorB);
            Vector3 cross = Vector3.Cross(vectorA, vectorB);

            if (cross.z > 0)
            {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// Returns the distance between a point and a line.
        /// </summary>
        public static float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointOnLine(point, lineStart, lineEnd) - point);
        }

        /// <summary>
        /// Projects a point on a line (perpendicularly) and returns the projected point.
        /// </summary>
        public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector3 lhs = vector2;
            if (magnitude > 1E-06f)
            {
                lhs = (Vector3)(lhs / magnitude);
            }
            float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
            return (lineStart + ((Vector3)(lhs * num2)));
        }
        
        public static bool TryGetLineSegmentsIntersectionPoint(Vector2 line1A, Vector2 line1B, Vector2 line2A, Vector3 line2B, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            var d = (line1B.x - line1A.x) * (line2B.y - line2A.y) - (line1B.y - line1A.y) * (line2B.x - line2A.x);

            if (d == 0.0f)
            {
                return false;
            }

            var u = ((line2A.x - line1A.x) * (line2B.y - line2A.y) - (line2A.y - line1A.y) * (line2B.x - line2A.x)) / d;
            var v = ((line2A.x - line1A.x) * (line1B.y - line1A.y) - (line2A.y - line1A.y) * (line1B.x - line1A.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
            {
                return false;
            }

            intersection.x = line1A.x + u * (line1B.x - line1A.x);
            intersection.y = line1A.y + u * (line1B.y - line1A.y);

            return true;
        }

        public static Vector2? LineSegmentsIntersection(Vector2 line1A, Vector2 line1B, Vector2 line2A, Vector3 line2B)
        {
            Vector2 intersection = Vector2.zero;

            var d = (line1B.x - line1A.x) * (line2B.y - line2A.y) - (line1B.y - line1A.y) * (line2B.x - line2A.x);

            if (d == 0.0f)
            {
                return null;
            }

            var u = ((line2A.x - line1A.x) * (line2B.y - line2A.y) - (line2A.y - line1A.y) * (line2B.x - line2A.x)) / d;
            var v = ((line2A.x - line1A.x) * (line1B.y - line1A.y) - (line2A.y - line1A.y) * (line1B.x - line1A.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
            {
                return null;
            }

            intersection.x = line1A.x + u * (line1B.x - line1A.x);
            intersection.y = line1A.y + u * (line1B.y - line1A.y);

            return intersection;
        }


        // Clamp vector
        public static Vector2 Clamp(Vector2 input, float minX, float minY, float maxX, float maxY)
        {
            return new Vector2(
                input.x < minX ? minX : (input.x > maxX ? maxX : input.x),
                input.y < minY ? minY : (input.y > maxY ? maxY : input.y));
        }

        public static Vector3 Clamp(Vector3 input, float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            return new Vector3(
                input.x < minX ? minX : (input.x > maxX ? maxX : input.x),
                input.y < minY ? minY : (input.y > maxY ? maxY : input.y),
                input.z < minZ ? minZ : (input.z > maxY ? maxZ : input.z));
        }

        public static Vector2 Clamp(Vector2 input, Vector2 min, Vector2 max)
        {
            return new Vector2(
                input.x < min.x ? min.x : (input.x > max.x ? max.x : input.x),
                input.y < min.y ? min.y : (input.y > max.y ? max.y : input.y));
        }

        public static Vector3 Clamp(Vector3 input, Vector3 min, Vector3 max)
        {
            return new Vector3(
                input.x < min.x ? min.x : (input.x > max.x ? max.x : input.x),
                input.y < min.y ? min.y : (input.y > max.y ? max.y : input.y),
                input.z < min.z ? min.z : (input.z > max.y ? max.z : input.z));
        }
    }
}