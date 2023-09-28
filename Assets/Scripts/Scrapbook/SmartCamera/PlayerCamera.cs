using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera pictureCamera;
    [SerializeField] private Image img;
    [SerializeField] private MeshFilter m;

    [SerializeField] private float maximumScanDistance = 20f;
    [SerializeField] private LayerMask ignoredPhotoLayers;

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
    private void Update()
    {
        //foreach(Vector3 v in AnalyzeSubjects())
        //{
        //    Debug.DrawLine(pictureCamera.transform.position, v);
        //}
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
        pictureCamera.gameObject.SetActive(true);

        RenderTexture screenTexture = new RenderTexture(Screen.height, Screen.height, 0);
        pictureCamera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        pictureCamera.Render();

        List<GameObject> picts = AnalyzeSubjects();
        foreach (GameObject g in picts)
        {
            Debug.Log("Found " + g.name + " in the picture!");
        }

        Texture2D renderedTexture = new Texture2D(Screen.height, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.height, Screen.height), 0 , 0);
        RenderTexture.active = null;

        string savingPath = path + "/Pictures/snap" + System.DateTime.UtcNow.Day + System.DateTime.UtcNow.Minute + System.DateTime.UtcNow.Second + ".png";
        byte[] byteArray = renderedTexture.EncodeToPNG();
        File.WriteAllBytes(savingPath, byteArray);

        Texture2D png = LoadTexture(savingPath);
        Sprite spr = Sprite.Create(png, new Rect(0, 0, png.width, png.height), Vector2.one * 0.5f);
        img.overrideSprite = spr;

        pictureCamera.gameObject.SetActive(false);

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    //Gizmos.DrawWireCube(pictureCamera.transform.position + pictureCamera.transform.forward * 5, pictureCamera.ScreenToWorldPoint(new(pictureCamera.pixelHeight, pictureCamera.pixelHeight)) * 0.5f);
    //    //Mesh m = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<Mesh>();
    //    //Gizmos.DrawWireMesh(m.mesh, pictureCamera.transform.position + pictureCamera.transform.forward, pictureCamera.transform.rotation);
    //}

    private List<GameObject> AnalyzeSubjects()
    {
        List<GameObject> result = new();

        RaycastHit[] hits = Physics.BoxCastAll(pictureCamera.transform.position, new(5f, 5f), pictureCamera.transform.forward, pictureCamera.transform.rotation, maximumScanDistance, ~ignoredPhotoLayers);

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(pictureCamera);
        foreach(RaycastHit hit in hits)
        {
            if (!GeometryUtility.TestPlanesAABB(planes, hit.collider.bounds))
                continue;

            Vector3 cwts = pictureCamera.WorldToScreenPoint(hit.point);
            if (cwts.x > 0 && cwts.x < 1 && cwts.y > 0 && cwts.y < 1 && cwts.z > 0)
            {
                result.Add(hit.transform.gameObject);
            }
        }

        //RaycastHit[] hits = Physics.BoxCastAll(pictureCamera.transform.position, new(5f,5f), pictureCamera.transform.forward, pictureCamera.transform.rotation, maximumScanDistance);
        //foreach(RaycastHit hit in hits)
        //{
        //    if(Physics.Raycast(pictureCamera.transform.position, hit.point - pictureCamera.transform.position, out RaycastHit testHit, maximumScanDistance))
        //    {
        //        if(testHit.transform == hit.transform)
        //        {
        //            result.Add(hit.transform.gameObject);
        //        }
        //    }
        //}
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
