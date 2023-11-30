using UnityEngine;

public class ShowOnlyAttribute : PropertyAttribute
{
    public int decimals;
    public ShowOnlyAttribute()
    {

    }

    public ShowOnlyAttribute(int decimalAmount) 
    {
        decimals = decimalAmount;
    }
}
