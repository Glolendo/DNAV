using System;
using UnityEngine;

public enum RecallPhase
{
    ImmediateRecall,
    DelayedRecall
}

public enum RouteDirection
{
    ToDestination,
    BackToLobby
}

public enum GazeCategory
{
    Wall,
    RoomNumber,
    Ceiling,
    Door,
    Hand,
    HandheldObject,
    Person,
    Other
}

[Serializable]
public class TrialConfig
{
    public string trialID;
    public string destinationName;
    public RecallPhase phase;
    public RouteDirection direction;
    public int optimalSteps;
    public int maxSteps;
}

[Serializable]
public class TrialMetricResult
{
    public string trialID;
    public string destinationName;
    public RecallPhase phase;
    public RouteDirection direction;

    public int actualSteps;
    public int optimalSteps;
    public int maxSteps;
    public int excessSteps;

    public float efficiencyRatio;
    public float percentInefficiency;
    public float trialTime;

    public int backtracks;
    public int repeatedNodeRevisits;
    public int wrongTurns;
    public int incorrectDoorAttempts;
}