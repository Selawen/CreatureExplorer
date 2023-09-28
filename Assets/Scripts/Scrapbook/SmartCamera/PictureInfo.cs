using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureInfo
{
    public List<GameObject> PictureObjects { get; private set; }

    public PictureInfo(List<GameObject> pictureObjects)
    {
        PictureObjects = pictureObjects;
    }
    
}
