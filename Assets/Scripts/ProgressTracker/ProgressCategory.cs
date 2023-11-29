using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressCategory : Progress
{
    [field: SerializeField] public Progress[] Progresses { get; private set; }

    public void Initialise()
    {
        foreach (Progress p in Progresses)
        {
            p.Initialise(this);
        }

        Update();
    }

    public void Update()
    {
        float averageProgress = 0;

        foreach (Progress p in Progresses)
        {
            averageProgress += p.Percentage;
        }

        averageProgress /= Progresses.Length;

        Percentage = averageProgress;
    }
}
