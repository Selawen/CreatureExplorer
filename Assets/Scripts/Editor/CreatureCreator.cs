using UnityEditor;
using UnityEngine;
using System.IO;

public class CreatureCreator : EditorWindow
{
    private GameObject creatureTemplate;
    private GameObject currentReference;

    private GameObject prefabInstance;

    private string path;
    private string creatureName;
    private float size;


    [MenuItem("Tools/Creature Creator")]
    public static void OpenWindow()
    {
        GetWindow(typeof(CreatureCreator));
    }

    public void CreateCreaturePrefab()
    {
        if(!Directory.Exists(Application.dataPath + "/Prefabs/" + path))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", path);
        }
        string localPath = $"Assets/Prefabs/{path}/{creatureName}.prefab";

        //using (FileStream stream = File.OpenWrite(path))
        //{

        //ApplyChanges();
        if (File.Exists(localPath))
        {
            currentReference = PrefabUtility.LoadPrefabContents(localPath);
        }

        if(currentReference == null)
        {
            PrefabUtility.SaveAsPrefabAsset(creatureTemplate, localPath, out bool succes);
            if (succes)
            {
                prefabInstance = PrefabUtility.LoadPrefabContents(localPath);
                prefabInstance.transform.localScale = Vector3.one * size;
                prefabInstance.name = creatureName;
                Debug.Log($"Succesfully created a prefab of: {creatureName} at {path}");
                return;
            }
            Debug.LogError($"Could not make prefab of {creatureName} at path: {path}!");
        }
        else
        {
            currentReference.transform.localScale = Vector3.one * size;
            //currentReference.name = creatureName;
            PrefabUtility.SavePrefabAsset(currentReference);
        }
        //}
    }

    private void OnValidate()
    {
        ApplyChanges();
    }

    private void ApplyChanges()
    {
        //if (currentReference == null)
        //{
        //    currentReference = Instantiate(creatureTemplate, Vector3.zero, Quaternion.identity);
        //}
        //currentReference.transform.localScale = Vector3.one * size;
        //currentReference.name = creatureName;
    }


    private void OnGUI()
    {
        creatureTemplate = EditorGUILayout.ObjectField("Template", creatureTemplate, typeof(GameObject), true) as GameObject;
        path = EditorGUILayout.TextField("Path", path);
        creatureName = EditorGUILayout.TextField("Creature Name", creatureName);
        size = EditorGUILayout.Slider("Size", size, 0.5f, 5f);

        if(GUILayout.Button("Create Prefab"))
        {
            CreateCreaturePrefab();
        } 
        if(GUILayout.Button("Apply Changes"))
        {

        }
    }
}
