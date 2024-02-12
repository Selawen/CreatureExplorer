using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRChecker : MonoBehaviour
{
    public static bool IsVR
    {
        get
        {
            System.Collections.Generic.List<UnityEngine.XR.InputDevice> inputDevices = new System.Collections.Generic.List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeadMounted, inputDevices);
            return inputDevices.Count > 0;
        }
    }
}
