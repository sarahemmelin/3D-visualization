using UnityEngine;

public class NebulaFriction : MonoBehaviour
{
    public float gasDensity = 2.5f;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            // Smoothly apply resistance based on velocity
            Vector3 resistance = -rb.linearVelocity * gasDensity * Time.fixedDeltaTime;
            rb.AddForce(resistance, ForceMode.Acceleration);
        }
    }
}