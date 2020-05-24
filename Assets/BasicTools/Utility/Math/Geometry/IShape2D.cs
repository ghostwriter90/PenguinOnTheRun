using UnityEngine;

namespace BasicTools.Utility
{
    public interface IShape2D
    {
        bool IsInsideShape(Vector2 point);
        float District();
        float Area();
    }
}
