using UnityEngine;

public class TrialMetricsManager : MonoBehaviour
{
    public NodeTracker nodeTracker;

    public RecallPhase currentPhase;
    public RouteDirection currentDirection;
    public string currentTrialID;
    public string currentDestinationName;

    private float trialStartTime;
    private bool trialActive = false;

    public void StartTrial(TrialConfig config)
    {
        currentPhase = config.phase;
        currentDirection = config.direction;
        currentTrialID = config.trialID;
        currentDestinationName = config.destinationName;

        trialStartTime = Time.realtimeSinceStartup;
        trialActive = true;

        if (nodeTracker != null)
        {
            nodeTracker.StartNodeTracking(config.optimalSteps, config.maxSteps);
        }

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent(
                "TrialStarted",
                config.trialID,
                $"Phase={config.phase}, Direction={config.direction}, Destination={config.destinationName}"
            );
        }

        Debug.Log($"Started trial: {config.trialID}");
    }

    public TrialMetricResult EndTrial()
    {
        if (!trialActive)
        {
            Debug.LogWarning("EndTrial called, but no trial is active.");
            return null;
        }

        trialActive = false;

        float totalTime = Time.realtimeSinceStartup - trialStartTime;

        if (nodeTracker != null)
        {
            nodeTracker.StopNodeTracking();
        }

        TrialMetricResult result = new TrialMetricResult();

        result.trialID = currentTrialID;
        result.destinationName = currentDestinationName;
        result.phase = currentPhase;
        result.direction = currentDirection;

        if (nodeTracker != null)
        {
            result.actualSteps = nodeTracker.actualSteps;
            result.optimalSteps = nodeTracker.optimalSteps;
            result.maxSteps = nodeTracker.maxSteps;
            result.excessSteps = nodeTracker.GetExcessSteps();
            result.efficiencyRatio = nodeTracker.GetEfficiencyRatio();
            result.percentInefficiency = nodeTracker.GetPercentInefficiency();
            result.backtracks = nodeTracker.backtracks;
            result.repeatedNodeRevisits = nodeTracker.repeatedNodeRevisits;
        }

        result.trialTime = totalTime;

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent(
                "TrialEnded",
                currentTrialID,
                $"Steps={result.actualSteps}, Time={result.trialTime:F2}, Efficiency={result.efficiencyRatio:F2}"
            );
        }

        Debug.Log($"Ended trial: {currentTrialID}");

        return result;
    }
}