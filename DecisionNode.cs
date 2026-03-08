using UnityEngine;

public class DecisionNode : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            TelemetryManager.Instance.LogEvent(
                "DecisionNodeEntered",
                gameObject.name
            );
        }
    }
}