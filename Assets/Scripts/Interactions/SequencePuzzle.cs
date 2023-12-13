using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencePuzzle : MonoBehaviour
{
    public float MaterialShiftTimer { get { return materialShiftTime; } }
    public bool Cleared { get; private set; }

    [SerializeField] private SequencePuzzleElement[] puzzleElements;
    [SerializeField] private bool canPingMultipleTimes;
    [SerializeField] private float materialShiftTime  = 2f;

    private int sequenceCount;
    private Queue<SequencePuzzleElement> pingOrder;

    private void Awake()
    {
        pingOrder = new Queue<SequencePuzzleElement>();
        foreach(SequencePuzzleElement element in puzzleElements)
        {
            if (!element.Subscribed)
            {
                element.SubscribeElement(this);
            }
        }
    }

    public void Ping(SequencePuzzleElement element)
    {
        if (pingOrder.Contains(element) && !canPingMultipleTimes) 
            return;

        pingOrder.Enqueue(element);
        sequenceCount++;
        if(sequenceCount == puzzleElements.Length)
        {
            Evaluate();
        }
    }

    private void Evaluate()
    {
        sequenceCount = 0;
        for (int i = 0; i < puzzleElements.Length; i++)
        {
            if(puzzleElements[i] != pingOrder.Dequeue())
            {
                // This isn't the right order, puzzle failed
                foreach(SequencePuzzleElement element in puzzleElements)
                {
                    element.OnPuzzleFailed();
                }
                pingOrder.Clear();
                return;
            }
        }
        foreach (SequencePuzzleElement element in puzzleElements)
        {
            element.OnPuzzleClear();
        }
        Cleared = true;
    }

    
}
