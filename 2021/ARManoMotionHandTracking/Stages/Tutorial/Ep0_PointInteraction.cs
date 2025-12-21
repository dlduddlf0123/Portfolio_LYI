using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ep0_PointInteraction : InteractionManager
{
    Collider m_collider;
    RayInteractObject m_ray;

    [SerializeField]
    bool isPoked = false;

    public int pushCount = 0;
    public float pushPower = 100f;
    Vector3 startVector;
    public ParticleSystem guide;

    Coroutine currentCoroutine = null;

    protected override void DoAwake()
    {
        m_collider = GetComponent<Collider>();
        m_ray = GetComponent<RayInteractObject>();

        m_collider.enabled = false;

        list_guidePosition.Add(stageMgr.arr_header[0].transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize);

        e_handIcon = HandIcon.INDEX;
    }

    private void Start()
    {
        m_ray.m_RayEvent.AddListener(() => EnterEvent());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 &&
            other.gameObject.CompareTag("Index")&&
            !gameMgr.handCtrl.manoHandMove.isPinch &&
            !isPoked)
        {
            EnterEvent();
        }
    }

    void EnterEvent()
    {
        if (m_ray.rayOriginTag != "Index")
        {
            return;
        }

        pushCount++;

        gameMgr.PlayParticleEffect(transform.position, ReadOnly.Defines.PREFAB_EFFECT_TOUCH);
        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_CLICK);

        Vector3 pushVector = stageMgr.arr_header[0].transform.position - gameMgr.handCtrl.manoHandMove.arr_handFollwer[1].transform.position;
        stageMgr.arr_header[0].GetComponent<Rigidbody>().AddForce(new Vector3(pushVector.x, 0, pushVector.z).normalized * pushPower);
        //isPoked = true;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(gameMgr.LateFunc(() => {
            stageMgr.arr_header[0].transform.position = startVector;
            stageMgr.arr_header[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
            currentCoroutine = null;
        }, 3f));

        if (pushCount > 3)
        {
            EndInteraction();
        }
    }

    IEnumerator FollowHeader()
    {
        while (gameObject.activeSelf)
        {
            transform.position = stageMgr.arr_header[0].transform.position;
            yield return new WaitForSeconds(0.01f);
        }
    }


    public override void StartInteraction()
    {
        m_collider.enabled = true;
        stageMgr.arr_header[0].GetComponent<Kanto>().isTouchable = false;
        startVector = stageMgr.arr_header[0].transform.position;

        StartCoroutine(FollowHeader());

        base.StartInteraction();


        PlayGuideParticle();
        list_guideParticle[0].transform.SetParent(stageMgr.arr_header[0].transform);
    }

    public override void EndInteraction()
    {
        m_collider.enabled = false;


        base.EndInteraction();

        list_guideParticle[0].transform.parent = episodeMgr.particlePool;
        StopGuideParticle();
    }

}
