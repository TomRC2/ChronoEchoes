using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Asegurate de tener TextMeshPro instalado

public class GameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject portalMessagePanel;

    [Header("HUD Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider timeEnergySlider;
    [SerializeField] private TextMeshProUGUI echoCounterText;

    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TimeRewindAbility timeRewindAbility;

    [Header("Buttons")]
    [SerializeField] private Button restartButton_GO;
    [SerializeField] private Button mainMenuButton_GO;
    [SerializeField] private Button restartButton_V;
    [SerializeField] private Button mainMenuButton_V;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (portalMessagePanel != null) portalMessagePanel.SetActive(false);

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

        if (timeRewindAbility != null)
        {
            timeRewindAbility.OnRewindEnergyChanged.AddListener(UpdateTimeEnergyUI);
            UpdateTimeEnergyUI(timeRewindAbility.GetCurrentRewindEnergy(), timeRewindAbility.GetMaxRewindEnergy());
        }
    }

    public void UpdateEchoCounter(int current, int target)
    {
        if (echoCounterText != null)
        {
            echoCounterText.text = $"Temporal Echoes: {current}/{target}";
        }
    }

    public void ShowPortalMessage()
    {
        if (portalMessagePanel != null)
        {
            portalMessagePanel.SetActive(true);
            Invoke("HidePortalMessage", 3f);
        }
    }

    private void HidePortalMessage()
    {
        if (portalMessagePanel != null) portalMessagePanel.SetActive(false);
    }

    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }
    public void ShowGameOverPanel() { if (gameOverPanel != null) { gameOverPanel.SetActive(true); Time.timeScale = 0f; } }
    public void ShowVictoryPanel() { if (victoryPanel != null) { victoryPanel.SetActive(true); Time.timeScale = 0f; } }
    public void UpdateHealthUI(int current, int max) { if (healthSlider != null) healthSlider.value = (float)current / max; }
    public void UpdateTimeEnergyUI(float current, float max) { if (timeEnergySlider != null) timeEnergySlider.value = current / max; }
}