using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PictureStorage : PageComponentInteractor
{
    [SerializeField] private Transform[] photoSpots;

    [SerializeField] private TMP_Text camStorageText;
    [SerializeField] private TMP_Text maxStorageText;

    [SerializeField] private Image storageBackgroundImage;
    [SerializeField] private Sprite storageBackgroundDefault;
    [SerializeField] private Sprite storageBackgroundFull;

    //private ushort currentPictureIndex;
    private Inventory<PagePicture> pictureInventory;

    private void Awake()
    {
        pictureInventory = new Inventory<PagePicture>((ushort)photoSpots.Length);
        maxStorageText.text = pictureInventory.GetCapacity().ToString();
        UpdateCameraStorageText();
    }

    public bool StorageIsFull() => pictureInventory.InventoryIsFull();

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        Debug.Log("A component has been dropped on the storage");
        if (component.GetType() != typeof(PagePicture)) return false;

        Debug.Log("Component has been id'ed as a picture");
        PagePicture picture = component as PagePicture;
        if (!pictureInventory.GetContents().Contains(picture))
        {
            if (StorageIsFull()) return false;

            foreach(Transform t in photoSpots)
            {
                if(t.childCount == 0)
                {
                    Debug.Log("Should now set picture as a parent of photo spot");
                    pictureInventory.AddItemToInventory(picture);

                    component.transform.SetPositionAndRotation(t.position, t.rotation);
                    component.transform.SetParent(t, true);

                    //picture.SetInteractor(this);

                    UpdateCameraStorageText();
                    return true;

                }
            }
        }
        return false;
    }

    public override void RemoveFromInteractor(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture)) return;

        pictureInventory.RemoveItemFromInventory(component as PagePicture);
        UpdateCameraStorageText();
    }
    private void UpdateCameraStorageText()
    {
        if (pictureInventory.InventoryIsFull())
        {
            camStorageText.color = Color.red;
            storageBackgroundImage.sprite = storageBackgroundFull;
        }
        else
        {
            camStorageText.color = Color.black;
            storageBackgroundImage.sprite = storageBackgroundDefault;
        }
        camStorageText.text = pictureInventory.GetItemCount().ToString();

    }
}
