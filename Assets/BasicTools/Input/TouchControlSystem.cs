using System;
using System.Collections.Generic;
using BasicTools.Utility;
using UnityEngine;

namespace BasicTools.Input {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
    public partial class TouchControlSystem : MonoBehaviour {
        public enum TouchControlType : byte { ButtonField, AnalogField, GestureField }
        // SETTINGS

        const float epsilon = 0.00001f; // Layout drawing distance = camera.nearClippingPlane * (1 + epsylon);
        public static float DPI = 432; // Native On Huawei P9

        public Camera targetCamera = null;
        public RectTransform screenRectTrans = null;

        public bool drawLayout = true;
        public bool drawHandleNames = false;

        public bool enableRenderers;

        public List<TouchControl> Controls;

        // PUBLIC FUNCTIONS

        public bool IsControlEnabled (string controlName) {
            for (int i = 0; i < Controls.Count; i++) {
                if (Controls[i].Name == controlName) { return Controls[i].ControlEnabled; }
            }
            return false;            
        }

        public bool TryEnableControl (string controlName, bool enable = true) {
            for (int i = 0; i < Controls.Count; i++) {
                if (Controls[i].Name == controlName) {
                    Controls[i].ControlEnabled = enable;
                    return true;
                }
            }
            return false;
        }

        public bool IsControlEnabled (int controlIndex) {
            if (controlIndex >= 0 && controlIndex < Controls.Count) { return Controls[controlIndex].ControlEnabled; }
            return false;
        }

        public bool TryEnableControl (int controlIndex, bool enable = true) {
            if (controlIndex >= 0 && controlIndex < Controls.Count) {
                Controls[controlIndex].ControlEnabled = enable;
                return true;
            }
            return false;
        }

        public void AddNewTouchControl () {
            if (Controls == null) { Controls = new List<TouchControl> (); }
            TouchControl newControl = new TouchControl (Controls.Count);
            CalculateGestureBorders (newControl);
            Controls.Add (newControl);
            FreshRendererTransforms (true);
        }

        public void DuplicateTouchControl (int index) {
            Controls.Insert (index + 1, new TouchControl (Controls[index]));
            FreshRendererTransforms (true);
        }

        public void DeleteTouchControl (int index) {
            Controls.RemoveAt (index);
            FreshRendererTransforms (true);
        }

        public void SwitchTouchControlItems (int index1, int index2) {
            if (index1 >= 0 && index2 >= 0 && index1 < Controls.Count && index2 < Controls.Count) {
                TouchControl temp = Controls[index1];
                Controls[index1] = Controls[index2];
                Controls[index2] = temp;
            }
            FreshRendererTransforms (true);
        }

        // PRIVATE FIELDS

        class TouchData {
            public Vector2 startPixelPoint;
            public float startTime;
            public Vector2 currentPixelPoint;
            public bool up;
            public bool down;

            // public HashSet<int> attachedControlIndexes = new HashSet<int>();
            public bool isTouchDead = false;

            public bool released = false;
            public bool justTouched = true;

            public TouchData (Vector2 startPointPixel, float startTime) {
                this.startPixelPoint = startPointPixel;
                this.startTime = startTime;
                currentPixelPoint = startPointPixel;
            }
        }

        /// <summary>
        /// Key     =   if touch -> TouchIndex, if mouse -> -1 
        /// Value   =   All valuable information about the touch
        /// </summary>
        Dictionary<int, TouchData> touchDatas = new Dictionary<int, TouchData> ();

        List<RealtimeTouchAndControlInformation> touchAndControlPairs = null;
        class RealtimeTouchAndControlInformation {
            public int touchID;
            public int controlIndex;
            public TouchData touch;
            public TouchControl control;

            public RealtimeTouchAndControlInformation (int touchID, int controlIndex, TouchData touch, TouchControl control) {
                this.touchID = touchID;
                this.controlIndex = controlIndex;
                this.touch = touch;
                this.control = control;
            }

            public Vector2 DistanceVector = Vector2.zero;
            public float InterpolatedDirectionalValue;

            //BUTTON FIELD
            public bool Button_StartInsideButton = false;
            public int Button_StartButtonIndex = -1;

            public int Button_NearestButtonIndex = -1;
            public bool Button_IsInsideButton = false;

            //AREA FIELD
            public Vector2? Analog_LastCenterPosition;
            public Direction8? Analog_Direction;
            public Vector2 Analog_DirectionalVectorInDistance;

            //GESTUR FIELD
            public int Gesture_SelectedSwipeIndex = -1;
            public bool Gestrure_SwipeEventAlreadySent = false;
        }

        // PRIVATE FUNCTIONS 

        bool cameraIsOrtographic;
        float cameraFieldOfView;
        float cameraOrthographicSize;
        float cameraNear;
        Vector2? screenPixelSize = null;
        float screenDiagonalInCm = 1;
        float screenDiagonalToWorldSize = 1;
        float screenCmToWorldSize = 1;

        float MostAccurateDPI () {
            if (Application.platform == RuntimePlatform.Android) {
                try {

                    AndroidJavaClass activityClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject> ("currentActivity");

                    AndroidJavaObject metrics = new AndroidJavaObject ("android.util.DisplayMetrics");
                    activity.Call<AndroidJavaObject> ("getWindowManager").Call<AndroidJavaObject> ("getDefaultDisplay").Call ("getMetrics", metrics);

                    float dpi = (metrics.Get<float> ("xdpi") + metrics.Get<float> ("ydpi")) * 0.5f;
                    return dpi;
                } catch (Exception) {
                    return Screen.dpi;
                }
            } else {
                return Screen.dpi;
            }
        }

        public void Awake () {
            if (targetCamera == null) { targetCamera = Camera.main; }
            if (screenRectTrans == null) { screenRectTrans = GetComponent<RectTransform> (); }

            if (targetCamera != null && screenRectTrans != null) {
                OnResized ();
                touchAndControlPairs = new List<RealtimeTouchAndControlInformation> ();
            }

            CalculateGestureBorders ();
        }

        void Update () {
            if (targetCamera != null && screenRectTrans != null) {
                // Camera Settings Changed
                bool cameraSettingsChanged =
                    cameraIsOrtographic != targetCamera.orthographic ||
                    targetCamera.fieldOfView != cameraFieldOfView ||
                    targetCamera.nearClipPlane != cameraNear ||
                    targetCamera.orthographicSize != cameraOrthographicSize;

                if (cameraSettingsChanged) {
                    cameraFieldOfView = targetCamera.fieldOfView;
                    cameraNear = targetCamera.nearClipPlane;
                    cameraIsOrtographic = targetCamera.orthographic;
                    cameraOrthographicSize = targetCamera.orthographicSize;
                }

                // Set RectTransform To Target Camera      

                // Screen Settings Chenged
                if (!screenPixelSize.HasValue || screenPixelSize.Value.x != Screen.width || screenPixelSize.Value.y != Screen.height || cameraSettingsChanged) {
                    OnResized ();
                }

                bool play = Application.isPlaying;

                if(!play){
                    // Additional Calculation
                    CalculateGestureBorders ();

                }else{

                    // Real Time Touch Functionality
                    CaptureTouchAndMouseEvents ();

                    foreach (RealtimeTouchAndControlInformation pair in touchAndControlPairs) {
                        TouchControl control = pair.control;
                        TouchData touch = pair.touch;
                        if (!control.ControlEnabled) { continue; }

                        TuchEventSource source = new TuchEventSource
                        {
                            system = this,
                            control = control,
                            controlIndex = pair.controlIndex,
                            controlName = control.Name,
                        };

                        switch (control.type) {
                            case TouchControlType.ButtonField:
                                TouchOnButtonField (pair, control, touch, pair.touchID, pair.touch.down, pair.touch.up, source);
                                break;
                            case TouchControlType.AnalogField:
                                TouchOnAnalogField (pair, control, touch, pair.touchID, pair.touch.down, pair.touch.up, source);
                                break;
                            case TouchControlType.GestureField:
                                TouchOnGestureField (pair, control, touch, pair.touchID, pair.touch.down, pair.touch.up, source);
                                break;
                        }
                    }
                }

                FreshRendererTransforms (!Application.isPlaying);
            }
        }

        void OnResized () {
            DPI = MostAccurateDPI ();
            screenPixelSize = new Vector2 (Screen.width, Screen.height);
            if (targetCamera != null) {
                float screenDiagonalInPixel = screenPixelSize.Value.magnitude;
                float heightToDiagonal = screenDiagonalInPixel / screenPixelSize.Value.y;
                if (targetCamera.orthographic) {
                    screenDiagonalToWorldSize = 2 * (targetCamera.orthographicSize / screenPixelSize.Value.y) * screenDiagonalInPixel;
                } else {
                    screenDiagonalToWorldSize = 2 * Mathf.Tan ((targetCamera.fieldOfView / 2f) * Mathf.Deg2Rad) * targetCamera.nearClipPlane * heightToDiagonal;
                }
                screenDiagonalInCm = (Screen.height / TouchControlSystem.DPI) * MathPlus.InchToCm * heightToDiagonal;
                screenCmToWorldSize = screenDiagonalToWorldSize / screenDiagonalInCm;
            }
        }

        void OnDisable () {
            touchDatas = new Dictionary<int, TouchData> ();
            touchAndControlPairs = new List<RealtimeTouchAndControlInformation> ();
        }

        /// <summary>
        /// Capture and sort any touch and mouse event
        /// </summary>
        List<int> deletables = new List<int> ();
        void CaptureTouchAndMouseEvents () {
            // Delete Release Touch Infos
            
            deletables.Clear();
            foreach (KeyValuePair<int, TouchData> touchData in touchDatas) {
                if (touchData.Value.released) { deletables.Add (touchData.Key); }
            }

            for (int i = 0; i < deletables.Count; i++) {
                int deletableTouchID = deletables[i];
                touchDatas.Remove (deletableTouchID);
                for (int j = 0; j < touchAndControlPairs.Count;) {
                    if (touchAndControlPairs[j].touchID == deletableTouchID) {
                        touchAndControlPairs.RemoveAt (j);
                    } else {
                        j++;
                    }
                }
            }
            

            float now = Time.time;
            if (UnityEngine.Input.touchCount == 0 && (UnityEngine.Input.GetMouseButton (0) || UnityEngine.Input.GetMouseButtonUp (0))) {
                Vector2 pos = UnityEngine.Input.mousePosition;
                // Mouse event
                if (UnityEngine.Input.GetMouseButtonDown (0)) {
                    if (!touchDatas.ContainsKey (-1)) {
                        TouchData newTouchData = new TouchData (pos, now);
                        touchDatas.Add (-1, newTouchData);
                        AnyTouchEvent (true, false, newTouchData, -1);
                    }
                } else if (UnityEngine.Input.GetMouseButtonUp (0)) {
                    if (touchDatas.ContainsKey (-1)) {
                        TouchData newTouchData = touchDatas[-1];
                        touchDatas[-1].released = true;
                        AnyTouchEvent (false, true, newTouchData, -1);
                    }
                } else if (UnityEngine.Input.GetMouseButton (0)) {
                    if (touchDatas.ContainsKey (-1)) {
                        TouchData newTouchData = touchDatas[-1];
                        newTouchData.currentPixelPoint = pos;
                        touchDatas[-1].justTouched = false;
                        AnyTouchEvent (false, false, newTouchData, -1);
                    }
                }
            } else {
                for (int i = 0; i < UnityEngine.Input.touchCount; i++) {
                    Touch touch = UnityEngine.Input.touches[i];
                    int id = touch.fingerId;
                    Vector2 pos = touch.position;

                    switch (touch.phase) {
                        case TouchPhase.Began:
                            if (!touchDatas.ContainsKey (id)) {
                                TouchData newTouchData = new TouchData (pos, now);
                                touchDatas.Add (id, newTouchData);
                                AnyTouchEvent (true, false, newTouchData, id);
                            }
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (touchDatas.ContainsKey (id)) {
                                TouchData newTouchData = touchDatas[id];
                                touchDatas[id].released = true;
                                AnyTouchEvent (false, true, newTouchData, id);
                            }
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            if (touchDatas.ContainsKey (id)) {
                                TouchData newTouchData = touchDatas[id];
                                newTouchData.currentPixelPoint = pos;
                                touchDatas[id].justTouched = false;
                                AnyTouchEvent (false, false, newTouchData, id);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// HANDLE ANY TOUCH EVENT
        /// </summary>
        void AnyTouchEvent (bool down, bool up, TouchData touch, int touchID) {
            if (!screenPixelSize.HasValue) { return; }

            // Add control Indexes To TouchData
            if (down) {
                for (int controlIndex = 0; controlIndex < Controls.Count; controlIndex++) {
                    TouchControl control = Controls[controlIndex];
                    if (!control.ControlEnabled) { continue; }
                    Vector2 startScreenPoint = new Vector2 (touch.startPixelPoint.x / screenPixelSize.Value.x, touch.startPixelPoint.y / screenPixelSize.Value.y);
                    if (control.Area.IsInside (startScreenPoint, screenPixelSize.Value)) {

                        bool alreadyBlocked = false;
                        if (control.type == TouchControlType.AnalogField) {
                            for (int i = 0; i < touchAndControlPairs.Count; i++) {
                                if (touchAndControlPairs[i].controlIndex == controlIndex) {
                                    alreadyBlocked = true;
                                    break;
                                }
                            }
                        }

                        if (!alreadyBlocked) { touchAndControlPairs.Add (new RealtimeTouchAndControlInformation (touchID, controlIndex, touch, control)); }

                        if (control.Area.HideLowerControlAreas) {
                            break;
                        }
                    }
                }
            }
            touch.up = up;
            touch.down = down;
        }

        // HANDLE BUTTON FIELD EVENT
        void TouchOnButtonField (RealtimeTouchAndControlInformation pair, TouchControl control, TouchData touch, int touchID, bool down, bool up, TuchEventSource source) {

            //TouchControl.ButtonFieldSettings buttonSet = control.ButtonSettings;
            Vector2 currentScreenPoint = new Vector2 (touch.currentPixelPoint.x / screenPixelSize.Value.x, touch.currentPixelPoint.y / screenPixelSize.Value.y);
            bool isInsideArea = down || control.Area.IsInside (currentScreenPoint, screenPixelSize.Value);
            int buttonIndex = -1;
            bool inCircle = false;
            if (isInsideArea) { inCircle = IsInsideButton (control, currentScreenPoint, out buttonIndex); }
            pair.Button_NearestButtonIndex = buttonIndex;
            pair.Button_IsInsideButton = inCircle;

            if (down) {
                pair.Button_StartButtonIndex = buttonIndex;
                pair.Button_StartInsideButton = inCircle;
            }
            
            GameEventManager.TriggerEvent(
                new TouchControlButtonInfo() {
                    source = source,
                    down = down,
                    release = up,
                    startButtonIndex = pair.Button_StartButtonIndex,
                    startButtonName = control.Buttons[pair.Button_StartButtonIndex].Name,
                    nearestButtonIndex = pair.Button_NearestButtonIndex,
                    nearestButtonName = pair.Button_NearestButtonIndex >= 0 ? control.Buttons[pair.Button_NearestButtonIndex].Name : null,
                    insideButton = pair.Button_IsInsideButton
                }
            );
        }

        bool IsInsideButton (TouchControl control, Vector2 screen01Point, out int index) {

            float minDistance = float.MaxValue;
            bool result = false;
            index = -1;
            bool uniformButtonSize = control.ButtonSettings.SameButtonSize;
            for (int i = 0; i < control.Buttons.Length; i++) {
                Vector2 buttonScreen01Point = AreaPositionToScreen01Position (control.Buttons[i].AreaCoordiante, control.Area);
                Vector2 distanceVec = control.Area.Screen01VectorToDistanceVector (buttonScreen01Point - screen01Point, screenPixelSize.Value);
                float buttonSize = uniformButtonSize ? control.ButtonSettings.ButtonSize : control.Buttons[i].ButtonSize;
                float realDistance = distanceVec.magnitude - buttonSize;
                if (realDistance <= 0) { result = true; }
                if (realDistance < minDistance) {
                    index = i;
                    minDistance = realDistance;
                }
            }
            return result;
        }

        // HANDLE ANALOG FIELD EVENT
        void TouchOnAnalogField(RealtimeTouchAndControlInformation pair, TouchControl control, TouchData touch, int touchID, bool down, bool up, TuchEventSource source)
        {
            TouchControl.AnalogFieldSettings analogSet = control.AnalogSettings;
            TouchControl.TouchArea area = control.Area;

            Vector2 currentScreenPoint = new Vector2(touch.currentPixelPoint.x / screenPixelSize.Value.x, touch.currentPixelPoint.y / screenPixelSize.Value.y);
            Vector2 centerScreenPoint;
            if (down)
            {
                if (analogSet.StartCenterAtClickPosition)
                {
                    centerScreenPoint = currentScreenPoint;
                    pair.Analog_LastCenterPosition = currentScreenPoint;
                }
                else
                {
                    centerScreenPoint = AreaPositionToScreen01Position(analogSet.CenterPosition, control.Area);
                    pair.Analog_LastCenterPosition = centerScreenPoint;
                }
            }
            else
            {
                centerScreenPoint = pair.Analog_LastCenterPosition ?? AreaPositionToScreen01Position(analogSet.CenterPosition, control.Area);
            }

            Vector2 screen01Vec = currentScreenPoint - centerScreenPoint;
            Vector2 distanceVec = area.Screen01VectorToDistanceVector(screen01Vec, screenPixelSize.Value);
            pair.DistanceVector = distanceVec;

            // Move ControlCenter If Needed
            if (control.AnalogSettings.EnableMoveCenter && !up)
            {

                Vector2 limitedDistanceVec = distanceVec;
                if (analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.Vertical) { limitedDistanceVec.x = 0; }
                if (analogSet.Direction == TouchControl.AnalogFieldSettings.AnalogDirection.Horizontal) { limitedDistanceVec.y = 0; }

                float moveDistance = limitedDistanceVec.magnitude - analogSet.moveControlDistance;

                if (moveDistance > 0)
                {
                    float notSmooth = analogSet.Smoothness == 0 ? 1 :
                        Mathf.Clamp(Time.deltaTime / (analogSet.Smoothness), 0.01f, 1);
                    Vector2 moveScreen01 = area.DistanceVectorToScreen01Vector(limitedDistanceVec.normalized * moveDistance * notSmooth, screenPixelSize.Value);
                    Vector2 newCenter = pair.Analog_LastCenterPosition.Value + moveScreen01;

                    Vector2 aMin = area.GetScreen01MinPoint(screenPixelSize.Value.x, screenPixelSize.Value.y);
                    Vector2 aSize = area.GetSizeInScreen01(screenPixelSize.Value.x, screenPixelSize.Value.y);

                    if (aMin.x > newCenter.x) { newCenter.x = aMin.x; }
                    if (aMin.y > newCenter.y) { newCenter.y = aMin.y; }
                    if (aMin.x + aSize.x < newCenter.x) { newCenter.x = aMin.x + aSize.x; }
                    if (aMin.y + aSize.y < newCenter.y) { newCenter.y = aMin.y + aSize.y; }

                    centerScreenPoint = newCenter;
                    pair.Analog_LastCenterPosition = newCenter;

                    screen01Vec = currentScreenPoint - centerScreenPoint;
                    distanceVec = area.Screen01VectorToDistanceVector(screen01Vec, screenPixelSize.Value);
                    pair.DistanceVector = distanceVec;
                }
            }

            // Calculate Direction
            Direction8? dir = null;
            float deg = MathPlus.Vector2DToRadian(distanceVec) * Mathf.Rad2Deg;
            if (deg < 0) { deg += 360; }
            switch (analogSet.Direction)
            {
                case TouchControl.AnalogFieldSettings.AnalogDirection.Vertical:
                    if (distanceVec.y == 0)
                    {
                        dir = null;
                    }
                    else
                    {
                        dir = distanceVec.y < 0 ? Direction8.Down : Direction8.Up;
                    }
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.Horizontal:
                    if (distanceVec.x == 0)
                    {
                        dir = null;
                    }
                    else
                    {
                        dir = distanceVec.x < 0 ? Direction8.Left : Direction8.Right;
                    }
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.General4:
                    if (deg <= 45 || deg > 315)
                    {
                        dir = Direction8.Right;
                    }
                    else if (deg > 45 && deg <= 135)
                    {
                        dir = Direction8.Up;
                    }
                    else if (deg > 135 && deg <= 225)
                    {
                        dir = Direction8.Left;
                    }
                    else
                    {
                        dir = Direction8.Down;
                    }
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.Diagonal4:
                    if (deg <= 90)
                    {
                        dir = Direction8.UpRight;
                    }
                    else if (deg > 90 && deg <= 180)
                    {
                        dir = Direction8.UpLeft;
                    }
                    else if (deg > 180 && deg <= 270)
                    {
                        dir = Direction8.DownLeft;
                    }
                    else
                    {
                        dir = Direction8.DownRight;
                    }
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.All8Dir:
                    if (deg <= 22.5f || deg > 337.5f)
                    {
                        dir = Direction8.Right;
                    }
                    else if (deg > 22.5f && deg <= 67.5f)
                    {
                        dir = Direction8.UpRight;
                    }
                    else if (deg > 67.5f && deg <= 112.5f)
                    {
                        dir = Direction8.Up;
                    }
                    else if (deg > 112.5f && deg <= 157.5f)
                    {
                        dir = Direction8.UpLeft;
                    }
                    else if (deg > 157.5f && deg <= 202.5f)
                    {
                        dir = Direction8.Left;
                    }
                    else if (deg > 202.5f && deg <= 247.5f)
                    {
                        dir = Direction8.DownLeft;
                    }
                    else if (deg > 247.5f && deg <= 292.5f)
                    {
                        dir = Direction8.Down;
                    }
                    else
                    {
                        dir = Direction8.DownRight;
                    }
                    break;

            }
            pair.Analog_Direction = dir;

            // Calculate Directional Vector

            switch (analogSet.Direction)
            {
                case TouchControl.AnalogFieldSettings.AnalogDirection.FreeMovement:
                    pair.Analog_DirectionalVectorInDistance = distanceVec;
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.Vertical:
                    pair.Analog_DirectionalVectorInDistance = new Vector2(0, distanceVec.y);
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.Horizontal:
                    pair.Analog_DirectionalVectorInDistance = new Vector2(distanceVec.x, 0);
                    break;
                case TouchControl.AnalogFieldSettings.AnalogDirection.General4:
                case TouchControl.AnalogFieldSettings.AnalogDirection.Diagonal4:
                case TouchControl.AnalogFieldSettings.AnalogDirection.All8Dir:
                    Vector2 dirVec = dir.Value.ToVector().normalized;
                    float length = Vector2.Dot(distanceVec, dirVec);
                    pair.Analog_DirectionalVectorInDistance = dirVec * length;
                    break;
            }

            // Iterpolate Value Between Min and Max
            float directionalDistance = pair.Analog_DirectionalVectorInDistance.magnitude;
            float interpolatedDistance = MathPlus.LerpWithoutBorders(directionalDistance, 0, 1, analogSet.minDistance, analogSet.maxDistance);
            pair.InterpolatedDirectionalValue = interpolatedDistance;

            GameEventManager.TriggerEvent(
                new TouchControlAnalogInfo()
                {
                    source = source,
                    down = down,
                    release = up,
                    direction = dir,
                    distanceVector = distanceVec,
                    interpolatedDirectionalValue = interpolatedDistance,
                    angleInDeg = deg
                }
            );
        }

        // HANDLE GESTUREFIELD EVENT
        void TouchOnGestureField(RealtimeTouchAndControlInformation pair, TouchControl control, TouchData touch, int touchID, bool down, bool up, TuchEventSource source)
        {


            TouchControl.GestureFieldTapSettings tapSet = control.TapSettings;
            TouchControl.GestureFieldSwipeSettings swipesSet = control.SwipeSettings;

            TouchControl.TouchArea area = control.Area;

            Vector2 startScreenPoint = new Vector2(touch.startPixelPoint.x / screenPixelSize.Value.x, touch.startPixelPoint.y / screenPixelSize.Value.y);
            Vector2 currentScreenPoint = new Vector2(touch.currentPixelPoint.x / screenPixelSize.Value.x, touch.currentPixelPoint.y / screenPixelSize.Value.y);

            Vector2 distanceVec = area.Screen01VectorToDistanceVector(currentScreenPoint - startScreenPoint, screenPixelSize.Value);

            pair.DistanceVector = distanceVec;
            float ditance = distanceVec.magnitude;
            float interpolatedDistance = MathPlus.LerpWithoutBorders(ditance, 0, 1,
                control.SwipeSettings.MinGestureDistance, control.SwipeSettings.MaxGestureDistance);

            pair.InterpolatedDirectionalValue = interpolatedDistance;

            bool inSwipeDistance = ditance >= control.SwipeSettings.MinGestureDistance;

            int selectedSwipeIndex = -1;

            if (inSwipeDistance)
            {
                for (int i = 0; i < control.Swipes.Length; i++)
                {
                    if (control.Swipes[i].leftBound == null) { continue; }
                    // Get gesture main angle
                    float dir = control.Swipes[i].direction;
                    float bound1 = control.Swipes[i].rightBound ?? dir;
                    float bound2 = control.Swipes[i].leftBound ?? dir;

                    float vecAngle = MathPlus.Vector2DToRadian(distanceVec) * Mathf.Rad2Deg;
                    if (AngleDedClockiseDistance(vecAngle, bound1) > 0 && AngleDedClockiseDistance(vecAngle, bound2) <= 0)
                    {
                        selectedSwipeIndex = i;
                        break;
                    }
                }
            }
            pair.Gesture_SelectedSwipeIndex = selectedSwipeIndex;

            float time = down ? 0 : Time.time - touch.startTime;

            if (pair.Gestrure_SwipeEventAlreadySent) { return; }

            // TAP EVENT
            if (tapSet.TapEnabled)
            {
                if (down)
                {
                    GameEventManager.TriggerEvent(
                        new TouchControlTapInfo()
                        {
                            source = source,
                            down = true,
                            release = false,
                        }
                    );
                }
                else if (up && (!tapSet.HaveTapTimeLimit || time <= tapSet.TapTimeLimit) && ditance <= tapSet.RelativeDeadZoneSize * swipesSet.MinGestureDistance)
                {
                    GameEventManager.TriggerEvent(
                        new TouchControlTapInfo()
                        {
                            source = source,
                            down = false,
                            release = true,
                        }
                    );
                }
            }

            if (inSwipeDistance && selectedSwipeIndex >= 0 && (!swipesSet.HaveSwipeTimeLimit || time <= swipesSet.SwipeTimeLimit))
            {
                if (up || interpolatedDistance >= 1)
                {
                    GameEventManager.TriggerEvent(
                        new TouchControlSwipeInfo
                        {
                            source = source,
                            interpolatedSwipeDitance = interpolatedDistance,
                            realSwipeDirectionInDeg = MathPlus.Vector2DToRadian(distanceVec) * Mathf.Rad2Deg,
                            swipeIndex = selectedSwipeIndex,
                            swipeMainDirectionInDeg = control.Swipes[selectedSwipeIndex].direction,
                            swipeName = control.Swipes[selectedSwipeIndex].Name
                        }
                    );
                    pair.Gestrure_SwipeEventAlreadySent = true;
                }
            }

        }

        public void CalculateGestureBorders () {
            if (Controls == null) { Controls = new List<TouchControl> (); }
            for (int i = 0; i < Controls.Count; i++) {
                TouchControl control = Controls[i];
                if (control.type == TouchControlType.GestureField) {
                    CalculateGestureBorders (control);
                }
            }
        }

        void CalculateGestureBorders (TouchControl control) {
            // Calculate Borders
            List<float> sortedDirections = new List<float> (control.Swipes.Length);

            for (int i = 0; i < control.Swipes.Length; i++) {
                float dirfloat = control.Swipes[i].direction;
                dirfloat %= 360;
                if (dirfloat < 0) { dirfloat += 360; }
                sortedDirections.Add (dirfloat);
                control.Swipes[i].direction = dirfloat;
            }

            sortedDirections.Sort ();
            for (int i = 0; i < sortedDirections.Count - 1; i++) {
                if (sortedDirections[i] == sortedDirections[i + 1]) {
                    sortedDirections.RemoveAt (i);
                } else {
                    i++;
                }
            }

            for (int i = 0; i < sortedDirections.Count; i++) {
                // Get gesture main angle
                float dir = sortedDirections[i];

                // Calculate middle positions
                int i1 = i - 1;
                int i2 = i + 1;
                if (i == 0) { i1 = sortedDirections.Count - 1; }
                if (i == sortedDirections.Count - 1) { i2 = 0; }

                float mid1 = (dir + sortedDirections[i1]) / 2;
                if (i == 0) { mid1 = ((dir + sortedDirections[i1] + 360) / 2) % 360; }
                float mid2 = (dir + sortedDirections[i2]) / 2;
                if (i == sortedDirections.Count - 1) { mid2 = ((dir + sortedDirections[i2] + 360) / 2) % 360; }

                // Calculate bound bositions
                float gap = (sortedDirections.Count == 1) ? 90 : control.SwipeSettings.GapBetweenDirections;
                float bound1 = mid1 + (gap / 2);
                float bound2 = mid2 - (gap / 2);

                float maxA = control.SwipeSettings.MaxAngleDiference;
                float minA = control.SwipeSettings.MinAngleDiference;

                if (maxA < minA) {
                    maxA = minA;
                    control.SwipeSettings.MaxAngleDiference = minA;
                }

                bool collideSide1 = true;
                bool collideSide2 = true;

                float angle1 = AngleDedClockiseDistance (dir, bound1);
                if (angle1 < minA) {
                    bound1 = dir - minA;
                } else if (angle1 > maxA) {
                    bound1 = dir - maxA;
                }
                if (sortedDirections.Count > 1 && AngleDedClockiseDistance (bound1, mid1) <= 0) {
                    bound1 = mid1;
                    collideSide1 = false;
                }

                float angle2 = AngleDedClockiseDistance (bound2, dir);
                if (angle2 < minA) {
                    bound2 = dir + minA;
                } else if (angle2 > maxA) {
                    bound2 = dir + maxA;
                }

                if (sortedDirections.Count > 1 && AngleDedClockiseDistance (mid2, bound2) <= 0) {
                    bound2 = mid2;
                    collideSide2 = false;
                }

                // Set Values
                for (int j = 0; j < control.Swipes.Length; j++) {
                    if (dir == control.Swipes[j].direction) {
                        control.Swipes[j].rightBound = bound1;
                        control.Swipes[j].leftBound = bound2;
                        control.Swipes[j].rightBoundMeetLeftSwipe = collideSide1;
                        control.Swipes[j].leftBoundMeetLeftSwipe = collideSide2;
                        break;
                    }
                }
            }
        }

        // a1 = 90, a2 = 80     ->      return 10
        // return between 180 (inclusive) and -180 (exlusive)
        float AngleDedClockiseDistance (float a1, float a2) {
            a1 %= 360;
            a2 %= 360;
            if (a1 < 0) { a1 += 360; }
            if (a2 < 0) { a2 += 360; }

            bool cross0 = a1 < a2;
            float result = cross0 ? 360 + a1 - a2 : a1 - a2;

            if (result > 180) { result -= 360; }
            return result;
        }

        // VECTOR TRANSFORMATION FUNCTIONS

        // Screen01 -> ScreenPixel
        Vector2 ScreenPixelPositionToScreen01Position (Vector2 pixelPosition) {
            return new Vector2 (
                pixelPosition.x / screenPixelSize.Value.x,
                pixelPosition.y / screenPixelSize.Value.y);
        }

        Vector2 Screen01PositionToScreenPixelPosition (Vector2 screen01Position) {
            return new Vector2 (
                screen01Position.x * screenPixelSize.Value.x,
                screen01Position.y * screenPixelSize.Value.y);
        }

        // Area -> Screen01
        Vector2 Screen01PositionToAreaPosition (Vector2 screen01Position, TouchControl.TouchArea area) {
            Vector2 areaMin = area.GetScreen01MinPoint (screenPixelSize.Value.x, screenPixelSize.Value.y);
            Vector2 areaMax = areaMin + area.GetSizeInScreen01 (screenPixelSize.Value.x, screenPixelSize.Value.y);

            return new Vector2 (
                MathPlus.LerpWithoutBorders (screen01Position.x, 0, 1, areaMin.x, areaMax.x),
                MathPlus.LerpWithoutBorders (screen01Position.y, 0, 1, areaMin.y, areaMax.y));
        }

        Vector2 AreaPositionToScreen01Position (Vector2 areaPosition, TouchControl.TouchArea area) {
            if (!screenPixelSize.HasValue) { return Vector2.zero; }

            Vector2 areaMin = area.GetScreen01MinPoint (screenPixelSize.Value.x, screenPixelSize.Value.y);
            Vector2 areaMax = areaMin + area.GetSizeInScreen01 (screenPixelSize.Value.x, screenPixelSize.Value.y);

            return new Vector2 (
                MathPlus.LerpWithoutBorders (areaPosition.x, areaMin.x, areaMax.x, 0, 1),
                MathPlus.LerpWithoutBorders (areaPosition.y, areaMin.y, areaMax.y, 0, 1));
        }

        // ScreenPixel -> Area
        Vector2 AreaPositionToScreenPixelPosition (Vector2 areaPosition, TouchControl.TouchArea area) {

            Vector2 screen01Point = AreaPositionToScreen01Position (areaPosition, area);
            return Screen01PositionToScreenPixelPosition (screen01Point);
        }

        Vector2 ScreenPixelPositionToAreaPosition (Vector2 screenPixelPosition, TouchControl.TouchArea area) {
            Vector2 screen01Point = ScreenPixelPositionToScreen01Position (screenPixelPosition);
            return Screen01PositionToAreaPosition (screen01Point, area);
        }

        // Screen01 -> Wold
        Vector3 Screen01ToCameraWorldPoint (Vector2 screenPoin01) {
            if (!screenPixelSize.HasValue || targetCamera == null) { return Vector2.zero; }
            if (float.IsNaN (screenPoin01.x) || float.IsNaN (screenPoin01.y)) { return Vector2.zero; }
            float delta = cameraNear * epsilon;
            Vector3 screenMinPixel = new Vector3 (screenPoin01.x * screenPixelSize.Value.x, screenPoin01.y * screenPixelSize.Value.y, targetCamera.nearClipPlane + delta);
            return targetCamera.ScreenToWorldPoint (screenMinPixel);
        }

        // Screen01 -> Wold
        Vector2 WorldPointToScreen01Point (Vector3 worldPoint) {
            Vector3 pixelPoint = targetCamera.WorldToScreenPoint (worldPoint);
            return new Vector2 (pixelPoint.x / screenPixelSize.Value.x, pixelPoint.y / screenPixelSize.Value.y);
        }

        public void SetRenderersEnable (bool enable) {
            foreach (RectTransform rt in screenRectTrans) {
                rt.gameObject.SetActive (enable);
            }
        }
    }
}