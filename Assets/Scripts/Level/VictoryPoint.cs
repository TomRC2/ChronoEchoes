using UnityEngine;

public class VictoryPoint : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private string nextLevelName = "Level2";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameUIManager != null)
            {
                gameUIManager.SetNextLevelName(nextLevelName);
                gameUIManager.ShowVictoryPanel();
            }
            else
            {
                Debug.LogWarning("VictoryPoint: GameUIManager no asignado.");
            }
        }
    }
}