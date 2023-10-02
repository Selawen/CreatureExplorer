using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureInfo
{
    public List<GameObject> PictureObjects { get; private set; }
    public Vector3 PictureLocation { get; private set; }

    public PictureInfo(List<GameObject> pictureObjects, Vector3 location)
    {
        PictureObjects = pictureObjects;
        PictureLocation = location;
    }
    
}
