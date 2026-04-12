using UnityEngine;

public class PlanetDestruction : MonoBehaviour
{
    [Header("'Ceres_Whole' child here")]
    public GameObject wholePlanet;

    [Header("Shrapnel Particle System here")]
    public ParticleSystem explosionParticles;

    public float explosionForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        // Changed from "Star" to "Main_Star" 
        // This stops it from exploding just because the Big_Star is near.
        if (other.gameObject.name.Contains("Main_Star"))
        {
            Debug.Log("Impact detected! BLOWN THE FUCK UP by: " + other.gameObject.name);
            Explode();
        }
    }

    void Explode()
    {
        if (explosionParticles != null)
        {
            ParticleSystem effect = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            effect.Play();

            GameObject bigStar = GameObject.Find("Big_Star");
            if (bigStar != null)
            {
                CelestialGravity gravity = bigStar.GetComponent<CelestialGravity>();
                if (gravity != null) gravity.RegisterShrapnel(effect);
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
}