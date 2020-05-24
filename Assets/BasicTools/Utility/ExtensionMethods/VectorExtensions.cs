using BasicTools.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ExtendToVector3(this Vector2 target, float newZValue)
    {
        return new Vector3(target.x, target.y, newZValue);
    }

    public static Vector2 Round(this Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }

    public static Vector3 Round(this Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static Vector2 ToVector2(this Vector3 vector, Axis deletable) {
        switch (deletable)
        {
            case Axis.X: return new Vector2(vector.y, vector.z);
            case Axis.Y: return new Vector2(vector.x, vector.z);
            case Axis.Z: return new Vector2(vector.x, vector.y);
        }
        return Vector2.zero;
    }

    public static Vector3 ToVector3(this Vector2 vector,  float newValue, Axis newAxis)
    {
        switch (newAxis)
        {
            case Axis.X: return new Vector3(newValue, vector.x, vector.y);
            case Axis.Y: return new Vector3(vector.x, newValue, vector.y);
            case Axis.Z: return new Vector3(vector.x, vector.y, newValue);
        }
        return Vector3.zero;
    }

    public static Vector3 MulipleAllAxis(this Vector3 vector, Vector3 multiplier)
    {
        return new Vector3(vector.x * multiplier.x, vector.y * multiplier.y, vector.z * multiplier.z);
    }

    public static Vector2 MulipleAllAxis(this Vector2 vector, Vector2 multiplier)
    {
        return new Vector2(vector.x * multiplier.x, vector.y * multiplier.y);
    }
}
    

