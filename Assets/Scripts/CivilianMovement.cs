using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CivilianMovement : MonoBehaviour
{
    public Transform target, planet;

    public                   Rigidbody rb;
    private                  float     maxHealth = 100.0f;
    private                  float     health;
    [SerializeField] private Image     image;

    private void Awake()
    {
        rb     = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    private void Start()
    {
        planet = PlanetManager.Instance.planet.transform;
        target = PlanetManager.Instance.player.transform;
        PlanetManager.Instance.civilianMovements.Add(this);
    }

    public void OnHit(float damage)
    {
        health           -= damage;
        image.fillAmount =  health / maxHealth;
        if (health <= 0.0f) { Destroy(gameObject); }
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.totalKills++;
        PlanetManager.Instance.civilianMovements.Remove(this);
        if (PlanetManager.Instance.population.Count > 0)
            PlanetManager.Instance.population.RemoveAt(PlanetManager.Instance.population.LastOrDefault());
    }

    private void Update()
    {
        if (NewGame.Instance.GameState == GameState.MainMenu) { Destroy(gameObject); }

        transform.up = transform.position - planet.position;
        var dir = (rb.position - target.position).normalized;
        rb.MovePosition(rb.position + dir * 3 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        var eUp = (rb.position - planet.transform.position).normalized;
        rb.AddForce(eUp * -9.81f);
    }
}