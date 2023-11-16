using UnityEngine;
using UnityEngine.UI;

public class Scrapbook : MonoBehaviour
{
    public static Scrapbook Instance { get; private set; }
    public delegate void TextTypingHandler();

    public static TextTypingHandler OnBeginType;
    public static TextTypingHandler OnEndType;

    public GraphicRaycaster Raycaster;
    public ScrapbookPage CurrentPage { get { return allPages[currentPageIndex]; } }

    [SerializeField] private int scrapbookPageCount = 6;

    [SerializeField] private RectTransform book;
    [SerializeField] private Button bookQuestButton;

    [SerializeField] private Vector2 menuPosition;
    [SerializeField] private Vector2 questDockPosition;
    [SerializeField] private Vector2 questExtendPosition;

    [SerializeField] private GameObject elementsPanel;
    [SerializeField] private GameObject extrasGroup;

    [SerializeField] private RectTransform pagesParent;

    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private GameObject nextPageButton;

    [SerializeField] private ScrapbookPage scrapbookPagePrefab;
    [SerializeField] private PageText textEntryPrefab;

    private int currentPageIndex;


    private ScrapbookPage[] allPages;

    private void Awake()
    {
        if(Instance != null)
        {
            throw new System.Exception("Multiple Scrapbooks exist in the world, this shouldn't happen!");
        }
        Instance = this;

        StaticQuestHandler.OnQuestOpened += OpenBookForQuest;
        StaticQuestHandler.OnQuestClosed += CloseBookForQuest;

        SetupScrapbook();

        previousPageButton.SetActive(false);
    }
    private void Start()
    {
        ClosePages();
    }

    public void ClosePages()
    {
        Cursor.lockState = CursorLockMode.Locked;
        extrasGroup.SetActive(false);
        elementsPanel.SetActive(false);

        StaticQuestHandler.OnQuestClosed?.Invoke();

    }

    public void OpenPages()
    {
        Cursor.lockState = CursorLockMode.Confined;
        elementsPanel.SetActive(true);
        extrasGroup.SetActive(true);
    }

    public void GoToNextPage()
    {
        allPages[currentPageIndex].gameObject.SetActive(false);
        currentPageIndex++;
        allPages[currentPageIndex].gameObject.SetActive(true);
        if (!previousPageButton.activeSelf)
        {
            previousPageButton.SetActive(true);
        }
        if (currentPageIndex + 1 == allPages.Length)
        {
            nextPageButton.SetActive(false);
        }

    }

    public void GoToPreviousPage()
    {
        allPages[currentPageIndex].gameObject.SetActive(false);
        currentPageIndex--;
        allPages[currentPageIndex].gameObject.SetActive(true);
        if (!nextPageButton.activeSelf)
        {
            nextPageButton.SetActive(true);
        }
        if (currentPageIndex == 0)
        {
            previousPageButton.SetActive(false);
        }
    }

    public void CreateNewTextEntry()
    {

        PageText newText = Instantiate(textEntryPrefab, CurrentPage.transform);
        newText.TextField.onSelect.AddListener((string s) => { OnBeginType?.Invoke(); });
        newText.TextField.onDeselect.AddListener((string s) => { OnEndType?.Invoke(); });
    }

    private void OpenBookForQuest()
    {
        elementsPanel.SetActive(true);
        book.transform.localPosition = questDockPosition;
        bookQuestButton.gameObject.SetActive(true);
        bookQuestButton.onClick.AddListener(UndockBook);
        bookQuestButton.transform.rotation = Quaternion.Euler(Vector3.forward * -90);

        PagePicture.OnPictureClicked += DockDelegate;
        PagePicture.OnBeginPictureDrag += DockBook;
    }

    private void CloseBookForQuest()
    {
        bookQuestButton.onClick.RemoveAllListeners();
        bookQuestButton.gameObject.SetActive(false);
        book.transform.localPosition = menuPosition;
        elementsPanel.SetActive(false);

        PagePicture.OnPictureClicked -= DockDelegate;
        PagePicture.OnBeginPictureDrag -= DockBook;
    }

    private void UndockBook()
    {
        book.transform.localPosition = questExtendPosition;
        bookQuestButton.onClick.RemoveListener(UndockBook);
        bookQuestButton.onClick.AddListener(DockBook);
        bookQuestButton.transform.rotation = Quaternion.Euler(Vector3.forward * 90);
    }

    private void DockDelegate(PagePicture pict)
    {
        DockBook();
    }

    private void DockBook()
    {
        book.transform.localPosition = questDockPosition;
        bookQuestButton.onClick.RemoveListener(DockBook);
        bookQuestButton.onClick.AddListener(UndockBook);
        bookQuestButton.transform.rotation = Quaternion.Euler(Vector3.forward * -90);
    }

    private void SetupScrapbook()
    {

        allPages = new ScrapbookPage[scrapbookPageCount];

        for (int i = 0; i < scrapbookPageCount; i++)
        {
            ScrapbookPage newPage = Instantiate(scrapbookPagePrefab, pagesParent);
            newPage.SetPageNumber(i + 1);
            newPage.gameObject.SetActive(i == 0);
            allPages[i] = newPage;
        }
    }

}
