using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRChecker : MonoBehaviour
{
    public static bool IsVR
    {
        get
        {
            var inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);
            /*
            System.Collections.Generic.List<UnityEngine.XR.InputDevice> inputDevices = new System.Collections.Generic.List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeadMounted, inputDevices);
            */
            Debug.Log(inputDevices.Count > 0);
            return inputDevices.Count > 0;
        }
    }
}
