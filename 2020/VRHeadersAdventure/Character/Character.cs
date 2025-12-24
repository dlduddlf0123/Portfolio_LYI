using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using ReadOnly;

#region States
public enum AnimState
{
    IDLE = 0,
    PATROL,
    HIT,
    RUN,
    HAPPY,
    CLEAN,
    SPECIAL,
};

//대가리 번호
public enum Headers
{
    NONE = 0,
    KANTO = 1,
    GINORA = 2,
    OODADA=3,
    COPAN=4,
    DOINK=5,
    TENA=6,
    ENEMY = 9,
}

public enum LikeState
{
    HATE = 0,
    NORMAL,
    FRIEND,
}
#endregion

public class CharacterAttribute
{
    public Headers header { get; set; }     // 대가리 캐릭터 종류
    public LikeState likeState { get; set; } //호감도 상태(좋다,싫다)

    public float likeMeter;      //호감도(%)
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
    public float specialCoolTime { get; set; }
}

public class Character : MonoBehaviour
{
    //외부 클래스
    protected GameManager gameMgr;
    SoundManager soundMgr;

    //캐릭터 내부 클래스
    public HeaderUI headerCanvas { get; set; }  //대가리 UI
    public MouthColl mouthColl; //캐릭터가 물건을 집기 위한 클래스
    public HeaderController headerCtrl;
    Checkpoint currentCheckpoint;

    public CharacterAttribute Status { get; set; }
    public Animator mAnimator { get; set; }
    public NavMeshAgent mNavAgent { get; set; }
    public Coroutine currentAI = null;

    //enum
    public AnimState statAnim;  //작동중인 애니메이션 상태
    public LikeState statLike;      //호감도 상태

    //캐릭터 대사리스트
    public List<List<List<object>>> list___dialog_kor { get; set; }
    public List<List<List<object>>> list___dialog_eng { get; set; }

    //Childs
    public BoxCollider[] platformerColliders;
    public GameObject interactionColliders;    //코,등,꼬리(만지기)
    public GameObject select;   //현재 선택중인지 표시용

    //Color
    public Material[] mMaterials;
    public Color bodyColor { get; set; } //몸 색깔
    SkinnedMeshRenderer[] mRenderers;

    //이동용 좌표들
    public Transform mainCam;   //=플레이어 위치
    public Transform laserPos;     //지시용 위치

    public Vector3 spawnPoint;
    public Transform movePoint;
    public  Transform[] movePoints;
    public Transform currentTarget;

    //Sounds
    public AudioClip sfx_move { get; set; }
    public AudioClip sfx_fast { get; set; }
    public AudioClip sfx_hit { get; set; }
    public AudioClip sfx_clean { get; set; }
    public AudioClip sfx_jump { get; set; }

    //내부 변수
    public Vector3 currentScale;

    public bool isSelect;   //선택 중인가
    public bool isCage;     //감옥에 갇힌 상태인가?(갇히면 AI동작 안함)

    public bool isDie;
    public bool isHit;
    public bool isSpecial = false;
    public bool isGround;
    public bool isPlatfomer;
    public bool isFleek = false;
    private int likeStateCount = 2;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        soundMgr = gameMgr.soundMgr;
        headerCanvas = transform.GetChild(1).GetComponent<HeaderUI>();
        mouthColl = GetComponentInChildren<MouthColl>();
        headerCtrl = GetComponent<HeaderController>();
        Status = new CharacterAttribute();

        mAnimator = GetComponent<Animator>();
        mNavAgent = GetComponent<NavMeshAgent>();

        platformerColliders = GetComponents<BoxCollider>();

        list___dialog_kor = new List<List<List<object>>>();
        list___dialog_eng = new List<List<List<object>>>();

        mRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        sfx_jump = soundMgr.LoadClip(Defines.SOUND_SFX_JUMP);

        currentScale = this.transform.localScale;

        DoAwake();
    }

    #region Virtual Funtions

    protected virtual void DoAwake() { }
    protected virtual void StatusInit() { }

    public virtual void SpecialMove() { } //각 캐릭터 특수 동작

    /// <summary>
    /// 강제로 동작 멈추기, 일부분 초기화
    /// </summary>
    public virtual void Stop()
    {
        StopAllCoroutines();
        StopCoroutine(currentAI);
        if (!isPlatfomer)
        {
            mNavAgent.speed = 0;
            mNavAgent.isStopped = true;
        }
    }
    /// <summary>
    /// 현재 호감도에 따라 상태 변경
    /// 호감도 상태가 변경되면 애니메이션 또한 변경
    /// 애니메이션을 변경하기 위해 각 캐릭터마다 적용시킬 것
    /// </summary>
    protected virtual void ChangeLikeState()
    {
        if (Status.likeMeter >= 100 && (int)statLike != likeStateCount)    //LikeState의 최대값
        {
            Status.likeMeter = 0;
            if ((int)statLike < likeStateCount)
            {
                statLike++;
            }
        }
        else if (Status.likeMeter <= 0 && statLike != 0) //LikeState의 최소값
        {
            Status.likeMeter = 100;
            if (statLike > 0)
            {
                statLike--;
            }
        }
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
        Debug.Log(this.gameObject.name + "AI:" + statAnim);
        SetAnim();

        int randPoint = Random.Range(0, movePoints.Length); //향할 맵 포인트
        float randDist = Random.Range(1f, 3f); //갈 거리

        Transform target = movePoints[randPoint];
        if (target == currentTarget)
        {
            randPoint = Random.Range(0, movePoints.Length);
            target = movePoints[randPoint];
        }
        currentTarget = target;

        mNavAgent.destination = currentTarget.position;
        mNavAgent.isStopped = false;
        mNavAgent.speed = Status.moveSpeed;

        yield return new WaitForSeconds(0.1f);

        while (isHit == false
               && isDie == false
               && mNavAgent.remainingDistance > randDist)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.maxSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }
        Debug.Log(this.gameObject.name + "AI:" + statAnim + " End");
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

        //currentAI = null;
        yield return new WaitForSeconds(0.1f);
        //AI_Move(2);
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
        mNavAgent.speed = Status.maxSpeed;

        yield return new WaitForSeconds(0.1f);

        while (mNavAgent.remainingDistance > 0.2f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.maxSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }

        isHit = false;
        mNavAgent.speed = Status.moveSpeed;
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
        mNavAgent.speed = 0;
        yield return new WaitForSeconds(4f);
        AI_Move(0);
    }

    /// <summary>
    /// AI: 04
    /// 카메라 쪽으로 달려옴
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Call()
    {
        statAnim = AnimState.HAPPY;
        SetAnim();

        mNavAgent.destination = mainCam.position;
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.isStopped = false;

        while (mNavAgent.remainingDistance > 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.isStopped = true;
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
        this.gameObject.SetActive(false);
    }
    #endregion

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
            }
            else
                end -= 1;
        }
        else if (end < 0)
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
        Debug.Log("친밀도:" + statLike + Status.likeMeter);
        //headerCanvas.SetLikeGauge(start, end, change);
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

        return (refusePercent < Random.Range(0, 100)) ? true : false;
    }


    /// <summary>
    /// 음식 먹기(영양가를 전달해서 취향을 파악, 호감도, 배고픔에 영향을 줌)
    /// </summary>
    /// <param name="_food">음식 오브젝트</param>
    public IEnumerator EatFood(Food _food)
    {
        //mouthColl.gameObject.SetActive(false);
        //interactionColliders.SetActive(false);

        CheckFood(_food.foodNum, _food.satiety, _food.taste);
        _food.transform.parent = _food.spawnTr;
        _food.gameObject.SetActive(false);
        //먹는소리
        soundMgr.PlaySfx(this.transform, soundMgr.LoadClip("eat_01"));

        int random = Random.Range(0, 2);
        headerCanvas.ShowText(2, random);

        if (!isPlatfomer)
        {
            mNavAgent.isStopped = true;
            if (currentAI != null)
                StopCoroutine(currentAI);
            //뭔가 먹는 모션
            AI_Move(3);
        }

        yield return new WaitForSeconds(2f);
        //interactionColliders.SetActive(true);
        //mouthColl.gameObject.SetActive(true);
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
            LikeChange(90);
        }
        else if (_foodNum == Status.favoriteFoodNum)
        {
            //가장 좋아하는 음식이면
            Status.hunger += _satistie * 3f;
            LikeChange(30);    //호감도 상승!
        }
    }
    #endregion

    #region 캐릭터 몸 관련
    public void PlayEffect(Vector3 _position, GameObject _go)
    {
        ParticleSystem _p = _go.transform.GetChild(0).GetComponent<ParticleSystem>();
        _p.transform.position = _position;
        _p.Play();
    }
    public IEnumerator Touched(Collider _coll)
    {
        _coll.enabled = false;
        yield return new WaitForSeconds(2f);
        _coll.enabled = true;
    }

    /// <summary>
    /// 몸 색깔 변경
    /// </summary>
    public void SetBodyColor(float _dirty)
    {
        if (mMaterials == null) { return; }
        //색 변경
        bodyColor = new Color(_dirty, _dirty, _dirty);
        foreach (Material _m in mMaterials)
        {
            _m.color = bodyColor;
        }
    }

    /// <summary>
    /// 게임모드에 따른 활성 충돌체 변경
    /// </summary>
    public void SetPlayerGameMode()
    {
        foreach (var coll in platformerColliders)
        {
            coll.enabled = false;
        }
        interactionColliders.SetActive(false);
        mouthColl.gameObject.SetActive(false);
        headerCanvas.gameObject.SetActive(false);

        switch (gameMgr.statGame)
        {
            case GameState.PLAY:
                foreach (var coll in platformerColliders)
                {
                    coll.enabled =true;
                }
                break;
            case GameState.CLEAR:
                interactionColliders.SetActive(true);
                mouthColl.gameObject.SetActive(true);
                headerCanvas.gameObject.SetActive(true);
                break;
        }
    }
    #endregion

    #region Controller Moves
    /// <summary>
    /// 현재 캐릭터 선택
    /// </summary>
    public void SetSelect(bool _isSelect)
    {
        //select.SetActive(_isSelect);
        isSelect = _isSelect;
    }

    /// <summary>
    /// 레이저 선택 지점으로 캐릭터 움직이기
    /// </summary>
    /// <param name="_hitPos">선택한 위치</param>
    /// <param name="_hitObj">선택한 물건</param>
    public virtual void MoveCharacter(Vector3 _hitPos, GameObject _hitObj)
    {
        if (!isSelect || isSpecial || isCage || isPlatfomer) { return; }
        Stop();
        currentAI = StartCoroutine(MoveToTarget(_hitPos, _hitObj));
    }

    /// <summary>
    /// 해당 방향으로 이동
    /// </summary>
    /// <param name="_hitObj"></param>
    public virtual IEnumerator MoveToTarget(Vector3 _hitPos, GameObject _hitObj)
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
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.maxSpeed * Time.deltaTime);
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
    #endregion

    #region Platformer Fuctions
    /// <summary>
    /// 캐릭터 사망 후 체크포인트로 이동하는 코드
    /// </summary>
    public virtual IEnumerator Respawn()
    {
        isDie = true;
        //사망모션
        //깜빡이기
        StartCoroutine(Fleek());
        yield return new WaitForSeconds(0.5f);
        if (currentCheckpoint == null)
        {
            currentCheckpoint = gameMgr.stageMgr.list_checkPoints[0].GetComponent<Checkpoint>();
        }
        Debug.Log("Respawn!: " + currentCheckpoint.name);
        transform.position = currentCheckpoint.transform.position;
        transform.rotation = currentCheckpoint.transform.rotation;

        isDie = false;
    }

    public IEnumerator Fleek()
    {
        if (isFleek == true)
        {
            yield break;
        }
        isFleek = true;
        float fleekTime = 1.0f;
        while (fleekTime > 0)
        {
            for (int i = 0; i < mRenderers.Length; i++)
            {
                mRenderers[i].enabled = false;
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < mRenderers.Length; i++)
            {
                mRenderers[i].enabled = true;
            }
            yield return new WaitForSeconds(0.1f);
            fleekTime -= 0.1f;
        }
        isFleek = false;
    }
    /// <summary>
    /// 캐릭터가 사망하는 코드
    /// </summary>
    protected virtual IEnumerator Die()
    {
        yield return new WaitForSeconds(3.0f);
    }

    public void SetCheckpoint(Checkpoint _checkpoint)
    {
        Debug.Log("CheckPoint!:" + _checkpoint.name);
        if (_checkpoint != null)
        {
            if (currentCheckpoint != null)
                currentCheckpoint.GetComponent<BoxCollider>().enabled = true;
            currentCheckpoint = _checkpoint;
            currentCheckpoint.GetComponent<BoxCollider>().enabled = false;
        }
    }
    #endregion

    #region Animation Settings
    /// <summary>
    /// 애니메이션 변경 시 호출, 현재 AnimState에 따른 애니메이션 적용
    /// bool type Anims
    /// </summary>
    public virtual void SetAnim()
    {
        switch (statAnim)
        {
            case AnimState.IDLE:
                mAnimator.SetBool("isMove", false);
                break;
            case AnimState.PATROL:
                mAnimator.SetBool("isMove", true);
                break;
            case AnimState.RUN:
                mAnimator.SetBool("isMove", true);
                break;
            case AnimState.HAPPY:
                break;
            case AnimState.CLEAN:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// trigger type Anims
    /// </summary>
    /// <param name="_num">0:Jump/1:Stretch/2:Shake/3:Hit</param>
    public virtual void SetAnim(int _num)
    {
        mAnimator.SetBool("isMove", false);
        mAnimator.SetFloat("TriggerNum", _num);
        mAnimator.SetTrigger("isTrigger");
    }

    public virtual void AnimationEnd()
    {
        interactionColliders.SetActive(true);
        AI_Move(3);
    }
    #endregion

    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:run/3:stay/4:call/5:jump/d:clear</param>
    public virtual void AI_Move(int _type)
    {
        //returns
        if (gameMgr.statGame == GameState.GAMEOVER) { return; }

        if (isPlatfomer)
        {
            mNavAgent.enabled = false;
            //headerCtrl.mCharCtrl.enabled = true;
            return;
        }

        if (isCage)
        {
            currentAI = StartCoroutine(Idle());
            return;
        }

        if (currentAI != null) { StopCoroutine(currentAI); }


        //headerCtrl.mCharCtrl.enabled = false;
        mNavAgent.enabled = true;

        //호감도 상태에 따른 랜덤 대사 출력하기
        int _random = Random.Range(0, 2);

        switch (_type)
        {
            case 0: //배회
                headerCanvas.ShowText(0, _random);
                currentAI = StartCoroutine(PatrolMove());
                break;
            case 1: //맞았을 때
                currentAI = StartCoroutine(Hit());
                break;
            case 2: //달릴 때
                headerCanvas.ShowText(0, _random);
                currentAI = StartCoroutine(Run());
                break;
            case 3: //가만히 있을 때, 정지할 때
                currentAI = StartCoroutine(Idle());
                break;
            case 4: //부르기
                currentAI = StartCoroutine(Call());
                break;
            default: //깨끗해졌을 때
                currentAI = StartCoroutine(Clean());
                break;
        }
    }

}