//using UnityEngine;
//using System.Collections.Generic;

//public class CelestialGravity : MonoBehaviour
//{
//    public float gravityIntensity = 300f;
//    public float captureRadius = 100f;
//    public float swirlStrength = 0.5f;

//    [Tooltip("Lower values (e.g. 0.95) slow objects down faster to prevent slingshotting.")]
//    public float damping = 0.98f;

//    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

//    void FixedUpdate()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
//        foreach (Collider hit in colliders)
//        {
//            if (hit.gameObject.name.Contains("Shard") ||
//                hit.gameObject.name.Contains("Melty") ||
//                hit.gameObject.name.Contains("Main_Star"))
//            {
//                Rigidbody rb = hit.GetComponent<Rigidbody>();
//                if (rb != null) ApplyPull(rb);
//            }
//        }

//        HandleTargetedParticles();
//    }

//    public void RegisterShrapnel(ParticleSystem ps)
//    {
//        if (!activeShrapnelSystems.Contains(ps))
//            activeShrapnelSystems.Add(ps);
//    }

//    void HandleTargetedParticles()
//    {
//        for (int s = activeShrapnelSystems.Count - 1; s >= 0; s--)
//        {
//            ParticleSystem ps = activeShrapnelSystems[s];
//            if (ps == null) { activeShrapnelSystems.RemoveAt(s); continue; }

//            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
//            int count = ps.GetParticles(particles);

//            bool isLocal = ps.main.simulationSpace == ParticleSystemSimulationSpace.Local;

//            for (int i = 0; i < count; i++)
//            {
//                Vector3 worldPos = isLocal ? ps.transform.TransformPoint(particles[i].position) : particles[i].position;
//                Vector3 dir = transform.position - worldPos;
//                float dist = dir.magnitude;

//                if (dist < captureRadius && dist > 0.1f)
//                {
//                    particles[i].velocity *= damping;

//                    Vector3 gravityPull = dir.normalized * (gravityIntensity / dist) * Time.fixedDeltaTime;
//                    Vector3 swirl = Vector3.Cross(Vector3.up, dir.normalized) * (gravityIntensity * swirlStrength / dist) * Time.fixedDeltaTime;
//                    Vector3 totalForce = gravityPull + swirl;

//                    if (isLocal) totalForce = ps.transform.InverseTransformDirection(totalForce);

//                    particles[i].velocity += totalForce;
//                }
//            }
//            ps.SetParticles(particles, count);
//        }
//    }

//    void ApplyPull(Rigidbody rb)
//    {
//        Vector3 direction = transform.position - rb.position;
//        float dist = direction.magnitude;

//        if (dist < captureRadius && dist > 1f)
//        {
//            rb.linearDamping = (1f - damping) * 50f;

//            float force = gravityIntensity / dist;
//            rb.AddForce(direction.normalized * force);

//            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
//            rb.AddForce(swirl * (force * swirlStrength));
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.name.Contains("Shard") || other.gameObject.name.Contains("Melty"))
//        {
//            Debug.Log("Big Star consumed: " + other.gameObject.name);
//            Destroy(other.gameObject);
//        }
//    }
//}


//using UnityEngine;
//using System.Collections.Generic;

//public class CelestialGravity : MonoBehaviour
//{
//    public float gravityIntensity = 300f;
//    public float captureRadius = 100f;
//    public float swirlStrength = 0.5f;
//    public float damping = 0.98f;

//    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

//    void FixedUpdate()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
//        foreach (Collider hit in colliders)
//        {
//            Rigidbody rb = hit.attachedRigidbody;
//            if (rb != null)
//            {
//                string n = rb.gameObject.name;
//                // Only pull shards, melty bits, or the Main_Star
//                if (n.Contains("Shard") || n.Contains("Melty") || n.Contains("Main_Star"))
//                {
//                    ApplyPull(rb);
//                }
//            }
//        }
//        HandleTargetedParticles();
//    }

//    public void RegisterShrapnel(ParticleSystem ps)
//    {
//        if (!activeShrapnelSystems.Contains(ps)) activeShrapnelSystems.Add(ps);
//    }

//    void HandleTargetedParticles()
//    {
//        for (int s = activeShrapnelSystems.Count - 1; s >= 0; s--)
//        {
//            ParticleSystem ps = activeShrapnelSystems[s];
//            if (ps == null) { activeShrapnelSystems.RemoveAt(s); continue; }

//            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
//            int count = ps.GetParticles(particles);
//            bool isLocal = ps.main.simulationSpace == ParticleSystemSimulationSpace.Local;

//            for (int i = 0; i < count; i++)
//            {
//                Vector3 worldPos = isLocal ? ps.transform.TransformPoint(particles[i].position) : particles[i].position;
//                Vector3 dir = transform.position - worldPos;
//                float dist = dir.magnitude;

//                if (dist < captureRadius && dist > 0.1f)
//                {
//                    particles[i].velocity *= damping;
//                    Vector3 gravityPull = dir.normalized * (gravityIntensity / dist) * Time.fixedDeltaTime;

//                    // Particle-specific swirl (matches the Shrapnel_Cloud rotation)
//                    Vector3 swirl = Vector3.Cross(Vector3.up, dir.normalized) * (gravityIntensity * swirlStrength / dist) * Time.fixedDeltaTime;

//                    Vector3 totalForce = gravityPull + swirl;
//                    if (isLocal) totalForce = ps.transform.InverseTransformDirection(totalForce);
//                    particles[i].velocity += totalForce;
//                }
//            }
//            ps.SetParticles(particles, count);
//        }
//    }

//    void ApplyPull(Rigidbody rb)
//    {
//        Vector3 direction = transform.position - rb.position;
//        float dist = direction.magnitude;
//        if (dist < captureRadius && dist > 0.5f)
//        {
//            rb.linearDamping = (1f - damping) * 50f;
//            float force = gravityIntensity / dist;
//            rb.AddForce(direction.normalized * force);

//            // Rigidbody-specific swirl (Restored to the stable 'yesterday' version)
//            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
//            rb.AddForce(swirl * (force * swirlStrength));
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.attachedRigidbody != null)
//        {
//            GameObject target = other.attachedRigidbody.gameObject;
//            if (target.name.Contains("Shard") || target.name.Contains("Melty"))
//            {
//                // Only destroy the shard if it's a child of a destroyed planet, 
//                // preventing accidental batcher deletions.
//                Destroy(target);
//            }
//        }
//    }
//}

//using UnityEngine;
//using System.Collections.Generic;

//public class CelestialGravity : MonoBehaviour
//{
//    public float gravityIntensity = 300f;
//    public float captureRadius = 100f;
//    public float swirlStrength = 0.5f;
//    public float damping = 0.98f;

//    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

//    void FixedUpdate()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
//        foreach (Collider hit in colliders)
//        {
//            Rigidbody rb = hit.attachedRigidbody;
//            if (rb != null)
//            {
//                string n = rb.gameObject.name;
//                //// LOG 1: See what is actually being detected
//                //if (n.Contains("Ceres") || n.Contains("Shard") || n.Contains("Main_Star"))
//                //{
//                //    Debug.Log($"<color=cyan>[FOUND]</color> Big Star is pulling: {n}");
//                //    ApplyPull(rb);
//                //}
//            }
//        }
//        HandleTargetedParticles();
//    }

//    public void RegisterShrapnel(ParticleSystem ps)
//    {
//        if (!activeShrapnelSystems.Contains(ps)) activeShrapnelSystems.Add(ps);
//    }

//    void HandleTargetedParticles()
//    {
//        for (int s = activeShrapnelSystems.Count - 1; s >= 0; s--)
//        {
//            ParticleSystem ps = activeShrapnelSystems[s];
//            if (ps == null) { activeShrapnelSystems.RemoveAt(s); continue; }
//            // Particle logic remains for the explosion clouds
//        }
//    }

//    void ApplyPull(Rigidbody rb)
//    {
//        Vector3 direction = transform.position - rb.position;
//        float dist = direction.magnitude;

//        if (dist < captureRadius && dist > 0.5f)
//        {
//            // Safety check for kinematic to avoid the velocity error
//            if (!rb.isKinematic)
//            {
//                rb.linearVelocity *= damping;
//            }

//            float force = gravityIntensity / dist;
//            rb.AddForce(direction.normalized * force);

//            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
//            rb.AddForce(swirl * (force * swirlStrength));
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.attachedRigidbody != null)
//        {
//            GameObject target = other.attachedRigidbody.gameObject;

//            // LOG 2: The only place this script can delete your shards
//            if (target.name.Contains("Ceres") || target.name.Contains("Shard"))
//            {
//                Debug.Log($"<color=red>[ATE]</color> Big Star just consumed: {target.name}");
//                Destroy(target);
//            }
//        }
//    }
//}

//using UnityEngine;
//using System.Collections.Generic;

//public class CelestialGravity : MonoBehaviour
//{
//    public float gravityIntensity = 300f;
//    public float captureRadius = 100f;
//    public float swirlStrength = 0.5f;
//    public float damping = 0.98f;

//    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

//    void FixedUpdate()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
//        foreach (Collider hit in colliders)
//        {
//            Rigidbody rb = hit.attachedRigidbody;
//            if (rb != null)
//            {
//                string n = rb.gameObject.name;

//                // 1. PULL THE DEBRIS (Shards and Ceres pieces)
//                if (n.Contains("Ceres") || n.Contains("Shard"))
//                {
//                    ApplyPull(rb);
//                }

//                // 2. PULL THE MAIN STAR (Separated so it doesn't break shard logic)
//                if (n == "Main_Star")
//                {
//                    ApplyPull(rb);
//                }
//            }
//        }
//        HandleTargetedParticles();
//    }

//    public void RegisterShrapnel(ParticleSystem ps)
//    {
//        if (!activeShrapnelSystems.Contains(ps)) activeShrapnelSystems.Add(ps);
//    }

//    void HandleTargetedParticles()
//    {
//        for (int s = activeShrapnelSystems.Count - 1; s >= 0; s--)
//        {
//            ParticleSystem ps = activeShrapnelSystems[s];
//            if (ps == null) { activeShrapnelSystems.RemoveAt(s); continue; }

//            // NEW: Only pull if the system name contains these specific strings
//            string psName = ps.gameObject.name;
//            if (!psName.Contains("Star_Bleed") && !psName.Contains("Shrapnel_Cloud"))
//            {
//                continue; // Ignore any other particle systems
//            }

//            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
//            int count = ps.GetParticles(particles);
//            bool isLocal = ps.main.simulationSpace == ParticleSystemSimulationSpace.Local;

//            for (int i = 0; i < count; i++)
//            {
//                // ... (The rest of your working math stays exactly the same)
//                Vector3 worldPos = isLocal ? ps.transform.TransformPoint(particles[i].position) : particles[i].position;
//                Vector3 dir = transform.position - worldPos;
//                float dist = dir.magnitude;

//                if (dist < captureRadius && dist > 0.1f)
//                {
//                    particles[i].velocity *= damping;
//                    Vector3 gravityPull = dir.normalized * (gravityIntensity / dist) * Time.fixedDeltaTime;
//                    Vector3 swirl = Vector3.Cross(Vector3.up, dir.normalized) * (gravityIntensity * swirlStrength / dist) * Time.fixedDeltaTime;
//                    Vector3 totalForce = gravityPull + swirl;

//                    if (isLocal) totalForce = ps.transform.InverseTransformDirection(totalForce);
//                    particles[i].velocity += totalForce;
//                }
//            }
//            ps.SetParticles(particles, count);
//        }
//    }

//    void ApplyPull(Rigidbody rb)
//    {
//        Vector3 direction = transform.position - rb.position;
//        float dist = direction.magnitude;

//        if (dist < captureRadius && dist > 0.5f)
//        {
//            if (!rb.isKinematic)
//            {
//                rb.linearVelocity *= damping;
//            }

//            // --- UPDATE TRIGGER START ---
//            if (rb.name == "Main_Star")
//            {
//                ParticleSystem bleed = rb.GetComponentInChildren<ParticleSystem>();
//                if (bleed != null)
//                {
//                    // Start the system if it's sleeping
//                    if (!bleed.isPlaying) bleed.Play();

//                    // Ensure it's in the suction list
//                    RegisterShrapnel(bleed);

//                    // Increase flow as it gets closer
//                    var emission = bleed.emission;
//                    float intensity = Mathf.InverseLerp(captureRadius, 2f, dist);
//                    emission.rateOverTime = intensity * 500f;
//                }
//            }
//            // --- UPDATE TRIGGER END ---

//            float force = gravityIntensity / dist;
//            rb.AddForce(direction.normalized * force);

//            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
//            rb.AddForce(swirl * (force * swirlStrength));
//        }
//    }



//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.attachedRigidbody != null)
//        {
//            GameObject target = other.attachedRigidbody.gameObject;

//            // WE ONLY EAT SHARDS. WE NEVER EAT THE MAIN_STAR.
//            if (target.name.Contains("Ceres") || target.name.Contains("Shard") || target.name.Contains("Melty"))
//            {
//                Debug.Log($"<color=red>[ATE]</color> {target.name}");
//                Destroy(target);
//            }
//        }
//    }
//}

using UnityEngine;
using System.Collections.Generic;

public class CelestialGravity : MonoBehaviour
{
    public float gravityIntensity = 300f;
    public float captureRadius = 100f;
    public float swirlStrength = 0.5f;
    public float damping = 0.98f;

    private List<ParticleSystem> activeShrapnelSystems = new List<ParticleSystem>();

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                string n = rb.gameObject.name;

                if (n.Contains("Ceres") || n.Contains("Shard"))
                {
                    ApplyPull(rb);
                }
                if (n == "Main_Star")
                {
                    ApplyPull(rb);
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
        for (int s = activeShrapnelSystems.Count - 1; s >= 0; s--)
        {
            ParticleSystem ps = activeShrapnelSystems[s];
            if (ps == null) { activeShrapnelSystems.RemoveAt(s); continue; }

            string psName = ps.gameObject.name;
            if (!psName.Contains("Star_Bleed") && !psName.Contains("Shrapnel_Cloud"))
            {
                continue;
            }

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
                    particles[i].velocity *= damping;
                    Vector3 gravityPull = dir.normalized * (gravityIntensity / dist) * Time.fixedDeltaTime;
                    Vector3 swirl = Vector3.Cross(Vector3.up, dir.normalized) * (gravityIntensity * swirlStrength / dist) * Time.fixedDeltaTime;
                    Vector3 totalForce = gravityPull + swirl;

                    if (isLocal) totalForce = ps.transform.InverseTransformDirection(totalForce);
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

        if (dist < captureRadius && dist > 0.5f)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity *= damping;
            }

            if (rb.name == "Main_Star")
            {
                ParticleSystem bleed = rb.GetComponentInChildren<ParticleSystem>();
                if (bleed != null && bleed.gameObject.name == "Star_Bleed")
                {
                    if (!bleed.isPlaying) bleed.Play();
                    RegisterShrapnel(bleed);
                    var emission = bleed.emission;
                    float intensity = Mathf.InverseLerp(captureRadius, 2f, dist);
                    emission.rateOverTime = intensity * 1200f;
                }
            }

            float force = gravityIntensity / dist;
            rb.AddForce(direction.normalized * force);

            Vector3 swirl = Vector3.Cross(Vector3.up, direction.normalized);
            rb.AddForce(swirl * (force * swirlStrength));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            GameObject target = other.attachedRigidbody.gameObject;

            if (target.name.Contains("Ceres") || target.name.Contains("Shard") || target.name.Contains("Melty"))
            {
                Debug.Log($"<color=red>[ATE]</color> {target.name}");
                Destroy(target);
            }
        }
    }
}