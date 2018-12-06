using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3    dir;
    public GameObject target;

    private void OnEnable()
    {
        time = 2.0f;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.transform.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<ResidentMovement>())
                other.gameObject.GetComponent<ResidentMovement>().OnHit(10);
            if (other.gameObject.GetComponent<CivilianMovement>())
                other.gameObject.GetComponent<CivilianMovement>().OnHit(50);
            gameObject.SetActive(false);
        }

        if (other.gameObject.GetComponent<Village>())
        {
            other.gameObject.GetComponent<Village>().OnHit(5);
            gameObject.SetActive(false);
        }

        if (other.gameObject.GetComponent<City>())
        {
            other.gameObject.GetComponent<City>().OnHit(5);
            gameObject.SetActive(false);
        }

        if (other.transform.CompareTag("Environment")) gameObject.SetActive(false);
    }

    private float time = 2.0f;

    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0.0f) gameObject.SetActive(false);
        transform.RotateAround(target.transform.position, dir, 50 * Time.deltaTime);
    }
}