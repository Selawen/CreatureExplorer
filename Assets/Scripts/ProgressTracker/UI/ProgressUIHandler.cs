using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIHandler : MonoBehaviour
{

    [Button("GetProgressCategories")]
    public bool update;

    [field: SerializeField] private GameObject progressIconPrefab;
    [field: SerializeField] private static Dictionary<ProgressObject, ProgressIcon> IconObjects;

    [field: SerializeField] protected Tracker trackerReference;
    private static Tracker trackedProgress;

    protected static ProgressCategory[] TrackedCategories
    {
        get => trackedProgress.ProgressCategories;
    }

    private void Awake()
    {
        GetProgressCategories();
    }

    private void GetProgressCategories()
    {
        IconObjects = new Dictionary<ProgressObject, ProgressIcon>();

        if (trackedProgress == null)
        {
            trackedProgress = trackerReference;
        }

        foreach(LayoutElement element in GetComponentsInChildren<LayoutElement>())
        {
            DestroyImmediate(element.gameObject);
        }

        foreach(ProgressCategory category in trackedProgress.ProgressCategories)
        {
            CreateIcon(category);
        }
    }

    public void CreateIcon(ProgressObject progressObject)
    {
        ProgressIcon iconObject = Instantiate(progressIconPrefab, transform).GetComponent<ProgressIcon>();

        iconObject.Initialise(progressObject);
        IconObjects.Add(progressObject, iconObject);
    }

    public static void UpdateTrackedProgress(ProgressObject progress)
    {
        progress.AddProgress();
        trackedProgress.UpdateProgress();

        if (progress.Completed)
        {
            IconObjects[progress].SetComplete();
        }
    }

}
