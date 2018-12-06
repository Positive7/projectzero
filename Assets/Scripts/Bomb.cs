using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject countDown;
    [SerializeField] private Image      bombImage;
    [SerializeField] private TMP_Text   bombText;
    public                   Sabotage   sabotage;
    private                  float      time = 3.0f;
    private                  bool       started;

    [SerializeField] private GameObject explosion;

    private City city;

    private void Awake()
    {
        enabled = false;
        city    = GetComponent<City>();
    }

    private void OnEnable()
    {
        countDown.SetActive(true);
    }

    private void OnDisable()
    {
        if (sabotage != null) sabotage.planted = false;
    }

    private void Update()
    {
        if (time > 0.0f)
        {
            time                 -= Time.deltaTime;
            bombImage.fillAmount =  time / 3;
            bombText.text        =  ((int) time).ToString();
        }

        if (time <= 0.0f && !started)
        {
            started = true;
            StartCoroutine(Kill());
        }
    }

    private IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.5f);
        countDown.SetActive(false);
        var rnd = Random.Range(20, 30);
        for (int i = 0; i < rnd; i++)
        {
            Instantiate(explosion, transform.position + Random.insideUnitSphere * 2, Quaternion.identity);
            city.currentCivilian -= 1;
            PlanetManager.Instance.population.Remove(PlanetManager.Instance.population.LastOrDefault());
            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
        time    = 3.0f;
        started = false;
        enabled = false;
        yield return null;
    }
}