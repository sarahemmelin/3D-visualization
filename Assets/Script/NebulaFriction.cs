using UnityEngine;

public class NebulaFriction : MonoBehaviour
{
    public float gasDensity = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearDamping = gasDensity;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearDamping = 0;
        }
    }
}