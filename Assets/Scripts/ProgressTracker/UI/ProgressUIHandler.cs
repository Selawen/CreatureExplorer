using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIHandler : MonoBehaviour
{

    [Button("GetProgressCategories")]
    public bool update;

    [field: SerializeField] private GameObject progressCategoryPrefab;
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

    /// <summary>
    /// Create ui for the root category 
    /// </summary>
    /// <param name="progressObject"></param>
    public void CreateIcon(ProgressCategory progressObject)
    {
        ProgressCategoryIcon iconObject = Instantiate(progressCategoryPrefab, transform).GetComponent<ProgressCategoryIcon>();

        iconObject.Initialise(progressObject);
        IconObjects.Add(progressObject, iconObject);
    }

    /// <summary>
    /// Add category icons to the shelf of the root category
    /// </summary>
    /// <param name="progressObject"></param>
    /// <param name="parentTransform"></param>
    public void CreateIcon(ProgressObject progressObject, Transform parentTransform)
    {
        ProgressIcon iconObject = Instantiate(progressIconPrefab, parentTransform).GetComponent<ProgressIcon>();

        iconObject.Initialise(progressObject);
        IconObjects.Add(progressObject, iconObject);
    }

    public static void UpdateTrackedProgress(ProgressObject progress)
    {
        progress.AddProgress();
        trackedProgress.UpdateProgress();

        IconObjects[progress].SetProgress();
    }

}
