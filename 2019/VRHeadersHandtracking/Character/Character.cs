using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


#region States
public enum AnimState
{
    IDLE = 0,
    PATROL,
    HIT,
    RUN,
    HAPPY,
    CLEAN,
};

//대가리 번호
public enum Headers
{
    NONE = 0,
    KANTO = 1,
    ZENORA = 2,
    ENEMY = 9,
}


public enum LikeState
{
    HATE = 0,
    //ALERT,
    NORMAL,
    //GOOD,
    FRIEND,
}

#endregion

/// <summary>
/// 캐릭터 스텟을 가지는 클래스
/// </summary>
public class CharacterAttribute
{
    // 대가리 캐릭터
    public Headers header { get; set; }
    public LikeState likeState { get; set; } //호감도 상태(좋다,싫다)

    public float likeMeter { get; set; }        //호감도(%)
    public float hunger { get; set; }       //배고픔(호감도에 영향)
    public int favoriteFoodNum { get; set; }    //가장 좋아하는 음식
    public int hateFoodNum { get; set; }    //가장 싫어하는 음식
    public int likeTaste { get; set; }      //좋아하는 맛
    public int hateTaste { get; set; }      //싫어하는 맛

    public float findRange { get; set; }       // 타겟 검색범위
    public int maxHp { get; set; }              // 최대 HP
    public int hp { get; set; }                    // HP
    public float moveSpeed { get; set; }    // 이동속도
    public float minSpeed { get; set; }
    public float maxSpeed { get; set; }
}


public class Character : MonoBehaviour
{
    //외부 클래스
    public GameManager gameMgr { get; set; }
    protected SoundManager soundMgr;
    public HeaderCanvas headerCanvas;
    public CharacterAttribute Status { get; set; }

    //만지기
    public GameObject bodyColls;    //코,등,꼬리
    public MouthColl mouthColl;     //입
    public SkinnedMeshRenderer[] mSkins;

    public Transform currentTarget;
    public Transform movePoint;
    Transform[] movePoints;
    Transform mainCam;

    //enum
    public AnimState statAnim;  //작동중인 애니메이션 상태
    public LikeState statLike;      //호감도 상태

    //내부 클래스들
    public Animator mAnimator { get; set; }
    public Rigidbody mRigidbody { get; set; }
    public BoxCollider mBoxCollider { get; set; }
    public NavMeshAgent mNavAgent { get; set; }
    public Coroutine currentAI = null;

    //내부 변수
    //캐릭터 대사
    public List<List<object>> list__FriendDialog { get; set; }  //2차원배열이라 언더바 2번?
    public List<List<object>> list__NormalDialog { get; set; }
    public List<List<object>> list__HateDialog { get; set; }

    public bool isAction;   //커맨드 수행중인지 여부
    public bool isDie;
    public bool isHit;

    public bool isGround;
    public bool isJudged;
    public bool isStart;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        soundMgr = gameMgr.soundMgr;

        Status = new CharacterAttribute();

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mBoxCollider = GetComponent<BoxCollider>();
        mNavAgent = GetComponent<NavMeshAgent>();

        mainCam = gameMgr.mainCam.transform;
        

        movePoints = movePoint.GetComponentsInChildren<Transform>();
        mSkins = this.GetComponentsInChildren<SkinnedMeshRenderer>();

        list__FriendDialog = new List<List<object>>();
        list__NormalDialog = new List<List<object>>();
        list__HateDialog = new List<List<object>>();

        DoAwake();
    }

    #region Virtual Funtions

    protected virtual void DoAwake() { }
    protected virtual void StatusInit() { }

    public virtual void SetAnim() { }    // bool type Anims
    public virtual void SetAnim(int _num) { }   // trigger type Anims
    public virtual void AnimationEnd() { }
    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:run/3:stay/4:call/5:jump/d:clear</param>
    public virtual void AI_Move(int _type) { }

    /// <summary>
    /// 현재 호감도에 따라 상태 변경
    /// 호감도 상태가 변경되면 애니메이션 또한 변경
    /// 애니메이션을 변경하기 위해 각 캐릭터마다 적용시킬 것
    /// </summary>
    protected virtual void ChangeLikeState() { }

    #endregion

    /// <summary>
    /// 강제로 동작 멈추기, 일부분 초기화
    /// </summary>
    public virtual void Stop()
    {
        StopAllCoroutines();
        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;
        isAction = false;
        //for (int i = 0; i < mSkins.Length; i++)
        //{
        //    mSkins[i].enabled = true;
        //}
    }

    public void MoveCharacter(Vector3 hitPoint, GameObject _targetObj)
    {
        Stop();
        isAction = true;
        currentAI = StartCoroutine(MoveToTarget(hitPoint, _targetObj));
    }

    /// <summary>
    /// 해당 방향으로 이동
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public IEnumerator MoveToTarget(Vector3 _target, GameObject _targetObj)
    {
        statAnim = AnimState.RUN;
        SetAnim();

        mNavAgent.destination = _target;
        mNavAgent.isStopped = false;
        mNavAgent.speed = Status.maxSpeed;

        float dist = 0.01f;

        if (_targetObj != null)
        {
            if (_targetObj.CompareTag("MainCamera"))
            {
                dist = 1.5f;
            }
            if (_targetObj.CompareTag("Item"))
            {
                _target = gameMgr.appleMgr.currentApple.transform.position;
            }
        }

        mNavAgent.destination = _target;
        yield return new WaitForSeconds(0.1f);

        while (isHit == false
               && isDie == false
               && mNavAgent.remainingDistance > dist)
        {
            if (_targetObj != null)
            {
                if (_targetObj.CompareTag("Item"))
                {
                    mNavAgent.destination = gameMgr.appleMgr.currentApple.transform.position;
                }
            }
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }
        mNavAgent.speed = Status.moveSpeed;
        isAction = false;

        AI_Move(3);

        if (_targetObj.CompareTag("MainCamera"))
        {
            mouthColl.DetachMouth();
        }
    }

    #region LikeMeters 호감도 관련 함수들

    /// <summary>
    /// (Interface) 호감도 상승,하락
    /// </summary>
    /// <param name="_like"></param>
    public void LikeChange(int _like)
    {
        float start = Status.likeMeter * 0.01f;
        int change = 0; //0=변화x, 1=상승, 2=하락

        Status.likeMeter += _like;

        float end = Status.likeMeter * 0.01f;

        if (end > 1)
        {
            if (statLike == LikeState.FRIEND)
            {
                end = 1;
                //호감도 MAX
                gameMgr.miniGameTable.SetActive(true);
            }
            else
                end -= 1;
        }
        else if(end < 0)
        {
            if (statLike == LikeState.HATE)
                end = 0;
            else
                end += 1;
        }

        if (Status.likeMeter > 100)
        {
            Status.likeMeter = 100f;

            if (statLike != LikeState.FRIEND)
            {
                change = 1;
                ChangeLikeState();
            }
        }
        else if (Status.likeMeter < 0)
        {
            Status.likeMeter = 0f;

            if (statLike != LikeState.HATE)
            {
                change = 2;
                ChangeLikeState();
            }
        }
        Debug.Log("친밀도:"+statLike+Status.likeMeter);
        headerCanvas.SetLikeGauge(start, end, change);
    }

    /// <summary>
    /// 호감도에 따른 확률적 동작 거절 함수
    /// </summary>
    /// <returns>true or false</returns>
    protected bool IsOrderRefuse()
    {
        int refusePercent = 50;
        switch (statLike)
        {
            case LikeState.HATE:
                refusePercent = 80;
                break;
            //case LikeState.ALERT:
            //    refusePercent = 60;
            //    break;
            case LikeState.NORMAL:
                refusePercent = 40;
                break;
            //case LikeState.GOOD:
            //    refusePercent = 20;
            //    break;
            case LikeState.FRIEND:
                refusePercent = 0;
                break;
            default:
                break;
        }

        return (refusePercent < Random.Range(0, 100))? true : false;
    }


    /// <summary>
    /// 음식 먹기(영양가를 전달해서 취향을 파악, 호감도, 배고픔에 영향을 줌)
    /// </summary>
    /// <param name="_food">음식 오브젝트</param>
    public IEnumerator EatFood(Food _food)
    {
        bodyColls.SetActive(false);
        mouthColl.gameObject.SetActive(false);
        CheckFood(_food.foodNum, _food.satiety, _food.taste);
        //먹는소리
        soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip("Sounds/SFX/eat_01"));
        _food.FoodDetach();
        _food.gameObject.SetActive(false);  //음식소멸 or 뱃속으로 이동해서 소화시키기??

        mNavAgent.isStopped = true;
        //뭔가 먹는 모션
        AI_Move(3);
        yield return new WaitForSeconds(1f);
        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
    }

    /// <summary>
    /// 음식 취향 확인, 호감도 상승!
    /// </summary>
    /// <param name="_foodNum">음식 종류</param>
    /// <param name="_satistie">포만도</param>
    /// <param name="_taste">맛</param>
    public void CheckFood(int _foodNum, int _satistie, int _taste)
    {
        if (statLike == LikeState.HATE)
        {
            LikeChange(110);
        }
        else if (_foodNum == Status.favoriteFoodNum)
        {
            //가장 좋아하는 음식이면
            Status.hunger += _satistie * 3f;
            LikeChange(30);    //호감도 상승!
        }
    }
    #endregion

    #region VR Hand Motion Settings
    /// <summary>
    /// 손동작 변경 검지
    /// </summary>
    public void SetSecondFinger(HandInteract _hand)
    {
        if (statAnim != AnimState.RUN &&
            statLike != LikeState.HATE)
        {
            //if (!IsOrderRefuse())
            //{
            //    //명령 거절 동작
            //    Stop();
            //    AI_Move(3);
            //  Debug.Log("명령 거절됨!");
            //    return;
            //}
            gameMgr.HandEffect(_hand);
            Stop();
            isAction = true;
            AI_Move(2);
        }
    }
    //오른손 주먹
    public void SetRock(HandInteract _hand)
    {
        if (statAnim != AnimState.HAPPY &&
            statLike != LikeState.HATE)
        {
            gameMgr.HandEffect(_hand);
            Stop();
            isAction = true;
            AI_Move(4);
            //MoveCharacter(mainCam.transform.position, mainCam.gameObject);
        }
    }
    //오른손 보자기
    public void SetPaper(HandInteract _hand)
    {
        if (!isAction|| statLike == LikeState.HATE)
        {
            return;
        }
        gameMgr.HandEffect(_hand);
        Stop();
        isAction = false;
        AI_Move(3);
    }

    //양손 주먹
    public void SetDoubleRock()
    {
        Stop();
        isAction = true;
        MoveCharacter(gameMgr.appleMgr.currentApple.transform.position, gameMgr.appleMgr.currentApple);
    }

    //양손 보자기
    public void SetDoublePaper()
    {

    }
    #endregion

    #region Normal AI Moves

    //----------------------Coroutines-------------------------
    /// <summary>
    /// AI: 00
    /// 걸으면서 돌아다니기
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PatrolMove()
    {
        statAnim = AnimState.PATROL;
        SetAnim();

        int randPoint = Random.Range(0, movePoints.Length); //향할 맵 포인트
        float randDist = Random.Range(1f, 5f); //갈 거리

        Transform target = movePoints[randPoint];
        if (target == currentTarget)
        {
            randPoint = Random.Range(0, movePoints.Length);
            target = movePoints[randPoint];
        }
        currentTarget = target;

        mNavAgent.destination = currentTarget.position;
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.isStopped = false;

        yield return new WaitForSeconds(0.1f);

        while (isHit == false
               && isDie == false
               && mNavAgent.remainingDistance > randDist)
        {
            //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }
        currentAI = null;
        if (randDist < randDist*0.5f)
            AI_Move(2);
        else
            AI_Move(3);
    }


    /// <summary>
    /// AI: 01
    /// 맞았을 시 잠시 정지(히트애니메이션)
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Hit()
    {
        statAnim = AnimState.HIT;
        SetAnim();
        isHit = true;

        currentAI = null;
        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// AI: 02
    /// 달려서 레이져 쪽으로
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Run()
    {
        statAnim = AnimState.RUN;
        SetAnim();

        float randDist = Random.Range(1f, 5f);
        int randPoint = Random.Range(0, movePoints.Length); //향할 맵 포인트
        Transform target = movePoints[randPoint];
        if (target == currentTarget)
        {
            randPoint = Random.Range(0, movePoints.Length);
            target = movePoints[randPoint];
        }
        currentTarget = target;
        mNavAgent.destination = currentTarget.position;
        mNavAgent.isStopped = false;
        mNavAgent.speed = Status.moveSpeed * 5f;

        yield return new WaitForSeconds(0.1f);

        while (mNavAgent.remainingDistance > randDist)
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }

        isHit = false;
        mNavAgent.speed = Status.moveSpeed;

        currentAI = null;
        if (randDist < randDist * 0.5f)
            AI_Move(0);
        else
            AI_Move(3);
    }

    /// <summary>
    /// AI: 03
    /// 대기 동작
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Idle()
    {
        statAnim = AnimState.IDLE;
        SetAnim();

        mNavAgent.isStopped = true;
        isAction = false;

        yield return new WaitForSeconds(1f);
        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        currentAI = null;
        AI_Move(0);
    }

    /// <summary>
    /// AI: 04
    /// 카메라 쪽으로 달려옴
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Call()
    {
        currentTarget = mainCam;
        mNavAgent.destination = currentTarget.position;
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.isStopped = false;

        switch (statLike)
        {
            case LikeState.HATE:
                statAnim = AnimState.PATROL;
                SetAnim();
                float t = 0.0f;
                float rand = Random.Range(0.5f, 3.0f);
                while (mNavAgent.remainingDistance > 1f)
                {
                    if (t > rand)
                    {
                        mNavAgent.isStopped = true;
                        statAnim = AnimState.IDLE;
                        SetAnim();
                        t = 0;
                        rand = Random.Range(0.5f, 3.0f);
                        yield return new WaitForSeconds(Random.Range(0.5f,2.0f));
                    }
                    else
                    {
                        mNavAgent.isStopped = false;
                        statAnim = AnimState.PATROL;
                        SetAnim();
                        t += Time.deltaTime;
                    }
                    yield return new WaitForSeconds(0.0167f);
                }
                mNavAgent.speed = Status.moveSpeed;
                mNavAgent.isStopped = true;
                currentAI = null;
                statAnim = AnimState.IDLE;
                SetAnim();
                break;
            default:
                statAnim = AnimState.HAPPY;
                SetAnim();

                mNavAgent.speed = Status.moveSpeed * 2f;

                while (mNavAgent.remainingDistance > 2f)
                {
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
                    yield return new WaitForSeconds(0.0167f);
                }
                mNavAgent.speed = Status.moveSpeed;
                mNavAgent.isStopped = true;
                currentAI = null;
                AI_Move(3);
                break;
        }
        
    }

    /// <summary>
    /// AI: 05
    /// 카메라 쪽으로 바라봄
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Jump()
    {
        statAnim = AnimState.IDLE;
        SetAnim(3);
        yield return new WaitForSeconds(0.25f);
        SetAnim(3);
        yield return new WaitForSeconds(0.25f);
        SetAnim(3);
        //currentTarget = mainCam;
        //nvAgent.destination = currentTarget.position;
        //nvAgent.isStopped = true;

        //float t = 0f;
        //while (t < 3f)
        //{
        //    t += Time.deltaTime;
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
        //    yield return new WaitForSeconds(0.0167f);
        //}

        Debug.Log(this.gameObject.name + "AI:" + statAnim + " End");
        currentAI = null;
        AI_Move(3);
    }

    /// <summary>
    /// AI: Last(Default)
    /// 사망 시
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Clean()
    {
        isDie = true;
        statAnim = AnimState.CLEAN;
        SetAnim();
        yield return new WaitForSeconds(2.0f);
        currentAI = null;
        this.gameObject.SetActive(false);
    }
    #endregion

    #region Collider Effects 손과 상호작용
    
    public IEnumerator BodyTouched()
    {
        statAnim = AnimState.IDLE;
        bodyColls.SetActive(false);
        mouthColl.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
    }
    #endregion

}
