using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BasicTools.Input
{
    [CustomPropertyDrawer(typeof(TouchControl))]
    public class TouchControlDrawer : PropertyDrawer
    {        
        const float HeaderHeight = 30;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TouchControlSystem TC_System = property.serializedObject.targetObject as TouchControlSystem;
            int contsolIndex = GetControlIndex(property);

            if (TC_System.Controls.Count <= contsolIndex || contsolIndex < 0) { return 0; }
            return TC_System.Controls[contsolIndex].InspectorHeight;
        }
        
        int GetControlIndex(SerializedProperty property)
        {
            string path = property.propertyPath;
            int pFrom = path.LastIndexOf('[');
            int pTo = path.LastIndexOf("]");
            string indexString = path.Substring(pFrom + 1, pTo - pFrom - 1);
            return int.Parse(indexString);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TouchControlSystem TC_System = property.serializedObject.targetObject as TouchControlSystem;
            string controlName = property.FindPropertyRelative("Name").stringValue;
            int controlIndex = GetControlIndex(property);
            
            GUI.Box(position, "");

            float w = position.size.x;
            float y = position.position.y;

            float height = HeaderHeight;

            TouchControlSystem.TouchControlType type = DrawHeader(y,w, property, controlName, TC_System, controlIndex);
            if (property.isExpanded) {
                height += DrawBasicControlInfo(y + HeaderHeight, w, property, controlName, TC_System);
                switch (type)
                {
                    case TouchControlSystem.TouchControlType.AnalogField:
                        height += DrawAnalogField(y + height, w, property, controlName, TC_System, controlIndex);
                        break;
                    case TouchControlSystem.TouchControlType.ButtonField:
                        height += DrawButtonField(y + height, w, property, controlName, TC_System, controlIndex);
                        break;
                    case TouchControlSystem.TouchControlType.GestureField:
                        height += DrawGestureField(y + height, w, property, controlName, TC_System, controlIndex);
                        break;
                }
            }

            if (TC_System.Controls.Count <= controlIndex || controlIndex < 0) { return; }
            TC_System.Controls[controlIndex].InspectorHeight = height;

            // EditorGUI.PropertyField(position, property, new GUIContent(controlName), true);
        }

        TouchControlSystem.TouchControlType DrawHeader(float y, float with, SerializedProperty property, string name, TouchControlSystem TC_System, int contsolIndex)
        {
            Rect headerLeft = new Rect(30, y + 7, 8, 18);
            Rect headerRight = new Rect(with - 285, y + 6, 100, 20);

            // Expand
            property.isExpanded = EditorGUI.Foldout(headerLeft, property.isExpanded, string.Empty);

            headerLeft.x += 5;

            // Enable
            headerLeft.width = 20;
            SerializedProperty controlEnabled = property.FindPropertyRelative("ControlEnabled");
            controlEnabled.boolValue = EditorGUI.Toggle(headerLeft, controlEnabled.boolValue);

            headerLeft.x += 20;


            // Name 
            headerLeft.width = with - 350;
            if (property.isExpanded)
            {
                //Name
                headerLeft.x += 4;
                SerializedProperty nameProperty = property.FindPropertyRelative("Name");
                nameProperty.stringValue = EditorGUI.TextField(headerLeft, name);
            }
            else
            {
                //Name
                headerLeft.x += 5;
                EditorGUI.LabelField(headerLeft, name);
            }

            //Type
            SerializedProperty typeProp = property.FindPropertyRelative("type");

            var type = (TouchControlSystem.TouchControlType)typeProp.enumValueIndex;
            GUIStyle TypeDropdownStyle = new GUIStyle("DropDownButton")
            {
                fontSize = 9
            };

            type = (TouchControlSystem.TouchControlType)EditorGUI.EnumPopup(headerRight, type, TypeDropdownStyle);

            typeProp.enumValueIndex = (int)type;

            headerRight.position += new Vector2(110, 1);
            headerRight.size = new Vector2(20, 20);

            // Visuals
            SerializedProperty drawingEnbled = property.FindPropertyRelative("Visualization.DrawingEnabled");
            drawingEnbled.boolValue = EditorGUI.Toggle(headerRight, drawingEnbled.boolValue);

            headerRight.y -= 2;
            headerRight.x += 20;

            SerializedProperty drawingColor = property.FindPropertyRelative("Visualization.Color");
            drawingColor.colorValue = EditorGUI.ColorField(headerRight, new GUIContent(), drawingColor.colorValue, false, false, false);

            // Buttons
            headerRight.x += 30;

            if (GUI.Button(headerRight, "↓"))
            {                
                TC_System.SwitchTouchControlItems(contsolIndex, contsolIndex + 1);
                Undo.RecordObject(TC_System, "Switch Controls in List");

            }
            headerRight.x += 25;
            if (GUI.Button(headerRight, "↑"))
            {
                Undo.RecordObject(TC_System, "Switch Controls in List");
                TC_System.SwitchTouchControlItems(contsolIndex, contsolIndex - 1);
            }

            headerRight.x += 25;
            headerRight.width += 20;

            if (GUI.Button(headerRight, "Dupl."))
            {
                Undo.RecordObject(TC_System, "Duplicate Touch Control");
                TC_System.DuplicateTouchControl(GetControlIndex(property));
            }
            headerRight.position += new Vector2(45, 0);
            if (GUI.Button(headerRight, "Del."))
            {
                Undo.RecordObject(TC_System, "Delete Touch Control");
                TC_System.DeleteTouchControl(GetControlIndex(property));
            }

            return type;
        }

        float DrawBasicControlInfo(float y, float width, SerializedProperty property, string controlName, TouchControlSystem tC_System)
        {
            float height = 0;
            Rect pos = new Rect(60, y + height, width - 50, 20);

            SerializedProperty areaProperty = property.FindPropertyRelative("Area");
            EditorGUI.PropertyField(pos, areaProperty, new GUIContent("Area"), true);
            height += EditorGUI.GetPropertyHeight(areaProperty) + 5;
            pos.y = height + y;

            return height;
        }

        private float DrawGestureField(float y, float width, SerializedProperty property, string controlName, TouchControlSystem tC_System, int controlIndex)
        {
            float height = 0;
            Rect pos = new Rect(60, y + height, width - 50, 20);
            
            SerializedProperty tapSettingsProperty = property.FindPropertyRelative("TapSettings");
            EditorGUI.PropertyField(pos, tapSettingsProperty, new GUIContent("TapSettings"), true);
            height += EditorGUI.GetPropertyHeight(tapSettingsProperty) + 5;
            pos.y = height + y;
            
            SerializedProperty gestureSettingsProperty = property.FindPropertyRelative("SwipeSettings");
            EditorGUI.PropertyField(pos, gestureSettingsProperty, new GUIContent("SwipeSettings"), true);
            height += EditorGUI.GetPropertyHeight(gestureSettingsProperty) + 5;
            pos.y = height + y;

            SerializedProperty swipesProperty = property.FindPropertyRelative("Swipes");
            EditorGUI.PropertyField(pos,swipesProperty , new GUIContent("Swipes"), true);
            height += EditorGUI.GetPropertyHeight(swipesProperty) + 5;
            pos.y = height + y;
            
            tC_System.CalculateGestureBorders ();

            return height;
        }

        private float DrawButtonField(float y, float width, SerializedProperty property, string controlName, TouchControlSystem tC_System, int controlIndex)
        {
            float height = 0;
            Rect pos = new Rect(60, y + height, width - 50, 20);

            SerializedProperty settingsProperty = property.FindPropertyRelative("ButtonSettings");
            EditorGUI.PropertyField(pos, settingsProperty, new GUIContent("ButtonsSettings"), true);
            height += EditorGUI.GetPropertyHeight(settingsProperty) + 5;
            pos.y = height + y;

            SerializedProperty buttonsProperty = property.FindPropertyRelative("Buttons");
            EditorGUI.PropertyField(pos, buttonsProperty, new GUIContent("Buttons"), true);
            height += EditorGUI.GetPropertyHeight(buttonsProperty) + 5;
            pos.y = height + y;



            pos.height = 20;
            pos.y = height + y;
            if (GUI.Button(pos, "Create Images To Buttons"))
            {
                TouchControl control = tC_System.Controls[controlIndex];
                tC_System.CreateButtonRenderers(control);
            }
            height += 25;


            return height;
        }

        private float DrawAnalogField(float y, float width, SerializedProperty property, string controlName, TouchControlSystem tC_System, int controlIndex)
        {
            float height = 0;
            Rect pos = new Rect(60, y + height, width - 50, 20);

            SerializedProperty limitsProperty = property.FindPropertyRelative("AnalogSettings");
            EditorGUI.PropertyField(pos, limitsProperty, new GUIContent("Analog Settings"), true);
            height += EditorGUI.GetPropertyHeight(limitsProperty) + 5;

            TouchControl control = tC_System.Controls[controlIndex];
            if (control.JoyPadInfo == null) {
                pos.height = 20;
                pos.y = height + y;
                if (GUI.Button(pos, "Create Rendered Joystick")) {
                    tC_System.CreateJoyStickRenderer(control);
                }
                height += 25;
            }
            
            return height;
        }
    }
}