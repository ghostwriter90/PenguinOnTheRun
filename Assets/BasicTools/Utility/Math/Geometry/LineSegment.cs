using UnityEngine;

namespace BasicTools.Utility
{
    public struct LineSegment
    {
        public Vector2 p0;
        public Vector2 p1;

        public LineSegment(Vector2 p0, Vector2 p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }


        public float Length
        {
            get
            {
                return Vector2.Distance(p0, p1);
            }
        }
    }
}