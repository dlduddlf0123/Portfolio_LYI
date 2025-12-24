using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ReadOnly;

public class Ginora : Character
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
        Status.header = Headers.GINORA;
        Status.likeState = (LikeState)PlayerPrefs.GetInt("GinoraLikeState", 0);//호감도 로드할 것
        Status.likeMeter = PlayerPrefs.GetInt("GinoraLikeMeter", 0);

        Status.moveSpeed = 2f;
        Status.minSpeed = 0.1f;
        Status.maxSpeed = 4f;
        Status.specialCoolTime = 1f;

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

    private void OnCollisionEnter(Collision coll)
    {

    }

    private void OnCollisionExit(Collision collision)
    {
    }

    private void OnTriggerEnter(Collider coll)
    {

    }

    ////충돌체크에서 나갈 때
    //private void OnTriggerExit(Collider coll)
    //{
    //    if (coll.CompareTag("Ground"))
    //    {
    //        isGround = false;
    //        mAnimator.SetBool("isGround", false);
    //    }

    //}


    /// <summary>
    /// Ginora Dash
    /// </summary>
    public override void SpecialMove()
    {
        headerCtrl.StartCoroutine(headerCtrl.Dash());
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
