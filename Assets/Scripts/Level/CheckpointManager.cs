using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SaveCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log("Punto de anclaje temporal guardado en: " + position);
    }

    public bool HasCheckpoint() => hasCheckpoint;

    public void RespawnPlayer(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = lastCheckpointPosition;

        Physics.SyncTransforms();

        player.GetComponent<PlayerHealth>().Heal(100);
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<TimeRewindAbility>().enabled = true;

        if (cc != null) cc.enabled = true;

        player.GetComponent<TimeRewindAbility>().RestoreFullEnergy();
        player.GetComponent<TimeRewindAbility>().ClearTimePoints();

        Time.timeScale = 1f;
        Debug.Log("Respawn completado en: " + lastCheckpointPosition);
    }
}