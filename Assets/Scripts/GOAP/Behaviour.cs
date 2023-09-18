using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour : MonoBehaviour
{
    public bool finished = false;

    public GameObject targetObj;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateBehaviour()
    {
        //cut action off after 1 minute
        timer += Time.deltaTime;
        if (timer > 60)
        {
            finished = true;
        }
    }
}
