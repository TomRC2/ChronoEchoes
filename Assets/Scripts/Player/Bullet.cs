using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;
    public float lifeTime = 3f;
    [SerializeField] private GameObject hitEffectPrefab;
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("IgnoreBullet"))
        {
            return;
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("¡Enemigo impactado!");

            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
