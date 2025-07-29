using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Game Over Buttons")]
    [SerializeField] private Button restartButton_GO;
    [SerializeField] private Button mainMenuButton_GO;

    [Header("Victory Buttons")]
    [SerializeField] private Button restartButton_V;
    [SerializeField] private Button mainMenuButton_V;

    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TimeRewindAbility timeRewindAbility;

    [Header("HUD Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider timeEnergySlider;


    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        if (restartButton_GO != null) restartButton_GO.onClick.AddListener(RestartGame);
        if (mainMenuButton_GO != null) mainMenuButton_GO.onClick.AddListener(LoadMainMenu);

        if (restartButton_V != null) restartButton_V.onClick.AddListener(RestartGame);
        if (mainMenuButton_V != null) mainMenuButton_V.onClick.AddListener(LoadMainMenu);

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied.AddListener(ShowGameOverPanel);
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }
        else
        {
            Debug.LogError("GameUIManager: PlayerHealth reference not set!");
        }
        if (timeRewindAbility != null)
        {
            timeRewindAbility.OnRewindEnergyChanged.AddListener(UpdateTimeEnergyUI);
            UpdateTimeEnergyUI(timeRewindAbility.GetCurrentRewindEnergy(), timeRewindAbility.GetMaxRewindEnergy());
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    public void UpdateTimeEnergyUI(float currentRewindTime, float maxRewindTime)
    {
        if (timeEnergySlider != null)
        {
            timeEnergySlider.value = currentRewindTime / maxRewindTime;
        }
    }
}