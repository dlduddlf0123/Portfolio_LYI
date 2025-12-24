using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
public class PhysicsProjectile : MonoBehaviour
{
    public Transform Target;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public Transform Projectile;

    public UnityAction onCurveEnd = null;
    public void StartCurve(Transform target, Vector3 end)
    {
        StartCoroutine(SimulateProjectile(target,end));
    }

    public IEnumerator SimulateProjectile(Transform target, Vector3 end)
    {
        // Calculate distance to target
        float target_Distance = Vector3.Distance(target.position, end);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        target.rotation = Quaternion.LookRotation(end - target.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            target.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
        target.transform.position = end;

        if (onCurveEnd != null)
        {
            onCurveEnd.Invoke();
        }
    }
}

