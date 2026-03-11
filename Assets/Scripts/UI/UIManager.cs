using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject portalMessagePanel;

    [Header("HUD Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider timeEnergySlider;
    [SerializeField] private TextMeshProUGUI echoCounterText;

    [Header("Tutorial Settings")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TimeRewindAbility timeRewindAbility;

    [Header("Victory Level Settings")]
    [SerializeField] private Button nextLevelButton_V;
    private string _nextLevelName;

    [Header("Buttons")]
    [SerializeField] private Button respawnButton_GO;
    [SerializeField] private Button mainMenuButton_GO;
    [SerializeField] private Button restartButton_V;
    [SerializeField] private Button mainMenuButton_V;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (portalMessagePanel != null) portalMessagePanel.SetActive(false);
        if (nextLevelButton_V != null)
        {
            nextLevelButton_V.onClick.AddListener(LoadNextLevel);
            nextLevelButton_V.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }
        if (respawnButton_GO != null)
        {
            respawnButton_GO.onClick.AddListener(HandleRetry);
            respawnButton_GO.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }

        if (mainMenuButton_GO != null)
        {
            mainMenuButton_GO.onClick.AddListener(LoadMainMenu);
            mainMenuButton_GO.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }
        if (restartButton_V != null)
        {
            restartButton_V.onClick.AddListener(RestartGame);
            restartButton_V.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }
        if (mainMenuButton_V != null) 
        {
            mainMenuButton_V.onClick.AddListener(LoadMainMenu);
            mainMenuButton_V.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied.AddListener(ShowGameOverPanel);
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
        }

        if (timeRewindAbility != null)
        {
            timeRewindAbility.OnRewindEnergyChanged.AddListener(UpdateTimeEnergyUI);
        }
        if (respawnButton_GO != null)
        {
            respawnButton_GO.onClick.AddListener(HandleRetry);
            respawnButton_GO.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }

        if (mainMenuButton_GO != null)
        {
            mainMenuButton_GO.onClick.AddListener(LoadMainMenu);
            mainMenuButton_GO.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }
    }
    public void ShowPortalMessage()
    {
        if (portalMessagePanel != null)
        {
            portalMessagePanel.SetActive(true);
            CancelInvoke(nameof(HidePortalMessage));
            Invoke(nameof(HidePortalMessage), 3f);
        }
    }
    public void ShowTutorial(string message, float duration)
    {
        if (tutorialPanel != null && tutorialText != null)
        {
            tutorialText.text = message;

            tutorialPanel.SetActive(true);

            CancelInvoke(nameof(HideTutorial));
            Invoke(nameof(HideTutorial), duration);
        }
    }
    private void HideTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }
    public void SetNextLevelName(string name)
    {
        _nextLevelName = name;
    }
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;

        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadNextLevel(_nextLevelName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_nextLevelName);
        }
    }
    private void HidePortalMessage()
    {
        if (portalMessagePanel != null) portalMessagePanel.SetActive(false);
    }
    public void HandleRetry()
    {
        if (CheckpointManager.Instance != null && CheckpointManager.Instance.HasCheckpoint())
        {
            StartCoroutine(RespawnSequence());
        }
        else
        {
            RestartGame();
        }
    }
    private IEnumerator RespawnSequence()
    {
        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.PlayStartTransition();
        }

        yield return new WaitForSecondsRealtime(1f);

        CheckpointManager.Instance.RespawnPlayer(playerHealth.gameObject);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;

        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.PlayEndTransition();
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadNextLevel(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void LoadNextLevel(string sceneName)
    {
        Time.timeScale = 1f;
        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadNextLevel(sceneName);
        }
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadNextLevel("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void UpdateHealthUI(int current, int max)
    {
        if (healthSlider != null) healthSlider.value = (float)current / max;
    }

    public void UpdateTimeEnergyUI(float current, float max)
    {
        if (timeEnergySlider != null) timeEnergySlider.value = current / max;
    }

    public void UpdateEchoCounter(int current, int target)
    {
        if (echoCounterText != null) echoCounterText.text = $"Temporal Echoes: {current}/{target}";
    }
}