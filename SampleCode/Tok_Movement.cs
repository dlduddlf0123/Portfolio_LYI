using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;
using UnityEngine.Events;
using VRTokTok.Interaction;

namespace VRTokTok.Character
{
   public enum MoveState
    {
        NONE = 0,
        START,
        WALK,
        RUN,
        STOP,
    }
    public class Tok_Movement : MonoBehaviour
    {
        protected GameManager gameMgr;

        public Tok_Character m_character;

        public Rigidbody m_rigidbody;
        [SerializeField]
        protected CapsuleCollider m_bodyColl;


        [Header("MMF_Player")]
        public MMF_Player[] arr_feedback;

        [Header("Transform Properties")]
        public Transform defaultParent; //플랫폼 parent 리셋용
        public Transform moveMarker; //이동용 마커, 점프 계산용
        public Transform tr_tower; //탑 쌓기 시 다른 캐릭터가 올라갈 위치

        //내부 변수
        private Vector3 moveTargetPos;

        [Header("GamePlay Properties")]
        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public float rotationSpeed = 10f;

        public float maxJumpDistance = 0.05f; //최대 점프 거리
        public float maxJumpHeight = 0.1f; //최대 점프 높이
        public float minJumpHeight = 0.03f; //최소 점프 높이

        public float actJumpHeight = 0.1f; //점프 높이
        public float actJumpDuration = 0.2f; //점프 시간
        public float jumpStartWaitTime = 0.16f;

        public float destinationDistance = 0.03f; //어느정도 거리에서 멈출 것인가
        public float shortMoveDistance = 0.06f; //짧은 이동거리 체크

        [Header("Move Status")]
        public bool isGround; //바닥에 닿아있는지

        public bool isTowerAble; //탑 쌓기가 가능한지

        [Header("Update Properties")]
        public float remainDistance; //현재 이동중에 남은 거리값
        public float movingVelocity; //현재 이동중인 속도

        public MoveState statMove = MoveState.NONE;

        public bool isMove = false; //이동 중인지 체크
        public bool isAction = false;    //커맨드 수행중인지 여부
        public bool isDie = false;
        public bool isHit = false;
        public bool isEvent = false; //타임라인 등을 통한 컷씬 진행 여부
        public bool isJump = false; //중복 점프 동작 방지
        public bool isCrushAble= false; //충돌 활성화

        float sturnTime = 1.2f;
        float crushAnimMoveTime = 0.12f;


        public Coroutine currentAI = null;

        bool isInit = false;

        int jumpRayMask;

        /// <summary>
        /// 7/13/2023-LYI
        /// 모든 캐릭터 동작 초기화 후 Idle
        /// 스테이지 시작, 재시작 시 호출
        /// </summary>
        public virtual void Init()
        {
            if (!isInit)
            {
                gameMgr = GameManager.Instance;
                //  m_character = GetComponent<Tok_Character>();
                m_rigidbody = GetComponent<Rigidbody>();

                defaultParent = transform.parent;

                jumpRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                    (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));


                isInit = true;
            }

            ResetParent();

            m_rigidbody.isKinematic = false;
            m_rigidbody.useGravity = true;

            transform.rotation = Quaternion.identity;
            isDie = false;

            Stop();
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

            Debug.Log(gameObject.name + "Stop()");

            if (currentAI != null)
            {
                StopCoroutine(currentAI);
                currentAI = null;
            }
            // m_AIPath.isStopped = true;
            isAction = false;
            isJump = false;
            isCrushAble = false;

            isMove = false;
            m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, false);
            m_rigidbody.velocity = Vector3.zero;

            // Debug.Log(gameObject.name + "AI: Stop()");
            m_character.SetAnimation(AnimationType.IDLE);
        }

        public void ResetParent()
        {
            transform.SetParent(defaultParent);
            //m_rigidbody.isKinematic = false;
        }

        public void SetParentMenu(Transform tr)
        {
            transform.SetParent(tr);
            m_rigidbody.isKinematic = true;
        }

        private void OnDisable()
        {
            Stop();
        }

        //private void OnCollisionEnter(Collision coll)
        //{
        //    if (coll.gameObject.layer == LayerMask.NameToLayer(Constants.Layer.LAYERMASK_GROUND))
        //    {
        //        isGround = true;
        //        //m_character.m_animator.SetBool("isGround", isGround);
        //    }
        //}
        //private void OnCollisionExit(Collision coll)
        //{
        //    if (coll.gameObject.layer == LayerMask.NameToLayer(Constants.Layer.LAYERMASK_GROUND))
        //    {
        //        isGround = false;
        //        //m_character.m_animator.SetBool("isGround", isGround);
        //    }
        //}

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Missile"))
            {
                //미사일 맞는 모션
                OnFail(GameOverType.MISSILE);
            }
            if (coll.gameObject.CompareTag("Trap"))
            {
                //함정 맞는 모션
                OnFail(GameOverType.SPIKE);
            }
        }


        private void Update()
        {
            RayCheckGround();
        }


        /// <summary>
        /// 5/13/2024-LYI
        /// 대포에서 호출
        /// 오브젝트 상호작용 시 캐릭터 정지
        /// </summary>
        /// <param name="isActive"></param>
        public void FixedMode(bool isActive)
        {
            Debug.Log(this.gameObject.name + "FixedMode: " + isAction.ToString());
            if (isActive)
            {
                Stop();
               // m_rigidbody.isKinematic = true;
                UseGravity(false);
                ColliderActive(false);
                 //isAction = true;
            }
            else
            {
                //m_rigidbody.isKinematic = false;
                UseGravity(true);
                ColliderActive(true);
                //isAction = false;
            }
        }
        public void ColliderActive(bool isActive)
        {
            m_bodyColl.enabled = isActive;
        }

        public void UseGravity(bool isActive)
        {
            m_rigidbody.useGravity = isActive;
        }
        public void ChangeFriction(PhysicMaterial mat = null)
        {
            if (mat != null)
            {
                m_bodyColl.material = mat;
            }
            else
            {
                m_bodyColl.material = null;
            }
        }

        /// <summary>
        /// 5/30/2024-LYI
        /// 캐릭터 발사
        /// </summary>
        public void FireHeader(Vector3 force, ForceMode mode)
        {
            isCrushAble = true;
            m_rigidbody.AddForce(force, mode);
        }


        /// <summary>
        /// 9/4/2023-LYI
        /// 동작 중단, 부딪힘 애니메이션 재생
        /// </summary>
        public void OnCrash()
        {
            //10/6/2023-LYI 속도 제한 관련 조건문 추가할것
            //6/26/2024-LYI 속도 제한 조건 제거
            if (!isInit)
            {
                return;
            }

            Debug.Log(gameObject.name + ": OnCrash()");

            //3/5/2024-LYI
            //클리어 했을 때 포탈 뒤 벽에 부딛히면 사라지는 효과 
            //+ 효과음 재생
            if (gameMgr.playMgr.statPlay == Manager.PlayStatus.CLEAR)
            {
                isAction = false;
                gameObject.SetActive(false);

                gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_STAGE_CLEAR);
                return;
            }

            //점프 중에 부딪힘만 남기기
            if (isAction || isGround)
            {
                return;
            }

            if (isCrushAble)
            {
                Stop();

                currentAI = StartCoroutine(CrashCoroutine());
            }
        }

        IEnumerator CrashCoroutine()
        {

            //give sturn
            isAction = true;

            //부딪힘 애니메이션 재생
            m_character.PlayTriggerAnimation(TriggerAnimationType.CRASH);
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_CRUSH); 

            m_rigidbody.isKinematic = true;

            float timeCount = 0;
            yield return new WaitForSeconds(0.03f);
            timeCount += 0.03f;
            //Transform moveObj = transform.GetChild(0);
            Vector3 startPos = transform.position;
            Vector3 destination = startPos - transform.forward * 0.0435f;


            float duration = crushAnimMoveTime;
            float elapsedTime = 0.0f;
            float t = 0;


            while (elapsedTime < crushAnimMoveTime)
            {
                t = elapsedTime / duration;

                transform.position = Vector3.Lerp(transform.position, destination, t);

                elapsedTime += 0.005f;
                yield return new WaitForSeconds(0.01f);
            }

            m_rigidbody.isKinematic = false;
            timeCount += elapsedTime;

            yield return new WaitForSeconds(sturnTime - timeCount);

            isAction = false;
        }

        /// <summary>
        /// 8/28/2023-LYI
        /// StageManager에서 호출
        /// 게임 시작 시, 재시작 시 등장 연출 호출
        /// </summary>
        public void OnCharacterAppear()
        {
            if (isEvent)
            {
                return;
            }
            gameObject.SetActive(true);

            isAction = true;
            currentAI = StartCoroutine(FlatJumpCoroutine(OnCharacterReady));

            //m_character.CharacterAppear(() =>
            //{
            //    m_rigidbody.isKinematic = false;
            //    OnCharacterReady();
            //});

            //for (int i = 0; i < arr_feedback.Length; i++)
            //{
            //    arr_feedback[i].PlayFeedbacks();
            //}
        }

        /// <summary>
        /// 8/28/2023-LYI
        /// OnCharacterAppear 이후 호출
        /// 등장 효과 이후 움직임 관련 준비
        /// </summary>
        public void OnCharacterReady()
        {
            isAction = false;
            if (gameMgr.playMgr.selectCharacterType == m_character.typeHeader)
            {
                if (isGround)
                {
                    gameMgr.playMgr.tokMgr.SelectCharacter(this);
                }
                else
                {
                   currentAI =  StartCoroutine(GroundWait());
                }
            }

            //m_character.efx_portal.Stop();
        }

        IEnumerator GroundWait()
        {
            while (!isGround)
            {
                yield return null;
            }
            gameMgr.playMgr.tokMgr.SelectCharacter(this);
        }


        /// <summary>
        /// 10/31/2023-LYI
        /// 실패 시 실패 동작
        /// </summary>
        public void OnFail(GameOverType type)
        {
            if (isDie)
            {
                return;
            }
            Stop();

            isAction = true;
            isDie = true;

            ResetParent();
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.isKinematic = true;

            switch (type)
            {
                case GameOverType.NONE:
                    gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_DIE_HIT);
                    break;
                case GameOverType.WATER:
                    gameMgr.soundMgr.PlaySfx(transform.position,Constants.Sound.SFX_HEADER_DIE_WATER);
                    break;
                case GameOverType.SPIKE:
                    gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_TRAP_SPIKE_HIT);
                    break;
                case GameOverType.FLAME:
                    gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_TRAP_FLAME_HIT);
                    break;
                case GameOverType.MISSILE:
                    gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_TRAP_MISSILE_HIT);
                    break;
                case GameOverType.PRESS:
                    break;
                default:
                    gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_DIE_HIT);
                    break;
            }

            m_character.OnGameOver(type, () =>
            {
                isAction = false;
                isDie = false;

                m_rigidbody.isKinematic = false;
                if (gameMgr.playMgr.currentStage !=null)
                {
                    gameMgr.playMgr.currentStage.RestartStage(true);
                }
            });

        }


        /// <summary>
        /// 10/31/2023-LYI
        /// 성공 시 성공 동작
        /// </summary>
        public void OnClear()
        {
            if (isDie)
            {
                return;
            }
            Stop();
            m_rigidbody.velocity = Vector3.zero;

            isAction = true;
            currentAI = StartCoroutine(FlatJumpCoroutine(() => isAction = false));
            //m_character.PlayTriggerAnimation(TriggerAnimationType.CLEAR);
        }


        #region Debug Jump Distance

        //10/16/2023-LYI
        //Editor 기즈모로 표시 변경
        //public int circleSegments = 36;

        //private void Update()
        //{
        //    for (int i = 0; i < circleSegments; i++)
        //    {
        //        float angle = i * (360f / circleSegments);
        //        Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
        //        Debug.DrawRay(transform.position, rayDirection * maxJumpDistance, Color.green);
        //    }
        //}
        #endregion

        #region Tok / TokTok Input Function


        /// <summary>
        /// 10/16/2023-LYI
        /// 캐릭터가 선택되었을 때 호출
        /// </summary>
        public virtual void OnSelect()
        {
            Debug.Log(gameObject.name + ": OnSelect()");

            m_character.PlayTriggerAnimation(TriggerAnimationType.TOK);

            StartCoroutine(EnableColliderWithDelay());
        }
        IEnumerator EnableColliderWithDelay()
        {
            // 컬리더 비활성화
            m_bodyColl.enabled = false;
            // 1프레임 대기
            yield return new WaitForEndOfFrame();
            // 컬리더 다시 활성화
            m_bodyColl.enabled = true;
        }


        /// <summary>
        /// 10/16/2023-LYI
        /// 캐릭터가 선택 이후 더블클릭된 경우?
        /// </summary>
        public virtual void OnDoubleSelect()
        {
            Debug.Log(gameObject.name + ": OnDoubleSelect()");
            //탑 해제
        }

        /// <summary>
        /// 8/30/2023-LYI
        /// Display TokManager에서 호출
        /// 톡 입력 시 행동 결정
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="go"></param>
        public virtual void OnTok(Vector3 destination, GameObject go)
        {
            if (isAction || isMove) { return; }

            Debug.Log(gameObject.name + ": OnTok()");
            AI_LookAt(destination);
        }


        /// <summary>
        /// 8/30/2023-LYI
        /// Display TokManager에서 호출
        /// TokManager에서 톡톡 입력됐을 때 현재 캐릭터 동작 호출
        /// 기본은 이동, 거리판단과 타겟 판단을 통해 상호작용, 이동, 점프를 나눈다
        /// </summary>
        public virtual void OnTokTok(Vector3 destination, GameObject go)
        {
            if (isAction || isJump) { return; }
            Debug.Log(gameObject.name + ": OnTokTok()");
            float targetDistance = Vector3.Distance(destination, transform.position);


            moveTargetPos = destination;
            moveMarker.position = moveTargetPos;
            moveMarker.transform.SetParent(go.transform);

            //탑쌓기가 가능한 경우
            //if (isTowerAble)
            //{
            //    Tok_Movement toUp;
            //    //탑 쌓기 액션 진행
            //    if (go.TryGetComponent<Tok_Movement>(out toUp))
            //    {
            //        Debug.Log(gameObject.name + ": Tower()");

            //    }
            //    return;
            //}

            // if (targetDistance < maxJumpDistance && isGround)

            //점프가 가능한 경우
            if (targetDistance < maxJumpDistance &&
                isGround)
            {
                //인터렉션인 경우
                if (gameMgr.playMgr.tokMgr.tokMarker.clickedInteraction != null &&
                    gameMgr.playMgr.currentStage.Data.typeStage != Manager.StageType.CROSSING)
                {
                    OnTokJumpInteract(destination, go);
                }
                else 
                {
                    OnTokJump(destination, go);
                }
            }
            else
            {
                //점프가 불가능한 경우
                AI_Move();
            }

        }

        protected void OnTokJump(Vector3 destination, GameObject go)
        {
            if (gameMgr.playMgr.currentStage.Data.typeStage == Manager.StageType.CROSSING)
            {
                if (HeightJumpRayCheck(destination))
                {
                    //Destination 높이 차이 확인
                    AI_HeightJump();
                }
                else
                {
                    //땅이 비었으면 점프
                    if (FlatJumpRayCheck(destination))
                    {
                        AI_FlatJump();
                    }
                    else
                    {
                        AI_Move();
                    }
                }
                isCrushAble = true;

                return;
            }

            if (HeightJumpRayCheck(destination) &&
                 ObstacleRayCheck(destination))
            {
                //Destination 높이 차이 확인
                AI_HeightJump();
            }
            else
            {
                //땅이 비었으면 점프
                if (FlatJumpRayCheck(destination) &&
                 ObstacleRayCheck(destination))
                {
                    AI_FlatJump();
                }
                else
                {
                    AI_Move();
                }
            }
        }

        protected void OnTokJumpInteract(Vector3 destination, GameObject go)
        {
            switch (gameMgr.playMgr.tokMgr.tokMarker.clickedInteraction.GetType().Name)
            {
                case Constants.InteractName.INTERACT_JUMP_BLOCK:
                case Constants.InteractName.INTERACT_BUTTON:
                    if (ObstacleGroundRayCheck(destination))
                    {
                        AI_FlatJump();
                    }
                    else
                    {
                        AI_Move();
                    }
                    break;
                case Constants.InteractName.INTERACT_PILLAR:
                    if (HeightJumpRayCheck(destination) &&
                  ObstacleGroundRayCheck(destination))
                    {
                        AI_HeightJump();
                    }
                    else if (FlatJumpRayCheck(destination) &&
                  ObstacleGroundRayCheck(destination))
                    {
                        AI_FlatJump();
                    }
                    else
                    {
                        AI_Move();
                    }
                    break;
                default:
                    OnTokJump(destination, go);
                    break;
            }
        }
        #endregion

        #region AI Movement Funtions
        /// <summary>
        /// 8/30/2023-LYI
        /// 톡 했을 때 방향 바라보기
        /// </summary>
        /// <param name="target"></param>
        private void AI_LookAt(Vector3 target)
        {
            if (!gameObject.activeSelf) { return; }

            if (currentAI != null)
            {
                Stop();
                currentAI = null;
            }
            currentAI = StartCoroutine(RotateToPosition(target));
        }


        /// <summary>
        /// 8/30/2023-LYI
        /// 클릭된 위치로 이동하기
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="go"></param>
        protected virtual void AI_Move()
        {
            if (!gameObject.activeSelf || isAction ||!isGround) { return; }

            if (currentAI != null)
            {
                Stop();
                currentAI = null;
            }

            Debug.Log(gameObject.name + ": Move()");
            currentAI = StartCoroutine(MoveToPositionCoroutine());

            //if (gameMgr.playMgr.tokMgr.tokMarker.clickedButton != null &&
            //    gameMgr.playMgr.tokMgr.tokMarker.clickedButton.type_button == Interaction.ButtonType.CLICK &&
            //    gameMgr.playMgr.tokMgr.tokMarker.clickedButton.isInteractable)
            //{
            //    //클릭 가능한 버튼인 경우

            //    Debug.Log(gameObject.name + ": ButtonMove()");
            //    currentAI = StartCoroutine(MoveToButtonCoroutine());
            //}
            //else
            //{
            //    Debug.Log(gameObject.name + ": Move()");
            //    currentAI = StartCoroutine(MoveToPositionCoroutine());
            //}

        }


        /// <summary>
        /// 8/30/2023-LYI
        /// 클릭된 위치로 점프하기
        /// </summary>
        private void AI_FlatJump()
        {
            if (!gameObject.activeSelf || isAction) { return; }

            ResetParent();

            if (currentAI != null)
            {
                Stop();
                currentAI = null;
            }

            Debug.Log(gameObject.name + ": FlatJump()");
            currentAI = StartCoroutine(FlatJumpCoroutine());
        }

        private void AI_HeightJump()
        {
            if (!gameObject.activeSelf || isAction) { return; }

            ResetParent();

            if (currentAI != null)
            {
                Stop();
                currentAI = null;
            }

            Debug.Log(gameObject.name + ": HeightJump()");
            currentAI = StartCoroutine(HeightJumpCoroutine());
        }


        #endregion

        #region Movement
        private IEnumerator MoveToPositionCoroutine()
        {
            Vector3 target = new Vector3(moveTargetPos.x, transform.position.y, moveTargetPos.z);
            remainDistance = Vector3.Distance(target, transform.position);

            if (statMove == MoveState.NONE)
            {
                statMove = MoveState.START;

                //4/4/2024-LYI
                //시작 시 동작 제거
              //  m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, true);

                if (Random.Range(0, 3) == 0 ||
                    Mathf.Abs(remainDistance - destinationDistance) < shortMoveDistance)
                {
                    statMove = MoveState.WALK;
                    m_character.SetAnimation(AnimationType.WALK);
                }
                else
                {
                    statMove = MoveState.RUN;
                    m_character.SetAnimation(AnimationType.RUN);

                    //Debug.Log(gameObject.name + "StartStoped()");
                    //isAction = true;
                    //yield return new WaitForSeconds(1f);
                    //isAction = false;
                }
              // m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, false);
            }


            if (statMove == MoveState.WALK)
            {
                m_character.SetAnimation(AnimationType.WALK);
            }
            if (statMove == MoveState.RUN)
            {
                m_character.SetAnimation(AnimationType.RUN);
            }

            Quaternion originRot = transform.rotation;
            Quaternion targetRotation = GetTargetRotation(target);
            float t = 0;

            isMove = true;

            WaitForSeconds wait = new WaitForSeconds(0.01f);

            // Move to the target position
            while (remainDistance > destinationDistance)
            {
                target = new Vector3(moveTargetPos.x, transform.position.y, moveTargetPos.z);
                remainDistance = Vector3.Distance(target, transform.position);
                // Debug.Log("RemainDistance: " + remainDistance);
                targetRotation = GetTargetRotation(target);

                transform.rotation = Quaternion.Lerp(originRot, targetRotation, t);

                Move((target - transform.position).normalized);

                t += 0.01f * rotationSpeed;
                yield return wait;
            }

            statMove = MoveState.STOP;

            m_rigidbody.velocity = Vector3.zero; // Stop the character
            isMove = false;

            m_character.SetAnimation(AnimationType.IDLE);

            //RunBreak();

            statMove = MoveState.NONE;
        }


        /// <summary>
        /// 11/22/2023-LYI
        /// 버튼으로 이동하는 경우
        /// 기존과 같이 이동하지만 버튼 앞에 멈춘 뒤 점프해서 누르고 돌아온다
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveToButtonCoroutine()
        {

            Vector3 target = new Vector3(moveTargetPos.x, transform.position.y, moveTargetPos.z);
            remainDistance = Vector3.Distance(target, transform.position);


            if (statMove == MoveState.NONE)
            {
                statMove = MoveState.START;
                m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, true);

                if (Random.Range(0, 3) == 0 ||
                    remainDistance - destinationDistance < shortMoveDistance)
                {
                    statMove = MoveState.WALK;
                    m_character.SetAnimation(AnimationType.WALK);
                }
                else
                {
                    statMove = MoveState.RUN;
                    m_character.SetAnimation(AnimationType.RUN);

                    Debug.Log(gameObject.name + "StartStoped()");
                    isAction = true;
                    yield return new WaitForSeconds(1f);
                    isAction = false;
                }
                m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, false);
            }


            if (statMove == MoveState.WALK)
            {
                m_character.SetAnimation(AnimationType.WALK);
            }
            if (statMove == MoveState.RUN)
            {
                m_character.SetAnimation(AnimationType.RUN);
            }

            Quaternion originRot = transform.rotation;
            Quaternion targetRotation = GetTargetRotation(target);
            float t = 0;

            isMove = true;

            WaitForSeconds wait = new WaitForSeconds(0.01f);

            // Move to the target position
            while (remainDistance > destinationDistance + shortMoveDistance)
            {
                target = new Vector3(moveTargetPos.x, transform.position.y, moveTargetPos.z);
                remainDistance = Vector3.Distance(target, transform.position);
                // Debug.Log("RemainDistance: " + remainDistance);
                targetRotation = GetTargetRotation(target);

                transform.rotation = Quaternion.Lerp(originRot, targetRotation, t);

                Move((target - transform.position).normalized);

                t += 0.01f * rotationSpeed;
                yield return wait;
            }

            statMove = MoveState.STOP;

            m_rigidbody.velocity = Vector3.zero; // Stop the character
            isMove = false;

            m_character.SetAnimation(AnimationType.IDLE);

            //RunBreak();

            statMove = MoveState.NONE;

            isAction = true;

            yield return new WaitForSeconds(0.5f);
            Vector3 startPos = transform.position;
            yield return StartCoroutine(FlatJumpCoroutine());

            yield return new WaitForSeconds(0.5f);
            moveMarker.position = startPos;
            yield return StartCoroutine(FlatJumpCoroutine());

            isAction = false;

            m_rigidbody.velocity = Vector3.zero; // Stop the character
            isMove = false;

            m_character.SetAnimation(AnimationType.IDLE);

            statMove = MoveState.NONE;

        }
        void RunBreak()
        {
            //if (isRun)
            //{
            //    //멈추는 동작 호출 시
            //    isAction = true;
            //    float lerpSpeed = 2f;
            //    t = 0;

            //    Vector3 startPos = transform.position;
            //    Vector3 targetPos = new Vector3(moveMarker.position.x, transform.position.y, moveMarker.position.z);

            //    while (t < 1 &&
            //        statMove == MoveState.STOP)
            //    {
            //        t += 0.01f * lerpSpeed;

            //        transform.position = Vector3.Lerp(startPos, targetPos, t);
            //        yield return wait;
            //    }
            //    isAction = false;
            //}
            //else
            //{
            //    m_character.SetAnimation(AnimationType.IDLE);
            //}
        }


        private void Move(Vector3 moveDirection)
        {
            // Calculate the movement direction
            moveDirection.y = 0f; // Ensure the movement stays on the ground plane
            moveDirection.Normalize();

            // Set the character's velocity directly
            m_rigidbody.velocity = moveDirection * moveSpeed + new Vector3(0f, m_rigidbody.velocity.y, 0f);
        }
        #endregion

        #region Jump
        

        /// <summary>
        /// 5/10/2024-LYI
        /// 점프대 등에서 호출
        /// 캐릭터를 지정된 위치로 점프 / 날리기?
        /// </summary>
        /// <param name="targetPos"></param>
        public void CharacterJump(Vector3 targetPos)
        {
            if (!gameObject.activeSelf || isAction) { return; }

            ResetParent();

            if (currentAI != null)
            {
                Stop();
                currentAI = null;
            }

            Debug.Log(gameObject.name + ": CharacterJump()");
            currentAI = StartCoroutine(CharacterJumpCoroutine(targetPos));
        }


        /// <summary>
        /// 10/17/2023-LYI
        /// LookRotation 관리 함수
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Quaternion GetTargetRotation(Vector3 target)
        {
            Vector3 ta = new Vector3(target.x, transform.position.y, target.z);
            Vector3 pos = ta - transform.position;
            return Quaternion.LookRotation(pos);
        }


        /// <summary>
        /// 8/30/2023-LYI
        /// Transform 기반 점프
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlatJumpCoroutine(UnityAction action = null)
        {
            isJump = true;

            Quaternion originRot = transform.rotation;
            Quaternion targetRot = GetTargetRotation(moveMarker.position);

            m_character.PlayTriggerAnimation(TriggerAnimationType.JUMP);

            yield return new WaitForSeconds(jumpStartWaitTime);


            float randomRange = 0.2f;
            float pitch = Random.Range(1 - randomRange, 1 + randomRange);
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_JUMP, pitch);

            float t = 0;

            Vector3 startJumpPosition = transform.position;
            Vector3 targetJumpPosition = moveMarker.position;
            float jumpStartTime = Time.time;

            while (Time.time - jumpStartTime < actJumpDuration)
            {
                t += 0.01f * rotationSpeed;

                transform.rotation = Quaternion.Lerp(originRot, targetRot, t);

                //float normalizedTime = (Time.time - jumpStartTime) / actJumpDuration;

                //targetJumpPosition = moveMarker.position;
                //Vector3 newPosition = Vector3.Lerp(startJumpPosition, targetJumpPosition, normalizedTime);
                //newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight;

                //transform.position = newPosition;

                float normalizedTime = (Time.time - jumpStartTime) / actJumpDuration;

                targetJumpPosition = moveMarker.position;
                Vector3 newPosition = Vector3.Lerp(startJumpPosition, targetJumpPosition, normalizedTime);

                // Adjust the following line to make the jump height increase more gradually
                //newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight * Mathf.Pow(1 - normalizedTime, 2);
                float additionalInterpolation = 1 - Mathf.Pow(1 - normalizedTime, 3);
                newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight *0.5f * additionalInterpolation;


                transform.position = newPosition;
                yield return null;
            }

            StartCoroutine(EnableColliderWithDelay());
            // Ensure the character reaches the final destination
            transform.position = targetJumpPosition;
            m_rigidbody.velocity = Vector3.zero;

            m_character.SetAnimation(AnimationType.IDLE);

            isJump = false;

            if (action != null)
            {
                action.Invoke();
            }
        }


        /// <summary>
        /// 10/16/2023-LYI
        /// 위, 아래 점프 시 호출
        /// </summary>
        /// <returns></returns>
        private IEnumerator HeightJumpCoroutine()
        {
            isJump = true;

            Quaternion originRot = transform.rotation;
            Quaternion targetRot = GetTargetRotation(moveMarker.position);

            m_character.PlayTriggerAnimation(TriggerAnimationType.JUMP);
            yield return new WaitForSeconds(jumpStartWaitTime);

            float randomRange = 0.2f;
            float pitch = Random.Range(1 - randomRange, 1 + randomRange);
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_JUMP, pitch);
            float t = 0;

            Vector3 startJumpPosition = transform.position;
            Vector3 targetJumpPosition = moveMarker.position;
            float jumpStartTime = Time.time;


            while (Time.time - jumpStartTime < actJumpDuration)
            {
                t += 0.01f * rotationSpeed;

                transform.rotation = Quaternion.Lerp(originRot, targetRot, t);

                targetJumpPosition = moveMarker.position + Vector3.up * 0.02f;
                float normalizedTime = (Time.time - jumpStartTime) / actJumpDuration;

                Vector3 newPosition = Vector3.Lerp(startJumpPosition, targetJumpPosition, normalizedTime);
                //newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight * Mathf.Pow(1 - normalizedTime, 2);
                float additionalInterpolation = 1 - Mathf.Pow(1 - normalizedTime, 3);
                newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight * additionalInterpolation;

                transform.position = newPosition;

                yield return null;
            }
            StartCoroutine(EnableColliderWithDelay());

            // Ensure the character reaches the final destination
            transform.position = targetJumpPosition;
            m_rigidbody.velocity = Vector3.zero;
            m_character.SetAnimation(AnimationType.IDLE);

            isJump = false;
        }

        /// <summary>
        /// 5/10/2024-LYI
        /// 타의적 점프
        /// 점프대 등에서 호출
        /// </summary>
        /// <returns></returns>
        private IEnumerator CharacterJumpCoroutine(Vector3 targetPos)
        {
            isJump = true;

            Quaternion originRot = transform.rotation;
            Quaternion targetRot = GetTargetRotation(targetPos);

            m_character.PlayTriggerAnimation(TriggerAnimationType.JUMP);
            yield return new WaitForSeconds(jumpStartWaitTime);

            float randomRange = 0.2f;
            float pitch = Random.Range(1 - randomRange, 1 + randomRange);
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_JUMP, pitch);
            float t = 0;

            Vector3 startJumpPosition = transform.position;
            Vector3 targetJumpPosition = targetPos;
            float jumpStartTime = Time.time;


            while (Time.time - jumpStartTime < actJumpDuration)
            {
                t += 0.01f * rotationSpeed;

                transform.rotation = Quaternion.Lerp(originRot, targetRot, t);

                targetJumpPosition = targetPos + Vector3.up * 0.02f;
                float normalizedTime = (Time.time - jumpStartTime) / actJumpDuration;

                Vector3 newPosition = Vector3.Lerp(startJumpPosition, targetJumpPosition, normalizedTime);
                //newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight * Mathf.Pow(1 - normalizedTime, 2);
                float additionalInterpolation = 1 - Mathf.Pow(1 - normalizedTime, 3);
                newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight * additionalInterpolation;

                transform.position = newPosition;

                yield return null;
            }
            StartCoroutine(EnableColliderWithDelay());

            // Ensure the character reaches the final destination
            transform.position = targetJumpPosition;
            m_rigidbody.velocity = Vector3.zero;
            m_character.SetAnimation(AnimationType.IDLE);

            isJump = false;
        }

        #endregion

        #region Rotate
        IEnumerator RotateToPosition(Vector3 target)
        {
            m_character.PlayTriggerAnimation(TriggerAnimationType.TOK);

            Quaternion originRot = transform.rotation;
            Quaternion targetRot = GetTargetRotation(target);

            float t = 0;
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (t < 1)
            {
                t += 0.01f * rotationSpeed;
                Quaternion q = Quaternion.Lerp(originRot, targetRot, t);
                transform.rotation = q;

                yield return wait;
            }

            m_character.SetAnimation(AnimationType.IDLE);
        }
        #endregion

        #region Raycast

        /// <summary>
        /// 8/22/2023-LYI
        /// Check ground to direction is empty
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        private bool FlatJumpRayCheck(Vector3 destination)
        {
            //높이 차 계산
            if (Mathf.Abs(transform.position.y - destination.y) > minJumpHeight)
            {
                //높으면 점프 안함
                return false;
            }

            Vector3 start = transform.position;
            Vector3 direction = new Vector3(destination.x - start.x, start.y, destination.z - start.z);

            float distance = direction.magnitude;
            Vector3 step = direction.normalized * 0.05f; // Adjust the step size based on your needs

            float minCheckDistance = 0.02f;

            int fullCount = 0;
            int falseCount = 0;
            float rayLengthY = 0.05f;

            // Perform a series of raycasts along the path
            for (float i = minCheckDistance; i < distance - minCheckDistance; i += step.magnitude)
            {
                fullCount++;
                Vector3 rayStart = start + direction.normalized * i + Vector3.up * 0.01f;
                rayStart = new Vector3(rayStart.x, start.y + minJumpHeight, rayStart.z);
                RaycastHit hit;


                if (Physics.Raycast(rayStart, Vector3.down, out hit, rayLengthY, ~jumpRayMask))
                {
                    Debug.DrawRay(rayStart, Vector3.down * rayLengthY, Color.red, 5f);

                    falseCount = 0;
                }
                else
                {
                    Debug.DrawRay(rayStart, Vector3.down * rayLengthY, Color.blue, 5f);

                    falseCount++;
                    if (falseCount > 2)
                    {
                        Debug.Log("Jump: true - " + fullCount + ":" + falseCount);
                        return true;
                    }
                }

            }

            Debug.Log("Jump: false - " + fullCount + ":" + falseCount);
            return false;
        }

        /// <summary>
        /// 3/7/2024-LYI
        /// 버튼 점프 시 정면 체크
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        private bool ObstacleGroundRayCheck(Vector3 destination)
        {
            Vector3 start = transform.position + Vector3.up * minJumpHeight;
            Vector3 direction = (destination + Vector3.up * minJumpHeight) - start;

            float targetDistance = Vector3.Distance(destination, transform.position);

            RaycastHit hit;

            int frontRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                    (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                       (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));

            if (Physics.Raycast(start, direction, out hit, targetDistance, ~frontRayMask))
            {
                Debug.DrawRay(start, direction.normalized * targetDistance, Color.magenta, 5f);
                Debug.Log(" - ObstacleCheck: true - Colled with " + hit.collider.gameObject.name);
                return false;
            }
            else
            {
                Debug.DrawRay(start, direction.normalized * targetDistance, Color.green, 5f);

                Debug.Log(" - ObstacleCheck: false");
                return true;
            }
        }
        private bool ObstacleRayCheck(Vector3 destination)
        {
            Vector3 start = transform.position + Vector3.up * minJumpHeight;
            Vector3 direction = (destination + Vector3.up * minJumpHeight) - start;

            float targetDistance = Vector3.Distance(destination, transform.position);

            RaycastHit hit;

            int frontRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                    (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_GROUND)) |
                       (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));

            if (Physics.Raycast(start, direction, out hit, targetDistance, ~frontRayMask))
            {
                Debug.DrawRay(start, direction.normalized * targetDistance, Color.magenta, 5f);
                Debug.Log(" - ObstacleCheck: true - Colled with " + hit.collider.gameObject.name);
                return false;
            }
            else
            {
                Debug.DrawRay(start, direction.normalized * targetDistance, Color.green, 5f);

                Debug.Log(" - ObstacleCheck: false");
                return true;
            }
        }

        /// <summary>
        /// 10/16/2023-LYI
        /// 점프 체크 조건식
        /// 점프 가능 범위 이내면 점프 가능 체크
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        private bool HeightJumpRayCheck(Vector3 destination)
        {
            //if (!isGround)
            //{ //공중이면 안함
            //    return false;
            //}
            //높이 차 계산
            if (Mathf.Abs(transform.position.y - destination.y) < minJumpHeight)
            {
                //너무 평평하면 높이 점프 안함
                Debug.Log(gameObject.name + "- HeightJump: false");
                return false;
            }

          


            Vector3 upRayMax = transform.position + Vector3.up * maxJumpHeight;
            Vector3 downRayMax = transform.position + Vector3.down * maxJumpHeight;

            Debug.DrawRay(upRayMax, transform.forward * maxJumpDistance, Color.red, 1f);
            Debug.DrawRay(downRayMax, transform.forward * maxJumpDistance, Color.red, 1f);

            float jumpDistance = destination.y - transform.position.y;
            //점프 가능 높이 범위일 때
            if (jumpDistance > -maxJumpHeight &&
                jumpDistance < maxJumpHeight)
            {
                //바닥 레이 체크 후 바닥이 없을 때
                if (jumpDistance > 0)
                {
                    //위쪽일 때 레이
                    //캐릭터 머리 위에 땅 확인
                    Vector3 rayStart = transform.position + Vector3.up * minJumpHeight;
                    float rayMax = destination.y- transform.position.y;
                    RaycastHit hit;

                    Debug.DrawRay(rayStart, Vector3.up * rayMax, Color.green, 3f);
                    if (Physics.Raycast(rayStart, Vector3.up, out hit, rayMax, ~jumpRayMask))
                    {
                        Debug.Log(gameObject.name + "- HeightJump: false");
                        return false;
                    }
                    else
                    {
                        Debug.Log(gameObject.name + "- HeightJump: true");
                        return true;
                    }
                }
                else
                {
                    //아래쪽일 때 레이
                    //목표 지점의 위에 땅이 있는지 확인
                    Vector3 rayStart = destination + Vector3.up * minJumpHeight;
                    float rayMax = transform.position.y - destination.y;

                    RaycastHit hit;

                    Debug.DrawRay(rayStart, Vector3.up * rayMax, Color.green, 3f);
                    if (Physics.Raycast(rayStart, Vector3.up, out hit, rayMax, ~jumpRayMask))
                    {
                        Debug.Log(gameObject.name + "- HeightJump: false");
                        return false;
                    }
                    else
                    {
                        Debug.Log(gameObject.name + "- HeightJump: true");
                        return true;
                    }
                }
            }
            else
            {
                Debug.Log(gameObject.name + "- HeightJump: false");
                return false;
            }
        }


        float groundRayLength = 0.03f;
        protected void RayCheckGround()
        {
            Vector3 startCenter = m_bodyColl.bounds.center;
            Vector3 offset1 = new Vector3(m_bodyColl.bounds.extents.x, 0, m_bodyColl.bounds.extents.z);
            Vector3 offset2 = new Vector3(-m_bodyColl.bounds.extents.x, 0, m_bodyColl.bounds.extents.z);
            Vector3 offset3 = new Vector3(m_bodyColl.bounds.extents.x, 0, -m_bodyColl.bounds.extents.z);
            Vector3 offset4 = new Vector3(-m_bodyColl.bounds.extents.x, 0, -m_bodyColl.bounds.extents.z);

            Vector3 start1 = startCenter - Vector3.up * m_bodyColl.bounds.size.y * 0.25f + offset1;
            Vector3 start2 = startCenter - Vector3.up * m_bodyColl.bounds.size.y * 0.25f + offset2;
            Vector3 start3 = startCenter - Vector3.up * m_bodyColl.bounds.size.y * 0.25f + offset3;
            Vector3 start4 = startCenter - Vector3.up * m_bodyColl.bounds.size.y * 0.25f + offset4;

            int frontRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));

            // 플랫폼 레이 체크
            RaycastHit groundRay;

            Color rayColor1 = Color.blue;
            Color rayColor2 = Color.blue;
            Color rayColor3 = Color.blue;
            Color rayColor4 = Color.blue;

            bool isGroundDetected = false;

            if (Physics.Raycast(start1, Vector3.down, out groundRay, groundRayLength, ~frontRayMask) && groundRay.collider.gameObject.CompareTag("Ground"))
            {
                isGroundDetected = true;
                rayColor1 = Color.green;
            }
            if (Physics.Raycast(start2, Vector3.down, out groundRay, groundRayLength, ~frontRayMask) && groundRay.collider.gameObject.CompareTag("Ground"))
            {
                isGroundDetected = true;
                rayColor2 = Color.green;
            }
            if (Physics.Raycast(start3, Vector3.down, out groundRay, groundRayLength, ~frontRayMask) && groundRay.collider.gameObject.CompareTag("Ground"))
            {
                isGroundDetected = true;
                rayColor3 = Color.green;
            }
            if (Physics.Raycast(start4, Vector3.down, out groundRay, groundRayLength, ~frontRayMask) && groundRay.collider.gameObject.CompareTag("Ground"))
            {
                isGroundDetected = true;
                rayColor4 = Color.green;
            }

            isGround = isGroundDetected;

            Debug.DrawRay(start1, Vector3.down * groundRayLength, rayColor1, 0.1f);
            Debug.DrawRay(start2, Vector3.down * groundRayLength, rayColor2, 0.1f);
            Debug.DrawRay(start3, Vector3.down * groundRayLength, rayColor3, 0.1f);
            Debug.DrawRay(start4, Vector3.down * groundRayLength, rayColor4, 0.1f);
        }

        #endregion

    }
}