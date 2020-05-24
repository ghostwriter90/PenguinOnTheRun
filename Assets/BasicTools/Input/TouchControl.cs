using BasicTools.Utility;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BasicTools.Input
{
    [Serializable]
    public partial class TouchControl
    {
        [Serializable] public class Renderable
            {
            public RectTransform rect;
            public Image image;

            public Renderable(RectTransform rect, Image image){
                this.rect = rect;
                this.image = image;
            }

            public void Set(Vector2 centerInAreaPosition, Vector2 size) {
                if (rect == null) { return; }
                rect.anchorMin = centerInAreaPosition;
                rect.anchorMax = centerInAreaPosition;
                rect.sizeDelta = size;
            }
        }

        [System.Serializable] public class EventWithInterpolatedValue : UnityEvent<float> { }

        public string Name;
        public TouchControlSystem.TouchControlType type;
        public bool ControlEnabled = true;
        public float InspectorHeight = 0;

        public float screenW;
        public float screenH;

        public RectTransform rectTrans = null;

        [Serializable]
        public struct TouchVisualization
        {
            public bool DrawingEnabled;
            public Color Color;
        }
        [HideInInspector] public TouchVisualization Visualization;


        public TouchArea Area;
    
        public ButtonFieldSettings ButtonSettings;
        public TouchButton[] Buttons;

        public GestureFieldSwipeSettings SwipeSettings;
        public GestureFieldTapSettings TapSettings;
        public Swipe[] Swipes;

        public AnalogFieldSettings AnalogSettings;
        [HideInInspector] public JoypadRenderInfo JoyPadInfo;

        Color GetDefaultColor(int i) {
            if (i % 6 == 0) return new Color(190, 240, 060, 255) / 255f;    // grass green
            if (i % 6 == 1) return new Color(255, 130, 110, 255) / 255f;    // redish
            if (i % 6 == 2) return new Color(100, 230, 230, 255) / 255f;    // light blue
            if (i % 6 == 3) return new Color(220, 130, 255, 255) / 255f;    // purple
            if (i % 6 == 4) return new Color(200, 210, 210, 255) / 255f;    // light gray
            else            return new Color(115, 255, 160, 255) / 255f;    // bluish green
        }
        
        Direction8 GetDefaulDock(int i)  {
            if (i % 8 == 0) return Direction8.DownRight;
            if (i % 8 == 1) return Direction8.DownLeft;
            if (i % 8 == 2) return Direction8.Down;
            if (i % 8 == 3) return Direction8.Right;
            if (i % 8 == 4) return Direction8.Left;
            if (i % 8 == 5) return Direction8.UpRight;
            if (i % 8 == 6) return Direction8.UpLeft;
            else            return Direction8.Up;
        }

        // NEW INSTANCE
        public TouchControl(int index)
        {
            Name = "New Control " + index;
            ControlEnabled = true;
            int typeCount = Enum.GetNames(typeof(TouchControlSystem.TouchControlType)).Length;
            type = (TouchControlSystem.TouchControlType)(index % typeCount);

            Visualization = new TouchVisualization() { DrawingEnabled = true, Color = GetDefaultColor(index) };
            Area = new TouchArea() { Dock = GetDefaulDock(index), HeightInScreenRatio = 0.25f, WidthInScreenRatio = 0.25f, HeightInCm = 1, WidthInCm = 1,
                KeepFullyInScreen = true, HideLowerControlAreas = true,
                DistanceDefinitonCmOrRelative = 0.5f,
                AdreaDistanceType = AdreaRelativeDistanceType.AreaDiagonal
            };

            AnalogSettings = new AnalogFieldSettings() {
                StartCenterAtClickPosition = false, EnableMoveCenter = true, Direction = AnalogFieldSettings.AnalogDirection.All8Dir,
                CenterPosition = new Vector2(0.5f, 0.5f), minDistance = 0.1f, maxDistance =0.3f , moveControlDistance =0.4f, Smoothness = 0.25f
            };

            ButtonSettings = new ButtonFieldSettings() {
                ButtonSize = 0.15f,
                SameButtonSize = true,
            };

            Buttons = new TouchButton[] {
                new TouchButton(){
                    Name = "A", ButtonSize = 0.1f,
                    AreaCoordiante = new Vector2(0.4f, 0.2f)},
                new TouchButton(){
                    Name = "B", ButtonSize = 0.1f,
                    AreaCoordiante = new Vector2(0.75f, 0.25f)},
                new TouchButton(){
                    Name = "X", ButtonSize = 0.2f,
                    AreaCoordiante = new Vector2(0.8f, 0.75f)}
            };

            TapSettings = new GestureFieldTapSettings() {
                HaveTapTimeLimit = true,
                TapTimeLimit = 0.5f,
                TapEnabled = true,
                RelativeDeadZoneSize = 0.6f
            };

            SwipeSettings = new GestureFieldSwipeSettings()
            {
                MinGestureDistance = 0.1f,
                MaxGestureDistance = 0.3f,
                HaveSwipeTimeLimit = true,
                SwipeTimeLimit = 1,
                GapBetweenDirections = 15f,
                MinAngleDiference = 30f,
                MaxAngleDiference = 75f,
            };

            Swipes = new Swipe[] {
                new Swipe(){
                    Name = "Up",
                    direction = 90f,
                },
                new Swipe(){
                    Name = "Right",
                    direction = 0f,
                },
                new Swipe(){
                    Name = "Left",
                    direction = 180f,
                },
            };
        }

        // COPY Of ANOTHER
        public TouchControl(TouchControl original)
        {
            Name = original.Name + " - COPY";
            ControlEnabled = original.ControlEnabled;
            type = original.type;
            
            Visualization = original.Visualization;
            Area = original.Area;

            AnalogSettings = original.AnalogSettings;
            ButtonSettings = original.ButtonSettings;
            TapSettings = original.TapSettings;
            SwipeSettings = original.SwipeSettings;

            Buttons = new TouchButton[original.Buttons.Length];
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i] = original.Buttons[i];
            }

            Swipes = new Swipe[original.Swipes.Length];
            for (int i = 0; i < Swipes.Length; i++)
            {
                Swipes[i] = original.Swipes[i];
            }
        }
    }
}
