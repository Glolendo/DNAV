using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    private Rigidbody rb;
    private bool isHeld = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab()
    {
        isHeld = true;
        rb.isKinematic = true;
    }

    public void Release()
    {
        isHeld = false;
        rb.isKinematic = false;
    }
}
