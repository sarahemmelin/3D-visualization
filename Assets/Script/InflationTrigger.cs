using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflationTrigger : MonoBehaviour
{
    [Header("Cosmic Elements")]
    public GameObject starPrefab;
    public int starCount = 300;
    public float inflationForce = 150f;

    [Header("Randomization Settings")]
    public float minSize = 0.2f;
    public float maxSize = 1.5f;
    public Gradient starColors;

    [Header("UI Reference")]
    public CanvasGroup flashGroup;
    public float fadeSpeed = 0.5f;

    private bool hasBigBangOccurred = false;

    void Start()
    {
        if (!hasBigBangOccurred)
        {
            StartCoroutine(ExecuteBigBang());
        }
    }

    IEnumerator ExecuteBigBang()
    {
        hasBigBangOccurred = true;
        flashGroup.alpha = 1f;

        StarTravel mainStar = FindFirstObjectByType<StarTravel>();
        if (mainStar != null)
        {
            mainStar.TriggerJourney();
        }
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < starCount; i++)
        {
            Vector3 spawnOffset = Random.insideUnitSphere * 1.5f;
            GameObject star = Instantiate(starPrefab, transform.position + spawnOffset, Quaternion.identity);

            // size
            float randomScale = Random.Range(minSize, maxSize);
            star.transform.localScale = Vector3.one * randomScale;

            // colors
            MeshRenderer mr = star.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Color pick = starColors.Evaluate(Random.value);
                mr.material.SetColor("_BaseColor", pick);

                mr.material.EnableKeyword("_EMISSION");
                mr.material.SetColor("_EmissionColor", pick * 2.0f);
            }

            // force
            Rigidbody rb = star.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = spawnOffset.normalized;
                rb.AddForce(direction * inflationForce, ForceMode.Impulse);
            }

            // hold back every 100 stars so my computer doesn't explode
            if (i % 100 == 0) yield return null;
        }

        // Fade out
        while (flashGroup.alpha > 0)
        {
            flashGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        Debug.Log("Big Bang Complete. Starfield randomized and Main Star triggered.");
    }
}