using UnityEngine;

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

        // Find the batcher once at the start to save performance
        batcher = Object.FindAnyObjectByType<StarBatcher>();
    }

    void Update()
    {
        if (!isFrozen && Vector3.Distance(transform.position, Vector3.zero) > boundaryRadius)
        {
            FreezeAndDehydrate();
        }
    }

    void FreezeAndDehydrate()
    {
        isFrozen = true;

        // 1. PHYSICS KILL
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        // 2. VISUAL DEHYDRATION
        if (billboardQuad != null)
        {
            Mesh meshInstance = Instantiate(billboardQuad);
            Color[] colors = new Color[meshInstance.vertexCount];

            // Twinkle offset goes in Alpha
            float randomOffset = Random.value * 100f;
            // Random scale before batching
            float size = Random.Range(0.5f, 2.0f);
            transform.localScale = Vector3.one * size;

            // Generate subtle tints (mostly 1.0, dipping slightly for color)
            // Red star: (1.0, 0.8, 0.8) | Blue star: (0.8, 0.8, 1.0)
            //float r = Random.Range(0.85f, 1.0f);
            //float g = Random.Range(0.85f, 1.0f);
            //float b = Random.Range(0.85f, 1.0f);
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

        // 3. CONSTELLATION BATCHING
        if (batcher != null)
        {
            transform.SetParent(batcher.transform, true);
            batcher.RegisterStar();
        }

        this.enabled = false;
    }
}