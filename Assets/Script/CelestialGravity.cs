using UnityEngine;
using System.Collections.Generic;

public class CelestialGravity : MonoBehaviour
{
    public float gravityIntensity = 300f;
    public float captureRadius = 100f;
    public float swirlStrength = 0.5f;
    public float damping = 0.98f;
    public float shrinkSpeed = 0.5f;
    public float eatRadius = 5f;
    [Header("Star Bleed Settings")]
    public float bleedStartDistance = 40f; // How close Main_Star needs to be before bleeding starts

    private bool canEat = false;
    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

    public void EnableEating()
    {
        canEat = true;
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                string n = rb.gameObject.name;
                if (n.Contains("Ceres") || n.Contains("Shard") || n == "Main_Star")
                {
                    ApplyPull(rb);
                }
            }
        }

        if (canEat)
        {
            Collider[] eatColliders = Physics.OverlapSphere(transform.position, eatRadius);
            foreach (Collider hit in eatColliders)
            {
                if (hit.attachedRigidbody != null)
                {
                    GameObject target = hit.attachedRigidbody.gameObject;
                    if (target.name.Contains("Ceres") || target.name.Contains("Shard") || target.name.Contains("Melty"))
                    {
                        Destroy(target);
                    }
                }
            }
        }

        HandleTargetedParticles();
    }

    public void RegisterShrapnel(ParticleSystem ps)
    {
        if (!activeShrapnelSystems.Contains(ps)) activeShrapnelSystems.Add(ps);
    }

    void HandleTargetedParticles()
    {
        Color consumedColor = new Color(0f, 1f, 1f, 1f) * 10f;

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

                if (dist < eatRadius)
                {
                    particles[i].remainingLifetime = -1f;
                    continue;
                }

                if (dist < captureRadius)
                {
                    // Shrapnel_Cloud and other registered systems — forced dive intact
                    float diveFactor = Mathf.InverseLerp(20f, 5f, dist);

                    particles[i].velocity *= Mathf.Lerp(damping, 0.7f, diveFactor);

                    Vector3 gravityPull = dir.normalized * (gravityIntensity / dist);
                    Vector3 swirl = Vector3.Cross(Vector3.up, dir.normalized) * (gravityIntensity * swirlStrength / dist) * (1f - diveFactor);
                    Vector3 forcedDive = dir.normalized * (gravityIntensity * 1.5f * diveFactor);
                    Vector3 totalForce = (gravityPull + swirl + forcedDive) * Time.fixedDeltaTime;

                    if (isLocal) totalForce = ps.transform.InverseTransformDirection(totalForce);
                    particles[i].velocity += totalForce;

                    float colorFactor = Mathf.InverseLerp(captureRadius, 2f, dist);
                    if (colorFactor > 0.05f)
                    {
                        particles[i].startColor = Color.Lerp(particles[i].startColor, consumedColor, colorFactor);
                    }
                }
            }
            ps.SetParticles(particles, count);
        }
    }

    void ApplyPull(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float dist = direction.magnitude;

        if (dist < captureRadius && dist > 0.2f)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity *= damping;
            }

            float witherFactor = 0f;

            if (rb.name == "Main_Star")
            {
                // Find Star_Bleed by name so we never grab the wrong particle system
                ParticleSystem bleed = null;
                foreach (ParticleSystem ps in rb.GetComponentsInChildren<ParticleSystem>())
                {
                    if (ps.gameObject.name == "Star_Bleed") { bleed = ps; break; }
                }

                StarTravel travel = rb.GetComponent<StarTravel>();
                if (travel != null && (travel.IsInFinalStretch || !travel.isMoving))
                {
                    if (!rb.isKinematic)
                    {
                        float speedBrake = Mathf.InverseLerp(12f, 0.5f, dist);
                        rb.linearVelocity *= Mathf.Lerp(1f, 0.75f, speedBrake);
                    }

                    witherFactor = Mathf.InverseLerp(10f, 0.5f, dist);

                    float currentScale = rb.transform.localScale.x;
                    float targetScale = Mathf.Lerp(1.0f, 0.01f, witherFactor);
                    rb.transform.localScale = Vector3.one * Mathf.MoveTowards(currentScale, targetScale, Time.deltaTime * shrinkSpeed);
                }

                // Only start bleeding within bleedStartDistance, scaling up with proximity and wither
                if (bleed != null && dist < bleedStartDistance)
                {
                    float bleedProximity = Mathf.InverseLerp(bleedStartDistance, 0.5f, dist);

                    if (!bleed.isPlaying) bleed.Play();
                    var emission = bleed.emission;
                    emission.rateOverTime = (bleedProximity * 500f) + (witherFactor * 1500f);
                }
                else if (bleed != null && dist >= bleedStartDistance)
                {
                    // Make sure bleed is off until in range
                    if (bleed.isPlaying) bleed.Stop();
                }
            }

            float force = gravityIntensity / dist;
            rb.AddForce(direction.normalized * force);

            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
            rb.AddForce(swirl * (force * swirlStrength * (1f - witherFactor)));
        }
    }

    // OnTriggerEnter removed — eating is now handled by eatRadius in FixedUpdate
}