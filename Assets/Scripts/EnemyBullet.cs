using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Rigidbody rb;
    private float time;

    private void Awake()
    {
        Physics.gravity = Vector3.zero;
        rb              = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        time = 2.0f;
        target = PlanetManager.Instance.player.transform;
        transform.forward = (target.position - transform.position).normalized;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().health -= 1.0f;
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0.0f) gameObject.SetActive(false);
        var eUp = (rb.position - Vector3.zero).normalized;
        rb.AddForce(eUp * -9.81f);
        rb.MovePosition(rb.position + transform.forward * 15 * Time.deltaTime);
    }
}