using UnityEngine;

public class CivilianMovement : Enemies
{
    protected override void Start()
    {
        PlanetManager.Instance.civilianMovements.Add(this);
        base.Start();
    }

    protected override void OnDestroy()
    {
        PlanetManager.Instance.civilianMovements.Remove(this);
        base.OnDestroy();
    }

    protected override void Update()
    {
        base.Update();
        if (destroying) return;
        transform.up = transform.position - planet.position;
        var dir = (rb.position - target.position).normalized;
        rb.MovePosition(rb.position + dir * 3 * Time.deltaTime);
    }
}