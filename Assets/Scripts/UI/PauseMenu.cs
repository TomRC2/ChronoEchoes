using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (victoryPanel.activeSelf || gameOverPanel.activeSelf) return;

            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.glitchSFX);
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.glitchSFX);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);

        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadNextLevel(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.glitchSFX);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.glitchSFX);
    }
}