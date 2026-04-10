using UnityEngine;

public class SimpleOrbit : MonoBehaviour
{
    public Transform target; 
    public float orbitSpeed = 20f;
    public Vector3 orbitAxis = Vector3.up;

    void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }
    }
}