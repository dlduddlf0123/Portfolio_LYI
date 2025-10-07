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
    CALL,
    SLEEP,
    LYING,
    CLEAN,
    BACKWALK,
    COMMUNICATION,
    POOP,
};

//대가리 번호
public enum Headers
{
    NONE = 0,
    KANTO = 1,
    GINORA = 2,
    OODADA = 3,
    COPHAN = 4,
    DOINK = 5,
    TENA = 6,
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

public enum HungerState
{
    DEADLY,
    HUNGRY,
    NOLMAL,
    FULL,
}
public enum EnergyState
{
    BLACKOUT,
    TIRED,
    NOLMAL,
    ENERGETIC,
}

#endregion

/// <summary>
/// 캐릭터 스텟을 가지는 클래스
/// </summary>
public class CharacterAttribute
{
    // 대가리 캐릭터
    public Headers header { get; set; }

    //0~100% 수치
    public float likeMeter { get; set; }        //호감도(0~100%)
    public float hungerMeter { get; set; }       //배고픔(호감도에 영향 / 0이되면?)
    public float energyMeter { get; set; }      //피로도(0이 되면 기절 / 30 이하 졸림, 수면 가능 / 잠자면 회복)
    
    //호감도 관련
    public FoodType likeFood { get; set; }    //가장 좋아하는 음식
    public FoodType hateFood { get; set; }    //가장 싫어하는 음식
    public int likeTaste { get; set; }      //좋아하는 맛
    public int hateTaste { get; set; }      //싫어하는 맛

    public int likeAct { get; set; }    //좋아하는 행동
    public int hateAct { get; set; }    //싫어하는 행동

    public int frontTouchLike { get; set; } //코 만질 때 호감도
    public int upTouchLike { get; set; }    //등 만질 때 호감도
    public int backTouchLike { get; set; }  //엉덩이 만질 때 호감도

    //이동 관련
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
    protected GameManager gameMgr;
    protected SoundManager soundMgr;

    public StageManager stageMgr;
    protected StatusUI statUI;

    public HeaderCanvas headerCanvas;
    public CharacterAttribute Status { get; set; }

    //만지기
    public GameObject bodyColls;    //코,등,꼬리
    public MouthColl mouthColl;     //입
    public SkinnedMeshRenderer[] mSkins;

    public Transform currentTarget;
    public Transform movePoint;
    public Transform[] movePoints;

    //enum
    public AnimState statAnim;  //작동중인 애니메이션 상태
    public LikeState statLike;      //호감도 상태
    public EnergyState statEnergy;
    public HungerState statHunger;

    //내부 클래스들
    public Animator mAnimator { get; set; }
    public NavMeshAgent mNavAgent { get; set; }
    public Coroutine currentAI = null;

    //내부 변수
    //캐릭터 대사리스트의 리스트
    public List<List<List<object>>> list___dialog_kor { get; set; }
    public List<List<List<object>>> list___dialog_eng { get; set; }

    public int touchStack = 0;

    public bool isAction;   //커맨드 수행중인지 여부
    public bool isDie;
    public bool isHit;
    public bool isEvent;
    
    float lyingTime = 0f;
    
    private void Awake()
    {

        headerCanvas = transform.GetChild(1).GetComponent<HeaderCanvas>();
        mouthColl = GetComponentInChildren<MouthColl>();
        Status = new CharacterAttribute();

        mAnimator = GetComponent<Animator>();
        mNavAgent = GetComponent<NavMeshAgent>();
        mSkins = this.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        list___dialog_kor = new List<List<List<object>>>();
        list___dialog_eng = new List<List<List<object>>>();

        DoAwake();
    }

    protected virtual void Start()
    {
        gameMgr = GameManager.Instance;
        soundMgr = gameMgr.soundMgr;
        statUI = stageMgr.statUI;
    }

    private void OnDisable()
    {
        Stop();
    }

    #region Virtual Funtions

    protected virtual void DoAwake() { }
    protected virtual void StatusInit() { }

    public virtual void PlayBoolAnimation() { }    // bool type Anims
    public virtual void PlayTriggerAnimation(int _num)
    {
        mAnimator.SetBool("isMove", false);
        mAnimator.SetFloat("TriggerNum", _num);
        mAnimator.SetTrigger("isTrigger");
        Debug.Log(gameObject.name + "TraggerNum: " + _num);
    }   // trigger type Anims

    public virtual void AnimationEnd() { }
    public virtual void CheckEvent() { }    //이벤트 발생 조건 체크 함수

    /// <summary>
    /// 각 동작 이후 다음 동작 설정 함수
    /// 각 동작이 끝났을 때 호출
    /// </summary>
    public virtual void NextAI(bool _choose)
    {
        isAction = false;
        switch (statAnim)
        {
            case AnimState.IDLE:
                if (_choose)
                    AI_Move(0);
                else
                    AI_Move(3);
                break;
            case AnimState.PATROL:
                if (_choose)
                    AI_Move(2);
                else
                    AI_Move(3);
                break;
            case AnimState.HIT:
                AI_Move(2);
                break;
            case AnimState.RUN:
                if (_choose)
                    AI_Move(0);
                else
                    AI_Move(3);
                break;
            case AnimState.CALL:
                AI_Move(3);
                break;
            case AnimState.SLEEP:
                AI_Move(3);
                break;
            case AnimState.LYING:
                AI_Move(3);
                break;
            case AnimState.BACKWALK:
                AI_Move(2);
                break;
            case AnimState.POOP:
                AI_Move(2);
                break;
            default:
                AI_Move(3);
                break;
        }
    }

    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:run/3:stay/4:call/5:jump/d:clear</param>
    public virtual void AI_Move(int _type)
    {
        if (currentAI != null)
        {
            StopCoroutine(currentAI);
        }
        //호감도 상태에 따른 랜덤 대사 출력하기
        int _random = Random.Range(0, 3);
        //AI 동작들
        switch (_type)
        {
            case 0: //배회
                headerCanvas.ShowText(0, _random);
                currentAI = StartCoroutine(PatrolMove());
                //GetEnergy(-1);
                //GetHunger(-3);
                break;
            case 1: //맞았을 때
                currentAI = StartCoroutine(Hit());
                break;
            case 2: //달릴 때
                currentAI = StartCoroutine(Run());
                //GetEnergy(-2);
                //GetHunger(-5);
                break;
            case 3: //가만히 있을 때, 정지할 때
                currentAI = StartCoroutine(Idle());
                break;
            case 4: //부르기
                currentAI = StartCoroutine(Call());
                break;
            case 5: //잠자기
                currentAI = StartCoroutine(Sleep());
                break;
            case 6: //발라당
                currentAI = StartCoroutine(Lying());
                break;
            default: //깨끗해졌을 때
                currentAI = StartCoroutine(Clean());
                break;
        }

       // Debug.Log(gameObject.name + "AI: " + _type);
    }

    /// <summary>
    /// 강제로 동작 멈추기, 일부분 초기화
    /// </summary>
    public virtual void Stop()
    {
        if (isEvent || !gameObject.activeSelf)
        {
            return;
        }

        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
        
        StopAllCoroutines();

        if (currentAI != null)
        {
            StopCoroutine(currentAI);
            currentAI = null;
        }
        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;
        isAction = false;
        lyingTime = 0;


        if (statAnim == AnimState.LYING)
        {
            StartCoroutine(LyingGetUp());
        }

        Debug.Log(gameObject.name + "Stop()");
    }

    /// <summary>
    /// 현재 호감도에 따라 상태 변경
    /// 호감도 상태가 변경되면 애니메이션 또한 변경
    /// 애니메이션을 변경하기 위해 각 캐릭터마다 적용시킬 것
    /// </summary>
    protected virtual void ChangeLikeState() { }

    #endregion


    public void MoveCharacter(Vector3 hitPoint, GameObject _targetObj = null)
    {
        if (statAnim == AnimState.LYING)
        {
            return;
        }
        Stop();
        isAction = true;
        //bodyColls.SetActive(false);
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
        PlayBoolAnimation();

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
        }

        mNavAgent.destination = _target;
        yield return new WaitForSeconds(0.1f);

        while (isHit == false
               && isDie == false
               && mNavAgent.remainingDistance > dist)
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_target - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }
        mNavAgent.speed = Status.moveSpeed;
        isAction = false;

        AI_Move(3);

        //bodyColls.SetActive(true);
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

        float likeMax = 100f;
        switch (statLike)
        {
            case LikeState.HATE:
                likeMax = 50;
                break;
            case LikeState.NORMAL:
                likeMax = 100;
                break;
            case LikeState.FRIEND:
                likeMax = 100;
                break;
        }

        if (end > likeMax*0.01f)
        {
            if (statLike == LikeState.FRIEND)
            {
                end = likeMax * 0.01f;
                //호감도 MAX
               // gameMgr.miniGameTable.SetActive(true);
            }
            else
                end -= likeMax * 0.01f;
        }
        else if(end < 0)
        {
            if (statLike == LikeState.HATE)
                end = 0;
            else
                end += likeMax * 0.01f;
        }

        if (Status.likeMeter > likeMax)
        {
            Status.likeMeter = likeMax;

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
        //Debug.Log("친밀도:"+statLike+Status.likeMeter);
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
        if (statEnergy == EnergyState.BLACKOUT) yield break;

        bodyColls.SetActive(false);
        mouthColl.gameObject.SetActive(false);

        CheckFood(_food.typeFood, _food.satiety, _food.taste);
        if (_food.typeFood == Status.hateFood)
        {
            yield break;
        }
        
        //먹는소리
        soundMgr.PlaySfx(this.transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_EAT));

        _food.FoodDetach();
        _food.gameObject.SetActive(false);  //음식소멸 or 뱃속으로 이동해서 소화시키기??

        mNavAgent.isStopped = true;

        //뭔가 먹는 모션
        PlayTriggerAnimation(7);
        yield return new WaitForSeconds(2f);
        //감정 표현 모션
        // SetAnim(2);
        //yield return new WaitForSeconds(2f);
        AI_Move(3);

        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
        ChangeMovePoint(1);
    }

    /// <summary>
    /// 음식 취향 확인, 호감도 상승!
    /// 대사 출력
    /// </summary>
    /// <param name="_type">음식 종류</param>
    /// <param name="_satistie">포만도</param>
    /// <param name="_taste">맛</param>
    public void CheckFood(FoodType _type, int _satistie, int _taste)
    {
        //GetHunger(_satistie);
        if (_type == Status.hateFood)
        {
            //싫어하는 음식
            headerCanvas.ShowText(7, Random.Range(0, 2));
            currentAI = StartCoroutine(BackWalk());
            LikeChange(-10);
        }
        else if (_type == Status.likeFood &&
            _taste == Status.likeTaste)
        {
            //좋아하는 음식
            headerCanvas.ShowText(5, Random.Range(0, 2));
            LikeChange(30);    //호감도 상승!
        }
        else
        {
            //보통 음식
            headerCanvas.ShowText(6, Random.Range(0, 2));
            LikeChange(10);    //호감도 상승!
        }
    }

    /// <summary>
    /// 피로도 체크 함수
    /// 현재 피로도 확인 후 행동으로 연결
    /// </summary>
    public virtual void ChangeEnergyState()
    {
        if (stageMgr.statUI == null) { return; }

        stageMgr.statUI.arr_gaugeImgs[2].transform.parent.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        if (Status.energyMeter <= 0)
        {
            //기절
            statEnergy = EnergyState.BLACKOUT;
            stageMgr.statUI.arr_gaugeImgs[2].transform.parent.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        else if (Status.energyMeter < 30)
        {
            //피곤함
            statEnergy = EnergyState.TIRED;
            stageMgr.statUI.arr_gaugeImgs[2].transform.parent.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        else if (Status.energyMeter >= 90)
        {
            //기운참
            statEnergy = EnergyState.ENERGETIC;
        }
        else
        {
            //정상
            statEnergy = EnergyState.NOLMAL;
        }
       // Debug.Log("FatigueState: " + statEnergy);
    }


    /// <summary>
    /// 배고픔 체크 함수
    /// 현재 배고픔 수치 확인 후 행동으로 연결
    /// </summary>
    public virtual void ChangeHungerState()
    {
        if (stageMgr.statUI == null)
        {
            return;
        }
        stageMgr.statUI.arr_gaugeImgs[1].transform.parent.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        if (Status.hungerMeter <= 10)
        {
            //기절
            statHunger = HungerState.DEADLY;
            stageMgr.statUI.arr_gaugeImgs[1].transform.parent.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        else if (Status.hungerMeter < 30)
        {
            //배고픔 호소
            statHunger = HungerState.HUNGRY;
            stageMgr.statUI.arr_gaugeImgs[1].transform.parent.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        else if (Status.hungerMeter >= 100)
        {
            //과식
            statHunger = HungerState.FULL;
        }
        else
        {
            //정상
            statHunger = HungerState.NOLMAL;
        }
       // Debug.Log("HungerState: " + statHunger);
    }

    /// <summary>
    /// 피로도 증가 함수
    /// </summary>
    public void GetEnergy(float _energy)
    {
        stageMgr.statUI.StartCoroutine(stageMgr.statUI.SetGauge(stageMgr.statUI.arr_gaugeImgs[2], Status.energyMeter, Status.energyMeter + _energy));
        Status.energyMeter += _energy;

        if (Status.energyMeter > 100)
            Status.energyMeter = 100;
        if (Status.energyMeter < 0)
        {
            Status.energyMeter = 0;
            LikeChange(-10);
        }

        //Debug.Log(gameObject.name + "Energe: " + Status.energyMeter);
        ChangeEnergyState();
    }

    public void GetHunger(float _hunger)
    {
        statUI.StartCoroutine(statUI.SetGauge(statUI.arr_gaugeImgs[1], Status.hungerMeter, Status.hungerMeter + _hunger));
        Status.hungerMeter += _hunger;

        if (Status.hungerMeter > 100)
            Status.hungerMeter = 100;
        if (Status.hungerMeter < 0)
        {
            Status.hungerMeter = 0;
            GetEnergy(-0.5f);
        }

        //Debug.Log(gameObject.name + "Hunger: " + Status.hungerMeter);

        ChangeHungerState();
    }
    #endregion

    #region VR Hand Motion Settings

    /// <summary>
    /// Order to Header
    /// </summary>
    /// <param name="_actionNum"> 0:Run/1:Call/2:Stay/3:GetIt!</param>
    public void OrderAction(int _actionNum)
    {
        //gameMgr.HandEffect(_hand);
        Stop();
        isAction = true;

        switch (_actionNum)
        {
            case 0: //달리기
                currentAI = StartCoroutine(Run());
               // GetEnergy(-2);
               // GetHunger(-5);
                break;
            case 1: //부르기
                currentAI = StartCoroutine(Call());
                break;
            case 2:
                //기다려
                isAction = false;
                currentAI = StartCoroutine(Idle());
                break;
            case 3:
                //뒤집어!
                currentAI = StartCoroutine(Lying());
                break;
        }
        Debug.Log(gameObject.name + "OrderAction: " + _actionNum);
    }

    /// <summary>
    /// 손동작 변경 검지
    /// </summary>
    public void SetSecondFinger(HandInteract _hand)
    {
        if (!isAction && 
            statAnim != AnimState.RUN &&
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
        if (!isAction &&
            statAnim != AnimState.CALL &&
            statLike != LikeState.HATE)
        {
            gameMgr.HandEffect(_hand);
            Stop();
            isAction = true;
            AI_Move(4);
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
    //public void SetDoubleRock()
    //{
    //    Stop();
    //    isAction = true;
    //    MoveCharacter(gameMgr.appleMgr.currentApple.transform.position, gameMgr.appleMgr.currentApple);
    //}

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
        PlayBoolAnimation();

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

        if (statEnergy <= EnergyState.TIRED)
        {
            mNavAgent.speed *= 0.5f;
        }

        yield return new WaitForSeconds(0.1f);

        while (isHit == false
               && isDie == false
               && mNavAgent.remainingDistance > randDist)
        {
            //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }

        bool move = false;
        if (randDist < randDist * 0.5f)
            move = true;
        else
            move = false;
        
        NextAI(move);
    }


    /// <summary>
    /// AI: 01
    /// 맞았을 시 잠시 정지(히트애니메이션)
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Hit()
    {
        statAnim = AnimState.HIT;
        PlayTriggerAnimation(3);
        isHit = true;

        ChangeMovePoint(0);
        yield return new WaitForSeconds(0.8f);
        isHit = false;

        NextAI(true);
    }

    /// <summary>
    /// AI: 02
    /// 달려서 레이져 쪽으로
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Run()
    {
        statAnim = AnimState.RUN;
        PlayBoolAnimation();

        ChangeMovePoint(0);

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

        if (statEnergy <= EnergyState.TIRED)
        {
            mNavAgent.speed *= 0.5f;
        }

        while (mNavAgent.remainingDistance > randDist)
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }

        isHit = false;
        mNavAgent.speed = Status.moveSpeed;

        bool move = false;
        if (randDist < randDist * 0.5f)
            move = true;
        else
            move = false;

        NextAI(move);
    }

    /// <summary>
    /// AI: 03
    /// 대기 동작
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Idle()
    {
        mAnimator.SetBool("IdleMove", false);
        statAnim = AnimState.IDLE;
        PlayBoolAnimation();

        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;
        isAction = false;


        if (mouthColl.attachObject != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (mouthColl.attachObject.CompareTag("Ball"))
            {
                headerCanvas.ShowText(13, Random.Range(0, 2));
            }
            mouthColl.DetachMouth();
        }

        yield return new WaitForSeconds(5f);
        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);

        bool move = false;
        if (Random.Range(0, 2) < 1)
        {
            move = true;
        }
        else
        {
            move = false;
            //if (statEnergy <= EnergyState.TIRED)
            //{
            //    mAnimator.SetFloat("TriggerNum", 6);
            //    headerCanvas.ShowText("피곤해.. 자고싶어..");
            //}
            //else if (statHunger <= HungerState.HUNGRY)
            //{
            //    mAnimator.SetFloat("TriggerNum", 4);
            //    headerCanvas.ShowText("배고파!");
            //}
            //else
            {
                mAnimator.SetBool("IdleMove", true);
            }

            yield return new WaitForSeconds(1f);
            mAnimator.SetBool("IdleMove", false);
            yield return new WaitForSeconds(2f);
        }

        NextAI(move);
    }


    /// <summary>
    /// AI: 04
    /// 카메라 쪽으로 달려옴
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Call()
    {
        statAnim = AnimState.CALL;

        currentTarget = gameMgr.mainCam.transform;
        mNavAgent.destination = currentTarget.position;
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.isStopped = false;

        if (statEnergy <= EnergyState.TIRED)
        {
            mNavAgent.speed *= 0.5f;
        }
        switch (statLike)
        {
            case LikeState.HATE:
                mAnimator.SetBool("isMove", true);
                mAnimator.SetFloat("Speed", 0);

                float t = 0.0f;
                float rand = Random.Range(0.5f, 3.0f);
                while (mNavAgent.remainingDistance > 1f)
                {
                    if (t > rand)
                    {
                        mNavAgent.isStopped = true;

                        mAnimator.SetBool("isMove", false);
                        mAnimator.SetBool("IdleMove", false);
                        mAnimator.SetFloat("TriggerNum", Random.Range(0, 2));

                        t = 0;
                        rand = Random.Range(0.5f, 3.0f);
                        yield return new WaitForSeconds(Random.Range(0.5f,2.0f));
                    }
                    else
                    {
                        mNavAgent.isStopped = false;

                        mAnimator.SetBool("isMove", true);
                        mAnimator.SetFloat("Speed", 0);

                        t += Time.deltaTime;
                    }
                    yield return new WaitForSeconds(0.0167f);
                }

                break;
            default:
                PlayBoolAnimation();

                mNavAgent.speed = Status.moveSpeed * 1.5f;

                if (statEnergy <= EnergyState.TIRED)
                {
                    mNavAgent.speed *= 0.5f;
                }
                while (mNavAgent.remainingDistance > 1.3f)
                {
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
                    yield return new WaitForSeconds(0.0167f);
                }

                break;
        }

        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;

        if (mouthColl.attachObject != null)
        {
            mouthColl.DetachMouth();
        }

        if (statHunger <= HungerState.HUNGRY)
        {
            //배고프다고 조르기
            PlayTriggerAnimation(4);
            yield return new WaitForSeconds(3.2f);
        }

        NextAI(true);
    }

    /// <summary>
    /// AI: 05
    /// 잠자기
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Sleep()
    {
        statAnim = AnimState.SLEEP;
        PlayBoolAnimation();
        bodyColls.SetActive(false);
        while (gameMgr.sunMove.statTime != TimeState.MORNING)
        {
            yield return new WaitForSeconds(5f);
        }
        bodyColls.SetActive(true);
        GetEnergy(100);
        mAnimator.SetTrigger("StateEnd");

        NextAI(true);
    }

    /// <summary>
    /// AI: 06
    /// 배 보이기(만졌을 때)
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Lying()
    {
        if (statAnim == AnimState.LYING)
        {
            yield break;
        }
        statAnim = AnimState.LYING;
        PlayBoolAnimation();

        headerCanvas.ShowText("배도 긁어줘!");
        Debug.Log(gameObject.name + "Lying!");

        bodyColls.transform.GetChild(0).gameObject.SetActive(false);
        bodyColls.transform.GetChild(1).gameObject.SetActive(true);
        bodyColls.transform.GetChild(2).gameObject.SetActive(false);
        bodyColls.transform.GetChild(1).transform.localPosition = Vector3.up * 0.5f;

        lyingTime = 5;


        while (lyingTime > 0)
        {
            lyingTime -= 1;
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(LyingGetUp());
    }

    public virtual IEnumerator LyingGetUp()
    {
        if (statAnim != AnimState.LYING)
        {
            yield break;
        }
        mAnimator.SetTrigger("StateEnd");
        Debug.Log(gameObject.name + "Lying GetUp!");

        mNavAgent.isStopped = true;
        mNavAgent.speed = 0;

        yield return new WaitForSeconds(0.875f);

        bodyColls.transform.GetChild(0).gameObject.SetActive(true);
        bodyColls.transform.GetChild(1).gameObject.SetActive(true);
        bodyColls.transform.GetChild(2).gameObject.SetActive(true);
        bodyColls.transform.GetChild(1).transform.localPosition = Vector3.up * 0.8f;

        mAnimator.ResetTrigger("StateEnd");
        statAnim = AnimState.IDLE;
        NextAI(true);
    }

    public IEnumerator Tingle()
    {
        mAnimator.SetFloat("StateNum", 2);
        mAnimator.SetTrigger("StateChange");
        lyingTime = 5;

        yield return new WaitForSeconds(0.875f);
        mAnimator.SetFloat("StateNum", 1);
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
        PlayBoolAnimation();
        yield return new WaitForSeconds(2.0f);
        currentAI = null;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 뒤로 걷기(안 친할 때)
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator BackWalk()
    {
        statAnim = AnimState.BACKWALK;
        PlayTriggerAnimation(5);
        
        yield return new WaitForSeconds(0.3f); //점프시간

        mNavAgent.isStopped = false;
        mNavAgent.destination = transform.position +
            (transform.position - gameMgr.mainCam.transform.position);

        mNavAgent.speed = Status.moveSpeed * 0.5f;

        float t = 1.7f; //애니메이션 재생시간
        while (t > 0)
        {
            t -= Time.deltaTime;
            //뒷걸음
            mNavAgent.destination = transform.position +
                (transform.position - gameMgr.mainCam.transform.position);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(gameMgr.mainCam.transform.position - transform.position),
                Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        mNavAgent.speed = Status.moveSpeed;
        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);

        NextAI(true);
    }

    /// <summary>
    /// 접시를 향해 이동 후 먹기
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveToBowl(GameObject _go, Transform[] _tr)
    {
        int rand = Random.Range(0, 2);
        headerCanvas.ShowText(17, rand);

        yield return StartCoroutine(MoveToTarget(_tr[0].position, null));

        if (rand > 1)
        {
            headerCanvas.ShowText(17, 2);
        }

        yield return StartCoroutine(MoveToTarget(_tr[1].transform.position, _go));

        isEvent = false;
    }
    #endregion

    #region Collider Effects 손과 상호작용

    //public virtual void BodyTouched(int _part = 0)
    //{
    //    switch (_part)
    //    {
    //        case 1://코

    //            break;
    //        case 2://머리

    //            break;
    //        case 3://엉덩이

    //            break;
    //        default:
    //            break;
    //    }
    //    soundMgr.PlaySfx(this.transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_CLICK));
    //    GameManager.Instance.PlayEffect(this.transform.position, GameManager.Instance.particles[1]);

    //    StartCoroutine(AfterTouch(_part));
    //}
    public virtual IEnumerator AfterTouch(int _part = 0)
    {
        bodyColls.SetActive(false);
        mouthColl.gameObject.SetActive(false);
        touchStack++;

        yield return new WaitForSeconds(3f);

        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
        
        if (_part == 3 )
        {
            AI_Move(2);
        }

        if (statAnim != AnimState.LYING)
        {
            if (statLike == LikeState.HATE)
            {

            }
            else
            {
                AI_Move(3);
            }
        }
    }
    #endregion

    #region QuestPoop
    public void MakePoop()
    {
        GameObject go = Instantiate(GameManager.Instance.b_prefabs.LoadAsset<GameObject>("Shit"));
        go.transform.position = bodyColls.transform.GetChild(2).position;
        //똥이 튀어나오는 소리
        soundMgr.PlaySfx(bodyColls.transform.GetChild(2), ReadOnly.Defines.SOUND_SFX_POOP);
    }

    //똥 싸는 동작
    public virtual IEnumerator Pooping()
    {
        Debug.Log("Poop Start");
        statAnim = AnimState.PATROL;
        PlayBoolAnimation();
        bodyColls.SetActive(false);
        mouthColl.gameObject.SetActive(false);

        isEvent = true;
        float t = 0;
        while (t < 1f)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(transform.position - gameMgr.mainCam.transform.position),
                Status.moveSpeed * 4 * Time.deltaTime);
            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        statAnim = AnimState.POOP;
        PlayTriggerAnimation(8);
        headerCanvas.ShowText(14, Random.Range(0, 2));   //똥 싼 뒤 대사

        yield return new WaitForSeconds(3.0f);  //똥싸는애니메이션 재생 시간 or 애니메이션에서 콜백받기
        MakePoop();
        yield return new WaitForSeconds(1.0f);


        headerCanvas.ShowText(15, Random.Range(0, 2));
        isEvent = false;
        bodyColls.SetActive(true);
        mouthColl.gameObject.SetActive(true);
        NextAI(true);
    }

    public IEnumerator HateSmell()
    {
        //냄새 맡기
        PlayTriggerAnimation(7);
        headerCanvas.ShowText(16, 0);
        yield return new WaitForSeconds(1.8f);
        //싫어하는 반응
        headerCanvas.ShowText(16, 1);
        yield return StartCoroutine(BackWalk());

        //AI_Move(2);
        ////도망가기(이벤트 종료)
        //gameMgr.questMgr.dic_quest["Poop"].questStep = 3;
        //gameMgr.questMgr.dic_quest["Poop"].CheckQuestStep();
    }

    /// <summary>
    /// 이동 포인트 변경
    /// </summary>
    /// <param name="_point">0: far / 1: close</param>
    public void ChangeMovePoint(int _point)
    {
        movePoint = stageMgr.arr_headersMovePoints[_point];
        movePoints = movePoint.GetComponentsInChildren<Transform>();
    }
    #endregion
}
