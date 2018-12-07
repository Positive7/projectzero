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

    private Vector3 vel;

    public void MoveToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.LookAt(PlanetManager.Instance.planet.transform);
    }

    private float time = 0.2f;

    private IEnumerator @in, @out;

    private void FixedUpdate()
    {
        if (target == null || !target.gameObject.activeSelf) { transform.RotateAround(Vector3.zero, Vector3.up, 0.15f); }
        else
        {
            if (Math.Abs(target.GetComponent<PlayerController>().rb.velocity.magnitude) < 0.01f)
            {
                time -= Time.deltaTime;
                if (time <= 0.0f)

                    if (@out == null)
                    {
                        @out = Lerp(x => distance = x, distance, 15.0f, 0.75f);
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
                    @in = Lerp(x => distance = x, distance, 7.5f, 5.0f);
                    StartCoroutine(@in);
                }
            }

            Vector3 wantedPosition = target.TransformPoint(0, height, -distance);
            transform.position = Vector3.SmoothDamp(transform.position, wantedPosition, ref vel, damping * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

            transform.LookAt(target, target.up);
        }
    }

    IEnumerator Lerp(Action<float> action, float min, float max, float time)
    {
        var t = 0.0f;
        while (t < 1.0f)
        {
            action(Mathf.Lerp(min, max, t));
            t += Time.deltaTime * time;
            yield return null;
        }

        action(max);
        @in = null;
        @out = null;
    }
}