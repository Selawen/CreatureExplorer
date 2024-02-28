using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventCaster : MonoBehaviour
{
    protected List<UnityEvent> frequentEvents;
    protected List<UnityEvent> standardEvents;
    protected List<UnityEvent> slowEvents;

    [SerializeField] protected float frequentInvokeTimer = 0.2f;
    [SerializeField] protected float standardInvokeTimer = 0.5f;
    [SerializeField] protected float slowInvokeTimer = 1.0f;

    protected CancellationTokenSource source;
    protected CancellationToken token;

    protected virtual void Start()
    {
        UpdateListeners();
        // Create cancellation tokens and start tasks for invoking
        source = new CancellationTokenSource();
        token = source.Token;
        StartInvokingEvents();
        //Task.Run(()=>SyncListeners(token));
    }

    private void OnDisable()
    {
        source.Cancel();
        source.Dispose();
    }

    protected abstract void UpdateListeners();

    private void StartInvokingEvents()
    {

        StartCoroutine(InvokeFrequentEvents(token));

        // TODO: decomment if standard or slow events are added
        //invokeStandardEvents(token);
        //invokeSlowEvents(token);
    }
    
    protected virtual async Task SyncListeners(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            // update listeners every second
            UpdateListeners();
            await Task.Delay(1000);
        }
    }

    protected abstract IEnumerator InvokeFrequentEvents(CancellationToken cancelToken);

    protected abstract Task InvokeStandardEvents(CancellationToken cancelToken);

    protected abstract Task InvokeSlowEvents(CancellationToken cancelToken);
}
