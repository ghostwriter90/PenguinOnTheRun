using System;
using BasicTools.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace BasicTools.Input {
    public partial class TouchControl {
        [Serializable]
        public struct TouchButton
        {
            public string Name;
            public Vector2 AreaCoordiante;
            public float ButtonSize;
            public Renderable Renderable;
        }

        [Serializable]  public struct ButtonFieldSettings{
            public bool SameButtonSize;
            public float ButtonSize;
        }
    }
}