using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolPointIndex = 0;

    [Header("Health")]
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [Header("Attack")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    private float nextFireTime;
    private Transform playerTransform;

    private void Start()
    {
        currentHealth = maxHealth;
        if (patrolPoints.Length < 2)
        {
            Debug.LogWarning("Enemy_Basic: Necesita al menos 2 puntos de patrulla para moverse. Asegúrate de asignarlos en el Inspector.");
            enabled = false;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("EnemyController: No se encontró un GameObject con el Tag 'Player'. El enemigo no podrá atacar.");
        }
    }

    private void Update()
    {
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
            FaceTarget(playerTransform.position);
            TryAttack();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector3 targetPosition = patrolPoints[currentPatrolPointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
        }
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void TryAttack()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (enemyBulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Enemy Bullet Prefab o Fire Point no asignados en EnemyController.");
            return;
        }
        Vector3 shootDirection = (playerTransform.position - firePoint.position).normalized;
        shootDirection.y = 0;

        GameObject bulletInstance = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
        EnemyBullet bulletScript = bulletInstance.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(shootDirection);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Enemy {name} took {damageAmount} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"Enemy {name} has died.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10);
            Destroy(other.gameObject);
        }
    }
}