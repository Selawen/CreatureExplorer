using UnityEngine;

public class ShowOnlyAttribute : PropertyAttribute
{
    public int decimals;
    public float indent = -1;
    public ShowOnlyAttribute()
    {

    }

    public ShowOnlyAttribute(int decimalAmount) 
    {
        decimals = decimalAmount;
    }

    public ShowOnlyAttribute(int decimalAmount, float indentPercentage) 
    {
        decimals = decimalAmount;
        indent = indentPercentage;
    }

    public ShowOnlyAttribute(float indentPercentage) 
    {
        indent = indentPercentage;
    }
}
