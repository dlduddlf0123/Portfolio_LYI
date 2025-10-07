using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    public enum AIState
    {
        IDLE = 0,
        WALK,
        RUN,
        HIT,
        CALL,
        SLEEP,
        POOP,
    }


    /// <summary>
    /// 240805 LYI
    /// Character AI management, holder
    /// Zenject, State, Command pattern
    /// AI decides what do next with state, perform command
    /// </summary>
    public class CharacterAIManager : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Character Classes")]
        public CharacterManager charMgr;

        public CharacterAIState State;
        public CharacterAICommand Command;

        [Header("MoveTransform")]
        public Transform currentTarget;
        public Transform movePoint;
        public Transform[] movePoints;

        //transform of interaction object
        [Header("Interaction Object Transform")]
        public Transform tr_poop;
        public Transform tr_syringe;

        [Header("Property")]
        public AIState statAI;

        public bool isAction;   //커맨드 수행중인지 여부
        public bool isHit;
        public bool isEvent;

        public Coroutine currentCoroutine = null;

        public virtual void Init()
        {
            gameMgr = GameManager.Instance;

            isEvent = false;
            isAction = false;
            isHit = false;

            AIMove(AIState.WALK);
        }

        /// <summary>
        /// 강제로 동작 멈추기, 일부분 초기화
        /// </summary>
        public virtual void Stop()
        {
            if (isEvent || !gameObject.activeInHierarchy)
            {
                return;
            }

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            isAction = false;

            Debug.Log(charMgr.gameObject.name + "_AI: Stop()");
        }



        /// <summary>
        /// 각 Type에 따른 코루틴을 실행시킨다
        /// </summary>
        /// <param name="_type"></param>
        public virtual void AIMove(AIState state)
        {
            Stop();

            statAI = state;
            Debug.Log(charMgr.gameObject.name + "_AI: " + statAI);

            //AI 동작들
            switch (state)
            {
                case AIState.IDLE: //가만히 있을 때, 정지할 때
                    currentCoroutine = StartCoroutine(Idle());
                    break;
                case AIState.WALK: //배회
                    currentCoroutine = StartCoroutine(PatrolMove());
                    //GetEnergy(-1);
                    //GetHunger(-3);
                    break;
                case AIState.RUN: //달릴 때
                    currentCoroutine = StartCoroutine(Run());
                    //GetEnergy(-2);
                    //GetHunger(-5);
                    break;
                case AIState.HIT: //맞았을 때
                    currentCoroutine = StartCoroutine(Hit());
                    break;
                case AIState.CALL: //부르기
                    currentCoroutine = StartCoroutine(Call());
                    break;
                case AIState.SLEEP:
                    break;
                case AIState.POOP:
                    currentCoroutine = StartCoroutine(Pooping());
                    break;
                default:
                    currentCoroutine = StartCoroutine(Idle());
                    break;
            }

            // Debug.Log(gameObject.name + "AI: " + _type);
        }


        /// <summary>
        /// 각 동작 이후 다음 동작 설정 함수
        /// 각 동작이 끝났을 때 호출
        /// </summary>
        public virtual void NextAI(bool choose)
        {
            isAction = false;

            //Debug.Log("NextAI:" + statAI);
            switch (statAI)
            {
                case AIState.IDLE:
                    if (choose)
                        AIMove(AIState.WALK);
                    else
                        AIMove(AIState.RUN);
                    break;
                case AIState.WALK:
                    if (choose)
                        AIMove(AIState.IDLE);
                    else
                        AIMove(AIState.WALK);
                    break;
                case AIState.RUN:
                    if (choose)
                        AIMove(AIState.IDLE);
                    else
                        AIMove(AIState.WALK);
                    break;
                case AIState.HIT:
                    AIMove(AIState.RUN);
                    break;
                case AIState.CALL:
                    AIMove(AIState.IDLE);
                    break;
                default:
                    AIMove(AIState.IDLE);
                    break;
            }
        }

        /// <summary>
        /// 이동 포인트 변경
        /// </summary>
        /// <param name="point">0: far / 1: close</param>
        public void ChangeMovePoint(int point)
        {
            movePoint = gameMgr.lifeMgr.arr_headersMovePoints[point];
            movePoints = movePoint.GetComponentsInChildren<Transform>();
        }

        Transform GetRandomPoint()
        {
            int randPoint = Random.Range(1, movePoints.Length); //향할 맵 포인트

            Transform target = movePoints[randPoint];

            if (currentTarget != null)
            {

                while (Vector3.Distance(target.position, currentTarget.position) < 0.1f)
                {
                    randPoint = Random.Range(1, movePoints.Length);
                    target = movePoints[randPoint];
                }
            }

            return target;
        }


        #region Normal AI Moves

        //----------------------Coroutines-------------------------
        /// <summary>
        /// AI: 00
        /// 걸으면서 돌아다니기
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator PatrolMove()
        {

            ChangeMovePoint(Random.Range(0, 2));

            currentTarget = GetRandomPoint();

            charMgr.Movement.SetMoveMarker(currentTarget);

            bool move = false;

            if (Random.Range(0, 2) == 0)
                move = true;
            else
                move = false;

            charMgr.Movement.StartMove(() => NextAI(move));
            charMgr.Status.OnMoveWalk();
            yield return null;
        }


        /// <summary>
        /// AI: 01
        /// 맞았을 시 잠시 정지(히트애니메이션)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Hit()
        {
            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.TOK);
            isHit = true;

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
            ChangeMovePoint(0);
            currentTarget = GetRandomPoint();

            charMgr.Movement.SetMoveMarker(currentTarget);

            bool move = false;
            if (Random.Range(0, 2) == 0)
                move = true;
            else
                move = false;

            charMgr.Movement.StartMove(() => NextAI(move));
            charMgr.Status.OnMoveRun();
            yield return null;
        }

        /// <summary>
        /// AI: 03
        /// 대기 동작
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Idle()
        {
            charMgr.Movement.Stop();
            charMgr.Movement.ResetMoveMarker();

            yield return new WaitForSeconds(5f);

            bool move = false;
            if (Random.Range(0, 2) < 1)
            {
                move = true;
            }
            else
            {
                move = false;
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
            currentTarget = GameManager.Instance.MRMgr.XR_Origin.Camera.transform;

            charMgr.Movement.SetMoveMarker(currentTarget);
            charMgr.Movement.StartMove(() => NextAI(true));
            yield return null;
        }

        #endregion


        /// <summary>
        /// 11/13/2024-LYI
        /// 이 캐릭터가 선택됐을 때
        /// </summary>
        public virtual void OnSelect()
        {
            charMgr.Stop();

            //캐릭터 플레이어 바라보기
            charMgr.Movement.LookTarget(GameManager.Instance.MRMgr.XR_Origin.Camera.transform
                , () => AIMove(AIState.IDLE));

            //선택 파티클 호출
            charMgr.Particle.PlayParticleOneShot(ParticleShotType.SELECT);

            Debug.Log(charMgr.gameObject.name + "- OnSelect()");
        }


        #region Touch Interactions

        /// <summary>
        /// 9/5/2024-LYI
        /// 터치 동작이 발생한 경우
        /// </summary>
        public virtual void OnTouchStart(TouchCollider_Direction direction, TouchCollider_HandType handType)
        {
            if (charMgr.Collider.isDelay)
            {
                return;
            }
            if (charMgr.AI.isEvent)
            {
                return;
            }

            charMgr.Stop();
            //charMgr.Animation.TouchAnim(direction);


            switch (handType)
            {
                case TouchCollider_HandType.NORMAL:
                    charMgr.Petting.PettingStart(direction, handType, charMgr.Collider.arr_touchCollider[(int)direction].colledGameObject);
                    break;
                case TouchCollider_HandType.POINT:
                    //찌르기 반응 동작
                    OnTouchPoke();
                    break;
                case TouchCollider_HandType.FIST:
                    //때리기 동작
                    OnTouchFist();
                    break;
                case TouchCollider_HandType.BATH:
                    charMgr.Petting.PettingStart(direction, handType, charMgr.Collider.arr_touchCollider[(int)direction].colledGameObject);
                    break;
                default:
                    charMgr.Petting.PettingStart(direction, handType, charMgr.Collider.arr_touchCollider[(int)direction].colledGameObject);
                    break;
            }

        }

        public virtual void OnTouchEnd(TouchCollider_Direction direction, TouchCollider_HandType handType)
        {
            charMgr.Petting.PettingEnd(direction, handType);
            if (handType == TouchCollider_HandType.BATH)
            {
                charMgr.Status.OnBath();
            }
            else
            {
                charMgr.Status.OnTouch(TouchCollider_HandType.NORMAL, direction);
            }

            charMgr.Particle.PlayParticleOneShot(ParticleShotType.HEART);

            if (charMgr.Gesture.isOnHand)
            {
                //손 위에 있는경우
                //gameMgr.lifeMgr.lifeUIMgr.StatUIRefresh();
            }
            else
            {
                //아닌경우 AI
                AIMove(AIState.IDLE);
            }

        }


        /// <summary>
        /// 12/2/2024-LYI
        /// 찌르기 반응
        /// </summary>
        public void OnTouchPoke()
        {
            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.TOK);
            charMgr.Status.OnTouch(TouchCollider_HandType.POINT, TouchCollider_Direction.NONE);
        }

        /// <summary>
        /// 12/2/2024-LYI
        /// 때리기 반응
        /// </summary>
        public void OnTouchFist()
        {
            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.HIT);
            charMgr.Status.OnTouch(TouchCollider_HandType.FIST, TouchCollider_Direction.NONE);
        }


        #endregion


        #region  Food

        /// <summary>
        /// 10/22/2024-LYI
        /// 음식을 먹었을 때 호출
        /// 스탯 상승, 동작 보여주기
        /// </summary>
        /// <param name="food"></param>
        public void EatFood(Food food)
        {
            if (isEvent)
            {
                return;
            }

            charMgr.Stop();

            charMgr.Status.OnFoodEat(food);


            // TODO: 이펙트, 사운드 추가

            if (food.foodType == charMgr.Status.likeFood)
            {
                //선호
                charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.EAT, (int)StatusLevel.VERY_GOOD);

            }
            else if (food.foodType == charMgr.Status.hateFood)
            {
                //불호
                charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.EAT, (int)StatusLevel.VERY_BAD);
            }
            else
            {
                //기타
                charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.EAT, (int)StatusLevel.GOOD);
            }


            AIMove(AIState.IDLE);
        }

        #endregion

        #region Poop

        //똥 싸는 동작
        public virtual IEnumerator Pooping()
        {
            Debug.Log("Poop Start");

            charMgr.Animation.SetAnimation(AnimationType.IDLE);

            isEvent = true;
            yield return new WaitForSeconds(1f);

            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.POOP);

            yield return new WaitForSeconds(3.0f);  //똥싸는애니메이션 재생 시간 or 애니메이션에서 콜백받기
            gameMgr.lifeMgr.itemSpawner.PoopSpawn(charMgr.AI.tr_poop.transform.position);
            yield return new WaitForSeconds(1.0f);


            isEvent = false;
            AIMove(AIState.IDLE);
        }

        #endregion

        #region On interaction with objects

        /// <summary>
        /// 11/26/2024-LYI
        /// 똥 닿으면
        /// </summary>
        public void OnPoopTouch()
        {

        }



        /// <summary>
        /// 11/26/2024-LYI
        /// 주사 넣을 시, 점프 동작 보여주기, 질병 효과 해제
        /// </summary>
        public void OnSyringeInjected()
        {
            charMgr.Stop();

            charMgr.Status.OnSyringe();
            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.SYRINGE);
        }


        /// <summary>
        /// 12/2/2024-LYI
        /// 중지 올렸을 때 반응
        /// </summary>
        public void OnMiddleUp()
        {
            Debug.Log(charMgr.gameObject.name + "_AI: OnMiddleUp()");
            //플레이어 쳐다보고 뒷걸음 치기 애니, 스탯 적용
            charMgr.Stop();
            charMgr.Status.OnMiddleUp();
            charMgr.Movement.OnMiddleUp();
        }


        /// <summary>
        /// 12/3/2024-LYI
        /// 약속 시 행동
        /// 좋아하기 표현
        /// 기분 MAX
        /// </summary>
        public void OnPromise()
        {
            Debug.Log(charMgr.gameObject.name + "_AI: OnPromise()");
            charMgr.Status.OnPrimise();

            charMgr.Animation.EyeShapeChange(EyeShapeType.SMILE);
            charMgr.Movement.LookTarget(GameManager.Instance.MRMgr.XR_Origin.Camera.transform
              , () => AIMove(AIState.IDLE));
            charMgr.Particle.PlayParticleOneShot(ParticleShotType.HEART);

            gameMgr.lifeMgr.SpawnCharacter();
        }


        /// <summary>
        /// 12/3/2024-LYI
        /// 에너지가 0이되면 호출
        /// 침대로 갈 때 호출
        /// </summary>
        public void OnGoingSleep()
        {
            if (charMgr.Gesture.isOnHand) { return; }

            charMgr.Stop();
            charMgr.Movement.isGoingSleep = true;
            charMgr.Movement.SetMoveMarker(gameMgr.lifeMgr.life_bed.transform);
            charMgr.Movement.StartMove();
        }

        /// <summary>
        /// 12/3/2024-LYI
        /// 잠들었을 때 호출
        /// </summary>
        public void OnSleeping()
        {
            charMgr.Stop();
            charMgr.Movement.isGoingSleep = false;
            charMgr.Status.StartSleep();

            isEvent = true;
            charMgr.Movement.SetFixedMode(true);

            charMgr.Animation.SetAnimation(AnimationType.SLEEP);
            charMgr.Animation.EyeLidChange(EyeLidShape.CLOSE);
            charMgr.Animation.EyeShapeChange(EyeShapeType.BASE);
        }

        public void EndSleep()
        {
            isEvent = false;
            charMgr.Movement.SetFixedMode(false);
            charMgr.Movement.ResetParent();
            charMgr.Stop();

            charMgr.Animation.EyeLidChange(EyeLidShape.OPEN);
            charMgr.Animation.EyeShapeReset();

            charMgr.Status.StopSleep();
            charMgr.Status.CheckAllStatLevel();

            AIMove(AIState.WALK);
        }


        #endregion
    }
}