using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;
    public float lifeTime = 3f;

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
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy!");
            Destroy(gameObject);
        }

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
