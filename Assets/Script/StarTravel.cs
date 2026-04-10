using UnityEngine;

public class StarTravel : MonoBehaviour
{
    public Transform[] pathPoints;
    public float travelSpeed = 5f;
    public bool isMoving = false;

    [Header("Wobble Settings")]
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 5f;

    private int currentPointIndex = 0;
    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        if (isMoving && pathPoints.Length > 0)
        {
            MoveAlongPath();
        }
    }

    void MoveAlongPath()
    {
        Vector3 targetPos = pathPoints[currentPointIndex].position;
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position += direction * travelSpeed * Time.deltaTime;

        float yOffset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        transform.position += Vector3.up * yOffset * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 1f)
        {
            if (currentPointIndex < pathPoints.Length - 1)
                currentPointIndex++;
            else
                isMoving = false;
        }
    }
}