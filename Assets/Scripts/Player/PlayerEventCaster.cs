using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventCaster : EventCaster
{
    private UnityEvent<Vector3, float> makeSound;

    protected override void Start()
    {
        // Initiate events and add listeners
        makeSound = new UnityEvent<Vector3, float>();

        base.Start();
    }

    protected override void UpdateListeners()
    {
        makeSound.RemoveAllListeners();
        foreach (Creature c in GameObject.FindObjectsOfType<Creature>())
        {
            makeSound.AddListener(c.HearPlayer);
        }
    }

    protected override async Task InvokeFrequentEvents(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay((int)(frequentInvokeTimer * 1000));

            // TODO: change hardcoded loudness to one dictated by player statemachine
            makeSound.Invoke(transform.position, GetComponent<CC_PlayerController>().Loudness);
        }
    }

    protected override async Task InvokeStandardEvents(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay((int)(standardInvokeTimer * 1000));

        }
    }

    protected override async Task InvokeSlowEvents(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay((int)(slowInvokeTimer * 1000));

        }
    }
}
