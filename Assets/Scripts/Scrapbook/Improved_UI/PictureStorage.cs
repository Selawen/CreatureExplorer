using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PictureStorage : PageComponentInteractor
{
    [SerializeField] private Transform[] photoSpots;
    [SerializeField] private TMP_Text camStorageText;

    private ushort currentPictureIndex;
    private Inventory<PagePicture> pictureInventory;

    private void Awake()
    {
        pictureInventory = new Inventory<PagePicture>((ushort)photoSpots.Length);
        UpdateCameraStorageText();
    }

    public bool StorageIsFull() => pictureInventory.InventoryIsFull();

    public override bool OnComponentDroppedOn(PageComponent component)
    {
        if (component.GetType() != typeof(PagePicture)) return false;

        PagePicture picture = component as PagePicture;
        if (!pictureInventory.GetContents().Contains(picture))
        {
            if (StorageIsFull()) return false;

            foreach(Transform t in photoSpots)
            {
                if(t.childCount == 0)
                {
                    pictureInventory.AddItemToInventory(picture);

                    component.transform.position = t.position;
                    component.transform.rotation = t.rotation;
                    component.transform.SetParent(t, true);

                    picture.SetInteractor(this);

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
        ushort storageLeft = (ushort)(pictureInventory.GetCapacity() - pictureInventory.GetItemCount());
        if (storageLeft < 3)
        {
            camStorageText.color = Color.red;
        }
        else
        {
            camStorageText.color = Color.white;
        }
        camStorageText.text = "Storage left: " + storageLeft.ToString();

    }
}
