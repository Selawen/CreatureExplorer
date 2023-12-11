using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedLighting : MonoBehaviour
{
    [field: SerializeField] public TimeOfDay[] TimeSettings { get; private set; }
    [field: SerializeField] private Light mainLight;

    private Skybox sky;
    private int currentTimeIndex = 0;
    private int nextTimeIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        sky = FindAnyObjectByType<Skybox>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float time = TimeKeeper.Instance.GetTime();
        if ((nextTimeIndex > currentTimeIndex && TimeSettings[nextTimeIndex].Start < time) || (nextTimeIndex < currentTimeIndex && TimeSettings[nextTimeIndex].Start < time && TimeSettings[currentTimeIndex].Start > time))
        {
            currentTimeIndex = nextTimeIndex;
            nextTimeIndex++;
            nextTimeIndex %= TimeSettings.Length;


            sky.material = TimeSettings[currentTimeIndex].SkyMaterial;
            mainLight.color = TimeSettings[currentTimeIndex].LightFilter;
            mainLight.colorTemperature = TimeSettings[currentTimeIndex].LightTemperature;
            mainLight.transform.rotation = TimeSettings[currentTimeIndex].lightRotation;
        }
    }
}

[Serializable]
public class TimeOfDay
{
    public Material SkyMaterial;
    public Color LightFilter;
    public int LightTemperature;
    public Quaternion lightRotation;

    public float Start;
}
