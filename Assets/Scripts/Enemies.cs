using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemies : MonoBehaviour
{
    public Transform target, planet;
    public Rigidbody rb;
    public Image     image;
    public int       hitWorth, destroyWorth;
    public float     damage;
    public bool      destroying;

    private float maxHealth = 100.0f;
    private float health;
    private bool  mainMenu;

    private void Awake()
    {
        rb     = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    protected virtual void Start()
    {
        planet = PlanetManager.Instance.planet.transform;
        target = PlanetManager.Instance.player.transform;
    }

    protected virtual void Update()
    {
        if (NewGame.Instance.GameState == GameState.MainMenu)
        {
            mainMenu = true;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        var eUp = (rb.position - planet.transform.position).normalized;
        rb.AddForce(eUp * -9.81f);
    }

    public void OnHit()
    {
        health           -= damage;
        image.fillAmount =  health / maxHealth;
        ScoreManager.Instance.ScoreAdd(hitWorth);
        if (health <= 0.0f)
        {
            destroying = true;
            StartCoroutine(D());
        }
    }

    private IEnumerator D()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        yield return null;
        Destroy(gameObject, 2.0f);
    }

    protected virtual void OnDestroy()
    {
        if (mainMenu) return;
        PlanetManager.Instance.Population--;
        ScoreManager.Instance.totalKills++;
        ScoreManager.Instance.ScoreAdd(destroyWorth);
    }
}