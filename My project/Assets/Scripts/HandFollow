using UnityEngine;

public class HandFollow : MonoBehaviour
{
    [SerializeField] private Transform target;   // The VR controller or tracked hand to follow
    [SerializeField] private float smoothSpeed = 10f;

    void Update()
    {
        if (target == null) return;

        // Smoothly follow target position and rotation
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * smoothSpeed);
    }
}
