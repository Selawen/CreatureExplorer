using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SliderValueText : MonoBehaviour
{
    [SerializeField] private float valueMultiplier = 1;
    private TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void SetValueText(float value)
    {
        textMesh.text = ((int)(value*valueMultiplier)).ToString();
    }
}
