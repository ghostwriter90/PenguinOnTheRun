using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BasicTools.Utility {
    public static class Drawer {
        public enum DrawingMode : byte { GizmoInOnGizmo, DebugDrawInUpdateInPlayMode }

        public static RaycastHit2D RayCast2D (
            Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance,
            LayerMask mask, Color color, bool enableDrawing = true) {
#if UNITY_EDITOR
            if (enableDrawing) {
                Debug.DrawRay (rayOriginPoint, rayDirection * rayDistance, color);
            }
#endif
            return Physics2D.Raycast (rayOriginPoint, rayDirection, rayDistance, mask);
        }

        public static RaycastHit Raycast3D (
            Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask,
            Color color, bool enableDrawing = false, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            if (enableDrawing) {
                if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                    Gizmos.color = color;
                    Gizmos.DrawRay (rayOriginPoint, rayDirection * rayDistance);
                } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                    Debug.DrawRay (rayOriginPoint, rayDirection * rayDistance, color);
                }
            }
#endif
            RaycastHit hit;
            Physics.Raycast (rayOriginPoint, rayDirection, out hit, rayDistance, mask);
            return hit;

        }

        public static void DrawArrow (
            Vector3 origin, Vector3 direction, float arrowLength, Color color, Vector3? normal = null,
            float baseLineLength = 0, float arrowHeadLength = 0.50f, float arrowHeadAngle = 50.0f,
            DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            direction.Normalize ();

            if (baseLineLength > 0) {
                Vector3 v = Vector3.Cross (direction, Vector3.forward).normalized * baseLineLength / 2;
                if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                    Gizmos.color = color;
                    Gizmos.DrawRay (origin - v, v * 2);
                } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                    Debug.DrawRay (origin - v, v * 2, color);
                }
            }
            arrowHeadLength = Mathf.Min (arrowHeadLength, arrowLength);

            if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                Gizmos.color = color;
                Gizmos.DrawRay (origin, direction * (arrowLength - arrowHeadLength));
            } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                Debug.DrawRay (origin, direction * (arrowLength - arrowHeadLength), color);
            }
            DrawArrowHead (origin + direction * arrowLength, direction, color, normal, arrowHeadLength, arrowHeadAngle, drawingMode);
#endif
        }

        public static void DrawArrow (Vector3 origin, Vector3 vector, Color color, Vector3? normal = null, float baseLineLength = 0, float arrowHeadLength = 0.50f, float arrowHeadAngle = 35.0f, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            DrawArrow (origin, vector, vector.magnitude, color, normal, baseLineLength, arrowHeadLength, arrowHeadAngle, drawingMode);
#endif
        }

        public static void DrawCross (Vector3 position, float crossSize, Color color, Quaternion? rotation = null, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            Quaternion rot = rotation ?? Quaternion.identity;

            float _1psqrt2 = 1 / Mathf.Sqrt (2f);
            Vector3 upRight = position + ((rot * new Vector3 (_1psqrt2, _1psqrt2)) * crossSize);
            Vector3 upLeft = position + ((rot * new Vector3 (-_1psqrt2, _1psqrt2)) * crossSize);
            Vector3 downRight = position + ((rot * new Vector3 (_1psqrt2, -_1psqrt2)) * crossSize);
            Vector3 downLeft = position + ((rot * new Vector3 (-_1psqrt2, -_1psqrt2)) * crossSize);

            if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                Gizmos.color = color;
                Gizmos.DrawLine (upRight, downLeft);
                Gizmos.DrawLine (upLeft, downRight);
            } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                Debug.DrawLine (upRight, downLeft, color);
                Debug.DrawLine (upLeft, downRight, color);
            }
#endif
        }

        public static void DrawBounds (Bounds bounds, Color color, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            if (drawingMode == DrawingMode.GizmoInOnGizmo) { Gizmos.color = color; }
            for (int i = 0; i < 2; i++) {
                float z = i == 0 ? bounds.min.z : bounds.max.z;
                Vector3[] rect = new Vector3[] {
                    new Vector3 (bounds.min.x, bounds.min.y, z),
                    new Vector3 (bounds.min.x, bounds.max.y, z),
                    new Vector3 (bounds.max.x, bounds.max.y, z),
                    new Vector3 (bounds.max.x, bounds.min.y, z),
                };
                if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                    Gizmos.DrawLine (rect[0], rect[1]);
                    Gizmos.DrawLine (rect[1], rect[2]);
                    Gizmos.DrawLine (rect[2], rect[3]);
                    Gizmos.DrawLine (rect[3], rect[0]);
                } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                    Debug.DrawLine (rect[0], rect[1], color);
                    Debug.DrawLine (rect[1], rect[2], color);
                    Debug.DrawLine (rect[2], rect[3], color);
                    Debug.DrawLine (rect[3], rect[0], color);
                }
            }

            for (int i = 0; i < 4; i++) {
                float x = i < 2 ? bounds.min.x : bounds.max.x;
                float y = i % 2 == 0 ? bounds.min.y : bounds.max.y;
                Vector3[] line = new Vector3[] {
                    new Vector3 (x, y, bounds.min.z),
                    new Vector3 (x, y, bounds.max.z)
                };
                if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                    Gizmos.DrawLine (line[0], line[1]);
                } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                    Debug.DrawLine (line[0], line[1], color);
                }
            }
#endif
        }

        public static void DrawRectangle (Rect rectangle, Color color, Transform space = null, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            DrawRectangle (rectangle.center, rectangle.size, color, space, drawingMode);
#endif
        }

        public static void DrawRectangle (Vector3 center, Vector2 size, Color color, Transform space = null, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            Vector3 v3TopLeft = new Vector3 (center.x - size.x / 2, center.y + size.y / 2, center.z);
            Vector3 v3TopRight = new Vector3 (center.x + size.x / 2, center.y + size.y / 2, center.z);
            Vector3 v3BottomRight = new Vector3 (center.x + size.x / 2, center.y - size.y / 2, center.z);
            Vector3 v3BottomLeft = new Vector3 (center.x - size.x / 2, center.y - size.y / 2, center.z);

            if (space != null) {
                v3TopLeft = space.TransformPoint (v3TopLeft);
                v3TopRight = space.TransformPoint (v3TopRight);
                v3BottomRight = space.TransformPoint (v3BottomRight);
                v3BottomLeft = space.TransformPoint (v3BottomLeft);
            }

            if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                Gizmos.color = color;
                Gizmos.DrawLine (v3TopLeft, v3TopRight);
                Gizmos.DrawLine (v3TopRight, v3BottomRight);
                Gizmos.DrawLine (v3BottomRight, v3BottomLeft);
                Gizmos.DrawLine (v3BottomLeft, v3TopLeft);
            } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                Debug.DrawLine (v3TopLeft, v3TopRight, color);
                Debug.DrawLine (v3TopRight, v3BottomRight, color);
                Debug.DrawLine (v3BottomRight, v3BottomLeft, color);
                Debug.DrawLine (v3BottomLeft, v3TopLeft, color);
            }
#endif
        }

        public static void DrawWireSphere (Vector3 center, float radius, Color color, int complexity = 1, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            if (complexity < 1) { complexity = 1; }
            for (int i = 0; i < complexity; i++) {
                float h = ((float) (i + 1) / (complexity + 1)) - 0.5f;
                float r1 = Mathf.Abs (h * 2);
                r1 = Mathf.Sqrt (1 - (r1 * r1));
                r1 *= radius;

                DrawCircle (center + new Vector3 (h, 0, 0), r1, Vector3.right, color, drawingMode, 20);
                DrawCircle (center + new Vector3 (0, h, 0), r1, Vector3.up, color, drawingMode, 20);
                DrawCircle (center + new Vector3 (0, 0, h), r1, Vector3.forward, color, drawingMode, 20);
            }
#endif
        }

        public static void DrawCircle (Vector3 center, float radius, Color color, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int fragmentCount = 20, int drawableleFragments = 1) {
#if UNITY_EDITOR
            DrawCircle (center, radius, color, null, drawingMode, fragmentCount, drawableleFragments);
#endif
        }

        public static void DrawCircle (Circle circle, Color color, Transform space = null, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int fragmentCount = 20, int drawableleFragments = 1) {
#if UNITY_EDITOR
            DrawCircle (circle.Center, circle.Radius, color, space, drawingMode, fragmentCount, drawableleFragments);
#endif
        }

        public static void DrawCircle (Vector3 center, float radius, Vector3 normal, Color color, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int fragmentCount = 20, int drawableleFragments = 1) {
            //TODO
#if UNITY_EDITOR
            normal = normal.normalized;
            /// Debug.Log(normal);
            Vector3 tempDir = normal == Vector3.up ? Vector3.right : Vector3.up;
            Vector3 radialVec1 = Vector3.Cross (tempDir, normal).normalized;
            Vector3 radialVec2 = Vector3.Cross (radialVec1, normal);

            radialVec1 *= radius;
            radialVec2 *= radius;

            Vector3[] points = new Vector3[fragmentCount];

            for (int i = 0; i < fragmentCount; i++) {
                float phase = Mathf.PI * 2 * (float) i / fragmentCount;
                points[i] = (Mathf.Sin (phase) * radialVec1) + (Mathf.Cos (phase) * radialVec2);
                points[i] += center;

            }

            DrawPolygon (points, color, drawingMode, drawableleFragments);
#endif
        }

        public static void DrawCircle (Vector3 center, float radius, Color color, Transform space = null, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int fragmentCount = 20, int drawableleFragments = 1) {
#if UNITY_EDITOR
            if (fragmentCount <= 0) { return; }

            Vector3 offset = space == null ? center : space.TransformPoint (center);
            bool doShift = offset != Vector3.zero;

            Vector3 radialVec1 = space == null ? Vector3.up : space.up;
            Vector3 radialVec2 = space == null ? Vector3.right : space.right;
            if (space != null) {
                radialVec1 *= space.lossyScale.y;
                radialVec2 *= space.lossyScale.x;
            }

            radialVec1 *= radius;
            radialVec2 *= radius;

            Vector3[] points = new Vector3[fragmentCount];

            if (drawableleFragments < 1) { drawableleFragments = 1; }
            for (int i = 0; i < fragmentCount; i++) {
                float phase = Mathf.PI * 2 * (float) i / fragmentCount;
                points[i] = (Mathf.Sin (phase) * radialVec1) + (Mathf.Cos (phase) * radialVec2);
                if (doShift) points[i] += offset;
            }

            DrawPolygon (points, color, drawingMode, drawableleFragments);
#endif
        }

        public static void DrawCircleSegment (Vector3 center, float radius, float startAngleInDeg, float endAngleInDeg, bool clockwiseWinding, Color color, Vector3 normal,
            DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int fullCirclefragmentCount = 40, int drawableleFragments = 1) {
            //TODO
#if UNITY_EDITOR
            normal = normal.normalized;
            /// Debug.Log(normal);
            Vector3 tempDir = normal == Vector3.up ? Vector3.right : Vector3.up;
            Vector3 radialVec1 = Vector3.Cross (tempDir, normal).normalized;
            Vector3 radialVec2 = Vector3.Cross (radialVec1, normal);

            radialVec1 *= radius;
            radialVec2 *= radius;

            startAngleInDeg %= 360;
            if (startAngleInDeg < 0) { startAngleInDeg += 360; }
            endAngleInDeg %= 360;
            if ( endAngleInDeg < 0) { endAngleInDeg += 360; }
            
            bool crossNull = (clockwiseWinding ^ startAngleInDeg > endAngleInDeg);

            float segmentLength = clockwiseWinding ?
                (crossNull ? (endAngleInDeg - startAngleInDeg) - 360: (endAngleInDeg - startAngleInDeg)) :
                (crossNull ? 360 - (startAngleInDeg - endAngleInDeg) : (endAngleInDeg - startAngleInDeg));

            float startAngleInRad = (startAngleInDeg + 90) * Mathf.Deg2Rad;
            fullCirclefragmentCount += 1;
            fullCirclefragmentCount = Mathf.Max(2, Mathf.CeilToInt(fullCirclefragmentCount * (Mathf.Abs(segmentLength)/360) ));

            segmentLength *= Mathf.Deg2Rad;

            Vector3[] points = new Vector3[fullCirclefragmentCount];

            for (int i = 0; i < fullCirclefragmentCount; i++) {
                float rate =  (float)i / (fullCirclefragmentCount - 1);
                float phase = startAngleInRad + (rate * segmentLength);
                points[i] = (Mathf.Sin (phase) * radialVec1) + (Mathf.Cos (phase) * radialVec2);
                points[i] += center;
            }

            DrawPolygon (points, color, drawingMode, drawableleFragments, false);
#endif
        }

        public static void DrawEllipse (Ellipse ellipse, float Z, Color color, Transform space, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int fragmentCount = 20) {
#if UNITY_EDITOR

            Vector3[] points = new Vector3[fragmentCount];
            Vector2 zero = Vector2.zero;
            Vector2 one = Vector2.right;
            for (int i = 0; i < fragmentCount; i++) {
                float phase = 360f * (i / (float) fragmentCount);
                points[i] = MathPlus.RotatePointAroundPivot (one, zero, phase);
                points[i].x *= ellipse.radiusHorizontal;
                points[i].y *= ellipse.radiusVertical;
                points[i] = MathPlus.RotatePointAroundPivot (points[i], zero, ellipse.rotationInDeg);
                points[i].x += ellipse.Center.x;
                points[i].y += ellipse.Center.y;
                points[i].z = Z;
            }

            DrawPolygon (points, color, drawingMode);
#endif
        }

        public static void DrawPolygon (Vector3[] points, Color color, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo, int drawableleFragments = 1, bool closeLine = true) {

#if UNITY_EDITOR
            if (points.Length <= 1) { return; }

            if (drawingMode == DrawingMode.GizmoInOnGizmo) { Gizmos.color = color; }

            if (drawableleFragments < 1) { drawableleFragments = 1; }

            for (int i = 0; i < points.Length - 1; i += drawableleFragments) {
                if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                    Gizmos.DrawLine (points[i], points[i + 1]);
                } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                    Debug.DrawLine (points[i], points[i + 1], color);
                }
            }

            if ( closeLine && (drawableleFragments == 1 || points.Length % drawableleFragments + 1 == 0)) {
                if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                    Gizmos.DrawLine (points[0], points.GetLast ());
                } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                    Debug.DrawLine (points[0], points.GetLast (), color);
                }
            }
#endif
        }

        public static void DrawLine (Vector3 start, Vector3 end, Color color, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            if (drawingMode == DrawingMode.GizmoInOnGizmo) {

                Gizmos.color = color;
                Gizmos.DrawLine (start, end);
            } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                Debug.DrawLine (start, end, color);
            }
#endif
        }

        public static void DrawArrowHead (Vector3 arrowEndPosition, Vector3 direction, Color color, Vector3? normal = null, float arrowHeadLength = 0.25f, float arrowHeadAngleInDeg = 40.0f, DrawingMode drawingMode = DrawingMode.GizmoInOnGizmo) {
#if UNITY_EDITOR
            if (direction == Vector3.zero) {
                return;
            }

            if (normal == null || normal == Vector3.zero) { normal = Vector3.forward; }

            direction = direction.normalized;
            Vector3 right = Vector3.Cross (direction, normal.Value.normalized);
            float arrowHeadWidth = Mathf.Tan (arrowHeadAngleInDeg * Mathf.Deg2Rad * 0.5f) * arrowHeadLength;

            Vector3 back = arrowEndPosition - (direction * arrowHeadLength);
            Vector3 arrowStartPoint1 = (right * arrowHeadWidth) + back;
            Vector3 arrowStartPoint2 = (-right * arrowHeadWidth) + back;

            if (drawingMode == DrawingMode.GizmoInOnGizmo) {
                Gizmos.color = color;
                Gizmos.DrawLine (arrowEndPosition, arrowStartPoint1);
                Gizmos.DrawLine (arrowEndPosition, arrowStartPoint2);
                Gizmos.DrawLine (arrowStartPoint1, arrowStartPoint2);
            } else if (drawingMode == DrawingMode.DebugDrawInUpdateInPlayMode) {
                Debug.DrawLine (arrowEndPosition, arrowStartPoint1, color);
                Debug.DrawLine (arrowEndPosition, arrowStartPoint2, color);
                Debug.DrawLine (arrowStartPoint1, arrowStartPoint2, color);
            }
#endif
        }
    }
}