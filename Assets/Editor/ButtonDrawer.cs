using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class InspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
        ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;
            Rect buttonRect = new Rect(position.x + position.width *0.1f, position.y, position.width*0.8f, buttonAttribute.ButtonHeight);
            if (GUI.Button(buttonRect, label.text))
            {
                System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
                string eventName = buttonAttribute.MethodName;

                if (_eventMethodInfo == null)
                    _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (_eventMethodInfo != null)
                    _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                else
                    Debug.LogWarning($"InspectorButton: Unable to find method {eventName} in {eventOwnerType}");
            }
        }
    }
#endif
