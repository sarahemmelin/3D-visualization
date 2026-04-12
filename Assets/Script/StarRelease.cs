using UnityEngine;

public class StarRelease : MonoBehaviour
{
    private Rigidbody rb;
    private bool isReleased = false;
    public MonoBehaviour pathingScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ReleaseToGravity()
    {
        if (isReleased) return;
        isReleased = true;

        if (pathingScript != null) pathingScript.enabled = false;

        rb.isKinematic = false;

        Vector3 tangentVelocity = Vector3.Cross(Vector3.up, (Vector3.zero - transform.position).normalized);
        rb.linearVelocity = tangentVelocity * 10f;
    }
}