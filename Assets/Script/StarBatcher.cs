using UnityEngine;
using System.Collections.Generic;

public class StarBatcher : MonoBehaviour
{
    [SerializeField] private int batchThreshold = 100;
    private int currentWaitingStars = 0;

    private List<CombineInstance> masterCombineList = new List<CombineInstance>();

    public void RegisterStar()
    {
        currentWaitingStars++;
        if (currentWaitingStars >= batchThreshold)
        {
            CombineStars();
            currentWaitingStars = 0;
        }
    }

    [ContextMenu("Combine Background Stars")]
    public void CombineStars()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length <= 1) return;

        for (int i = 1; i < meshFilters.Length; i++)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = meshFilters[i].sharedMesh;

            ci.transform = meshFilters[i].transform.localToWorldMatrix;
            masterCombineList.Add(ci);
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(masterCombineList.ToArray());
        combinedMesh.RecalculateBounds();
        mf.mesh = combinedMesh;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null) mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = meshFilters[1].GetComponent<MeshRenderer>().sharedMaterial;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"Galaxy expanded. Total stars in mesh: {masterCombineList.Count}");
    }
}