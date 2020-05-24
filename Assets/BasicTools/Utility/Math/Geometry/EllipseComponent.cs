using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicTools.Utility
{
    public class EllipseComponent : Shape2DComponent
    {
        public const float ellipseDrawingPrecision = 300;
        public Ellipse ellipse = new Ellipse(new Vector3(0,0,0), 2,1,0);

        public override bool IsInsideShape(Vector2 point)
        {
            ThrowExceptionIfShapeIsIn3DMode();
            return ellipse.IsInsideShape(WorldPointToShapeSpace(point));
        }
        
        public override List<HandlePoint> GetHandles()
        {
            if (!DrawingSettings.EnableDrawing || !DrawingSettings.EnableHandles) { return null; }

            Vector2 right = MathPlus.RadianToVector2D(ellipse.rotationInDeg * Mathf.Deg2Rad);
            Vector2 up = MathPlus.RadianToVector2D((90 + ellipse.rotationInDeg) * Mathf.Deg2Rad);
            Vector3 ph = (ellipse.Center + (right * ellipse.radiusHorizontal)).ToVector3(Z,Axis.Z);
            Vector3 pv = (ellipse.Center + (up * ellipse.radiusVertical));
            return new List<HandlePoint> {
                new HandlePoint(ellipse.Center, null, HandlePointShape.Circle),
                new HandlePoint(ph, null, HandlePointShape.Circle),
                new HandlePoint(pv, null, HandlePointShape.Circle)
            };
        }

        public override void SetHandle(int i, Vector3 newPoint)
        {
            switch (i) {
                case 0:
                    ellipse.Center = newPoint;
                    return;
                case 1:
                    ellipse.radiusHorizontal = (newPoint - Center3D).magnitude;
                    return;
                case 2:
                    ellipse.radiusVertical= (newPoint - Center3D).magnitude;
                    return;
            }   
        }

        Vector3 Center3D
        {
            get {  return new Vector3(ellipse.Center.x, ellipse.Center.y, Z); }
        }
        
        protected override void DrawShape(Drawer.DrawingMode drawingMode)
        {
            int ellipseFragments = GetFragmentCount(ellipseDrawingPrecision);
            if (transformType == Shape2DTransformType.Self3D)
            {
                Drawer.DrawEllipse(ellipse, Z, DrawingSettings.Color, transform, drawingMode, ellipseFragments);
            }
            else
            {
                Vector3 center = ShapeSpacePointToWorldSpace(ellipse.Center);
                Ellipse transformedEllipse = ellipse;
                transformedEllipse.Center = center;
                Drawer.DrawEllipse(transformedEllipse, Z, DrawingSettings.Color, null, drawingMode, ellipseFragments);
            }
        }        
    }
}