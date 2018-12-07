using UnityEngine;

public class GuardMovement : Enemies
{
    protected override void Update()
    {
        if (destroying) return;
        transform.up = transform.position - planet.position;
        var dir = (target.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * 3 * Time.deltaTime);
    }
}