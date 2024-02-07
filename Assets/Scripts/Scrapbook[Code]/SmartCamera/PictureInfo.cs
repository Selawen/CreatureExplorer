using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureInfo
{
    public string PicturePath;
    public List<QuestableObject> PictureObjects { get; private set; }
    public Vector3 PictureLocation { get; private set; }

    public PictureInfo(List<QuestableObject> pictureObjects, Vector3 location)
    {
        PictureObjects = pictureObjects;
        PictureLocation = location;
    }
    
}
