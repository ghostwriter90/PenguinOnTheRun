using System;
using System.Collections.Generic;
using BasicTools.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BasicTools.Input {
    public partial class TouchControlSystem : MonoBehaviour {

#if UNITY_EDITOR

        const float alpha = 0.3f;
        const float arrowHeadLength = 0.15f;
        const float arrowHeadPos = 0.85f;
        const float arrowHeadAngle = 90f;
        
        struct HandleSource {
            public TouchControlType type;
            public int controlIndex;
            public int handleInControlIndex;
        }

        List<HandleSource> handleSource = new List<HandleSource> (); // X = ControlIndex, Y = ButtonIndex

        void OnDrawGizmos () {
            if (targetCamera == null) { return; }
            if (!drawLayout) { return; }
            if (Controls == null) { return; }
            if (screenPixelSize == null) { return; }

            bool playing = Application.isPlaying;
            Vector3 normal = targetCamera.transform.forward;

            // Draw Controls
            for (int controlIndex = 0; controlIndex < Controls.Count; controlIndex++) {
                // Draw Area
                TouchControl control = Controls[controlIndex];

                if (!control.ControlEnabled || !control.Visualization.DrawingEnabled) { continue; }

                Vector2 min = control.Area.GetScreen01MinPoint (screenPixelSize.Value.x, screenPixelSize.Value.y);
                Vector2 size = control.Area.GetSizeInScreen01 (screenPixelSize.Value.x, screenPixelSize.Value.y);
                Vector2 corner1 = new Vector2 (min.x, min.y + size.y);
                Vector2 corner2 = new Vector2 (min.x + size.x, min.y);

                Vector3[] cornerPoints = new Vector3[] {
                    Screen01ToCameraWorldPoint (min),
                    Screen01ToCameraWorldPoint (corner1),
                    Screen01ToCameraWorldPoint (min + size),
                    Screen01ToCameraWorldPoint (corner2)
                };

                Color col = control.Visualization.Color;
                Color colA = new Color (col.r, col.g, col.b, col.a * alpha);

                Drawer.DrawPolygon (cornerPoints, colA);

                // Get Touches
                List<RealtimeTouchAndControlInformation> touches = new List<RealtimeTouchAndControlInformation>();
                if (playing) {
                    
                    foreach (RealtimeTouchAndControlInformation pair in touchAndControlPairs) {
                        if (pair.controlIndex == controlIndex) {
                            touches.Add(pair);
                        }
                    }
                }

                float distanceToWorldSize = control.Area.AreaDistanceToWorldSize (screenPixelSize.Value, screenDiagonalToWorldSize, screenCmToWorldSize);
                
                // Draw Button Field
                if (control.type == TouchControlType.ButtonField) {
                    DrawButtonFieldGizmos (control, touches, normal, distanceToWorldSize, col, colA);
                }
                // Draw Analog Field                
                else if (control.type == TouchControlType.AnalogField) {
                    DrawAnalogFieldGizmos (control, touches, normal, distanceToWorldSize, col, colA);
                }
                // Draw GestureField Field  
                else if (control.type == TouchControlType.GestureField) {
                    DrawGestureFieldGizmos (control, touches, normal, distanceToWorldSize, col, colA);
                }
            }
        }

        void DrawButtonFieldGizmos (TouchControl control, List<RealtimeTouchAndControlInformation> pairs, Vector3 normal, float distanceToWorldSize, Color col, Color colA) {
            if (control.Buttons == null || control.Buttons.Length == 0) { return; }

            distanceToWorldSize /= 2f;

            // Draw Voranoi if the buttons are the seme size
            if (control.ButtonSettings.SameButtonSize) {
                List<Vector2> controlPointsInPixelSpace = new List<Vector2> ();
                for (int i = 0; i < control.Buttons.Length; i++) {
                    Vector2 coord = AreaPositionToScreenPixelPosition (control.Buttons[i].AreaCoordiante, control.Area);
                    if (!controlPointsInPixelSpace.Contains (coord)) {
                        controlPointsInPixelSpace.Add (coord);
                    }
                }

                Voronoi voranoi = new Voronoi (0);
                Vector2 areaMinPixel = AreaPositionToScreenPixelPosition (Vector2.zero, control.Area);
                Vector2 areaMaxPixel = AreaPositionToScreenPixelPosition (Vector2.one, control.Area);

                List<GraphEdge> edges = voranoi.GenerateVoronoi (controlPointsInPixelSpace.ToArray (), areaMinPixel, areaMaxPixel);

                if (voranoi != null && edges != null) {
                    for (int i = 0; i < edges.Count; i++) {
                        Vector2 screenPos1 = ScreenPixelPositionToScreen01Position (new Vector2 (edges[i].x1, edges[i].y1));
                        Vector2 screenPos2 = ScreenPixelPositionToScreen01Position (new Vector2 (edges[i].x2, edges[i].y2));

                        Vector3 worldPos1 = Screen01ToCameraWorldPoint (screenPos1);
                        Vector3 worldPos2 = Screen01ToCameraWorldPoint (screenPos2);
                        Drawer.DrawLine (worldPos1, worldPos2, colA);
                    }
                }
            }
            
            HashSet<int> buttonsNearTouch = new HashSet<int>();
            HashSet<int> buttonsInTouch = new HashSet<int>();
            HashSet<int> buttonsStartTouch = new HashSet<int>();
            if (pairs != null)
            {
                foreach (RealtimeTouchAndControlInformation pair in pairs)
                {
                    if (pair.Button_IsInsideButton)
                    {
                        buttonsInTouch.Add(pair.Button_NearestButtonIndex);
                    }
                    else {
                        buttonsNearTouch.Add(pair.Button_NearestButtonIndex);
                    }

                    buttonsStartTouch.Add(pair.Button_StartButtonIndex);
                }
            }


            for (int i = 0; i < control.Buttons.Length; i++)
            {

                float size = control.ButtonSettings.SameButtonSize ? control.ButtonSettings.ButtonSize : control.Buttons[i].ButtonSize;
                Vector2 screenPos = AreaPositionToScreen01Position(control.Buttons[i].AreaCoordiante, control.Area);
                Vector3 worldPos = Screen01ToCameraWorldPoint(screenPos);
                if (buttonsStartTouch.Contains(i)) { Drawer.DrawCross(worldPos, screenCmToWorldSize * 0.2f, col, targetCamera.transform.rotation); }
                
                Drawer.DrawCircle(worldPos, distanceToWorldSize * size, targetCamera.transform.forward, col);
                if (buttonsNearTouch.Contains(i))
                {
                    Drawer.DrawCircle(worldPos, distanceToWorldSize * (size * 1.2f), targetCamera.transform.forward, colA);
                }
                else if (buttonsInTouch.Contains(i))
                {
                    Drawer.DrawCircle(worldPos, distanceToWorldSize * (size * 0.8f), targetCamera.transform.forward, colA);
                }
            }
        }

        void DrawAnalogFieldGizmos (TouchControl control, List<RealtimeTouchAndControlInformation> pairs, Vector3 normal, float distanceToWorldSize, Color col, Color colA) {
            
            TouchControl.AnalogFieldSettings analogSet = control.AnalogSettings;

            Vector2 screenPos = pairs!=null && pairs.Count>0 ?
                pairs[0].Analog_LastCenterPosition.Value :
                AreaPositionToScreen01Position (analogSet.CenterPosition, control.Area);

            Vector3 worldPos = Screen01ToCameraWorldPoint (screenPos);

            // DrawCenter
            Drawer.DrawCross (worldPos, screenCmToWorldSize * 0.2f, col, targetCamera.transform.rotation);

            // Draw Circle Areas
            distanceToWorldSize /= 2f;
            float dist1InWorld = analogSet.maxDistance * distanceToWorldSize;

            Drawer.DrawCircle (worldPos, dist1InWorld , normal, col);

            float dist0InWorld = analogSet.minDistance * distanceToWorldSize;
            if (dist0InWorld > 0) {
                Drawer.DrawCircle (worldPos, dist0InWorld, normal, colA);
            }

            float distdistanceFloatInWorld = analogSet.moveControlDistance * distanceToWorldSize;
            if (analogSet.EnableMoveCenter) {
                Drawer.DrawCircle (worldPos, distdistanceFloatInWorld , normal, colA, Drawer.DrawingMode.GizmoInOnGizmo, 30, 2);
            }

            // Draw Arrows Heads
            if (analogSet.Direction != TouchControl.AnalogFieldSettings.AnalogDirection.FreeMovement) {

                Direction8? dir = null;
                Vector2? vec = null;
                if (pairs != null && pairs.Count >= 1) {
                    dir = pairs[0].Analog_Direction;
                    if (pairs[0].InterpolatedDirectionalValue < 0) { dir = null; }
                    vec = pairs[0].Analog_DirectionalVectorInDistance;
                }

                Vector3 right = (targetCamera.transform.right * dist1InWorld) * arrowHeadPos;
                Vector3 up = (targetCamera.transform.up * dist1InWorld) * arrowHeadPos;
                Vector3 rightSel = vec.HasValue ? targetCamera.transform.right * dist1InWorld * vec.Value.magnitude : right;
                Vector3 upSel = vec.HasValue ? targetCamera.transform.up * dist1InWorld * vec.Value.magnitude : up;

                // Left Right
                if (analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.All8Dir ||
                    analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.General4 ||
                    analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.Horizontal) {

                    Color rightColor = dir.HasValue && dir.Value == Direction8.Right ? col : colA;
                    Color leftColor = dir.HasValue && dir.Value == Direction8.Left ? col : colA;
                    Vector3 rightPos = worldPos + (dir == Direction8.Right ? rightSel / analogSet.maxDistance : right);
                    Vector3 leftPos = worldPos - (dir == Direction8.Left ? rightSel / analogSet.maxDistance : right);
                    Drawer.DrawArrowHead (rightPos, right, rightColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                    Drawer.DrawArrowHead (leftPos, -right, leftColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                }

                // Up Down
                if (analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.All8Dir ||
                    analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.General4 ||
                    analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.Vertical) {

                    Color upColor = dir.HasValue && dir.Value == Direction8.Up ? col : colA;
                    Color downColor = dir.HasValue && dir.Value == Direction8.Down ? col : colA;
                    Vector3 upPos = worldPos + (dir == Direction8.Up ? upSel / analogSet.maxDistance : up);
                    Vector3 downPos = worldPos - (dir == Direction8.Down ? upSel / analogSet.maxDistance : up);
                    Drawer.DrawArrowHead (upPos, up, upColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                    Drawer.DrawArrowHead (downPos, -up, downColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                }

                // Diagonal
                if (analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.All8Dir ||
                    analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.Diagonal4) {

                    float multiply = 1 / Mathf.Sqrt (2f);
                    Vector3 upRight = dir == Direction8.UpRight ? (rightSel + upSel) / analogSet.maxDistance : (right + up);
                    Vector3 upLeft = dir == Direction8.UpLeft ? (-rightSel + upSel) / analogSet.maxDistance : (-right + up);
                    Vector3 downRight = dir == Direction8.DownRight ? (rightSel - upSel) / analogSet.maxDistance : (right - up);
                    Vector3 downLeft = dir == Direction8.DownLeft ? (-rightSel - upSel) / analogSet.maxDistance : (-right - up);

                    Color upRightColor = dir.HasValue && dir.Value == Direction8.UpRight ? col : colA;
                    Color upLeftColor = dir.HasValue && dir.Value == Direction8.UpLeft ? col : colA;
                    Color downRightColor = dir.HasValue && dir.Value == Direction8.DownRight ? col : colA;
                    Color downLeftColor = dir.HasValue && dir.Value == Direction8.DownLeft ? col : colA;

                    Drawer.DrawArrowHead (worldPos + upRight * multiply, upRight, upRightColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                    Drawer.DrawArrowHead (worldPos + upLeft * multiply, upLeft, upLeftColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                    Drawer.DrawArrowHead (worldPos + downRight * multiply, downRight, downRightColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                    Drawer.DrawArrowHead (worldPos + downLeft * multiply, downLeft, downLeftColor, normal, dist1InWorld * arrowHeadLength, arrowHeadAngle);
                }
            }

            // Draw Arrow In Play Mode
            if (pairs!=null && pairs.Count >= 1)
            {
                Vector3 worldPosNow = Screen01ToCameraWorldPoint (ScreenPixelPositionToScreen01Position (pairs[0].touch.currentPixelPoint));
                Vector3 worldPosStart = Screen01ToCameraWorldPoint(pairs[0].Analog_LastCenterPosition.Value);

                Vector3 dir = worldPosNow - worldPosStart;
                Drawer.DrawArrow (worldPosStart, dir, dir.magnitude, control.Visualization.Color, normal);
            }
        }

        void DrawGestureFieldGizmos (TouchControl control, List<RealtimeTouchAndControlInformation> pairs, Vector3 normal, float distanceToWorldSize, Color col, Color colA) {

            distanceToWorldSize /= 2f;
            Vector2 areaCenterInScreen01 =
                control.Area.GetScreen01MinPoint (screenPixelSize.Value.x, screenPixelSize.Value.y) +
                (control.Area.GetSizeInScreen01 (screenPixelSize.Value.x, screenPixelSize.Value.y) / 2);
            
            if (pairs != null && pairs.Count > 0) {
                foreach (RealtimeTouchAndControlInformation pair in pairs) {
                    Vector2 screenPosStart = ScreenPixelPositionToScreen01Position (pair.touch.startPixelPoint);
                    DrawGestureFieldGizmosToOneTouch (screenPosStart, control, pair, normal, distanceToWorldSize, col, colA);
                }
            } else {
                DrawGestureFieldGizmosToOneTouch (areaCenterInScreen01, control, null, normal, distanceToWorldSize, col, colA);
            }

        }

        void DrawGestureFieldGizmosToOneTouch (Vector3 centerInScreen01, TouchControl control, RealtimeTouchAndControlInformation pair, Vector3 normal, float distanceToWorldSize, Color col, Color colA) {

            Vector3 worldPos = Screen01ToCameraWorldPoint (centerInScreen01);
            Vector2 moveScreenVec = Vector2.zero;
            Vector2 moveDistanceVec = Vector2.zero;
            Vector3 moveWorldVec = Vector3.zero;
            TouchData touch = pair != null ? pair.touch : null;
            
            // Draw Arrow In Play Mode
            if (touch != null && screenPixelSize.HasValue) {
                Vector2 screenPosStart = ScreenPixelPositionToScreen01Position (touch.startPixelPoint);
                Vector2 screenPosEnd = ScreenPixelPositionToScreen01Position (touch.currentPixelPoint);
                moveScreenVec = screenPosEnd - screenPosStart;

                Vector3 worldPosEnd = Screen01ToCameraWorldPoint (screenPosEnd);
                moveWorldVec =  worldPosEnd -  worldPos;
                Drawer.DrawArrow (worldPos, moveWorldVec, moveWorldVec.magnitude, control.Visualization.Color, normal);
                moveDistanceVec = pair.DistanceVector;
            }

            Drawer.DrawCross (worldPos , screenCmToWorldSize * 0.2f, col, targetCamera.transform.rotation);
            

            float worldMin = control.SwipeSettings.MinGestureDistance * distanceToWorldSize;
            float worldMax = control.SwipeSettings.MaxGestureDistance * distanceToWorldSize;

            // Tap
            if (control.TapSettings.TapEnabled) {

                float cicleRate = -1;
                bool tapHighlight = true;

                if (touch != null) {

                    // ChechTimeLimit
                    if (control.TapSettings.HaveTapTimeLimit) {
                        float now = Time.time;
                        cicleRate = (now - touch.startTime) / control.TapSettings.TapTimeLimit;
                        if (cicleRate >= 1) {
                            tapHighlight = false;
                        }
                    }

                    // Check Distance
                    if (moveDistanceVec.magnitude >= control.TapSettings.RelativeDeadZoneSize * control.SwipeSettings.MinGestureDistance) {
                        cicleRate = -1;
                        tapHighlight = false;
                    }
                }

                if (cicleRate <= 0 || cicleRate >= 1) {
                    Drawer.DrawCircle (worldPos,
                        control.TapSettings.RelativeDeadZoneSize * control.SwipeSettings.MinGestureDistance * distanceToWorldSize,
                        normal, tapHighlight ? col : colA, Drawer.DrawingMode.GizmoInOnGizmo);
                } else {
                    const float startAngle = 90;
                    float endAngle = ((360 * (1 - cicleRate)) + startAngle) % 360;
                    if (endAngle == startAngle) { endAngle -= 1; }

                    Drawer.DrawCircleSegment (worldPos,
                        control.TapSettings.RelativeDeadZoneSize * control.SwipeSettings.MinGestureDistance * distanceToWorldSize,
                        startAngle, endAngle, false,
                        col, normal, Drawer.DrawingMode.GizmoInOnGizmo);
                    Drawer.DrawCircleSegment (worldPos,
                        control.TapSettings.RelativeDeadZoneSize * control.SwipeSettings.MinGestureDistance * distanceToWorldSize,
                        startAngle, endAngle, true,
                        colA, normal, Drawer.DrawingMode.GizmoInOnGizmo);
                }
            }

            // Swipes
            if (control.Swipes != null && control.Swipes.Length != 0) {

                float cicleRate = -1;
                bool swipeHighlight = true;
                bool inTime = !control.SwipeSettings.HaveSwipeTimeLimit;

                if (touch != null && control.SwipeSettings.HaveSwipeTimeLimit) {
                    float now = Time.time;
                    cicleRate = (now - touch.startTime) / control.SwipeSettings.SwipeTimeLimit;
                    if (cicleRate >= 1) { swipeHighlight = false; }
                    else{
                        inTime = true;
                    }
                }

                // Swipe Max Circle
                if (cicleRate <= 0 || cicleRate >= 1) {
                    Drawer.DrawCircle (worldPos,
                        control.SwipeSettings.MaxGestureDistance * distanceToWorldSize,
                        normal, swipeHighlight? col : colA, Drawer.DrawingMode.GizmoInOnGizmo, 40);

                } else {

                    const float startAngle = 90;
                    float endAngle = ((360 * (1 - cicleRate)) + startAngle) % 360;
                    if (endAngle == startAngle) { endAngle -= 1; }
                    Drawer.DrawCircleSegment (worldPos, control.SwipeSettings.MaxGestureDistance * distanceToWorldSize,
                        startAngle, endAngle, false, col, normal, Drawer.DrawingMode.GizmoInOnGizmo);
                    Drawer.DrawCircleSegment (worldPos, control.SwipeSettings.MaxGestureDistance * distanceToWorldSize,
                        startAngle, endAngle, true, colA, normal, Drawer.DrawingMode.GizmoInOnGizmo);
                }

                // Arrows
                Vector3 r = targetCamera.transform.right;
                Vector3 u = targetCamera.transform.up;
                
                for (int i = 0; i < control.Swipes.Length; i++) {

                    if(control.Swipes[i].leftBound==null){ continue; }
                    // Get gesture main angle
                    float dir = control.Swipes[i].direction;
                    float bound1= control.Swipes[i].rightBound ?? dir;
                    float bound2= control.Swipes[i].leftBound ?? dir;
                    bool drawSide2= control.Swipes[i].leftBoundMeetLeftSwipe ?? false;

                    // Calculate World Position 
                    Vector2 dir2D = MathPlus.RadianToVector2D (dir * Mathf.Deg2Rad);
                    Vector2 bound1_2D = MathPlus.RadianToVector2D (bound1 * Mathf.Deg2Rad);
                    Vector2 bound2_2D = MathPlus.RadianToVector2D (bound2 * Mathf.Deg2Rad);

                    Vector3 dirWorld = (dir2D.x * r) + (dir2D.y * u);
                    Vector3 bound1World = (bound1_2D.x * r) + (bound1_2D.y * u);
                    Vector3 bound2World = (bound2_2D.x * r) + (bound2_2D.y * u);

                    bool isSelected = pair!=null && pair.Gesture_SelectedSwipeIndex == i;

                    // Draw Geature
                    Drawer.DrawArrowHead (worldPos + (dirWorld) * (isSelected ? (moveDistanceVec.magnitude * distanceToWorldSize) :(worldMax * arrowHeadPos)), 
                    dirWorld, isSelected && inTime ? col: colA, normal, arrowHeadLength * worldMax, arrowHeadAngle);

                    Drawer.DrawLine (
                        worldPos + (bound1World * worldMax),
                        worldPos + (bound1World * worldMin), colA);

                    if (drawSide2) {
                        Drawer.DrawLine (
                            worldPos + (bound2World * worldMax),
                            worldPos + (bound2World * worldMin), colA);
                    }

                    Drawer.DrawCircleSegment (worldPos, worldMin, bound1, bound2, false, colA, normal);
                }
            }
        }

        public List<HandlePoint> GetHandles () {
            if (!drawLayout || !enabled || Controls == null) { return null; }
            List<HandlePoint> handles = new List<HandlePoint> ();
            handleSource = new List<HandleSource> ();

            for (int controlIndex = 0; controlIndex < Controls.Count; controlIndex++) {

                TouchControl control = Controls[controlIndex];
                if (!control.ControlEnabled || !control.Visualization.DrawingEnabled) { continue; }

                if (touchAndControlPairs != null) {
                    bool used = false;
                    foreach (RealtimeTouchAndControlInformation pair in touchAndControlPairs) {
                        if (pair.controlIndex == controlIndex) {
                            used = true;
                            break;
                        }
                    }
                    if (used) { continue; }
                }


                Vector2 screen01Pos;
                Vector3 worldPoint;
                switch (control.type) {
                    case TouchControlType.ButtonField:
                        if (control.Buttons == null) { break; }

                        for (int buttonIndex = 0; buttonIndex < control.Buttons.Length; buttonIndex++)
                        {
                            TouchControl.TouchButton button = control.Buttons[buttonIndex];
                            screen01Pos = AreaPositionToScreen01Position(button.AreaCoordiante, control.Area);
                            worldPoint = Screen01ToCameraWorldPoint(screen01Pos);

                            handles.Add(new HandlePoint(drawHandleNames ? button.Name : null, worldPoint, control.Visualization.Color, HandlePointShape.Square));
                            handleSource.Add(
                                new HandleSource() { type = TouchControlType.ButtonField, controlIndex = controlIndex, handleInControlIndex = buttonIndex }
                            );
                        }
                        break;
                    case TouchControlType.AnalogField:
                        screen01Pos = AreaPositionToScreen01Position(control.AnalogSettings.CenterPosition, control.Area);
                        worldPoint = Screen01ToCameraWorldPoint(screen01Pos);

                        handles.Add(new HandlePoint(null, worldPoint, control.Visualization.Color, HandlePointShape.Square));
                        handleSource.Add(
                            new HandleSource() { type = TouchControlType.AnalogField, controlIndex = controlIndex }
                        );
                        break;
                    case TouchControlType.GestureField:
                        
                        if (control.Swipes == null || control.SwipeSettings.MaxGestureDistance <= 0 || !screenPixelSize.HasValue) { break; }
                        float dist = control.SwipeSettings.MaxGestureDistance + 0.1f;
                                                
                        for (int swipeIndex = 0; swipeIndex < control.Swipes.Length; swipeIndex++)
                        {
                            
                            TouchControl.Swipe swipe = control.Swipes[swipeIndex];
                            screen01Pos = AreaPositionToScreen01Position(new Vector2(0.5f,0.5f), control.Area);
                            Vector2 offsetDist = MathPlus.RadianToVector2D(swipe.direction * Mathf.Deg2Rad) * dist;
                            Vector2 offsetScreen01Pos = control.Area.DistanceVectorToScreen01Vector(offsetDist, screenPixelSize.Value);
                            
                            worldPoint = Screen01ToCameraWorldPoint(screen01Pos + offsetScreen01Pos);

                            handles.Add(new HandlePoint(drawHandleNames?swipe.Name:null, worldPoint, control.Visualization.Color, HandlePointShape.Circle));
                            handleSource.Add(
                                new HandleSource() { type = TouchControlType.GestureField, controlIndex = controlIndex, handleInControlIndex = swipeIndex }
                            );
                            
                        }
                        break;
                }                
            }
            return handles;
        }

        public void SetHandle (int i, Vector3 newPoint) {
            HandleSource source = handleSource[i];
            TouchControl control = Controls[source.controlIndex];
            Vector2 screen01Pos = WorldPointToScreen01Point (newPoint);
            Vector2 areaPos;

            FreshRendererTransforms(false);

            switch (source.type)
            {
                case TouchControlType.ButtonField:
                    areaPos = Screen01PositionToAreaPosition(screen01Pos, control.Area);
                    areaPos = MathPlus.Clamp01(areaPos);
                    control.Buttons[source.handleInControlIndex].AreaCoordiante = areaPos;
                    break;
                case TouchControlType.AnalogField:
                    areaPos = Screen01PositionToAreaPosition(screen01Pos, control.Area);
                    areaPos = MathPlus.Clamp01(areaPos);
                    control.AnalogSettings.CenterPosition = areaPos;
                    break;
                case TouchControlType.GestureField:
                    Vector2 areaCenterinScreen01 = control.Area.GetScreen01MinPoint(screenPixelSize.Value.x, screenPixelSize.Value.y ) +
                        (control.Area.GetSizeInScreen01(screenPixelSize.Value.x, screenPixelSize.Value.y) / 2f);
                    Vector2 distanceVec = control.Area.Screen01VectorToDistanceVector(screen01Pos - areaCenterinScreen01, screenPixelSize.Value);
                    float angle = MathPlus.Vector2DToRadian(distanceVec) * Mathf.Rad2Deg;

                    const float magnetAngles = 45f;
                    const float magnetBorder = 3.5f;

                    if (angle < 0) { angle += 360; }
                    float clamp = angle % magnetAngles;
                    if (clamp > magnetAngles / 2f) { clamp -= magnetAngles; }
                    if (clamp < magnetBorder && clamp > -magnetBorder) {  angle -= clamp; }

                    control.Swipes[source.handleInControlIndex].direction = angle;

                    CalculateGestureBorders(control);
                    break;                    
            }
        }

#endif
    }
}