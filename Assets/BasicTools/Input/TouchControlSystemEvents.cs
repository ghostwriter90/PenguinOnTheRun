using BasicTools.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace BasicTools.Input
{
    public struct TuchEventSource
    {
        public TouchControlSystem system;
        public TouchControl control;
        public int controlIndex;
        public string controlName;
    }

    // Event System Event Infos
    public struct TouchControlTapInfo {
        public TuchEventSource source;

        public bool down;
        public bool release;
    }

    public struct TouchControlSwipeInfo
    {
        public TuchEventSource source;

        public int swipeIndex;
        public string swipeName;
        public float interpolatedSwipeDitance;
        public float realSwipeDirectionInDeg; 
        public float swipeMainDirectionInDeg; 
    }
    
    public struct TouchControlButtonInfo
    {
        public TuchEventSource source;

        public int startButtonIndex;
        public string startButtonName;

        public int nearestButtonIndex;
        public string nearestButtonName;

        public bool insideButton;
    
        public bool down;
        public bool release;
    }

    public struct TouchControlAnalogInfo
    {
        public TuchEventSource source;

        public float angleInDeg;
        public Direction8? direction;
        public Vector2 distanceVector;
        public float interpolatedDirectionalValue;

        public bool release;
        public bool down;
    }
}