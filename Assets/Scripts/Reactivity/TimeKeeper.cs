using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    public static TimeKeeper Instance { get; private set; }

    [SerializeField] private int secondsPerHour = 60;
    [SerializeField] private int hoursPerDay = 24;

    private float clock = 10;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void FixedUpdate()
    {
        clock += Time.deltaTime / secondsPerHour;

        clock %= hoursPerDay;
    }

    public float GetTime()
    {
        return clock;
    }

    public float GetDayProgression()
    {
        return clock/hoursPerDay;
    }

    public bool IsRightTime(float startTime, float endTime)
    {
        if (startTime < endTime)
            return (clock >= startTime && clock <= endTime);
        else
            return (clock >= startTime || clock <= endTime);
    }
}
