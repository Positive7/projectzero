using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    [SerializeField] private Transform              player;
    [SerializeField] private float                  gravity = -9.81f;

    private void FixedUpdate()
    {
        Vector3 up = (player.position - transform.position).normalized;
        player.GetComponent<Rigidbody>().AddForce(up * gravity);
    }
}