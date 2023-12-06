using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressIcon : MonoBehaviour
{
   [SerializeField] private Image iconImage;
   [SerializeField] private TextMeshProUGUI iconText;

    private ProgressObject progressData;

    public void Initialise(ProgressObject progressInfo)
    {
        progressData = progressInfo;

        iconImage.sprite = progressData.UnfinishedIcon;
        iconText.text = progressData.Name;
    }

    public void SetComplete()
    {
        iconImage.sprite = progressData.FinishedIcon;
    }
}
