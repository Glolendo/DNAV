using System.Collections.Generic;
using UnityEngine;

public class NodeTracker : MonoBehaviour
{
    public int actualSteps;
    public int optimalSteps;
    public int maxSteps;

    public int backtracks;
    public int repeatedNodeRevisits;

    private bool isCounting = false;
    private string lastNodeID = "";
    private HashSet<string> visitedNodes = new HashSet<string>();

    public void StartNodeTracking(int optimal, int max)
    {
        actualSteps = 0;
        backtracks = 0;
        repeatedNodeRevisits = 0;

        optimalSteps = optimal;
        maxSteps = max;

        visitedNodes.Clear();
        lastNodeID = "";
        isCounting = true;

        Debug.Log($"Node tracking started. Optimal={optimalSteps}, Max={maxSteps}");
    }

    public void StopNodeTracking()
    {
        isCounting = false;
        Debug.Log("Node tracking stopped.");
    }

    public void RegisterNode(string nodeID)
    {
        if (!isCounting) return;

        if (actualSteps >= maxSteps)
        {
            actualSteps = maxSteps;
            StopNodeTracking();
            return;
        }

        actualSteps++;

        if (visitedNodes.Contains(nodeID))
        {
            repeatedNodeRevisits++;
        }

        visitedNodes.Add(nodeID);

        if (!string.IsNullOrEmpty(lastNodeID) && nodeID == lastNodeID)
        {
            backtracks++;
        }

        lastNodeID = nodeID;

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent(
                "NodeEntered",
                nodeID,
                $"Step={actualSteps}, Optimal={optimalSteps}, Max={maxSteps}"
            );
        }

        if (actualSteps >= maxSteps)
        {
            actualSteps = maxSteps;
            StopNodeTracking();

            if (TelemetryManager.Instance != null)
            {
                TelemetryManager.Instance.LogEvent(
                    "MaxStepsReached",
                    nodeID,
                    $"Trial capped at max steps: {maxSteps}"
                );
            }
        }
    }

    public int GetExcessSteps()
    {
        return Mathf.Max(0, actualSteps - optimalSteps);
    }

    public float GetEfficiencyRatio()
    {
        if (actualSteps <= 0) return 0f;
        return (float)optimalSteps / actualSteps;
    }

    public float GetPercentInefficiency()
    {
        if (optimalSteps <= 0) return 0f;
        return (float)(actualSteps - optimalSteps) / optimalSteps;
    }
}