using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RectExtensions
{
    public static Vector2 TopLeft(this Rect self)
    {
        return new Vector2(self.xMin, self.yMin);
    }

    public static Vector2 TopRight(this Rect self)
    {
        return new Vector2(self.xMax, self.yMin);
    }

    public static Vector2 BottomLeft(this Rect self)
    {
        return new Vector2(self.xMin, self.yMax);
    }

    public static Vector2 BottomRight(this Rect self)
    {
        return new Vector2(self.xMax, self.yMax);
    }

    public static Vector2 LeftPoint(this Rect self)
    {
        return new Vector2(self.xMin, self.center.y);
    }

    public static Vector2 RightPoint(this Rect self)
    {
        return new Vector2(self.xMax, self.center.y);
    }

    public static Vector2 TopPoint(this Rect self)
    {
        return new Vector2(self.center.x, self.yMin);
    }

    public static Vector2 BottomPoint(this Rect self)
    {
        return new Vector2(self.center.x, self.yMax);
    }
}

