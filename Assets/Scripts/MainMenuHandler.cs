using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject quitPromptPanel;
    [SerializeField] private TMP_Text quitPromptTitle;
    [SerializeField] private TMP_Text quitPromptMessage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        quitPromptPanel.SetActive(false);
    }

    public void OnQuitGamePrompted()
    {
        if (Application.isEditor)
        {
            CreateEditorPrompt();
            return;
        }
        CreateRuntimePrompt();
    }

    private void CreateEditorPrompt()
    {
        quitPromptTitle.text = "HEY! DON'T DO THAT!";
        quitPromptMessage.text = "THIS IS THE EDITOR, YOU CAN'T QUIT LIKE THAT!\nYOU SHOULD KNOW THAT! YOU'RE WORKING ON THIS GAME!";

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => quitPromptPanel.SetActive(false));
        cancelButton.GetComponentInChildren<TMP_Text>().text = "I'm sorry...";

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => quitPromptPanel.SetActive(false));
        confirmButton.GetComponentInChildren<TMP_Text>().text = "I understand";

        quitPromptPanel.SetActive(true);
    }

    private void CreateRuntimePrompt()
    {
        quitPromptTitle.text = "ARE YOU SURE?";
        quitPromptMessage.text = "ARE YOU CERTAIN YOU WANT TO QUIT?\nALL UNSAVED PROGRESS WILL BE LOST!";

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => quitPromptPanel.SetActive(false));
        cancelButton.GetComponentInChildren<TMP_Text>().text = "Cancel";

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => Application.Quit());
        confirmButton.GetComponentInChildren<TMP_Text>().text = "Quit";

        quitPromptPanel.SetActive(true);
    }

}
