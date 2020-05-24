using UnityEngine;

namespace BasicTools.Utility
{
    public enum HandlePointShape { Circle, Square }

    public class HandlePoint
    {
        public string label = null;
        public Vector3 position;
        public Color color;
        public HandlePointShape shape = HandlePointShape.Circle;

        public HandlePoint() { }

        public HandlePoint(Vector3 position, string label = null, HandlePointShape shape = HandlePointShape.Circle)
        {
            this.label = label;
            this.position = position;
            this.shape = shape;
        }

        public HandlePoint(string label, Vector3 position, Color color, HandlePointShape shape = HandlePointShape.Circle)
        {
            this.label = label;
            this.position = position;
            this.color = color;
            this.shape = shape;
        }
    }
}
