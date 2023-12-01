using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLeaves : MonoBehaviour
{
    [SerializeField] private SceneHandler sceneHandler;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [SerializeField] private Slider progressBar;

    private System.Action onAnimationEnded;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        progressBar.gameObject.SetActive(false);
    }

    public void OpenOptions()
    {
        onAnimationEnded = null;
        mainMenuPanel.SetActive(false);
        animator.Play("LeavesScrollFillScreen");
        onAnimationEnded += () => settingsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        onAnimationEnded = null;
        settingsPanel.SetActive(false);
        animator.Play("ResetLeavesAnimation");
        onAnimationEnded += () => mainMenuPanel.SetActive(true);
    }

    public void StartGame()
    {
        onAnimationEnded = null;
        mainMenuPanel.SetActive(false);
        animator.Play("LeavesScrollFillScreen");

        sceneHandler.onLoadCompleted += (AsyncOperation op) => 
        {
            onAnimationEnded = null;
            onAnimationEnded += () => sceneHandler.UnloadSceneAsync(0);
            animator.Play("LeavesScrollStartGame");
        };

        onAnimationEnded += () =>
        {
            sceneHandler.LoadSceneAsync(1, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            progressBar.gameObject.SetActive(true);
            sceneHandler.onProgressChanged += (float progress) => progressBar.value = progress;
        };
    }

    public void OnAnimationEnd()
    {
        onAnimationEnded?.Invoke();
    }
}
