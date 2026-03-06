using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Objective Settings")]
    [SerializeField] private int echoesToCollect = 3;
    private int currentEchoes = 0;

    [Header("References")]
    [SerializeField] private GameUIManager uiManager;
    [SerializeField] private GameObject victoryPortal;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (victoryPortal != null) victoryPortal.SetActive(false);

        if (uiManager != null) uiManager.UpdateEchoCounter(0, echoesToCollect);
    }

    public void EchoCollected()
    {
        currentEchoes++;

        if (uiManager != null)
        {
            uiManager.UpdateEchoCounter(currentEchoes, echoesToCollect);
        }

        if (currentEchoes >= echoesToCollect)
        {
            OpenTimeRift();
        }
    }

    private void OpenTimeRift()
    {
        if (victoryPortal != null)
        {
            victoryPortal.SetActive(true);
            Debug.Log("¡La grieta temporal se ha abierto!");
        }

        if (uiManager != null)
        {
            uiManager.ShowPortalMessage();
        }
    }
}