using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Village : Settlement
{
    [SerializeField] private Image image;

    private const            float      MaxHealth = 100.0f;
    private                  float      health;
    [SerializeField] private GameObject guard, civilian, addHealth;
    [SerializeField] private TMP_Text   populationInfo;

    private void Awake()
    {
        health = MaxHealth;
    }

    public void OnHit(float damage)
    {
        health           -= damage;
        image.fillAmount =  health / MaxHealth;
        if (health <= 0.0f)
        {
            var rnd = Random.Range(1, 5);
            if (rnd == 3) { Instantiate(addHealth, transform.position + transform.up, transform.rotation); }
            PlanetManager.Instance.OnCityDestroyed(population);
            Destroy(gameObject);
        }

        SpawnGuards();
    }

    private void SpawnGuards()
    {
        int rnd = Random.Range(1, 15);
        if (rnd == 2 || rnd == 3) SpawnMember(rnd);

        void SpawnMember(int i)
        {
            GameObject x  = null;
            if (i == 2) x = civilian;
            if (i == 3) x = guard;

            var g = Instantiate(x, transform.position + transform.up, Quaternion.identity);
            if (i == 2)
            {
                currentCivilian -= 1;
                var residentMovement = g.GetComponent<CivilianMovement>();
                residentMovement.target = PlanetManager.Instance.player.transform;
                residentMovement.planet = PlanetManager.Instance.planet.transform;
            }

            if (i == 3)
            {
                currentGuards -= 1;
                var residentMovement = g.GetComponent<GuardMovement>();
                residentMovement.target = PlanetManager.Instance.player.transform;
                residentMovement.planet = PlanetManager.Instance.planet.transform;
            }
        }
    }

    private void Update()
    {
        if (Math.Abs(health) < 0.1f) return;
        population          = currentCivilian                      + currentGuards;
        populationInfo.text = "Population : " + population + " / " + maxPopulation;
        if (population <= 0) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.ScoreAdd(500);
        ScoreManager.Instance.totalSettlementsDestroyed++;
        if (PlanetManager.Instance.cityManagers.Contains(this))
            PlanetManager.Instance.cityManagers.Remove(this);
    }
}