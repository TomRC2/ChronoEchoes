using UnityEngine;

public class VictoryPoint : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameUIManager != null)
            {
                gameUIManager.ShowVictoryPanel();
            }
            else
            {
                Debug.LogWarning("VictoryPoint: GameUIManager no asignado.");
            }
        }
    }
}