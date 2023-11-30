using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This attribute can only be applied to fields because its
/// associated PropertyDrawer only operates on fields (either
/// public or tagged with the [SerializeField] attribute) in
/// the target script.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ButtonAttribute : PropertyAttribute
{
    public static float DefaultButtonheight = 30;

    public readonly string MethodName;

    private float buttonHeight = DefaultButtonheight;
    public float ButtonHeight
    {
        get { return buttonHeight; }
        set { buttonHeight = value; }
    }

    public ButtonAttribute(string MethodName)
    {
        this.MethodName = MethodName;
    }
}
