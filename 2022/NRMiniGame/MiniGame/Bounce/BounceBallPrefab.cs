using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class BounceBallPrefab : MonoBehaviour
{
    Animator m_anim;
    Rigidbody m_rigidbody;

    public UnityAction onDamage;
    public UnityAction onEnd;

    bool isHand = false;
    bool isEnd = false;
    bool isHit = false;

    public float bouncePower = 100f;
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Hit"))
        {
            if (isHit)
                return;
            isHit = true;
            //데미지, 점수 다운
            onDamage.Invoke();
            isHit = false;
        }
        if (coll.gameObject.CompareTag("Player"))
        {
            if (isHand) 
                return;
            isHand = true;

            GameManager.Instance.miniGameMgr.currentMiniGame.GetScore(10);
            m_rigidbody.AddForce(coll.gameObject.transform.forward * bouncePower);

            GameManager.Instance.PlayParticleEffect(transform.position, GameManager.Instance.transform.GetChild(0).GetChild(1).gameObject);
            GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_JUMP);

            StartCoroutine(GameManager.Instance.LateFunc(() =>
            {
                isHand = false;
            },0.01f));
        }
        if (coll.gameObject.CompareTag("End"))
        {
            if (isEnd)
                return;
            isEnd = true;

            //점수 상승, 사라지고 풀로 복귀
            GameManager.Instance.miniGameMgr.currentMiniGame.GetScore(1000);

            StartCoroutine(EndMove(coll.transform));
        }
    }
    IEnumerator EndMove(Transform endPos)
    {
        m_anim.SetBool("isEmpty", true);
        m_anim.SetFloat("TriggerNum", Random.Range(0, 3));
        m_anim.SetTrigger("isTrigger");

        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;

        transform.position = endPos.GetChild(0).position;
        transform.rotation = Quaternion.Euler(Vector3.up * 90);

        //사운드, 이펙트 효과
        GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);

        yield return new WaitForSeconds(m_anim.GetCurrentAnimatorClipInfo(0).Length);

        isEnd = false;
        onEnd.Invoke();
    }

}
