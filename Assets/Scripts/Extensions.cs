using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{

    public static IEnumerator BoingLikeInterpolation(Action<Vector3> action, Vector3 min, Vector3 max, float time)
    {
        PlanetManager.Instance.generating = true;
        var t = 0.0f;
        while (t < 1.0f)
        {
            action(Boing(min, max, t));
            t += Time.deltaTime * time;
            yield return null;
        }

        action(max);
        PlanetManager.Instance.generating = false;
    }

    public static IEnumerator BoingLikeInterpolation2(Action<Vector3> action, Vector3 min, Vector3 max, float time)
    {
        var t = 0.0f;
        while (t < 1.0f)
        {
            action(Boing(min, max, t));
            t += Time.deltaTime * time;
            yield return null;
        }

        action(max);
    }

    public static float Boing(float start, float end, float time)
    {
        time = Mathf.Clamp01(time);
        time = (Mathf.Sin(time * Mathf.PI * (0.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time)  * (1f + 1.2f * (1f - time));
        return start + (end                                                                                  - start) * time;
    }

    public static Vector3 Boing(Vector3 start, Vector3 end, float time)
    {
        return new Vector3(Boing(start.x, end.x, time), Boing(start.y, end.y, time), Boing(start.z, end.z, time));
    }
}