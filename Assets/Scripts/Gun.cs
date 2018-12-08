using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform  spawnBullets;
    [SerializeField] private GameObject planet;

    private AudioSource audioSource;
    private float       nextFire;

    private List<GameObject> bullets = new List<GameObject>();

    private void Start()
    {
        planet      = PlanetManager.Instance.planet;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource == null) return;
        if (CanvasManager.Instance == null) return;
        audioSource.volume = CanvasManager.Instance.gunEffect;
    }

    private GameObject GetPooled()
    {
        if (bullets.Count < 30)
        {
            var b = Instantiate(bullet, planet.transform);
            b.transform.localScale /= 400.0f;
            b.name                 =  $"Bullet{bullets.Count + 1}";
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

    private void Update()
    {
        if (NewGame.Instance.GameState != GameState.NewGame) return;
        if (!Input.GetMouseButton(0)) return;
        if (Time.time < nextFire || GetPooled() == null) return;
        var b = GetPooled();
        nextFire = Time.time + 0.2f;
        if (!b.activeSelf)
        {
            var dir = spawnBullets.forward;
            b.transform.position            = spawnBullets.position;
            b.transform.rotation            = spawnBullets.rotation;
            b.GetComponent<Bullet>().dir    = dir;
            b.GetComponent<Bullet>().target = planet;
            b.SetActive(true);
        }

        audioSource.Play();
    }
}