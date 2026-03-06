using UnityEngine;

public class ElevatorLogic : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform targetPoint; // Poné un objeto vacío donde querés que baje

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
        // El ascensor decide hacia dónde ir según si el jugador está en el trigger o no
        Vector3 destination = isPlayerDetected ? targetPosition : initialPosition;

        // Movimiento fluido hacia el destino actual
        if (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }

    // Métodos para que el Trigger los llame
    public void SetPlayerDetected(bool detected)
    {
        isPlayerDetected = detected;

        if (detected) Debug.Log("Cámara: ¡Objetivo detectado! Bajando...");
        else Debug.Log("Cámara: Objetivo perdido. Reseteando posición...");
    }
}