using System.Collections;
using UnityEngine;

public class AddHealth : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Regen(Random.Range(25, 50));
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, 50.0f * Time.deltaTime);
    }
}