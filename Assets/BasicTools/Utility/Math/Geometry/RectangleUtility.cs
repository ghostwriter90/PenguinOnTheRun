using System.Collections;


namespace BasicTools.Utility
{
    public static class RectangleUtility
    {
        public static bool IsOverlappingRectangle(float startA, float endA, float startB, float endB)
        {
            return (startA < endB) && (endA > startB);
        }
    }
}