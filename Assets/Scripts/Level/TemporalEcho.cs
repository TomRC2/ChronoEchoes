using UnityEngine;

public class TemporalEcho : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private GameObject collectEffectPrefab;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (CheckpointManager.Instance != null)
            {
                CheckpointManager.Instance.SaveCheckpoint(transform.position);
            }

                PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null) health.Heal(health.maxHealth);

            TimeRewindAbility rewind = other.GetComponent<TimeRewindAbility>();
            if (rewind != null) rewind.RestoreFullEnergy();

            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.EchoCollected();
            }
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.glitchSFX);
            Debug.Log("Eco recolectado y Checkpoint guardado.");
            Destroy(gameObject);
             
        }
    }
}