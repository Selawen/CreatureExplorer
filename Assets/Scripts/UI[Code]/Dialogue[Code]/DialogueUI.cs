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
        SwitchInputs();
        textField.text = shownText;
        UIObject.SetActive(true);
    }

    public static void ShowText(string[] shownTexts)
    {
        if (shownTexts.Length > 0)
        {
            SwitchInputs();
            dialogueIndex = 0;
            dialogueStrings = shownTexts;
            textField.text = shownTexts[0];
        } else
        {
            Debug.Log("Something went wrong, Dialogue was triggered but no text was given");
            return;
        }
        UIObject.SetActive(true);
    }

    private static void SwitchInputs()
    {
        // TODO: remove gameobject.find
        GameObject.FindObjectOfType<PlayerController>().LinkModuleToDialogue();
        playerInput.SwitchCurrentActionMap("Dialogue");
        Cursor.lockState = CursorLockMode.None;
    }

    public static void HideText()
    {
        textField.text = "Dialogue box should be disabled";
        UIObject.SetActive(false);

        // TODO: remove gameobject.find
        GameObject.FindObjectOfType<PlayerController>().LinkModuleToOverworld();
        playerInput.SwitchCurrentActionMap("Overworld");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GetContinueInput(InputAction.CallbackContext context)
    {
        if (dialogueStrings.Length< 1)
        {
            HideText();
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
