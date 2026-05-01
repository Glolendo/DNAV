using System.Collections.Generic;
using UnityEngine;

public class TrialTimerMetrics : MonoBehaviour
{
    public float longPauseThreshold = 3f;

    private float decisionStartTime;
    private bool timingDecision = false;

    private List<float> decisionLatencies = new List<float>();

    public int longPauseCount = 0;

    public void StartDecisionTimer()
    {
        decisionStartTime = Time.realtimeSinceStartup;
        timingDecision = true;
    }

    public void EndDecisionTimer()
    {
        if (!timingDecision) return;

        float latency = Time.realtimeSinceStartup - decisionStartTime;
        decisionLatencies.Add(latency);

        if (latency >= longPauseThreshold)
        {
            longPauseCount++;
        }

        timingDecision = false;

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent(
                "DecisionLatency",
                "Intersection",
                $"Latency={latency:F2}s"
            );
        }
    }

    public float GetAverageDecisionLatency()
    {
        if (decisionLatencies.Count == 0) return 0f;

        float total = 0f;

        foreach (float latency in decisionLatencies)
        {
            total += latency;
        }

        return total / decisionLatencies.Count;
    }

    public float GetMaxDecisionLatency()
    {
        float max = 0f;

        foreach (float latency in decisionLatencies)
        {
            if (latency > max)
            {
                max = latency;
            }
        }

        return max;
    }
}