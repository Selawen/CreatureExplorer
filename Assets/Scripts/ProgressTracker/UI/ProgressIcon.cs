using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI iconText;

    protected ProgressObject progressData;

    public virtual void Initialise(ProgressCategory progressInfo) => Initialise(progressInfo as ProgressObject);

    public virtual void Initialise(ProgressObject progressInfo)
    {
        progressData = progressInfo;

        iconImage.sprite = progressData.UnfinishedIcon;
        iconText.text = progressData.Name;
    }

    public virtual void SetProgress()
    {
        if (progressData.Completed)
            iconImage.sprite = progressData.FinishedIcon;
    }
}
