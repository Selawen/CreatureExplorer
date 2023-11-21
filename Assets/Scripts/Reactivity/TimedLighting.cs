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

    // Start is called before the first frame update
    void Start()
    {
        sky = FindAnyObjectByType<Skybox>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TimeSettings[currentTimeIndex+1].Start < TimeKeeper.Instance.GetTime())
        {
            currentTimeIndex++;
            currentTimeIndex %= TimeSettings.Length;


            sky.material = TimeSettings[currentTimeIndex].SkyMaterial;
            mainLight.color = TimeSettings[currentTimeIndex].LightFilter;
            mainLight.colorTemperature = TimeSettings[currentTimeIndex].LightTemperature;
        }
    }
}

[Serializable]
public class TimeOfDay
{
    public Material SkyMaterial;
    public Color LightFilter;
    public int LightTemperature;

    public float Start;
}
