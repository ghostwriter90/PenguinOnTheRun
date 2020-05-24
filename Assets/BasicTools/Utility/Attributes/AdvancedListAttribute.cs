using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BasicTools.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AdvancedListAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AdvancedListAttribute))]
    public class AdvancedListPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // use the default property height, which takes into account the expanded state
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty arrayProp = property.GetEndProperty();
            arrayProp = property.FindPropertyRelative("GestureEnabled");

            // draw the default property editor
            EditorGUI.PropertyField(position, property, label, true);

            // display a warning to discourage users from manually editing child properties on a quaternion
            if (property.isExpanded)
            {
                Rect buttonRect = new Rect(position.xMin + 50, position.yMin, position.width - 50, 15);
                
                // MoveArrayElement(int srcIndex, int dstIndex); !!!
                if (GUI.Button(buttonRect, "Test???    -"+arrayProp + "-      "+property.propertyPath))
                {
                    Debug.Log("+");
                }
            }
        }
    }
#endif
}