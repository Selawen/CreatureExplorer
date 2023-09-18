using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Behaviour
{
    

    public void FixedUpdateBehaviour()
    {
        GetComponent<Rigidbody>().MovePosition(targetObj.transform.position);

        if ((transform.position - targetObj.transform.position).magnitude < 0.5f)
        {
            finished = true;
        }
    }
}
