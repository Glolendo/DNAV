using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public bool isCorrectDoor;

    public void OpenDoor()
    {
        if (isCorrectDoor)
        {
            TelemetryManager.Instance.LogEvent("CorrectDoorOpened", gameObject.name);
        }
        else
        {
            TelemetryManager.Instance.LogEvent("WrongDoorOpened", gameObject.name);
        }
    }
}