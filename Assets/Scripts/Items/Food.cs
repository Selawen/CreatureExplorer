using UnityEngine;

public class Food : MonoBehaviour
{
    private void Awake()
    {
        if (GetComponent<Rigidbody>().useGravity != true)
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    public void ActivatePhysics()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().isTrigger = false;
        transform.parent = null;
    }
}
