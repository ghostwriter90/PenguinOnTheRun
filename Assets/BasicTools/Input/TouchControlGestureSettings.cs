using System;
using System.Collections.Generic;
using BasicTools.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace BasicTools.Input {
    public partial class TouchControl {

        [Serializable] public struct GestureFieldTapSettings {
            public bool TapEnabled;
            [Range(0,1)] public float RelativeDeadZoneSize;
            public bool HaveTapTimeLimit;
            public float TapTimeLimit;
        }

        [Serializable] public struct GestureFieldSwipeSettings {
            public float MinGestureDistance;
            public float MaxGestureDistance;

            public bool HaveSwipeTimeLimit;
            public float SwipeTimeLimit;
            [Space]
            [Range (0, 90)] public float GapBetweenDirections;
            [Range (0, 90)] public float MinAngleDiference;
            [Range (0, 90)] public float MaxAngleDiference;
        }

        [Serializable] public struct Swipe : IComparable{
            public string Name;
            public float direction;
            
            public float? rightBound{get; set;}
            public bool? rightBoundMeetLeftSwipe{get; set;}
            public float? leftBound{get; set;}
            public bool? leftBoundMeetLeftSwipe{get; set;}

            public int CompareTo(object obj)
            {
                return direction.CompareTo(((Swipe)obj).direction);
            }
        }    
    }
}