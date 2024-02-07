using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TextBackground : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Activate(string activationText)
    {
        if (activationText == "")
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        } else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }

        gameObject.SetActive(true);
    }
}
