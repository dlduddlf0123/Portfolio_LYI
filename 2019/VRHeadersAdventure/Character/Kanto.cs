using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ReadOnly;

public class Kanto : Character
{
    //  public AnimationCurve curve = new AnimationCurve();
    protected override void DoAwake()
    {
        StatusInit();

        statAnim = AnimState.IDLE;
        currentTarget = null;

        //Navigations
        mNavAgent.speed = 0;
        mNavAgent.enabled = true;
        mNavAgent.autoTraverseOffMeshLink = false;
        
    }


    void Start()
    {
        SetPlayerGameMode();

        StatusInit();

        list___dialog_kor.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.KANTO_HATE_DIALOG_KOR));
        list___dialog_kor.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.KANTO_NORMAL_DIALOG_KOR));
        list___dialog_kor.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.KANTO_FRIEND_DIALOG_KOR));
        list___dialog_eng.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.KANTO_HATE_DIALOG_ENG));
        list___dialog_eng.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.KANTO_NORMAL_DIALOG_ENG));
        list___dialog_eng.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.KANTO_FRIEND_DIALOG_ENG));

        ChangeLikeState();

        if (isCage == true)
        {
            mNavAgent.enabled = false;
        }
        else
        {
            //AI_Move(0);
        }
    }

    protected override void StatusInit()
    {
        Debug.Log(this.gameObject.name + "Init");
        Status.header = Headers.KANTO;
        Status.likeState = (LikeState)PlayerPrefs.GetInt("KantoLikeState", 0);//호감도 로드할 것
        Status.likeMeter = PlayerPrefs.GetInt("KantoLikeMeter", 0);

        Status.moveSpeed = 2f;
        Status.minSpeed = 0.1f;
        Status.maxSpeed = 4f;
        Status.specialCoolTime = 0.5f;

        Status.maxHp = 5;
        Status.hp = Status.maxHp;


        select.SetActive(false);
        isSelect = false;

        isDie = false;
        isHit = false;
        isGround = true;
    }

    public override void Stop()
    {
        isHit = false;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        mNavAgent.speed = 0;
        StatusInit();
    }
    
    /// <summary>
    /// Kanto Jump
    /// </summary>
    public override void SpecialMove()
    {
        //headerCtrl.StartCoroutine(headerCtrl.Dash());
    }

    /// <summary>
    /// 레이저 커서 위치로 이동
    /// </summary>
    /// <param name="_hitPos">racast hit</param>
    /// <param name="_hitObj">hit Object</param>
    /// <returns></returns>
    public override IEnumerator MoveToTarget(Vector3 _hitPos, GameObject _hitObj)
    {
        statAnim = AnimState.RUN;
        SetAnim();

        mNavAgent.destination = _hitPos;
        mNavAgent.isStopped = false;
        mNavAgent.speed = Status.maxSpeed;
        float dist = 0.01f;
        if (_hitObj.CompareTag("Player"))
        {
            dist = 1.5f;
        }
        yield return new WaitForSeconds(0.1f);

        while (isHit == false
               && isDie == false
               && mNavAgent.remainingDistance > dist)
        {
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.maxSpeed * Time.deltaTime);
            if (mNavAgent.isOnOffMeshLink)
            {
                yield return StartCoroutine(Parabola(mNavAgent));
                mNavAgent.CompleteOffMeshLink();
                SetAnim();
            }
            yield return new WaitForSeconds(0.0167f);
        }

        mNavAgent.speed = Status.moveSpeed;

        if (_hitObj.CompareTag("Item"))
        {
            mouthColl.DetachMouth();
            //집어드는 애니메이션 넣기
            mouthColl.AttachMouth(_hitObj);
        }

        AI_Move(3);
        yield return new WaitForSeconds(0.5f);
        if (_hitObj.CompareTag("Player"))
        {
            mouthColl.DetachMouth();
        }
    }

    /// <summary>
    /// 오프메시링크에서 뛰는 기능
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    IEnumerator Parabola(NavMeshAgent agent)
    {
        isGround = false;
        mAnimator.SetBool("isGround", false);
        mAnimator.SetTrigger("isJump");
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        Debug.Log(Vector3.Distance(data.startPos, data.endPos));
        float height = Vector3.Distance(data.startPos, data.endPos);
        float duration = height * 0.2f;
        float normalizedTime = 0.0f;

        SetAnim(4);
        isSpecial = true;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(endPos), Status.maxSpeed * Time.deltaTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        isGround = true;
        mAnimator.SetBool("isGround", true);
        SetAnim(5);
        isSpecial = false;
    }

    /// <summary>
    /// 호감도 상태 변경, 각 캐릭터에 따른 대사 설정
    /// </summary>
    protected override void ChangeLikeState()
    {
        LikeState _before = statLike;

        base.ChangeLikeState();

        int _random = Random.Range(0, 2);
        switch (gameMgr.language)
        {
            case 0:
                headerCanvas.list__currentDialog = list___dialog_kor[(int)statLike];
                break;
            case 1:
                headerCanvas.list__currentDialog = list___dialog_eng[(int)statLike];
                break;
            default:
                break;
        }

        switch (statLike)   //호감도 상승, 감소할 때 대사
        {
            case LikeState.HATE:
                headerCanvas.ShowText(4, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_hate") as RuntimeAnimatorController;
                break;
            case LikeState.NORMAL:
                headerCanvas.ShowText((_before < statLike) ? 3 : 4, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_normal") as RuntimeAnimatorController;
                break;
            case LikeState.FRIEND:
                headerCanvas.ShowText(3, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_friend") as RuntimeAnimatorController;
                break;
        }
        //soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip("Sounds/SFX/level_up"));
        //gameMgr.uiMgr.SetCommandUI((int)statLike);  //커맨드 UI 변경

        Debug.Log(gameObject.name + " Like =" + statLike);
    }


}
