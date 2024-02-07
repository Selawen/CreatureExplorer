using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIHandler : MonoBehaviour
{

    [Button("GetProgressCategories")]
    public bool update;

    [field: SerializeField] private GameObject progressCategoryGrid;
    [field: SerializeField] private GameObject progressDetailPages;
    [field: SerializeField] private GameObject backButton;

    [Header("Prefabs")]
    [field: SerializeField] private GameObject progressCategoryPrefab;
    [field: SerializeField] private GameObject progressIconPrefab;

    [field: SerializeField] protected Tracker trackerReference;
    public static Tracker trackedProgress { get; private set; }

    private static Dictionary<ProgressObject, ProgressIcon> IconObjects;

    public static ProgressCategory[] TrackedCategories
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

        foreach(ProgressIcon element in GetComponentsInChildren<ProgressIcon>())
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
        ProgressCategoryIcon iconObject = Instantiate(progressCategoryPrefab, progressCategoryGrid.transform).GetComponent<ProgressCategoryIcon>();

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

        if (progressObject.DetailPage != null && progressDetailPages.transform.Find(progressObject.DetailPage.name + "(Clone)") == null) 
            Instantiate(progressObject.DetailPage, progressDetailPages.transform).SetActive(false);

    }

    public void OpenPage(GameObject pagePrefab)
    {
        // to get the layout to work properly, add an empty object at the end if the number of pages is uneven (wrong way around because there is always a button that should not be counted)
        if (progressDetailPages.transform.childCount %2 == 0)
        {
            GameObject emptyPage = new GameObject();
            emptyPage.AddComponent<RectTransform>();
            Instantiate(emptyPage, progressDetailPages.transform).SetActive(false);
        }
        progressCategoryGrid.SetActive(false);
        progressDetailPages.SetActive(true);
        backButton.SetActive(true);

        Transform page = progressDetailPages.transform.Find(pagePrefab.name + "(Clone)");

        if (page != null)
        {
            page.gameObject.SetActive(true);

            int otherPageNumber = page.GetSiblingIndex();
            otherPageNumber += (otherPageNumber % 2 == 1) ? 1:-1;       //looks like it's the wrong way around, but there is always a button that comes first in the hierarchy
            progressDetailPages.transform.GetChild(otherPageNumber).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Page not found");
        }
    }

    public void CloseDetailPage()
    {
        progressCategoryGrid.SetActive(true);

        for (int x = progressDetailPages.transform.childCount-1; x >= 0; x--)
        {
            progressDetailPages.transform.GetChild(x).gameObject.SetActive(false);

        }

        progressDetailPages.SetActive(false);

    }

    public static void UpdateTrackedProgress(ProgressObject progress)
    {
        //progress.AddProgress();
        trackedProgress.UpdateProgress();

        if (IconObjects.TryGetValue(progress, out ProgressIcon icon))
            icon.SetProgress();
    }

}
