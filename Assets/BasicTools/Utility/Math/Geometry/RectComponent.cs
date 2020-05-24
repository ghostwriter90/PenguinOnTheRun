using System;
using System.Collections.Generic;
using UnityEngine;


namespace BasicTools.Utility
{
    class RectComponent : Shape2DComponent
    {
        public Rect rect;
        
        public override bool IsInsideShape(Vector2 point)
        {
            ThrowExceptionIfShapeIsIn3DMode();
            return rect.Contains(point);
        }

        protected override void DrawShape(Drawer.DrawingMode drawingMode)
        {
            if (transformType == Shape2DTransformType.Self3D)
            {
                Drawer.DrawRectangle(rect.position, rect.size, DrawingSettings.Color, transform, drawingMode);
            }
            else
            {
                Vector3 c = ShapeSpacePointToWorldSpace(rect.position);
                Drawer.DrawRectangle(c, rect.size, DrawingSettings.Color, null, drawingMode);
            }
        }

        public override List<HandlePoint> GetHandles()
        {
            if (!DrawingSettings.EnableDrawing || !DrawingSettings.EnableHandles) { return null; }

            Vector3 right = new Vector3(rect.position.x + (rect.size.x / 2f), rect.position.y);
            Vector3 left = new Vector3(rect.position.x - (rect.size.x / 2f), rect.position.y);
            Vector3 up = new Vector3(rect.position.x, rect.position.y + (rect.size.y / 2f));
            Vector3 down = new Vector3(rect.position.x, rect.position.y - (rect.size.y / 2f));
            Vector3 upRiht = new Vector3(rect.position.x + (rect.size.x / 2f), rect.position.y + (rect.size.y / 2f));

            return new List<HandlePoint> {
                new HandlePoint(rect.position, null, HandlePointShape.Circle),  // 0 Center
                new HandlePoint(right, null, HandlePointShape.Circle),          // 1 Right
                new HandlePoint(left, null, HandlePointShape.Circle),           // 2 Left
                new HandlePoint(up, null, HandlePointShape.Circle),             // 3 Up
                new HandlePoint(down, null, HandlePointShape.Circle),           // 4 Down
                new HandlePoint(upRiht, null, HandlePointShape.Square),         // 5 Rescale
            };
        }
        public override void SetHandle(int i, Vector3 newPoint)
        {
            float rescale = 0;
            float offset = 0;

            switch (i)
            {
                case 0: // Center
                    rect.position = newPoint;
                    return;
                case 1: // Right
                    rescale = ((newPoint.x - rect.position.x) - (rect.size.x / 2f));
                    offset = (newPoint.x - (rect.position.x + (rect.size.x / 2))) / 2f;
                    break;
                case 2: // Left
                    rescale = ((rect.position.x - newPoint.x) - (rect.size.x / 2f));
                    offset = (newPoint.x - (rect.position.x - (rect.size.x / 2))) / 2f;
                    break;
                case 3: // Up
                    rescale = ((newPoint.y - rect.position.y) - (rect.size.y / 2f));
                    offset = (newPoint.y - (rect.position.y + (rect.size.y / 2))) / 2f;
                    break;
                case 4: // Down
                    rescale = ((rect.position.y - newPoint.y) - (rect.size.y / 2f));
                    offset = (newPoint.y - (rect.position.y - (rect.size.y / 2))) / 2f;
                    break;
                case 5: // Rescale
                    float rescaleX = (newPoint.x - rect.position.x)*2;
                    float rescaleY = (newPoint.y- rect.position.y)*2;
                    rect.size = new Vector2(rescaleX, rescaleY);
                    return;
            }

            if (i == 1 || i == 2) // Horizontal
            {
                rect.size = new Vector2(rect.size.x + rescale, rect.size.y);
                rect.position = new Vector2(rect.position.x + offset, rect.position.y);
            }
            if (i == 3 || i == 4) // Vertical
            {
                rect.size = new Vector2(rect.size.x , rect.size.y+ rescale);
                rect.position = new Vector2(rect.position.x, rect.position.y + offset);
            }
        }
    }
}

