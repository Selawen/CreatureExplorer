using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dissolveController : MonoBehaviour
{
    public bool creatureDied;
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    public SkinnedMeshRenderer skinnedMesh;
    private Material[] skinnedMaterials;
    
    void Start()
    {
        if(skinnedMesh != null)
        {
            skinnedMaterials = skinnedMesh.materials;
        }
    }

    void Update()
    {
        if(creatureDied)
        {
            StartCoroutine(DissolveCo());
        }
    }

    //increase dissolveAmount at dissolveRate until fully dissolved
    IEnumerator DissolveCo()
    {
        if(skinnedMaterials.Length > 0)
        {
            float counter = 0;
            while (skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for(int i=0; i<skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
