using UnityEngine;

public class DecisionNode : MonoBehaviour
{
    public string nodeID = "Node";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        NodeTracker tracker = FindFirstObjectByType<NodeTracker>();

        if (tracker != null)
        {
            tracker.RegisterNode(nodeID);
        }

        Debug.Log("Decision node entered: " + nodeID);
    }
}