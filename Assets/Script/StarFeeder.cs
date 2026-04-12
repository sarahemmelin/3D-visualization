using UnityEngine;

public class StarFeeder : MonoBehaviour
{
    public Transform mainStar;

    [Header("Detection Settings")]
    public float detectionRadius = 25f;
    public float surfaceRadius = 8f;

    [Header("Visual Response")]
    public float baseIntensity = 1.0f;
    public float maxPulseIntensity = 12.0f;
    [Range(0.1f, 5.0f)]
    public float pulseSpeedMultiplier = 1.0f;

    [Header("Background Star Influence")]
    public float starAgitationStrength = 0.5f;
    public float agitationDecaySpeed = 2.0f;
    private float currentAgitation = 0f;
    private float frameAgitation = 0f;

    private Material starMat;
    private Light starLight;
    private Color originalColor;
    private float pulsePhase = 0f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        starMat = renderer.material;
        starLight = GetComponentInChildren<Light>();

        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc == null) sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = detectionRadius / Mathf.Max(transform.localScale.x, 0.0001f);

        if (starMat.HasProperty("_Color"))
            originalColor = starMat.GetColor("_Color");
        else
            originalColor = Color.white;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<StarBehavior>(out StarBehavior star))
        {
            float dist = Vector3.Distance(transform.position, other.transform.position);
            float prox = 1.0f - Mathf.Clamp01((dist - surfaceRadius) / (detectionRadius - surfaceRadius));

            frameAgitation += prox * starAgitationStrength;
        }
    }

    void Update()
    {
        if (frameAgitation > currentAgitation)
            currentAgitation = frameAgitation;
        else
            currentAgitation = Mathf.MoveTowards(currentAgitation, 0f, Time.deltaTime * agitationDecaySpeed);

        float swarmEffect = Mathf.Clamp(currentAgitation, 0f, 5f);
        float totalIntensity = baseIntensity + swarmEffect;

        if (mainStar != null)
        {
            float distance = Vector3.Distance(transform.position, mainStar.position);
            if (distance <= detectionRadius)
            {
                float proximity = 1.0f - Mathf.Clamp01((distance - surfaceRadius) / (detectionRadius - surfaceRadius));
                float targetSpeed = Mathf.Lerp(2f, 35f, proximity) * pulseSpeedMultiplier;

                pulsePhase += Time.deltaTime * targetSpeed;
                float pulse = Mathf.Sin(pulsePhase);

                float proximityThrob = Mathf.Lerp(0f, maxPulseIntensity - baseIntensity, (pulse + 1f) / 2f * proximity);
                totalIntensity += proximityThrob;
            }
            else
            {
                pulsePhase = 0f;
            }
        }

        Color finalColor = originalColor * totalIntensity;
        starMat.SetColor("_Color", finalColor);
        if (starLight != null) starLight.intensity = totalIntensity;

        frameAgitation = 0f;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, surfaceRadius);
    //}
}