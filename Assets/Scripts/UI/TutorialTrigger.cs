using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Mensaje")]
    [TextArea][SerializeField] private string message = "Presioná Q para rebobinar";
    [SerializeField] private float duration = 5f;
    [SerializeField] private bool onlyOnce = true;

    private bool _alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && !_alreadyTriggered)
        {
            UIManager ui = FindFirstObjectByType<UIManager>();

            if (ui != null)
            {
                ui.ShowTutorial(message, duration);

                if (onlyOnce) _alreadyTriggered = true;
            }
        }
    }
}