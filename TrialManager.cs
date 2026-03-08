using UnityEngine;

public class TrialManager : MonoBehaviour
{
    void Start()
    {
        TelemetryManager.Instance.StartTrial();
    }

    void Update()
    {
        // Press E to end trial during testing
        if (Input.GetKeyDown(KeyCode.E))
        {
            TelemetryManager.Instance.EndTrial();
        }
    }
}