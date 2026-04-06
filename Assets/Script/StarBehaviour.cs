using UnityEngine;

public class StarBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFrozen = false;
    private float boundaryRadius = 245f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isFrozen) return;

        // The Distance Check
        if (Vector3.Distance(transform.position, Vector3.zero) > boundaryRadius)
        {
            FreezeStar();
        }
    }

    void FreezeStar()
    {
        isFrozen = true;

        // 1. KILL PHYSICS
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // This is the performance savior

        // 2. DISABLE COLLIDER (No need to check for hits anymore)
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        // 3. CLEANUP
        // Disable this script to stop the Update() check
        this.enabled = false;

        Debug.Log("Star reached the horizon and solidified.");
    }
}