using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowText(string shownText)
    {
        textField.text = shownText;
        gameObject.SetActive(true);
    }

    public void HideText()
    {
        textField.text = "Dialogue box should be disabled";
        gameObject.SetActive(false);
    }
}
