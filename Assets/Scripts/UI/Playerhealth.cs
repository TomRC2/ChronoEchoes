using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnPlayerDied;

    private void Awake()
    {
        if (OnHealthChanged == null)
            OnHealthChanged = new UnityEvent<int, int>();
        if (OnPlayerDied == null)
            OnPlayerDied = new UnityEvent();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log($"Player took {damageAmount} damage. Current Health: {currentHealth}");

        OnHealthChanged.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Player healed {healAmount} health. Current Health: {currentHealth}");

        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        OnPlayerDied.Invoke();

        GetComponent<PlayerController>().enabled = false;
        GetComponent<TimeRewindAbility>().enabled = false;
    }
}