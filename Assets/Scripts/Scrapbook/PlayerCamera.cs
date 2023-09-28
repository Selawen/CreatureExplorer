using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image img;

    public void SnapPicture(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            StartCoroutine(Snap());
        }
    }

    private IEnumerator Snap()
    {
        canvas.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenCap = ScreenCapture.CaptureScreenshotAsTexture();
        Sprite spr = Sprite.Create(screenCap, new Rect((screenCap.width - screenCap.height) * 0.5f, 0, screenCap.height, screenCap.height), Vector2.one * 0.5f);
        img.overrideSprite = spr;

        canvas.gameObject.SetActive(true);
    }

}
