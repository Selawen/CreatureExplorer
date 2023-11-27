using System.Collections;
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
        StartCoroutine(Decay(10));
    }

    private IEnumerator Decay(float timer)
    {
        yield return new WaitForSeconds(timer);

        DestroyImmediate(gameObject);
    }
}
