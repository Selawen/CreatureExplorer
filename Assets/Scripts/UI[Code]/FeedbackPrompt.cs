using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPrompt : MonoBehaviour
{
    [SerializeField] private float feedbackDuration;
    [SerializeField] private AnimationCurve scalingCurve;

    // Start is called before the first frame update
    void Start()
    {
        StaticQuestHandler.OnShrineCompleted += Activate;
        gameObject.SetActive(false);
    }

    private void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(ScaleOverTime());
    }

    private IEnumerator ScaleOverTime()
    {
        float timer = 0;

        while (timer < feedbackDuration)
        {
            transform.localScale = Vector3.one * scalingCurve.Evaluate(timer/feedbackDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
