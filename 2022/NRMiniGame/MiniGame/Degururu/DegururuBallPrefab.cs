using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// 대가리들 공
/// </summary>
public class DegururuBallPrefab : MonoBehaviour
{
    Animator m_anim;

    public UnityAction onGoal;
    public UnityAction onFall;
    public bool isFall = false;
    bool isHand = false;

    public Vector3 endPos = Vector3.zero;

    public float firingAngle = 60.0f;
    public float gravity = 3f;

    public UnityAction onCurveEnd = null;
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        isFall = false;
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Hit"))
        {
            if (!isFall)
            {
                //데미지, 점수 다운
                onFall.Invoke();

                isFall = true;
            }
        }
        else if (coll.gameObject.CompareTag("Player"))
        {
            if (!isHand)
            {
                isHand = true;
            }

            isHand = false;

            RecieveBallAct();
        }
    }


    /// <summary>
    /// 손에 공이 닿았을 때
    /// </summary>
    void RecieveBallAct()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;

        GameManager.Instance.PlayParticleEffect(transform.position,GameManager.Instance.transform.GetChild(0).GetChild(1).gameObject);
        GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_JUMP);
        
        StartCoroutine(SimulateProjectile(this.transform, endPos));
    }

    /// <summary>
    /// 포물선 계산, 움직임
    /// </summary>
    /// <param name="target"></param>
    /// <param name="end"></param>
    /// <returns></returns>
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

        CurveEnd(this.gameObject, end);
    }

    /// <summary>
    /// 포물선 계산 이후 도착했을 때
    /// </summary>
    /// <param name="go"></param>
    void CurveEnd(GameObject go, Vector3 endPos)
    {
        go.transform.position = endPos;
        go.transform.rotation = Quaternion.Euler(Vector3.up * 180f);

        StartCoroutine(GoalAnim());
    }

    /// <summary>
    /// 바닥에 도착한 뒤
    /// </summary>
    /// <returns></returns>
    public IEnumerator GoalAnim()
    {
        m_anim.SetBool("isEmpty", true);
        m_anim.SetFloat("TriggerNum", Random.Range(0, 3));
        m_anim.SetTrigger("isTrigger");

        //사운드, 이펙트 효과
        GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);

        yield return new WaitForSeconds(m_anim.GetCurrentAnimatorClipInfo(0).Length);

        onGoal.Invoke();
    }
}
