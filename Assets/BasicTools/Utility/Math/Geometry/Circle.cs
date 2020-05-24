using System;
using UnityEngine;

namespace BasicTools.Utility
{
    [Serializable]
    public struct Circle : IShape2D
    {
        public Vector2 Center;
        public float Radius;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(float cx, float cy, float radius)
        {
            Center = new Vector2(cx, cy);
            Radius = radius;
        }


        public float Diameter
        {
            get { return 2f * Radius; }
            set { Radius = value / 2f; }
        }

        public float Area()
        {
            return Radius * Radius * Mathf.PI;
        }

        public float District()
        {
            return 2f * Radius * Mathf.PI;
        }

        public bool IsInsideShape(Vector2 point)
        {
            return (point - Center).magnitude <= Radius;
        }

        public float? Intersect(Ray ray) {
            float centerLineDistance =MathPlus.DistanceBetweenPointAndLine(Center, ray.origin, (ray.origin+ray.direction));
            if (centerLineDistance < Radius) { return null; }
            else if (centerLineDistance == Radius) { return 0; }
            else {
                throw new NotImplementedException();
            }
        }
    }

}
