using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;

    private float trialStartTime;
    private List<EventData> events = new List<EventData>();

    [Serializable]
    public class EventData
    {
        public float timeSinceStart;
        public string eventType;
        public string objectID;
        public string details;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartTrial()
    {
        trialStartTime = Time.realtimeSinceStartup;
        events.Clear();
        Debug.Log("Trial Started");
    }

    public void LogEvent(string eventType, string objectID, string details = "")
    {
        EventData newEvent = new EventData();
        newEvent.timeSinceStart = Time.realtimeSinceStartup - trialStartTime;
        newEvent.eventType = eventType;
        newEvent.objectID = objectID;
        newEvent.details = details;

        events.Add(newEvent);
        Debug.Log("Logged Event: " + eventType + " | " + objectID);
    }

    public void EndTrial()
    {
        SaveToJson();
        Debug.Log("Trial Ended and Saved");
    }

    void SaveToJson()
    {
        string json = JsonUtility.ToJson(new EventList(events), true);
        string path = Application.persistentDataPath + "/DNAVI_Trial_" + DateTime.Now.ToString("HHmmss") + ".json";
        File.WriteAllText(path, json);

        Debug.Log("Saved to: " + path);
    }

    [Serializable]
    public class EventList
    {
        public List<EventData> events;

        public EventList(List<EventData> e)
        {
            events = e;
        }
    }
}