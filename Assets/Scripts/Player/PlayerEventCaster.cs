using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventCaster : MonoBehaviour
{
    [SerializeField] private float frequentInvokeTimer = 0.2f;
    [SerializeField] private float standardInvokeTimer = 0.5f;
    [SerializeField] private float slowInvokeTimer = 1.0f;
 
    private CancellationTokenSource source;
    private CancellationToken token;

    private UnityEvent<Vector3, float> makeSound;

    private void Start()
    {
        // Initiate events and add listeners
        makeSound = new UnityEvent<Vector3, float>();

        foreach (Creature c in GameObject.FindObjectsOfType<Creature>())
        {
            makeSound.AddListener(c.HearPlayer);
        }

        StartInvokingEvents();
    }

    private void OnDisable()
    {
        source.Cancel();
        source.Dispose();
    }
    
    private void StartInvokingEvents()
    { 
        // Create cancellation tokens and start tasks for invoking
        source = new CancellationTokenSource();
        token = source.Token;

        invokeFrequentEvents(token);

        // TODO: decomment if standard or slow events are added
        //invokeStandardEvents(token);
        //invokeSlowEvents(token);
    }

    private async Task invokeFrequentEvents(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay((int)(frequentInvokeTimer * 1000));

            // TODO: change hardcoded loudness to one dictated by player statemachine
            makeSound.Invoke(transform.position, 5);
        }
    }
    private async Task invokeStandardEvents(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay((int)(standardInvokeTimer * 1000));

        }
    }
    private async Task invokeSlowEvents(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay((int)(slowInvokeTimer * 1000));

        }
    }
}
