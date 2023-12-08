using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ProgressDetailIcon : ProgressIcon
{
    [SerializeField] private string path;

    private void OnEnable()
    {
        Debug.Log("Page enabled");
        string[] splitPath = path.Split('/');

        ProgressCategory[] categories = ProgressUIHandler.TrackedCategories;
        ProgressCategory rightCategory = null;

        for (int x = 0; x< splitPath.Length-1; x++)
        {
            foreach (ProgressCategory progress in categories)
            {
                if (progress.IsCategory(splitPath[x], out rightCategory))
                {
                    categories = rightCategory.Tracked;
                    break;
                }
            }
        }

        if (rightCategory == null)
            Debug.Log("Invalid category path");

        if (rightCategory.HasID(splitPath[splitPath.Length-1], out progressData))
        {
            Initialise(progressData);
            Debug.Log($"found! {progressData.Name}");
        }
    }
}
