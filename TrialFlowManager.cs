using System.Collections.Generic;
using UnityEngine;

public class TrialFlowManager : MonoBehaviour
{
    public TrialMetricsManager trialMetricsManager;

    private List<TrialConfig> immediateRecallTrials = new List<TrialConfig>();
    private List<TrialConfig> delayedRecallTrials = new List<TrialConfig>();

    private List<TrialMetricResult> allResults = new List<TrialMetricResult>();

    private int currentTrialIndex = 0;
    private bool inImmediateRecall = true;

    void Awake()
    {
        BuildTrialConfigs();
    }

    void BuildTrialConfigs()
    {
        immediateRecallTrials = new List<TrialConfig>
        {
            new TrialConfig { trialID = "IR_T1", destinationName = "Radiology Check-In", phase = RecallPhase.ImmediateRecall, direction = RouteDirection.BackToLobby, optimalSteps = 8,  maxSteps = 14 },
            new TrialConfig { trialID = "IR_T2", destinationName = "Pathology Lab",       phase = RecallPhase.ImmediateRecall, direction = RouteDirection.BackToLobby, optimalSteps = 10, maxSteps = 18 },
            new TrialConfig { trialID = "IR_T3", destinationName = "X-ray Room",          phase = RecallPhase.ImmediateRecall, direction = RouteDirection.BackToLobby, optimalSteps = 18, maxSteps = 25 },
            new TrialConfig { trialID = "IR_T4", destinationName = "Financial Office",    phase = RecallPhase.ImmediateRecall, direction = RouteDirection.BackToLobby, optimalSteps = 22, maxSteps = 31 },
            new TrialConfig { trialID = "IR_T5", destinationName = "Pharmacy",            phase = RecallPhase.ImmediateRecall, direction = RouteDirection.BackToLobby, optimalSteps = 31, maxSteps = 50 }
        };

        delayedRecallTrials = new List<TrialConfig>
        {
            new TrialConfig { trialID = "DR_T1", destinationName = "Radiology Check-In", phase = RecallPhase.DelayedRecall, direction = RouteDirection.ToDestination, optimalSteps = 8,  maxSteps = 18 },
            new TrialConfig { trialID = "DR_T2", destinationName = "Pathology Lab",       phase = RecallPhase.DelayedRecall, direction = RouteDirection.ToDestination, optimalSteps = 10, maxSteps = 24 },
            new TrialConfig { trialID = "DR_T3", destinationName = "X-ray Room",          phase = RecallPhase.DelayedRecall, direction = RouteDirection.ToDestination, optimalSteps = 18, maxSteps = 38 },
            new TrialConfig { trialID = "DR_T4", destinationName = "Financial Office",    phase = RecallPhase.DelayedRecall, direction = RouteDirection.ToDestination, optimalSteps = 22, maxSteps = 43 },
            new TrialConfig { trialID = "DR_T5", destinationName = "Pharmacy",            phase = RecallPhase.DelayedRecall, direction = RouteDirection.ToDestination, optimalSteps = 31, maxSteps = 50 }
        };
    }

    public void StartTrialFlow()
    {
        inImmediateRecall = true;
        currentTrialIndex = 0;
        allResults.Clear();

        StartCurrentTrial();
    }

    public void StartCurrentTrial()
    {
        List<TrialConfig> activeList = inImmediateRecall ? immediateRecallTrials : delayedRecallTrials;

        if (currentTrialIndex >= activeList.Count)
        {
            if (inImmediateRecall)
            {
                inImmediateRecall = false;
                currentTrialIndex = 0;
                StartCurrentTrial();
                return;
            }

            EndAllTrials();
            return;
        }

        TrialConfig config = activeList[currentTrialIndex];

        if (trialMetricsManager != null)
        {
            trialMetricsManager.StartTrial(config);
        }
    }

    public void CompleteCurrentTrial()
    {
        if (trialMetricsManager != null)
        {
            TrialMetricResult result = trialMetricsManager.EndTrial();

            if (result != null)
            {
                allResults.Add(result);
            }
        }

        currentTrialIndex++;
        StartCurrentTrial();
    }

    public void EndAllTrials()
    {
        Debug.Log("All DNAVI trials complete.");

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent(
                "AllTrialsComplete",
                "Session",
                $"Total Trials={allResults.Count}"
            );
        }
    }

    public List<TrialMetricResult> GetAllResults()
    {
        return allResults;
    }
}