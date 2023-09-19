using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrapbook : MonoBehaviour
{
    public ScrapbookPage CurrentPage { get { return allPages[currentPageIndex]; } }

    [SerializeField] private int scrapbookPageCount = 6;
    [SerializeField] private ushort maximumUnplacedPictureCount = 10;
    [SerializeField] private ScrapbookPage scrapbookPagePrefab;
    [SerializeField] private RectTransform pagesParent;

    private int currentPageIndex;

    private Inventory<PagePicture> collectedPictures;

    private ScrapbookPage[] allPages;

    private void Awake()
    {
        allPages = new ScrapbookPage[scrapbookPageCount];
        collectedPictures = new Inventory<PagePicture>(maximumUnplacedPictureCount);
        for (int i = 0; i < scrapbookPageCount; i++)
        {
            ScrapbookPage newPage = Instantiate(scrapbookPagePrefab, pagesParent);
            newPage.gameObject.SetActive(i == 0);
            allPages[i] = newPage;
        }
    }

    public void GoToNextPage()
    {
        if(currentPageIndex + 1 < allPages.Length)
        {
            allPages[currentPageIndex].gameObject.SetActive(false);
            currentPageIndex++;
            allPages[currentPageIndex].gameObject.SetActive(true);
        }
    }

    public void GoToPreviousPage()
    {
        if (currentPageIndex - 1 >= 0)
        {
            allPages[currentPageIndex].gameObject.SetActive(false);
            currentPageIndex--;
            allPages[currentPageIndex].gameObject.SetActive(true);
        }
    }

    public void AddSnappedPicture(PagePicture snappedPicture)
    {
        if (collectedPictures.AddItemToInventory(snappedPicture))
        {
            // To do: create a representation of the new picture in the placement panel
            return;
        }
        // To do: send out a message that the scrapbook's picture storage is full.
    }

}
