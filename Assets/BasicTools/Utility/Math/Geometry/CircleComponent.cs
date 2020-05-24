using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace BasicTools.Utility
{
    class CircleComponent : Shape2DComponent
    {
        public const float circleDrawingPrecision = 300;
        
        public Circle circle;

        public override bool IsInsideShape(Vector2 point)
        {
            ThrowExceptionIfShapeIsIn3DMode();
            return circle.IsInsideShape(WorldPointToShapeSpace(point));
        }

        protected override void DrawShape(Drawer.DrawingMode drawingMode)
        {
            int circleFragments = GetFragmentCount(circleDrawingPrecision);
            if (transformType == Shape2DTransformType.Self3D)
            {
                Drawer.DrawCircle(circle, DrawingSettings.Color, transform, drawingMode, circleFragments);
            }
            else
            {
                Vector3 center = ShapeSpacePointToWorldSpace( circle.Center);
                Drawer.DrawCircle(center, circle.Radius, DrawingSettings.Color, null, drawingMode, circleFragments);
            }
        }

        public override void SetHandle(int i, Vector3 newPoint)
        {
            if (i == 0)
            {
                circle.Center = newPoint;
            }
            else {
                circle.Radius = newPoint.x - circle.Center.x;
            }
        }

        public override List<HandlePoint> GetHandles()
        {
            if (!DrawingSettings.EnableDrawing || !DrawingSettings.EnableHandles ) { return null; }
            return new List<HandlePoint> {
                new HandlePoint(circle.Center, null, HandlePointShape.Circle),
                new HandlePoint(circle.Center + (Vector2.right * circle.Radius), null, HandlePointShape.Circle)
            };
        }
        
    }        
}