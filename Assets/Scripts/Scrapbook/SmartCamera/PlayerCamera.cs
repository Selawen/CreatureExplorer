using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera pictureCamera;
    [SerializeField] private float zoomSensitivity;

    [SerializeField] private PagePicture picturePrefab;

    [SerializeField] private float maximumScanDistance = 20f;
    [SerializeField] private LayerMask ignoredPhotoLayers;
    [SerializeField] private TMPro.TMP_Text exceptionText;

    [SerializeField, Range(1, 100)] private int photoAccuracy = 50;

    [SerializeField] private Animator shutterTop, shutterBottom;

    private float originalZoom;

    private PlayerInput input;

    private bool snapping;

    private string path;

    private void OnValidate()
    {
        if (pictureCamera == null)
        {
            pictureCamera = Camera.main;
        }
    }

    private void Awake()
    {
        originalZoom = pictureCamera.fieldOfView;

        input = GetComponent<PlayerInput>();
        if (Application.isEditor)
        {
            path = Application.dataPath;
            return;
        }
        path = Application.dataPath;
    }

    private void OnDisable()
    {
        pictureCamera.fieldOfView = originalZoom;
    }

    public void ZoomCamera(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            pictureCamera.fieldOfView -= callbackContext.ReadValue<Vector2>().y * zoomSensitivity;
            pictureCamera.fieldOfView = Mathf.Clamp(pictureCamera.fieldOfView, 0, 60);
        }
    }

    public void SnapPicture(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (!Scrapbook.Instance.CollectionIsFull && !snapping)
            {
                //shutterTop.SetTrigger("Snap");
                //shutterBottom.SetTrigger("Snap");
                StartCoroutine(Snap());
            }
        }
    }

    private IEnumerator Snap()
    {
        if (pictureCamera != Camera.main)
            pictureCamera.gameObject.SetActive(true);
        snapping = true;

        yield return new WaitForEndOfFrame();

        RenderTexture screenTexture = new RenderTexture(Screen.height, Screen.height, 16);
        pictureCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        pictureCamera.Render();

        PictureInfo pictureInfo = new PictureInfo(AnalyzeSubjects(), transform.position);

        Texture2D renderedTexture = new Texture2D(Screen.height, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.height, Screen.height), 0, 0);
        RenderTexture.active = null;

        try
        {
            string savingPath = path + "/Resources/snap" + System.DateTime.UtcNow.Day + System.DateTime.UtcNow.Minute + System.DateTime.UtcNow.Second + ".png";

            byte[] byteArray = renderedTexture.EncodeToPNG();
            File.WriteAllBytes(savingPath, byteArray);

            Texture2D png = LoadTexture(savingPath);
            Sprite spr = Sprite.Create(png, new Rect(0, 0, png.width, png.height), Vector2.one * 0.5f);

            PagePicture newPagePicture = Instantiate(picturePrefab);
            newPagePicture.SetPicture(spr);
            newPagePicture.LinkPictureInformation(pictureInfo);
            Scrapbook.Instance.AddPictureToCollection(newPagePicture);
        }
        catch (System.Exception exception)
        {
            if (exceptionText != null)
            {
                exceptionText.text = exception.ToString();
            }
        }

        if (pictureCamera != Camera.main)
            pictureCamera.gameObject.SetActive(false);
        else
            pictureCamera.targetTexture = null;

        snapping = false;

    }
    private void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(pictureCamera.transform.position - transform.position, transform.rotation, transform.lossyScale);

        Gizmos.matrix = rotationMatrix;

        float camStep = pictureCamera.pixelHeight / photoAccuracy;
        float xStart = (pictureCamera.pixelWidth - pictureCamera.pixelHeight) * 0.5f;

        for (int x = 0; x <= photoAccuracy; x++)
        {
            for (int y = 0; y <= photoAccuracy; y++)
            {
                Ray ray = pictureCamera.ScreenPointToRay(new Vector3(xStart + x * camStep, y * camStep));
                Gizmos.DrawRay(ray.origin, ray.direction * maximumScanDistance);
            }
        }

        Gizmos.matrix = originalMatrix;
    }

    private List<QuestableObject> AnalyzeSubjects()
    {
        List<QuestableObject> result = new();

        float camStep = pictureCamera.pixelHeight / photoAccuracy;
        float xStart = (pictureCamera.pixelWidth - pictureCamera.pixelHeight) * 0.5f;

        for (int x = 0; x <= photoAccuracy; x++)
        {
            for (int y = 0; y <= photoAccuracy; y++)
            {
                Ray ray = pictureCamera.ScreenPointToRay(new Vector3(xStart + x * camStep, y * camStep));
                if(Physics.Raycast(ray, out RaycastHit hit, maximumScanDistance, ~ignoredPhotoLayers))
                {
                    if (hit.transform.TryGetComponent(out QuestableObject questableObject))
                    {
                        if (!result.Contains(questableObject))
                        {
                            result.Add(questableObject);
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
