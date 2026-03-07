using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private EnemyController enemy;
    private Camera mainCam;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyController>();
        mainCam = Camera.main;

        if (enemy != null)
        {
            slider.maxValue = enemy.maxHealth;
            slider.value = enemy.maxHealth;
        }
    }

    private void Update()
    {
        if (enemy == null) return;

        slider.value = enemy.GetCurrentHealth();

        transform.LookAt(transform.position + mainCam.transform.forward);
    }
}