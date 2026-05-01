using UnityEngine;
using TMPro;

public class TrialManager : MonoBehaviour
{
    public GameObject trialStatusPanel;
    public TMP_Text trialStatusText;

    private bool trialEnded = false;

    void Start()
    {
        trialEnded = false;
        Time.timeScale = 1f;

        if (trialStatusPanel != null)
            trialStatusPanel.SetActive(true);

        if (trialStatusText != null)
            trialStatusText.text = "Trial Running";

        if (TelemetryManager.Instance != null)
            TelemetryManager.Instance.StartTrial();

        Debug.Log("TrialManager started");
    }

    void Update()
    {
        if (trialEnded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTrial();
        }
    }

    public void EndTrial()
    {
        if (trialEnded) return;

        trialEnded = true;

        if (TelemetryManager.Instance != null)
            TelemetryManager.Instance.EndTrial();

        if (trialStatusText != null)
            trialStatusText.text = "Trial Ended\nResults Saved";

        Debug.Log("Trial Ended");

        Invoke(nameof(QuitGame), 2f);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
