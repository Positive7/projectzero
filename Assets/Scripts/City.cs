using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class City : Settlement
{
    [SerializeField] private GameObject guard, civilian, addHealth;
    [SerializeField] private Image      image;

    [SerializeField] private TMP_Text populationInfo;

    private const float MaxHealth = 10.0f;
    public        float health;

    private void Awake()
    {
        health = MaxHealth;
    }

    protected override void Start()
    {
        base.Start();
        populationInfo.text = $"Population : {population} / {maxPopulation}";
    }

    public void OnHit(float damage)
    {
        health           -= damage;
        image.fillAmount =  health / MaxHealth;

        if (health <= 0.0f)
        {
            var rnd = Random.Range(1, 5);
            if (rnd == 3) { Instantiate(addHealth, transform.position + transform.up, transform.rotation); }

            Destroy(gameObject);
        }

        SpawnGuards();
    }

    private void SpawnGuards()
    {
        int rnd = Random.Range(1, 10);
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
        PlanetManager.Instance.OnCityDestroyed(population);
    }
}