using UnityEngine;

public class ElevatorLogic : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform targetPoint;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isPlayerDetected = false;

    private void Start()
    {
        initialPosition = transform.position;
        if (targetPoint != null) targetPosition = targetPoint.position;
    }

    private void Update()
    {
        Vector3 destination = isPlayerDetected ? targetPosition : initialPosition;

        if (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }

    public void SetPlayerDetected(bool detected)
    {
        isPlayerDetected = detected;

        if (detected) Debug.Log("Cámara: ¡Objetivo detectado! Bajando...");
        else Debug.Log("Cámara: Objetivo perdido. Reseteando posición...");
    }
}