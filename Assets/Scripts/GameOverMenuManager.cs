using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenuManager : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private GameObject gameOverPanel;

    private CanvasGroup canvasGroup;
    private GameObject textAndButtons;

    private void Awake()
    {
        canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
        textAndButtons = gameOverPanel.transform.GetChild(0).gameObject;

        Health.OnGameOver += Health_OnGameOver;
    }
    private void OnDisable()
    {
        Health.OnGameOver -= Health_OnGameOver;
    }

    private void Health_OnGameOver()
    {
        StartCoroutine(Fade());
    }
    private IEnumerator Fade()
    {
        float time = 0f;
        textAndButtons.SetActive(false);
        Time.timeScale = 0f;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = 0f;
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        textAndButtons.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
