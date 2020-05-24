using System.Collections.Generic;
using BasicTools.Input;
using BasicTools.Utility;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TouchControlSystem), true)]
[InitializeOnLoad]

public class TouchControlSystemEditor : Editor {
    Vector3 textShift = new Vector3 (1, 1.5f, 0);

    SerializedProperty listOfTouchContrlosProperty;
    public void OnEnable () {
        listOfTouchContrlosProperty = serializedObject.FindProperty ("Controls");
    }

    public override void OnInspectorGUI () {
        TouchControlSystem touchControlSystem = (target as TouchControlSystem);

        // Drawing script pinger

        GUI.enabled = false;
        EditorGUILayout.ObjectField ("Script", MonoScript.FromMonoBehaviour (touchControlSystem), typeof (TouchControlSystem), false);
        GUI.enabled = true;

        // Drawing aniting else

        Camera cam = (Camera) EditorGUILayout.ObjectField ("Target Camera", touchControlSystem.targetCamera, typeof (Camera), true);
        if (cam != touchControlSystem.targetCamera) {
            Undo.RecordObject (touchControlSystem, "Camera Changed");
            touchControlSystem.targetCamera = cam;
        }
        bool dl = EditorGUILayout.Toggle ("Draw Control Layout", touchControlSystem.drawLayout);
        if (dl != touchControlSystem.drawLayout) {
            Undo.RecordObject (touchControlSystem, "Enable Drawing Layout Changed");
            touchControlSystem.drawLayout = dl;
        }
        bool dn = EditorGUILayout.Toggle ("Draw Handle Names", touchControlSystem.drawHandleNames);
        if (dn != touchControlSystem.drawHandleNames) {
            Undo.RecordObject (touchControlSystem, "Enable Drawing Names Changed");
            touchControlSystem.drawHandleNames = dn;
        }

        bool enableRenderers = EditorGUILayout.Toggle ("Enable Renderer Objects", touchControlSystem.enableRenderers);
        if (enableRenderers != touchControlSystem.enableRenderers) {
            Undo.RecordObject (touchControlSystem, "Enable Renderers Changed");
            touchControlSystem.enableRenderers = enableRenderers;
            touchControlSystem.FreshRendererTransforms (true);
            touchControlSystem.SetRenderersEnable (enableRenderers);
        }

        if (touchControlSystem.targetCamera == null) {
            EditorGUILayout.HelpBox ("\nFirst You Have To Set The Target Camera!\n", MessageType.Warning);
            serializedObject.ApplyModifiedProperties ();
            return;
        }
        EditorGUILayout.Space ();

        for (int i = 0; i < listOfTouchContrlosProperty.arraySize; i++) {
            SerializedProperty control = listOfTouchContrlosProperty.GetArrayElementAtIndex (i);

            if (control != null) {
                EditorGUILayout.PropertyField (control, true);
            }
        }

        if (GUILayout.Button ("Add New Control", GUILayout.Height (30))) {
            Undo.RecordObject (touchControlSystem, "Add New Touch Control");
            touchControlSystem.AddNewTouchControl ();
        }

        serializedObject.ApplyModifiedProperties ();

    }

    protected virtual void OnSceneGUI () {
        TouchControlSystem controlSystem = (target as TouchControlSystem);
        List<HandlePoint> handles = controlSystem.GetHandles ();

        if (handles != null) {
            GUIStyle style = new GUIStyle ();

            Undo.RecordObject (controlSystem, "Free Move Handle");
            for (int i = 0; i < handles.Count; i++) {
                Handles.color = handles[i].color;
                style.normal.textColor = handles[i].color;
                Vector3 pos = handles[i].position;

                // draws a movable handle
                Handles.Label (pos + textShift, handles[i].label, style);

                Handles.CapFunction drawHandleFunction = null;
                if (handles[i].shape == HandlePointShape.Circle) { drawHandleFunction = Handles.CircleHandleCap; } else if (handles[i].shape == HandlePointShape.Square) { drawHandleFunction = Handles.RectangleHandleCap; }

                Vector3 newPoint = Handles.FreeMoveHandle (pos, Quaternion.identity, 0.3f, new Vector3 (.15f, .15f, .15f), drawHandleFunction);

                if (newPoint != handles[i].position) {
                    controlSystem.SetHandle (i, newPoint);
                }
            }
        }
    }
}