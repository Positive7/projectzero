using System;
using System.Collections;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float     distance = 3.0f;
    public float     height   = 3.0f;
    public float     damping  = 5.0f;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;

    }

    private void OnEnable()
    {
        MoveToOriginalPosition();
    }

    private Vector3 vel;

    public void MoveToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.LookAt(PlanetManager.Instance.planet.transform);
    }

    private float time = 0.2f;

    private IEnumerator @in, @out;

    private bool IsActive => target != null && target.gameObject.activeSelf;

    private void Update()
    {
        if (!IsActive) transform.RotateAround(Vector3.zero, Vector3.up, 0.15f);
    }

    private void FixedUpdate()
    {
        if (!IsActive) return;
        if (target !=  Math.Abs(target.GetComponent<PlayerController>().rb.velocity.magnitude) < 0.01f)
        {
            time -= Time.deltaTime;
            if (time <= 0.0f)

                if (@out == null)
                {
                    @out = LinearInterpolation(x => distance = x, distance, 15.0f, 0.75f);
                    StartCoroutine(@out);
                }
        }
        else
        {
            time = 0.2f;
            if (@out != null)
                StopCoroutine(@out);
            if (@in == null)
            {
                @in = LinearInterpolation(x => distance = x, distance, 7.5f, 5.0f);
                StartCoroutine(@in);
            }
        }

        Vector3 wantedPosition = target.TransformPoint(0, height, -distance);
        transform.position = Vector3.SmoothDamp(transform.position, wantedPosition, ref vel, damping * Time.deltaTime);
        //transform.position = Vector3.LinearInterpolation(transform.position, wantedPosition, Time.deltaTime * damping);

        transform.LookAt(target, target.up);
    }

    private IEnumerator LinearInterpolation(Action<float> action, float min, float max, float time)
    {
        var t = 0.0f;
        while (t < 1.0f)
        {
            action(Mathf.Lerp(min, max, t));
            t += Time.deltaTime * time;
            yield return null;
        }

        action(max);
        @in  = null;
        @out = null;
    }
}