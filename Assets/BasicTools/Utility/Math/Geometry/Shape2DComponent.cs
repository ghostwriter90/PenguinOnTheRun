using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BasicTools.Utility
{
    public class ShapeIsIn3DMoeException : Exception {
        public override string Message
        { get { return "Shape is in 3D mode, and you are trying to use 2D function."; } }
    }

    public abstract class Shape2DComponent : MonoBehaviour
    {
        public enum Shape2DTransformType { Self2D, Parent2D, World2D, Self3D }
        public Shape2DTransformType transformType = Shape2DTransformType.Self2D;
        public float Z = 0;

        protected void ThrowExceptionIfShapeIsIn3DMode()
        {
            if (transformType == Shape2DTransformType.Self3D)
            {
                throw new ShapeIsIn3DMoeException();
            }
        }

        protected Vector3 WorldPointToShapeSpace(Vector3 worldPoint)
        {
            if (transformType == Shape2DTransformType.Parent2D)
            {
                if (transform.parent != null) { return worldPoint - transform.parent.position; }
                return worldPoint;
            }
            else if (transformType == Shape2DTransformType.Self2D)
            {
                return worldPoint - transform.position;
            }
            else if (transformType == Shape2DTransformType.Self3D)
            {
                return transform.InverseTransformPoint(worldPoint);
            }
            return worldPoint;
        }

        protected Vector3 ShapeSpacePointToWorldSpace(Vector3 shapeSpacePoint)
        {
            if (transformType == Shape2DTransformType.Parent2D)
            {
                if (transform.parent != null) { return shapeSpacePoint + transform.parent.position; }
                return shapeSpacePoint;
            }
            else if (transformType == Shape2DTransformType.Self2D)
            {
                return shapeSpacePoint + transform.position;
            }
            else if (transformType == Shape2DTransformType.Self3D)
            {
                return transform.TransformPoint(shapeSpacePoint);
            }
            return shapeSpacePoint;
        }


        [Serializable]
        public class ShapeDrawingSettings
        {
            public bool EnableDrawing = true;
            public bool EnableHandles = true;
            public Color Color = Color.white;
            public Drawer.DrawingMode DrawMode;
        }

        public ShapeDrawingSettings DrawingSettings = new ShapeDrawingSettings();

        public abstract List<HandlePoint> GetHandles();

        public abstract void SetHandle(int i, Vector3 newPoint);

        public enum ObjectDimension { _2D, _3D }

        public abstract bool IsInsideShape(Vector2 point);

        
        public void Update()
        {
            if (DrawingSettings.EnableDrawing && DrawingSettings.DrawMode == Drawer.DrawingMode.DebugDrawInUpdateInPlayMode)
            {
                DrawShape(Drawer.DrawingMode.DebugDrawInUpdateInPlayMode);
            }
        }

        
        public virtual void OnDrawGizmos()
        {
            if (DrawingSettings.EnableDrawing && DrawingSettings.DrawMode == Drawer.DrawingMode.GizmoInOnGizmo)
            {
                DrawShape(Drawer.DrawingMode.GizmoInOnGizmo);
            }
        }

        protected int GetFragmentCount(float drawingPrecision)
        {
            int circleFragments = 20;
            #if UNITY_EDITOR
            Camera sceneCam = null;

            SceneView view = SceneView.currentDrawingSceneView;
            if (view != null)
            {
                sceneCam = view.camera;

            }
            if (sceneCam == null)
            {
                sceneCam = Camera.main;
            }


            Vector3 viewPos = sceneCam.transform.position;
            float dist = (viewPos - transform.position).magnitude;
            circleFragments = (int)(drawingPrecision / Mathf.Sqrt(dist));
            #endif
            return circleFragments;
        }

        protected abstract void DrawShape(Drawer.DrawingMode drawingMode);
    }
}
