using UnityEngine;
using System.Collections;

public class StarBehavior : MonoBehaviour
{
    [SerializeField] private Material backgroundStarMaterial;

    private Rigidbody rb;
    private MeshFilter meshFilter;
    private StarBatcher batcher;

    [Header("Boundary Settings")]
    private float boundaryRadius = 245f;
    private bool isFrozen = false;

    [Header("Optimization Assets")]
    [SerializeField] private Mesh billboardQuad;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        batcher = Object.FindAnyObjectByType<StarBatcher>();
    }

    void Update()
    {
        if (!isFrozen && Vector3.Distance(transform.position, Vector3.zero) > boundaryRadius)
        {
            StartCoroutine(ShrinkAndDehydrate());
        }
    }

    IEnumerator ShrinkAndDehydrate()
    {
        isFrozen = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (TryGetComponent<Collider>(out Collider col)) col.enabled = false;

 
        Vector3 startScale = transform.localScale;
        Vector3 shrinkTarget = Vector3.one * 0.05f;
        float elapsed = 0;
        float duration = 1.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, shrinkTarget, elapsed / duration);
            yield return null;
        }

        float backgroundSize = Random.Range(0.5f, 2.0f);
        transform.localScale = Vector3.one * backgroundSize;

        if (billboardQuad != null)
        {
            Mesh meshInstance = Instantiate(billboardQuad);
            Color[] colors = new Color[meshInstance.vertexCount];
            float randomOffset = Random.value * 100f;

            float r = Random.Range(0.2f, 1.0f);
            float g = Random.Range(0.2f, 1.0f);
            float b = Random.Range(0.2f, 1.0f);

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(r, g, b, randomOffset);
            }

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
}