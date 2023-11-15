using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, TimeKeeper.Instance.GetDayProgression()*360));
    }
}
