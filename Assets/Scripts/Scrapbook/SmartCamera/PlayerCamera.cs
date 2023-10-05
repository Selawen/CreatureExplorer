using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera pictureCamera;

    [SerializeField] private PagePicture picturePrefab;

    [SerializeField] private float maximumScanDistance = 20f;
    [SerializeField] private LayerMask ignoredPhotoLayers;

    [SerializeField, Range(1, 100)] private int photoAccuracy = 50;

    private string path;

    private void Awake()
    {
        if (Application.isEditor)
        {
            path = Application.dataPath;
            return;
        }
        path = Application.persistentDataPath;
    }
    public void SnapPicture(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            Snap();
        }
    }

    private void Snap()
    {
        if (Scrapbook.Instance.CollectionIsFull)
            return;

        pictureCamera.gameObject.SetActive(true);

        RenderTexture screenTexture = new RenderTexture(Screen.height, Screen.height, 16);
        pictureCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        pictureCamera.Render();

        PictureInfo pictureInfo = new PictureInfo(AnalyzeSubjects(), transform.position);

        Texture2D renderedTexture = new Texture2D(Screen.height, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.height, Screen.height), 0, 0);
        RenderTexture.active = null;

        string savingPath = path + "/Pictures/snap" + System.DateTime.UtcNow.Day + System.DateTime.UtcNow.Minute + System.DateTime.UtcNow.Second + ".png";
        byte[] byteArray = renderedTexture.EncodeToPNG();
        File.WriteAllBytes(savingPath, byteArray);

        Texture2D png = LoadTexture(savingPath);
        Sprite spr = Sprite.Create(png, new Rect(0, 0, png.width, png.height), Vector2.one * 0.5f);

        PagePicture newPagePicture = Instantiate(picturePrefab);
        newPagePicture.SetPicture(spr);
        newPagePicture.LinkPictureInformation(pictureInfo);
        Scrapbook.Instance.AddPictureToCollection(newPagePicture);

        pictureCamera.gameObject.SetActive(false);

    }
    private void OnDrawGizmos()
    {
        float camStep = pictureCamera.pixelHeight / photoAccuracy;
        float xStart = (pictureCamera.pixelWidth - pictureCamera.pixelHeight) * 0.5f;

        for (int x = 0; x <= photoAccuracy; x++)
        {
            for (int y = 0; y <= photoAccuracy; y++)
            {
                Ray ray = pictureCamera.ScreenPointToRay(new Vector3(xStart + x * camStep, y * camStep));
                Gizmos.DrawLine(ray.origin, ray.direction * maximumScanDistance);
            }
        }
    }

    private List<IIdentifiable> AnalyzeSubjects()
    {
        List<IIdentifiable> result = new();

        float camStep = pictureCamera.pixelHeight / photoAccuracy;
        float xStart = (pictureCamera.pixelWidth - pictureCamera.pixelHeight) * 0.5f;

        for (int x = 0; x <= photoAccuracy; x++)
        {
            for (int y = 0; y <= photoAccuracy; y++)
            {
                Ray ray = pictureCamera.ScreenPointToRay(new Vector3(xStart + x * camStep, y * camStep));
                if(Physics.Raycast(ray, out RaycastHit hit, maximumScanDistance, ~ignoredPhotoLayers))
                {
                    if (hit.transform.TryGetComponent(out IIdentifiable identifiable))
                    {
                        if (!result.Contains(identifiable))
                        {
                            result.Add(identifiable);
                        }
                    }
                }
            }
        }
        return result;
    }


    private Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);
            if (Tex2D.LoadImage(FileData))
            {
                return Tex2D;                
            }          
        }
        return null;                    
    }
}