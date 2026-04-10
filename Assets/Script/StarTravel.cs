using UnityEngine;

public class StarTravel : MonoBehaviour
{
    public float travelSpeed = 5f;
    public bool isMoving = false;

    [Header("Wobble Settings")]
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 5f;

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.forward * travelSpeed * Time.deltaTime);
            float yOffset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            transform.position = new Vector3(transform.position.x, startY + yOffset, transform.position.z);
        }
    }
}