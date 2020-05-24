using System;
using System.Collections.Generic;
using BasicTools.Utility;
using UnityEditor;
using UnityEngine;

namespace BasicTools.Input {
    [CustomPropertyDrawer (typeof (TouchControl.TouchArea))]
    class TouchAreaInspectorDrawer : PropertyDrawer {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {

            SerializedProperty lockProp = property.FindPropertyRelative ("Dock");
            Direction8 lockDir = (Utility.Direction8) lockProp.enumValueIndex;
            return property.isExpanded ? (lockDir.IsMainDirection()?190:160) : 16;
            
        }
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            property.isExpanded = EditorGUI.Foldout (position, property.isExpanded, label);

            if (property.isExpanded) {

                float titleLength = (position.width * 0.45f) - 10;
                float floatFieldLength = (position.width - titleLength) / 2;
                EditorGUIUtility.labelWidth = titleLength;
                float startX = position.x;

                // Lock
                position.y += 20;
                position.height = 18;
                SerializedProperty lockProp = property.FindPropertyRelative ("Dock");
                Direction8 lockDir = (Utility.Direction8) lockProp.enumValueIndex;
                lockProp.enumValueIndex = (int) (Utility.Direction8) EditorGUI.EnumPopup ((Rect) position, new GUIContent ("Dock"), lockDir);

                // Keep Fully In Screen Option
                position.y += 20;
                SerializedProperty keepFullyInScreenProp = property.FindPropertyRelative("KeepFullyInScreen");
                keepFullyInScreenProp.boolValue = EditorGUI.Toggle((Rect)position, new GUIContent("Keep Fully In Screen"), keepFullyInScreenProp.boolValue);

                // Hide Lower Controls Option
                position.y += 20;
                SerializedProperty hideLowerControlsProp = property.FindPropertyRelative("HideLowerControlAreas");
                hideLowerControlsProp.boolValue = EditorGUI.Toggle((Rect)position, new GUIContent("Hide Lower Areas"), hideLowerControlsProp.boolValue);

                EditorGUIUtility.labelWidth = 50;

                // With
                position.y += 30;
                position.width = titleLength;
                EditorGUI.LabelField (position, "Width");

                position.x += position.width;
                position.width = floatFieldLength;
                SerializedProperty W1Prop = property.FindPropertyRelative ("WidthInScreenRatio");

                W1Prop.floatValue = EditorGUI.FloatField (position, "Screen", W1Prop.floatValue);
                position.x += position.width;
                SerializedProperty W2Prop = property.FindPropertyRelative ("WidthInCm");
                W2Prop.floatValue = EditorGUI.FloatField (position, " +  Cm", W2Prop.floatValue);
                position.y += 20;

                // Height
                position.x = startX;
                position.width = titleLength;
                EditorGUI.LabelField (position, "Height");

                position.x += position.width;
                position.width = floatFieldLength;
                SerializedProperty H1Prop = property.FindPropertyRelative ("HeightInScreenRatio");
                H1Prop.floatValue = EditorGUI.FloatField (position, "Screen", H1Prop.floatValue);
                position.x += position.width;
                SerializedProperty H2Prop = property.FindPropertyRelative ("HeightInCm");
                H2Prop.floatValue = EditorGUI.FloatField (position, " +  Cm", H2Prop.floatValue);
                position.y += 30;

                // Offset
                if ( lockDir.IsMainDirection()) {
                    position.x = startX;
                    position.width = titleLength;
                    string offsetString = lockDir.IsVertical() ? "Offset Right": "Offset Up";
                    EditorGUI.LabelField (position, offsetString);

                    position.x += position.width;
                    position.width = floatFieldLength;
                    SerializedProperty O1Prop = property.FindPropertyRelative ("OffsetInScreenRatio");

                    O1Prop.floatValue = EditorGUI.FloatField (position, "Screen", O1Prop.floatValue);
                    position.x += position.width;
                    SerializedProperty O2Prop = property.FindPropertyRelative ("OffsetInCm");
                    O2Prop.floatValue = EditorGUI.FloatField (position, " +  Cm", O2Prop.floatValue);
                    position.y += 30;
                }

                // Definition Of Distance In Area
                position.x = startX;
                position.width = titleLength;
                EditorGUI.LabelField(position, "Definition Of Distance In Area");

                SerializedProperty distanceDefValueProp = property.FindPropertyRelative("DistanceDefinitonCmOrRelative");
                SerializedProperty distanceDefTypeProp = property.FindPropertyRelative("AdreaDistanceType");

                position.x += position.width;
                position.width = 60;
                EditorGUI.LabelField(position, (1- distanceDefValueProp.floatValue).ToString("0.00") + " Cm");
                position.x += position.width;
                position.width = (floatFieldLength*2) - 161;
                distanceDefValueProp.floatValue = EditorGUI.Slider(position, distanceDefValueProp.floatValue, 0, 1) ;

                position.y += 1;
                position.x += position.width+2;
                position.width = 100;
                TouchControl.AdreaRelativeDistanceType areaDistType = (TouchControl.AdreaRelativeDistanceType)distanceDefTypeProp.enumValueIndex;
                distanceDefTypeProp.enumValueIndex = (int)(TouchControl.AdreaRelativeDistanceType)EditorGUI.EnumPopup(position, areaDistType);
            }
        }
    }
}