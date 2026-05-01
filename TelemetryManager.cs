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

    [Serializable]
    public class EventList
    {
        public List<EventData> events;

        public EventList(List<EventData> e)
        {
            events = e;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keeps it alive if scene changes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTrial()
    {
        trialStartTime = Time.realtimeSinceStartup;
        events.Clear();

        Debug.Log("Trial Started");
    }

    public void LogEvent(string eventType, string objectID, string details = "")
    {
        EventData newEvent = new EventData
        {
            timeSinceStart = Time.realtimeSinceStartup - trialStartTime,
            eventType = eventType,
            objectID = objectID,
            details = details
        };

        events.Add(newEvent);

        Debug.Log($"[LOG] {eventType} | {objectID} | {details}");
    }

    public void EndTrial()
    {
        SaveToJson();

        Debug.Log("Trial Ended and Data Saved");
    }

    void SaveToJson()
    {
        string json = JsonUtility.ToJson(new EventList(events), true);

        string fileName = "DNAVI_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, json);

        Debug.Log("Saved to: " + path);
    }
}
