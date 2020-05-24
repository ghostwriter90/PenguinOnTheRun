using System;
using BasicTools.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace BasicTools.Input {
    public partial class TouchControl {

        [Serializable]  public struct AnalogFieldSettings{ 
            public enum AnalogDirection : byte { FreeMovement, Horizontal, Vertical, General4, Diagonal4, All8Dir }
            public bool StartCenterAtClickPosition;
            public bool EnableMoveCenter;
            [Range(0,1)] public float Smoothness;
            public AnalogDirection Direction;

            public Vector2 CenterPosition;

            public float minDistance;
            public float maxDistance;
            public float moveControlDistance;            
        }                
    }
}