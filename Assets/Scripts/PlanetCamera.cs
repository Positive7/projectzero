using UnityEngine;

public class PlanetCamera : MonoBehaviour
{
    private void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, 0.15f);
    }
}