using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

namespace BasicTools.Utility
{	
	[AttributeUsage( AttributeTargets.Field)]
	public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;
        public readonly string ButtonLabel;
        public readonly bool UseValueAsParameter;

        public InspectorButtonAttribute(string methodName)
        {
            MethodName = methodName;
            ButtonLabel = methodName;
            UseValueAsParameter = false;
        }

        public InspectorButtonAttribute(string methodName, string buttonLabel)
        {
            MethodName = methodName;
            ButtonLabel = buttonLabel;
            UseValueAsParameter = false;
        }

        public InspectorButtonAttribute(string methodName, string buttonLabel, bool useValueAsParameter)
        {
            MethodName = methodName;
            ButtonLabel = buttonLabel;
            UseValueAsParameter = useValueAsParameter;
        }
    }

	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
	public class InspectorButtonPropertyDrawer : PropertyDrawer
	{
		private MethodInfo _eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            position.size = new Vector2(position.size.x/2, position.size.y);
            EditorGUI.PropertyField(position, prop, label, true);

            InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
            
            Rect buttonRect = new Rect(position.size.x, position.y, position.size.x, position.height);


            if (GUI.Button(buttonRect, inspectorButtonAttribute.ButtonLabel))
            {
                object[] objects = null;
                if (inspectorButtonAttribute.UseValueAsParameter)
                {
                    switch (prop.type)
                    {
                        case "int":
                            objects = new object[] { prop.intValue };
                            break;
                        case "bool":
                            objects = new object[] { prop.boolValue };
                            break;
                        case "float":
                            objects = new object[] { prop.floatValue };
                            break;
                        case "Color":
                            objects = new object[] { prop.colorValue };
                            break;
                        case "Vector2":
                            objects = new object[] { prop.vector2Value };
                            break;
                        case "Vector3":
                            objects = new object[] { prop.vector3Value };
                            break;
                        case "Vector2Int":
                            objects = new object[] { prop.vector2IntValue };
                            break;
                        case "Vector3Int":
                            objects = new object[] { prop.vector3IntValue };
                            break;
                    }
                }

                System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
				string eventName = inspectorButtonAttribute.MethodName;

				if (_eventMethodInfo == null)
				{
					_eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}

				if (_eventMethodInfo != null)
				{
					_eventMethodInfo.Invoke(prop.serializedObject.targetObject, new object[] { objects } );
				}
				else
				{
					Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
				}
			}            
		}
	}
	#endif
}