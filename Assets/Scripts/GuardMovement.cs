using UnityEngine;

public class GuardMovement : Enemies
{
    protected override void Update()
    {
        base.Update();
        if (destroying) return;
        /*var lookPos = target.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);*/
        Vector3    up  = (transform.position - planet.transform.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(transform.up, up) * rb.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10 * Time.deltaTime);


        var dir = (target.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * 3 * Time.deltaTime);
    }
}