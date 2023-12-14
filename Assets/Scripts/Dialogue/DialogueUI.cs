using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;

    public void ShowText(string shownText)
    {
        textField.text = shownText;
        gameObject.SetActive(true);
    }

    public void HideText()
    {
        textField.text = "";
        gameObject.SetActive(false);
    }
}
