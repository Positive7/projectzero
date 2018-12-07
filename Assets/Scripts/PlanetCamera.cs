using System;
using UnityEngine;

[Obsolete("Not used any more. Use SmoothFollow component", true)]
public class PlanetCamera : MonoBehaviour
{
    private void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, 0.15f);
    }
}