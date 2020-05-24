using System;
using UnityEngine;

namespace BasicTools.Utility
{
    [Serializable]
    public struct Ellipse : IShape2D
    {
        public Vector2 Center;
        public float radiusHorizontal;
        public float radiusVertical;
        public float rotationInDeg;

        public Ellipse(Vector3 center, float radiusHorizontal, float radiusVertical, float rotationInDeg)
        {
            Center = center;
            this.radiusHorizontal = radiusHorizontal;
            this.radiusVertical = radiusVertical;
            this.rotationInDeg = rotationInDeg;
        }

        public float Area()
        {
            throw new NotImplementedException();
        }

        public float District()
        {
            throw new NotImplementedException();
        }

        public bool IsInsideShape(Vector2 point)
        {
            Vector2 vec = point - Center;
            vec = MathPlus.RotateVector2Deg(vec, rotationInDeg);
            vec = new Vector2(vec.x / radiusHorizontal, vec.y / radiusVertical);
            return vec.magnitude <= 1;
        }
        
    }
}
