using UnityEngine;

public class DestinationTrigger : MonoBehaviour
{
    public string destinationID = "Destination";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent(
                "DestinationReached",
                destinationID,
                "Player reached destination"
            );
        }

        TrialFlowManager flowManager = FindFirstObjectByType<TrialFlowManager>();

        if (flowManager != null)
        {
            flowManager.CompleteCurrentTrial();
        }

        Debug.Log("Destination reached: " + destinationID);
    }
}