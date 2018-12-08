using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject planet;
    [SerializeField] private float      speed = 5.0f, sprintSpeed = 10.0f;

    public  Rigidbody rb;
    private float     h, v;
    private Vector3   moveDirection;

    private void Awake()
    {
        Physics.gravity = Vector3.zero;
        rb              = GetComponent<Rigidbody>();
    }


    public void Regen(int i)
    {
        StartCoroutine(RegenerateHealth(i));
    }

    public IEnumerator RegenerateHealth(int i)
    {
        while (health <= 100.0f && i > 0)
        {
            i--;
            health++;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Start()
    {
        planet  = PlanetManager.Instance.planet;
        health  = maxHealth;
        stamina = maxStamina;
    }

    public void Initialize()
    {
        healthImage.fillAmount  = health / maxHealth;
        healthAmount.text       = $"{health} / {maxHealth}";
        staminaImage.fillAmount = stamina / maxStamina;
        staminaAmount.text      = $"{stamina} / {maxStamina}";
    }

    public  float health    = 100;
    private float maxHealth = 100;
    public float stamina   = 100.0f;
    private float maxStamina = 100.0f;

    public Image    healthImage,  staminaImage;
    public TMP_Text healthAmount, staminaAmount;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<Enemies>().destroying) return;
            health -= 5;
        }
    }

    [SerializeField] private bool sprint;

    private void Update()
    {
        if (NewGame.Instance.GameState != GameState.NewGame) return;
        healthImage.fillAmount = health / maxHealth;
        healthAmount.text      = $"{health} / {maxHealth}";
        if (health <= 0.0f)
        {
            NewGame.Instance.GameState = GameState.End;
            ScoreManager.Instance.Summary("You Died!");
        }

        float x = Input.GetAxis("Mouse X") * 2.0f;
        transform.Rotate(0, x, 0);
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        moveDirection = new Vector3(h, 0, v).normalized;
        if (!Input.GetKey(KeyCode.LeftShift) && stamina < maxStamina) stamina += 5.0f * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0.0f)
        {
            stamina -= 20.0f * Time.deltaTime;
            sprint  =  stamina > 0.0f;
        }

        staminaImage.fillAmount = stamina / maxStamina;
        staminaAmount.text      = $"{(int) stamina} / {maxStamina}";
        Vector3    up  = (transform.position - planet.transform.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(rb.transform.up, up) * rb.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10 * Time.deltaTime);
        if (Input.GetKeyUp(KeyCode.LeftShift)) sprint = false;
    }

    private void FixedUpdate()
    {
        var eUp = (rb.position - planet.transform.position).normalized;
        rb.AddForce(eUp * -9.81f);
        if (sprint) { rb.MovePosition(rb.position + transform.TransformDirection(moveDirection) * sprintSpeed * Time.deltaTime); }
        else { rb.MovePosition(rb.position        + transform.TransformDirection(moveDirection) * speed       * Time.deltaTime); }
    }
}