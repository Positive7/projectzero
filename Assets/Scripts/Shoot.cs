using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private                  float            nextFire;
    private                  List<GameObject> bullets = new List<GameObject>();
    [SerializeField] private GameObject       bullet;
    [SerializeField] private GameObject       planet;

    private void Start()
    {
        planet = PlanetManager.Instance.planet;
    }

    private void Update()
    {
        if (NewGame.Instance.GameState != GameState.NewGame) return;
        if (Time.time < nextFire || GetPooled() == null) return;
        var b = GetPooled();
        nextFire = Time.time + 2.0f;
        if (!b.activeSelf)
        {
            b.transform.position            = transform.position;
            b.transform.rotation            = transform.rotation;
            b.SetActive(true);
        }
    }

    private GameObject GetPooled()
    {
        if (bullets.Count < 30)
        {
            var b = Instantiate(bullet, null);
            b.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
            b.name                 =  $"EnemyBullet{bullets.Count + 1}";
            b.SetActive(false);
            bullets.Add(b);
            return b;
        }

        foreach (var b in bullets)
        {
            if (!b.activeInHierarchy) { return b; }
        }

        return null;
    }
}