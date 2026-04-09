using UnityEngine;

public class StarTravel : MonoBehaviour
{
    public float travelSpeed = 5f;
    public bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            // Moves the star forward along the Z-axis
            transform.Translate(Vector3.forward * travelSpeed * Time.deltaTime);
        }
    }
}