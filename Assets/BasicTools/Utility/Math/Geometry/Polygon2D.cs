using UnityEngine;
using System.Collections.Generic;
using System;

namespace BasicTools.Utility
{
    [Serializable]
    public struct Polygon2D : IShape2D
    {
        public List<Vector2> verticles;

        public Polygon2D(List<Vector2> vertices)
        {
            verticles = vertices;
        }

        public float Area()
        {
            return Mathf.Abs(SignedDoubleArea() * 0.5f);
        }

        public float District()
        {
            int index, nextIndex;
            int n = verticles.Count;
            Vector2 point, next;
            float district = 0;
            for (index = 0; index < n; ++index)
            {
                nextIndex = (index + 1) % n;
                point = verticles[index];
                next = verticles[nextIndex];
                district += (point - next).magnitude;
            }
            return district;
        }

        public Winding GetWinding()
        {
            float signedDoubleArea = SignedDoubleArea();
            if (signedDoubleArea < 0)
            {
                return Winding.Clockwise;
            }
            if (signedDoubleArea > 0)
            {
                return Winding.CounterClockwise;
            }
            return Winding.Non;
        }

        public bool IsInsideShape(Vector2 point)
        {
            throw new System.NotImplementedException();
        }

        float SignedDoubleArea()
        {
            int index, nextIndex;
            int n = verticles.Count;
            Vector2 point, next;
            float signedArea = 0;
            for (index = 0; index < n; ++index) {
                nextIndex = (index + 1) % n;
                point = verticles[index];
                next = verticles[nextIndex];
                signedArea += point.x * next.y - next.x * point.y;
            }
            return signedArea;
        }
    }
}