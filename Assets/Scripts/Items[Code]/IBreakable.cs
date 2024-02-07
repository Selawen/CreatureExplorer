using UnityEngine;
public interface IBreakable
{
    public GameObject gameObject { get; }
    public void Break();
}
