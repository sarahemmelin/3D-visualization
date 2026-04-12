using UnityEngine;
using System.Collections;

public class StarBehavior : MonoBehaviour
{
    [SerializeField] private Material backgroundStarMaterial;
    [SerializeField] private Mesh billboardQuad;

    [Header("Scaling Logic")]
    [SerializeField] private float innerRadius = 50f;
    [SerializeField] private float outerRadius = 245f;
    [SerializeField] private float minTravelScale = 0.05f;

    private Rigidbody rb;
    private MeshFilter meshFilter;
    private StarBatcher batcher;
    private bool isDehydrated = false;
    private float initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        batcher = Object.FindAnyObjectByType<StarBatcher>();

        initialScale = transform.localScale.x;

        StartCoroutine(TravelAndShrink());
    }

    IEnumerator TravelAndShrink()
    {
        while (!isDehydrated)
        {
            float dist = Vector3.Distance(transform.position, Vector3.zero);
            if (dist > innerRadius)
            {
                float t = Mathf.Clamp01((dist - innerRadius) / (outerRadius - innerRadius));

                float currentScale = Mathf.Lerp(initialScale, minTravelScale, t);
                transform.localScale = Vector3.one * currentScale;
            }

            if (dist >= outerRadius)
            {
                TransitionToBackground();
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void TransitionToBackground()
    {
        isDehydrated = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (TryGetComponent<Collider>(out Collider col)) col.enabled = false;

        if (billboardQuad != null)
        {
            float backgroundSize = Random.Range(0.5f, 2.0f);
            transform.localScale = Vector3.one * backgroundSize;

            Mesh meshInstance = Instantiate(billboardQuad);
            Color[] colors = new Color[meshInstance.vertexCount];
            float r = Random.Range(0.2f, 1.0f);
            float g = Random.Range(0.2f, 1.0f);
            float b = Random.Range(0.2f, 1.0f);
            float randomOffset = Random.value * 100f;

            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(r, g, b, randomOffset);

            meshInstance.colors = colors;
            meshFilter.mesh = meshInstance;

            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.material = backgroundStarMaterial;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }

        if (batcher != null)
        {
            transform.SetParent(batcher.transform, true);
            batcher.RegisterStar();
        }

        this.enabled = false;
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(Vector3.zero, innerRadius);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(Vector3.zero, outerRadius);
    //}
}