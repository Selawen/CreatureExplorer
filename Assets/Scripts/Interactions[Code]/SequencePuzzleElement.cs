using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencePuzzleElement : MonoBehaviour, IInteractable
{
    public bool Subscribed { get; private set; }
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Interact";

    [SerializeField] private Material pingMaterial;
    [SerializeField] private Material completionMaterial;
    [SerializeField] private Material wrongMaterial;

    private MeshRenderer meshRenderer;
    private Material defaultMaterial;
    private SequencePuzzle sequencer;

    private bool internalClear;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;
    }

    public void Interact() => PingSequencer();

    public void SubscribeElement(SequencePuzzle sequence)
    {
        sequencer = sequence;
        Subscribed = true;
    }

    public void OnPuzzleFailed()
    {
        StopCoroutine(nameof(MaterialShift));
        StartCoroutine(MaterialShift(wrongMaterial));
    }
    
    public void OnPuzzleClear()
    {
# if UNITY_EDITOR
        Debug.Log("Cleared");
#endif
        internalClear = true;
        StopCoroutine(nameof(MaterialShift));
        meshRenderer.material = completionMaterial;
    }

    private void PingSequencer()
    {
        if (sequencer.Cleared)
            return;

        StopCoroutine(nameof(MaterialShift));
        StartCoroutine(MaterialShift(pingMaterial));
        sequencer.Ping(this);
    }

    private IEnumerator MaterialShift(Material targetMaterial)
    {
        float timer = 0f;
        meshRenderer.material = targetMaterial;

        while(timer < sequencer.MaterialShiftTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!internalClear)
        {
            meshRenderer.material = defaultMaterial;
        }
    }

}
