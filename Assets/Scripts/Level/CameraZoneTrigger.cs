using UnityEngine;
using Unity.Cinemachine;

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera pasilloCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pasilloCamera.Priority = 20;
            Debug.Log("Entrando al corredor: Subiendo cámara.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pasilloCamera.Priority = 5;
            Debug.Log("Saliendo del corredor: Bajando cámara.");
        }
    }
}