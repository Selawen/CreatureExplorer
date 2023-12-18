using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;
    [SerializeField] private static TextMeshProUGUI textField;

    private static string[] dialogueStrings;
    private static int dialogueIndex = 0;

    private void Awake()
    {
        Instance = this;
        textField = GetComponentInChildren<TextMeshProUGUI>();
        dialogueStrings = new string[0];
        gameObject.SetActive(false);
    }

    public static void ShowText(string shownText)
    {
        textField.text = shownText;
        Instance.gameObject.SetActive(true);
    }

    public static void ShowText(string[] shownTexts)
    {
        if (shownTexts.Length > 0)
        {
            dialogueIndex = 0;
            dialogueStrings = shownTexts;
            textField.text = shownTexts[0];
        } else
        {
            textField.text = "Something went wrong";
        }
        Instance.gameObject.SetActive(true);
    }

    public static void HideText()
    {
        textField.text = "Dialogue box should be disabled";
        Instance.gameObject.SetActive(false);
    }

    public void GetContinueInput(InputAction.CallbackContext context)
    {
        if (dialogueStrings.Length< 1)
        {
            return;
        }

        dialogueIndex += context.started? 1:0;
        if (dialogueIndex < dialogueStrings.Length)
        {
            textField.text = dialogueStrings[dialogueIndex];
        }
        else
        {
            HideText();
        }
    }
}
