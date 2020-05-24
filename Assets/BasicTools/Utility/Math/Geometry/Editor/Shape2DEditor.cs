using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BasicTools.Utility
{
    [CustomEditor(typeof(Shape2DComponent), true)]
    [InitializeOnLoad]

    public class Shape2DComponentEditor : Editor
    {
        Vector3 textShift = new Vector3(1,1.5f,0);

        protected virtual void OnSceneGUI()
        {
            Shape2DComponent shapeTarget = (target as Shape2DComponent);
            List<HandlePoint> handles = shapeTarget.GetHandles();
           
            if (handles != null)
            {
                Handles.color = shapeTarget.DrawingSettings.Color;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = shapeTarget.DrawingSettings.Color;

                Vector3 targetPoint = Vector3.zero;
                switch (shapeTarget.transformType)
                {
                    case Shape2DComponent.Shape2DTransformType.Parent2D:
                        if(shapeTarget.transform.parent!=null) targetPoint += shapeTarget.transform.parent.transform.position;
                        break;
                    case Shape2DComponent.Shape2DTransformType.Self2D:
                        targetPoint += shapeTarget.transform.position;
                        break;
                }

                
                Undo.RecordObject(shapeTarget, "Free Move Handle");
                for (int i = 0; i < handles.Count; i++)
                {
                    Vector3 pos = targetPoint + handles[i].position;

                    if (shapeTarget.transformType == Shape2DComponent.Shape2DTransformType.Self3D)
                    {
                        pos = shapeTarget.transform.TransformPoint(handles[i].position);
                    }

                    // draws a movable handle
                    Handles.Label(pos + textShift, handles[i].label, style);

                    Handles.CapFunction drawHandleFunction = null;
                    if (handles[i].shape == HandlePointShape.Circle)
                    { drawHandleFunction = Handles.CircleHandleCap; }
                    else if (handles[i].shape == HandlePointShape.Square)
                    { drawHandleFunction = Handles.RectangleHandleCap; }

                    Vector3 newPoint = Handles.FreeMoveHandle(pos, Quaternion.identity, .5f, new Vector3(.25f, .25f, .25f), drawHandleFunction);
                    newPoint = newPoint - targetPoint;

                    if (shapeTarget.transformType == Shape2DComponent.Shape2DTransformType.Self3D)
                    {
                        newPoint = shapeTarget.transform.InverseTransformPoint(newPoint);
                    }

                    if (newPoint != handles[i].position) {
                        
                        shapeTarget.SetHandle(i, newPoint);                       
                    }
                }
            }
        }
    }
}