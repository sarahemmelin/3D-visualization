using UnityEngine;

public class StarTravel : MonoBehaviour
{
    public Transform[] pathPoints;
    public float cruiseSpeed = 10f;
    public float burstSpeed = 50f;
    public bool isMoving = false;

    [Header("Wobble Settings")]
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 5f;

    [Header("Movement Polish")]
    public float acceleration = 1f;
    public float turnSpeed = 1f;
    private float currentVelocity;
    private float targetSpeed;
    private bool hasBurst = false;

    private int currentPointIndex = 0;
    private float startY;

    void Start()
    {
        startY = transform.position.y;
        targetSpeed = cruiseSpeed;
        currentVelocity = 0f;
    }

    void Update()
    {
        if (isMoving && pathPoints.Length > 0)
        {
            if (!hasBurst)
            {
                currentVelocity = burstSpeed;
                hasBurst = true;
            }

            MoveAlongPath();
        }
    }

    void MoveAlongPath()
    {
        Transform targetTransform = pathPoints[currentPointIndex];
        Vector3 targetPos = targetTransform.position;

        Vector3 directionToTarget = (targetPos - transform.position).normalized;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        currentVelocity = Mathf.Lerp(currentVelocity, targetSpeed, Time.deltaTime * acceleration);
        transform.position += transform.forward * currentVelocity * Time.deltaTime;

        float yOffset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        transform.position += Vector3.up * yOffset * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 2f)
        {
            StarWaypoint wp = targetTransform.GetComponent<StarWaypoint>();
            if (wp != null)
            {
                targetSpeed = wp.speedToNextPoint;
            }

            if (currentPointIndex < pathPoints.Length - 1)
                currentPointIndex++;
            else
                isMoving = false;
        }
    }
}