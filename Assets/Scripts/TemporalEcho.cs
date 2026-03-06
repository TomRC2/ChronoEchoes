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
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null) health.Heal(health.maxHealth); //

            TimeRewindAbility rewind = other.GetComponent<TimeRewindAbility>();
            if (rewind != null) rewind.RestoreFullEnergy(); //

            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.EchoCollected();
            }

            Debug.Log("Eco recolectado con efectos visuales.");
            Destroy(gameObject);
        }
    }
}