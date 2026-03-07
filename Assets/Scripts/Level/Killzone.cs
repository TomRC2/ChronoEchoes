using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(999);
                Debug.Log("Jugador caído al vacío. Desincronizando...");
            }
        }

        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}