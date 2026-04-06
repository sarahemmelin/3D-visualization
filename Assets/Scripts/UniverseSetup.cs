using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Sets up the Universe environment:
/// - Builds an inverted sphere mesh (normals point inward) as the "Universe" boundary.
/// - Applies a black unlit material so no light reflects off the inner surface.
/// - Sets global physics gravity to zero inside the Universe.
/// - Disables all directional lights in the scene.
/// - Creates a global post-processing volume with Bloom enabled (for stars).
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class UniverseSetup : MonoBehaviour
{
    [Tooltip("Radius of the Universe sphere in world units.")]
    public float radius = 500f;

    [Tooltip("Number of latitude (vertical) segments on the sphere mesh.")]
    public int latSegments = 30;

    [Tooltip("Number of longitude (horizontal) segments on the sphere mesh.")]
    public int lonSegments = 30;

    void Awake()
    {
        // 1. Set global gravity to zero — no gravity inside the Universe.
        Physics.gravity = Vector3.zero;

        // 2. Build the inverted sphere mesh (faces and normals pointing inward).
        GetComponent<MeshFilter>().mesh = CreateInvertedSphereMesh(radius, latSegments, lonSegments);

        // 3. Apply an unlit black material — no lighting should reflect off the Universe boundary.
        Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (unlitShader != null)
        {
            Material blackMaterial = new Material(unlitShader);
            blackMaterial.name = "UnlitBlack";
            blackMaterial.color = Color.black;
            blackMaterial.hideFlags = HideFlags.DontSave;
            GetComponent<MeshRenderer>().sharedMaterial = blackMaterial;
        }
        else
        {
            Debug.LogWarning("UniverseSetup: Could not find 'Universal Render Pipeline/Unlit' shader. " +
                             "Ensure the Universal Render Pipeline package is installed.");
        }

        // 4. Disable all directional lights in the scene.
        foreach (Light light in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (light.type == LightType.Directional)
                light.enabled = false;
        }

        // 5. Set up a global post-processing volume with Bloom (used for star glow later).
        SetupBloom();
    }

    /// <summary>
    /// Finds or creates a global Volume and enables Bloom on its profile.
    /// </summary>
    private void SetupBloom()
    {
        Volume volume = FindAnyObjectByType<Volume>();
        if (volume == null)
        {
            GameObject volumeGO = new GameObject("PostProcessingVolume");
            volume = volumeGO.AddComponent<Volume>();
            volume.isGlobal = true;
        }

        if (volume.profile == null)
            volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();

        if (!volume.profile.TryGet(out Bloom bloom))
            bloom = volume.profile.Add<Bloom>(true);

        bloom.active = true;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 0.5f;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 0.9f;
        bloom.highQualityFiltering.overrideState = true;
        bloom.highQualityFiltering.value = true;
    }

    /// <summary>
    /// Builds a sphere mesh with reversed winding order and inward-facing normals
    /// so the surface is visible and lit from inside.
    /// </summary>
    private static Mesh CreateInvertedSphereMesh(float radius, int latSegments, int lonSegments)
    {
        Mesh mesh = new Mesh();
        mesh.name = "InvertedSphere";

        int vertCount = (latSegments + 1) * (lonSegments + 1);
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals  = new Vector3[vertCount];
        Vector2[] uvs      = new Vector2[vertCount];

        int idx = 0;
        for (int lat = 0; lat <= latSegments; lat++)
        {
            float theta    = lat * Mathf.PI / latSegments;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float phi = lon * 2f * Mathf.PI / lonSegments;
                Vector3 dir = new Vector3(
                    Mathf.Cos(phi) * sinTheta,
                    cosTheta,
                    Mathf.Sin(phi) * sinTheta
                );
                vertices[idx] = dir * radius;
                normals[idx]  = -dir; // Inward-facing normals
                uvs[idx]      = new Vector2((float)lon / lonSegments, (float)lat / latSegments);
                idx++;
            }
        }

        // Reversed winding order so faces are visible from inside.
        int[] triangles = new int[latSegments * lonSegments * 6];
        int t = 0;
        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int cur = lat * (lonSegments + 1) + lon;
                int nxt = cur + lonSegments + 1;

                triangles[t++] = cur;
                triangles[t++] = cur + 1;
                triangles[t++] = nxt;

                triangles[t++] = nxt;
                triangles[t++] = cur + 1;
                triangles[t++] = nxt + 1;
            }
        }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.uv        = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        return mesh;
    }
}
