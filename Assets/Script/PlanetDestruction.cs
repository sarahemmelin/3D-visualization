using UnityEngine;
using Unity.Cinemachine;
public class PlanetDestruction : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    [Header("'Ceres_Whole' child here")]
    public GameObject wholePlanet;

    [Header("Shrapnel Particle System here")]
    public ParticleSystem explosionParticles;

    public float explosionForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Main_Star"))
        {
            Debug.Log("Impact detected! BLOWN THE FUCK UP by: " + other.gameObject.name);
            Explode();
        }
    }

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Explode()
    {
        // Shake the camera on impact
        if (impulseSource != null) impulseSource.GenerateImpulse(50f);

        if (explosionParticles != null)
        {
            ParticleSystem effect = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            effect.Play();

            GameObject bigStar = GameObject.Find("Big_Star");
            if (bigStar != null)
            {
                CelestialGravity gravity = bigStar.GetComponent<CelestialGravity>();
                if (gravity != null)
                {
                    gravity.RegisterShrapnel(effect);
                    gravity.EnableEating();
                }
            }

            Destroy(effect.gameObject, 200f);
        }

        if (wholePlanet != null)
        {
            wholePlanet.SetActive(false);
        }

        foreach (Transform child in transform)
        {
            if (child.gameObject != wholePlanet)
            {
                child.gameObject.SetActive(true);

                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDir = Random.insideUnitSphere;
                    rb.AddForce(randomDir * explosionForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * explosionForce, ForceMode.Impulse);
                }
            }
        }

        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
    //void Explode()
    //{
    //    if (explosionParticles != null)
    //    {
    //        ParticleSystem effect = Instantiate(explosionParticles, transform.position, Quaternion.identity);
    //        effect.Play();

    //        GameObject bigStar = GameObject.Find("Big_Star");
    //        if (bigStar != null)
    //        {
    //            CelestialGravity gravity = bigStar.GetComponent<CelestialGravity>();
    //            if (gravity != null)
    //            {
    //                gravity.RegisterShrapnel(effect);
    //                gravity.EnableEating(); // Unlock eating only after actual impact
    //            }
    //        }

    //        Destroy(effect.gameObject, 200f);
    //    }

    //    if (wholePlanet != null)
    //    {
    //        wholePlanet.SetActive(false);
    //    }

    //    foreach (Transform child in transform)
    //    {
    //        if (child.gameObject != wholePlanet)
    //        {
    //            child.gameObject.SetActive(true);

    //            Rigidbody rb = child.GetComponent<Rigidbody>();
    //            if (rb != null)
    //            {
    //                Vector3 randomDir = Random.insideUnitSphere;
    //                rb.AddForce(randomDir * explosionForce, ForceMode.Impulse);
    //                rb.AddTorque(Random.insideUnitSphere * explosionForce, ForceMode.Impulse);
    //            }
    //        }
    //    }
}