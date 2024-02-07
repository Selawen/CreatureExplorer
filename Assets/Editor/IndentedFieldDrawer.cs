using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FieldIndentAttribute))]
public class IndentedFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label.text);

        float indent = (attribute as FieldIndentAttribute).DefaultIndent;

        Rect propertyPosition = new Rect(position.x+ position.width* indent, position.y, position.width * (1- indent), position.height);
        EditorGUI.PropertyField(propertyPosition, property, GUIContent.none);
        //base.OnGUI(position, property, label);
    }
}
