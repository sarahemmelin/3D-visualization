using UnityEngine;
using System.Collections.Generic;

public class CelestialGravity : MonoBehaviour
{
    public float gravityIntensity = 300f;
    public float captureRadius = 100f;
    public float swirlStrength = 0.5f;

    [Tooltip("Lower values (e.g. 0.95) slow objects down faster to prevent slingshotting.")]
    public float damping = 0.98f;

    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.name.Contains("Shard") || hit.gameObject.name.Contains("Melty"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null) ApplyPull(rb);
            }
        }

        HandleTargetedParticles();
    }

    public void RegisterShrapnel(ParticleSystem ps)
    {
        if (!activeShrapnelSystems.Contains(ps))
            activeShrapnelSystems.Add(ps);
    }

    void HandleTargetedParticles()
    {
        for (int s = activeShrapnelSystems.Count - 1; s >= 0; s--)
        {
            ParticleSystem ps = activeShrapnelSystems[s];
            if (ps == null) { activeShrapnelSystems.RemoveAt(s); continue; }

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
            int count = ps.GetParticles(particles);

            bool isLocal = ps.main.simulationSpace == ParticleSystemSimulationSpace.Local;

            for (int i = 0; i < count; i++)
            {
                Vector3 worldPos = isLocal ? ps.transform.TransformPoint(particles[i].position) : particles[i].position;
                Vector3 dir = transform.position - worldPos;
                float dist = dir.magnitude;

                if (dist < captureRadius && dist > 0.1f)
                {
                    // Damping:
                    particles[i].velocity *= damping;

                    Vector3 gravityPull = dir.normalized * (gravityIntensity / dist) * Time.fixedDeltaTime;

                    Vector3 swirl = Vector3.Cross(dir.normalized, Vector3.up) * (gravityIntensity * swirlStrength / dist) * Time.fixedDeltaTime;

                    Vector3 totalForce = gravityPull + swirl;

                    if (isLocal)
                    {
                        totalForce = ps.transform.InverseTransformDirection(totalForce);
                    }

                    particles[i].velocity += totalForce;
                }
            }
            ps.SetParticles(particles, count);
        }
    }

    void ApplyPull(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float dist = direction.magnitude;

        if (dist < captureRadius && dist > 1f)
        {
            // Damping
            rb.linearDamping = (1f - damping) * 50f;

            float force = gravityIntensity / dist;
            rb.AddForce(direction.normalized * force);

            // Swirl for shards
            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
            rb.AddForce(swirl * (force * swirlStrength));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Shard") || other.gameObject.name.Contains("Melty"))
        {
            Debug.Log("Big Star consumed: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}