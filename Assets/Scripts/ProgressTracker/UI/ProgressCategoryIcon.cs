using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressCategoryIcon : ProgressIcon
{
    [SerializeField] private Slider categoryProgressBar;
    [SerializeField] private Transform iconShelf;

    public override void Initialise(ProgressCategory progressInfo)
    {
        base.Initialise(progressInfo);

        categoryProgressBar.value = progressData.GetProgress();

        ProgressUIHandler uiHandler = GetComponentInParent<ProgressUIHandler>();

        foreach (ProgressObject obj in progressInfo.Tracked)
        {
            uiHandler.CreateIcon(obj, iconShelf);
        }

        foreach (ProgressObject obj in progressInfo.TrackedObjects)
        {
            uiHandler.CreateIcon(obj, iconShelf);
        }
    }

    public override void SetProgress()
    {
        base.SetProgress();

        categoryProgressBar.value = progressData.GetProgress();
    }
}
