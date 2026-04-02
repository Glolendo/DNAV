using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [Header("Player Setup")]
    [Tooltip("Drag the XR Origin / player object here.")]
    public Transform playerRoot;

    [Tooltip("Assign all destination trigger objects here.")]
    public List<DestinationTrigger> destinations = new List<DestinationTrigger>();

    [Header("Trial Flow")]
    [Tooltip("Sequence of destinations the player must visit in order.")]
    public List<string> taskSequence = new List<string>();

    private int currentTaskIndex = 0;

    [Header("Turn Tracking")]
    public float turnAngleThreshold = 30f;
    public float turnCooldownSeconds = 0.35f;

    [Header("Distance / Steps")]
    public float metersPerStep = 0.75f;

    // Runtime
    private bool isRunning;
    private float startTime;
    private float elapsedTime;

    private Vector3 lastPos;
    private float distanceMeters;

    private Vector3 lastForwardFlat;
    private int turnCount;
    private float lastTurnTime;

    void Start()
    {
        BeginRun();
    }

    void Update()
    {
        if (!isRunning || playerRoot == null) return;

        // Time
        elapsedTime = Time.time - startTime;

        // Distance
        Vector3 currentPos = playerRoot.position;
        distanceMeters += Vector3.Distance(currentPos, lastPos);
        lastPos = currentPos;

        // Turn tracking
        Vector3 fwd = playerRoot.forward;
        fwd.y = 0f;

        if (fwd.sqrMagnitude > 0.0001f)
        {
            fwd.Normalize();
            float angle = Vector3.Angle(lastForwardFlat, fwd);

            if (angle >= turnAngleThreshold && (Time.time - lastTurnTime) >= turnCooldownSeconds)
            {
                turnCount++;
                lastTurnTime = Time.time;
                lastForwardFlat = fwd;
            }
        }
    }

    public void BeginRun()
    {
        if (playerRoot == null)
        {
            Debug.LogError("NavigationManager: playerRoot not assigned.");
            return;
        }

        if (taskSequence == null || taskSequence.Count == 0)
        {
            Debug.LogError("NavigationManager: No tasks assigned in taskSequence.");
            return;
        }

        currentTaskIndex = 0;

        // Reset metrics
        isRunning = true;
        startTime = Time.time;
        elapsedTime = 0f;

        lastPos = playerRoot.position;
        distanceMeters = 0f;

        Vector3 fwd = playerRoot.forward;
        fwd.y = 0f;
        lastForwardFlat = (fwd.sqrMagnitude > 0.0001f) ? fwd.normalized : Vector3.forward;

        turnCount = 0;
        lastTurnTime = -999f;

        Debug.Log($"🚀 Run started. First task: {taskSequence[currentTaskIndex]}");
    }

    public void EndRun()
    {
        if (!isRunning) return;
        isRunning = false;

        float steps = (metersPerStep > 0f) ? (distanceMeters / metersPerStep) : 0f;

        Debug.Log("=== 🎉 TRIAL COMPLETE ===");
        Debug.Log($"Total Time (s): {elapsedTime:F2}");
        Debug.Log($"Distance (m): {distanceMeters:F2}");
        Debug.Log($"Estimated steps: {steps:F0}");
        Debug.Log($"Turns: {turnCount}");
    }

    // Called by DestinationTrigger
    public void NotifyReachedDestination(DestinationTrigger reached)
    {
        if (!isRunning || reached == null) return;

        string expected = taskSequence[currentTaskIndex];

        Debug.Log($"📍 Player reached: {reached.destinationId}");
        Debug.Log($"🎯 Current task: {expected}");

        if (reached.destinationId == expected)
        {
            Debug.Log($"✅ Correct destination!");

            currentTaskIndex++;

            if (currentTaskIndex >= taskSequence.Count)
            {
                EndRun();
            }
            else
            {
                Debug.Log($"➡️ Next Task: {taskSequence[currentTaskIndex]}");
            }
        }
        else
        {
            Debug.Log($"❌ Wrong destination! Expected: {expected}");
        }
    }
}
