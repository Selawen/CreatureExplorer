using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerHandler : MonoBehaviour
{
    [SerializeField] private Button stickerButtonPrefab;
    [SerializeField] private LayoutGroup stickerPanel;

    private ScrapbookSticker[] availableStickers;

    private void Awake()
    {
        availableStickers = Resources.LoadAll<ScrapbookSticker>("");
        foreach(ScrapbookSticker sticker in availableStickers)
        {
            Instantiate(sticker, stickerPanel.transform);
            //Button stickerButton = Instantiate(stickerButtonPrefab, stickerPanel.transform);
            //stickerButton.GetComponent<Image>().sprite = sticker.GetComponent<Image>().sprite;
            //// For debugging purposes, we're checking if the name corresponds to the right sticker. 
            //// Later on, we'll spawn a new sticker, while keeping the button intact.
            //stickerButton.onClick.AddListener(() => Debug.Log("Sticker of type: " + sticker.name + " has been clicked"));
        }
    }
}
