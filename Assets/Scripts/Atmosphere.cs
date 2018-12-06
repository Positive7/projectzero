using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            PlanetManager.Instance.OnCityDestroyed(1);
            Debug.Log("To infinity and beyond!");
            Destroy(other.gameObject);
        }
    }
}