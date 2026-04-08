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

        // 2. VISUAL DEHYDRATION (Swap to Quad + Unique Twinkle Color)
        if (billboardQuad != null)
        {
            // We create the unique mesh instance here so the StarBatcher can 'harvest' its color
            Mesh meshInstance = Instantiate(billboardQuad);
            Color[] colors = new Color[meshInstance.vertexCount];
            float randomOffset = Random.value * 100f;

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(randomOffset, 0, 0, 1);
            }

            meshInstance.colors = colors;
            meshFilter.mesh = meshInstance;

            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.material = backgroundStarMaterial;

            // Kill shadows immediately
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }

        // 3. CONSTELLATION BATCHING
        if (batcher != null)
        {
            transform.SetParent(batcher.transform, true);

            batcher.RegisterStar();
        }

        // 4. CPU CLEANUP
        this.enabled = false;
    }
}