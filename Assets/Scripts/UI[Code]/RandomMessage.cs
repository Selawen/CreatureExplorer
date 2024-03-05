using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomMessage : MonoBehaviour
{
    [SerializeField] private string[] messages;
    [SerializeField] private TextMeshProUGUI textBox;

    [SerializeField] private bool randomiseOnEnable = true;

    private void OnEnable()
    {
        if (messages.Length <1)
        {
# if UNITY_EDITOR
            Debug.LogError("No messages to choose from");
#endif
            this.enabled = false;
            return;
        }

        if (randomiseOnEnable)
        {
            SetRandomMessage();
        }
    }

    public void SetRandomMessage()
    {
        textBox.text = messages[Random.Range(0, messages.Length)];
    }

    public IEnumerator FadeIn(float duration)
    {
        Color fadeColor = Color.white;
        Color originalTextColor = textBox.color;
        originalTextColor.a = 1;
        float timer = duration;

        Image[] images = GetComponentsInChildren<Image>();

        // Fade in vision obscurer, move player, then fade it out again
        while (timer > 0)
        {
            fadeColor.a = Mathf.InverseLerp(duration, 0, timer);
            foreach (Image i in images)
            {
                i.color = fadeColor;
            }

            textBox.color = originalTextColor*fadeColor;
            timer -= Time.deltaTime;
            yield return null;
        }

        fadeColor = Color.white; 
        
        foreach (Image i in images)
        {
            i.color = fadeColor;
        }

        textBox.color = originalTextColor * fadeColor;
    }

    public IEnumerator FadeOut(float duration, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Color fadeColor = Color.white;
        Color originalTextColor = textBox.color;
        originalTextColor.a = 1;
        float timer = duration;

        Image[] images = GetComponentsInChildren<Image>();

        // Fade in vision obscurer, move player, then fade it out again
        while (timer > 0)
        {
            fadeColor.a = Mathf.InverseLerp(0, duration, timer);
            foreach (Image i in images)
            {
                i.color = fadeColor;
            }

            textBox.color = originalTextColor * fadeColor;
            timer -= Time.deltaTime;
            yield return null;
        }

        fadeColor = new Color(1,1,1,0);

        foreach (Image i in images)
        {
            i.color = fadeColor;
        }

        textBox.color = originalTextColor * fadeColor;
    }
}
