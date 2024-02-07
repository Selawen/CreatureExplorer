using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class FieldIndentAttribute : PropertyAttribute
{
    public float DefaultIndent = 0.2f;


    public FieldIndentAttribute()
    {

    }
    /// <summary>
    /// Set a custom indent for this field
    /// </summary>
    /// <param name="indentPercentage">The percentage that the field should be from the left of the property box</param>
    public FieldIndentAttribute(float indentPercentage)
    {
        DefaultIndent = indentPercentage;
    }
}
