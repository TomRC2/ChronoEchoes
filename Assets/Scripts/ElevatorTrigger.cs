using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private ElevatorLogic elevator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevator.SetPlayerDetected(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevator.SetPlayerDetected(false);
        }
    }
}