using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressCategory : Progress
{
    [field: SerializeField] public Progress[] Progresses { get; private set; }

    public void Initialise()
    {
        TotalAmount = Progresses.Length;

        foreach (Progress p in Progresses)
        {
            p.Initialise(this);
        }

        Update();
    }

    public void Update()
    {
        if (completed)
            return;

        float averageProgress = 0;
        int completedCount = 0;

        foreach (Progress p in Progresses)
        {
            averageProgress += p.Percentage;

            completedCount += p.completed ? 1 : 0;
        }

        averageProgress /= Progresses.Length;

        Percentage = averageProgress;

        SetProgress(completedCount);

        if (Percentage >= 1)
        {
            completed = true;
        }
    }
}
