using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [SerializeField] private GameObject shownUI;
    private static GameObject UIObject;

    private static TextMeshProUGUI textField;

    private static string[] dialogueStrings;
    private static int dialogueIndex = 0;

    private static PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;

        playerInput = GetComponentInParent<PlayerInput>();

        textField = GetComponentInChildren<TextMeshProUGUI>();
        if (UIObject == null)
        {
            UIObject = shownUI;
        }
        dialogueStrings = new string[0];
        UIObject.SetActive(false);
    }

    public static void ShowText(string shownText)
    {
        textField.text = shownText;
        UIObject.SetActive(true);
    }

    public static void ShowText(string[] shownTexts)
    {
        if (shownTexts.Length > 0)
        {
            playerInput.SwitchCurrentActionMap("Dialogue");
            Cursor.lockState = CursorLockMode.None;

            dialogueIndex = 0;
            dialogueStrings = shownTexts;
            textField.text = shownTexts[0];
        } else
        {
            textField.text = "Something went wrong";
        }
        UIObject.SetActive(true);
    }

    public static void HideText()
    {
        textField.text = "Dialogue box should be disabled";
        UIObject.SetActive(false);

        playerInput.SwitchCurrentActionMap("Overworld");
        Cursor.lockState = CursorLockMode.Locked;
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
