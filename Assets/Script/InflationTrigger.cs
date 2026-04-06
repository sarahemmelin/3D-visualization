using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 1. ADD THIS

public class InflationTrigger : MonoBehaviour
{
    [Header("Cosmic Elements")]
    public GameObject starPrefab;
    public int starCount = 300;
    public float inflationForce = 150f;

    [Header("UI Reference")]
    public CanvasGroup flashGroup;
    public float fadeSpeed = 0.5f;

    private bool hasBigBangOccurred = false;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !hasBigBangOccurred)
        {
            StartCoroutine(ExecuteBigBang());
        }
    }

    IEnumerator ExecuteBigBang()
    {
        hasBigBangOccurred = true;
        flashGroup.alpha = 1f;

        for (int i = 0; i < starCount; i++)
        {
            Vector3 spawnOffset = Random.insideUnitSphere * 2.0f;
            // Use transform.position to ensure stars center on this manager
            GameObject star = Instantiate(starPrefab, transform.position + spawnOffset, Quaternion.identity);

            Rigidbody rb = star.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = spawnOffset.normalized;
                rb.AddForce(direction * inflationForce, ForceMode.Impulse);
            }
        }

        while (flashGroup.alpha > 0)
        {
            flashGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}