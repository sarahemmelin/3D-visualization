using UnityEngine;

public class Ball : MonoBehaviour
{
    private Renderer ballRenderer;

    void Start()
    {
        // Cache the renderer at the start for better performance
        ballRenderer = GetComponent<Renderer>();
    }

    // Unity won't find this if it's "onCollisionEnter" (lower case 'o')
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            if (ballRenderer != null)
            {
                ballRenderer.material.color = Random.ColorHSV();
                
                // Squash the ball: thin on Y, wider on X and Z
                transform.localScale = new Vector3(1.2f, 0.7f, 1.2f);
            }
        }
    }

    private void Update()
{
    // Smoothly return to the original size (1, 1, 1) over time
    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 10f);
}
    public void OnCollisionStay(Collision collision)
    {
        
    }

    public void OnCollisionExit(Collision collision)
    {
        
    }
}
