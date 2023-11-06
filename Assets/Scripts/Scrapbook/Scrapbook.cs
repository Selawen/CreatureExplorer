using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scrapbook : MonoBehaviour
{
    public static Scrapbook Instance { get; private set; }

    public GraphicRaycaster Raycaster;
    public ScrapbookPage CurrentPage { get { return allPages[currentPageIndex]; } }
    //public bool CollectionIsFull { get { return collectedPictures.InventoryIsFull(); } }

    [SerializeField] private int scrapbookPageCount = 6;
    //[SerializeField] private ushort maximumUnplacedPictureCount = 10;
    //[SerializeField] private float rotationRate = 5f;

    [SerializeField] private GameObject elementsPanel;
    [SerializeField] private RectTransform pagesParent;
    //[SerializeField] private LayoutGroup picturePanel;
    [SerializeField] private TMPro.TMP_Text camStorageText;

    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private GameObject nextPageButton;

    [SerializeField] private ScrapbookPage scrapbookPagePrefab;
    [SerializeField] private PageText textEntryPrefab;

    private int currentPageIndex;

    //private MoveablePageComponent targetComponent;
    //private Inventory<PagePicture> collectedPictures;

    private ScrapbookPage[] allPages;

    private void Awake()
    {
        if(Instance != null)
        {
            throw new System.Exception("Multiple Scrapbooks exist in the world, this shouldn't happen!");
        }
        Instance = this;

        allPages = new ScrapbookPage[scrapbookPageCount];
        //collectedPictures = new Inventory<PagePicture>(maximumUnplacedPictureCount);

        //UpdateCameraStorageText();

        for (int i = 0; i < scrapbookPageCount; i++)
        {
            ScrapbookPage newPage = Instantiate(scrapbookPagePrefab, pagesParent);
            newPage.SetPageNumber(i + 1);
            newPage.gameObject.SetActive(i == 0);
            allPages[i] = newPage;
        }
        previousPageButton.SetActive(false);

        ClosePages();
    }

    //public void GetClickInput(InputAction.CallbackContext callbackContext)
    //{
    //    if(callbackContext.started && targetComponent == null)
    //    {
    //        // Begin drawing a group selection rectangle (to be implemented)
    //        return;
    //    }
    //    if(callbackContext.performed && targetComponent != null)
    //    {
    //        // Can now start dragging the element
    //    }
    //    if (callbackContext.canceled)
    //    {
    //        if (targetComponent == null)
    //        {
    //            // End drawing a group selection rectangle (to be implemented) and create a group of all elements within that rectangle
    //            return;
    //        }
    //    }
    //}

    //public void GetTurnAndScaleInput(InputAction.CallbackContext callbackContext)
    //{
    //    if (targetComponent == null || !targetComponent.PlacedOnPage) return;

    //    if (callbackContext.performed)
    //    {
    //        targetComponent.transform.Rotate(new(0, 0, rotationRate * callbackContext.ReadValue<Vector2>().x));
    //        if (targetComponent.GetType() == typeof(PagePicture) || targetComponent.GetType() == typeof(ScrapbookSticker))
    //        {
    //            targetComponent.transform.localScale = Mathf.Clamp(targetComponent.transform.localScale.x + 0.1f * callbackContext.ReadValue<Vector2>().y, 1, 3) * Vector3.one;
    //        }
    //    }
    //}

    public void ClosePages()
    {
        elementsPanel.SetActive(false);
    }

    public void OpenPages()
    {
        elementsPanel.SetActive(true);
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

    //public bool AddPictureToCollection(PagePicture snappedPicture)
    //{
    //    if (collectedPictures.AddItemToInventory(snappedPicture))
    //    {
    //        snappedPicture.transform.SetParent(picturePanel.transform, false);
    //        UpdateCameraStorageText();
    //        return true;
    //    }
    //    return false;
    //    To do: send out a message that the scrapbook's picture storage is full.
    //}

    //public bool RemovePictureFromCollection(PagePicture removedPicture)
    //{
    //    if (collectedPictures.RemoveItemFromInventory(removedPicture))
    //    {
    //        UpdateCameraStorageText();
    //        return true;
    //    }
    //    return false;
    //}

    //public List<PagePicture> GetCollectedPictures()
    //{
    //    return collectedPictures.GetContents();
    //}

    public void CreateNewTextEntry() => Instantiate(textEntryPrefab, CurrentPage.transform);

    //public void SwapTargetComponent(MoveablePageComponent newComponent) => targetComponent = newComponent;

    //private void UpdateCameraStorageText()
    //{
    //    if (camStorageText == null) return;

    //    ushort storageLeft = (ushort)(collectedPictures.GetCapacity() - collectedPictures.GetItemCount());
    //    if(storageLeft < 3)
    //    {
    //        camStorageText.color = Color.red;
    //    }
    //    else
    //    {
    //        camStorageText.color = Color.white;
    //    }
    //    camStorageText.text = "Storage left: " + storageLeft.ToString();

    //}

}
