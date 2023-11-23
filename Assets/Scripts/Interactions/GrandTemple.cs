using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandTemple : MonoBehaviour
{
    //public static GrandTemple Instance { get; private set; }
    public static System.Action<int> OnStatueCleared;
    public static System.Action OnRingExtended;

    [SerializeField] private Animator topRing, middleRing, bottomRing;
    [SerializeField] private int topRingRequirement = 2, middleRingRequirement = 3, bottomRingRequirement = 4;

    private int topRingClears, middleRingClears, bottomRingClears;

    private void Awake()
    {
        OnStatueCleared += EvaluateStatue;
    }

    public void EvaluateStatue(int index)
    {
        switch (index)
        {
            case 0:
                topRingClears++;
                Debug.Log(topRingClears);
                if (topRingClears == topRingRequirement)
                {
                    OnRingExtended?.Invoke();
                    topRing.SetTrigger("Extend");
                }
                break;
            case 1:
                middleRingClears++;
                Debug.Log(middleRingClears);
                if (middleRingClears == middleRingRequirement)
                {
                    OnRingExtended?.Invoke();
                    middleRing.SetTrigger("Extend");
                }
                break;
            case 2:
                bottomRingClears++;
                Debug.Log(bottomRingClears);
                if (bottomRingClears == bottomRingRequirement)
                {
                    OnRingExtended?.Invoke();
                    bottomRing.SetTrigger("Extend");
                }
                break;
            default:
                throw new System.IndexOutOfRangeException("Index is larger than the amount of supported rings");
        }
    }
}
