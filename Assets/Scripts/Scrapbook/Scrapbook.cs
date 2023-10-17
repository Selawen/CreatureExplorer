using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Scrapbook : MonoBehaviour
{
    public static Scrapbook Instance { get; private set; }
    public ScrapbookPage CurrentPage { get { return allPages[currentPageIndex]; } }
    public bool CollectionIsFull { get { return collectedPictures.InventoryIsFull(); } }

    [SerializeField] private int scrapbookPageCount = 6;
    [SerializeField] private ushort maximumUnplacedPictureCount = 10;

    [SerializeField] private GameObject elementsPanel;
    [SerializeField] private RectTransform pagesParent;
    [SerializeField] private LayoutGroup picturePanel;

    [SerializeField] private ScrapbookPage scrapbookPagePrefab;
    [SerializeField] private PageText textEntryPrefab;
    [SerializeField] private PlayerInput input;

    private int currentPageIndex;

    private Inventory<PagePicture> collectedPictures;

    private ScrapbookPage[] allPages;

    private void Awake()
    {
        if(Instance != null)
        {
            throw new System.Exception("Multiple Scrapbooks exist in the world, this shouldn't happen!");
        }
        Instance = this;

        allPages = new ScrapbookPage[scrapbookPageCount];
        collectedPictures = new Inventory<PagePicture>(maximumUnplacedPictureCount);
        for (int i = 0; i < scrapbookPageCount; i++)
        {
            ScrapbookPage newPage = Instantiate(scrapbookPagePrefab, pagesParent);
            newPage.gameObject.SetActive(i == 0);
            allPages[i] = newPage;
        }
    }

    public void OpenPages()
    {
        elementsPanel.SetActive(true);
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

    public bool AddPictureToCollection(PagePicture snappedPicture)
    {
        if (collectedPictures.AddItemToInventory(snappedPicture))
        {
            snappedPicture.transform.SetParent(picturePanel.transform, false);
            return true;
        }
        return false;
        // To do: send out a message that the scrapbook's picture storage is full.
    }
    
    public List<PagePicture> GetCollectedPictures()
    {
        return collectedPictures.GetContents();
    }

    public void CreateNewTextEntry()
    {
        PageText writeableText = Instantiate(textEntryPrefab, CurrentPage.transform);
        writeableText.TextField.onSelect.AddListener((string s) => input.SwitchCurrentActionMap("Typing"));
        writeableText.TextField.onDeselect.AddListener((string s) => input.SwitchCurrentActionMap("Scrapbook"));
    }

}
