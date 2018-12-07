using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Sabotage : MonoBehaviour
{
    [SerializeField] private GameObject sabotage;
    [SerializeField] private Image      sabotageFill;
    [SerializeField] private GameObject guard;

    public bool planted;

    private float          time;
    private bool           playerInside;
    private WaitForSeconds waitTime = new WaitForSeconds(1.0f);
    private City newCity;

    private void Start()
    {
        newCity = GetComponentInParent<City>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { playerInside = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { playerInside = false; }
    }

    private void Update()
    {
        if (playerInside)
        {
            if (planted)
            {
                sabotage.SetActive(false);
                return;
            }

            sabotage.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                time                    += Time.deltaTime;
                sabotageFill.fillAmount =  time / 2.5f;
                if (time >= 2.5f && !planted)
                {
                    planted                                                  = true;
                    transform.parent.gameObject.GetComponent<Bomb>().enabled = true;
                    newCity.OnHit(100.0f);
                    if (newCity.currentGuards > 0) StartCoroutine(SpawnGuards());
                }
            }
            else
            {
                time                    = 0.0f;
                sabotageFill.fillAmount = 0.0f;
            }
        }
        else { sabotage.SetActive(false); }
    }


    private IEnumerator SpawnGuards()
    {
        for (int i = 0; i < Random.Range(3, 5); i++)
        {
            newCity.currentGuards -= 1;
            var g                = Instantiate(guard, transform.position + transform.up, Quaternion.identity);
            var residentMovement = g.GetComponent<GuardMovement>();
            residentMovement.target = PlanetManager.Instance.player.transform;
            residentMovement.planet = PlanetManager.Instance.planet.transform;
            yield return waitTime;
        }

        yield return null;
    }
}