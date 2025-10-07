using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BoingKit;

namespace Burbird
{
    /// <summary>
    /// 플레이어의 입력과 캐릭터 움직임, 애니메이션 관련 스크립트
    /// </summary>
    public class PlayerController2D : CharacterController
    {
        StageManager stageMgr;

        public Player player { get; set; }
        public PlayerShooter shooter { get; set; }

        //물리 계산
        protected Rigidbody2D m_rigidbody;

        [Header("Player")]
        public Collider2D[] arr_collider;
        public Transform centerTr;
        public VariableJoystick variableJoystick;

        //플랫폼 충돌 체크
        public Collider2D platformColl;


        //애니메이션
        protected Animator m_animator;
        protected Animator m_spriteAnim;

        BoingBones boingBones;

        //깃털
        [Header("Feather")]
        public GameObject[] arr_featherState;

        public int currentFeatherCount = 0;
        public int maxFeatherCount = 10;

        //입력
        protected Vector2 input_move;
        protected Vector2 input_aim;

        //속도 관련
        [Header("Movement Parameter")]
        public float moveSpeed;
        public float maxSpeed;
        public float maxVelocity = 10f;

        public float jumpPower;
        public float jumpingPower;

        public float knockBackPower = 1f;

        protected int flyCount = 0;

        //동작 상태 관련
        [Header("State Parameter")]
        public bool isGround = true;
        public bool isAimClicking = false;
        public bool isJumpClicking = false;
        public bool isSprint = false;
        public bool isLeft = false;

        //모바일 조작용
        bool isJoystickY = false;
        float jumpTimer = 0f;

        float downTime = 0f;

        Vector3 originScale;

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            player = GetComponent<Player>();
            shooter = GetComponent<PlayerShooter>();
            player.centerTr = centerTr;

            m_rigidbody = GetComponent<Rigidbody2D>();
            m_animator = transform.GetChild(0).GetComponent<Animator>();
            m_spriteAnim = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            boingBones = GetComponent<BoingBones>();

            originScale = transform.GetChild(0).localScale;

            Init();
        }

        void Start()
        {
            //boingBones.BoneChains = new BoingBones.Chain[18 * transform.GetChild(1).childCount];
            //for (int j = 0; j < transform.GetChild(1).childCount; j++)
            //{
            //    for (int i = 0; i < 18; i++)
            //    {
            //        list_feather.Add(Instantiate(prefab_feather[Random.Range(0, 2)], transform.GetChild(1).GetChild(j).GetChild(i)));
            //        BoingBones.Chain chain = new BoingBones.Chain();
            //        chain.Root = list_feather[i + 18 * j].transform;
            //        chain.AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailHalf;

            //        boingBones.BoneChains[i+18*j] = chain;
            //    }
            //}


        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Move();
            MobileJoystickInput();
            RayCheckGround();
            if (isJumpClicking)
            {
                jumpTimer -= Time.deltaTime;
                Jumping(jumpTimer);
            }

            //3/22/2023-LYI
            //Fixed for landing slow down
            if (isGround &&
                downTime < 0.1f)
            {
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, Mathf.Max(0f, m_rigidbody.velocity.y));
            }
            
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = coll.gameObject.GetComponentInParent<Enemy>();
                //접촉 시 근접 데미지
                player.GetDamage(enemy.Status.ATKDamage, enemy.transform.position);
            }
        }

        //private void OnCollisionStay2D(UnityEngine.Collision2D coll)
        //{
        //    if (coll.gameObject.CompareTag("Ground") && !isGround)
        //    {
        //        flyCount = 3;
        //        isGround = true;
        //        m_animator.SetBool("isGround", isGround);
        //        SetColliderActive(0);
        //    }
        //}

        //private void OnCollisionExit2D(UnityEngine.Collision2D coll)
        //{
        //    if (coll.gameObject.CompareTag("Ground"))
        //    {
        //        isGround = false;
        //        m_animator.SetBool("isGround", isGround);
        //    }

        //}


        /// <summary>
        /// 캐릭터 상태 초기화
        /// </summary>
        public void Init()
        {
            //SetColliderActive(0);

            currentFeatherCount = maxFeatherCount;
            ChangeFeatherState();
        }
        public void SetPlayerPos(Vector2 vec)
        {
            transform.position = vec;
            m_rigidbody.velocity = Vector2.zero;
        }
        /// <summary>
        /// 넉백 효과
        /// </summary>
        /// <param name="point"></param>
        public void KnockBack(Vector3 point)
        {
            int dir = (transform.position.x > point.x) ? 1 : -1;
            m_rigidbody.velocity = Vector2.zero;
            m_rigidbody.AddForce(transform.right * dir * 10f * knockBackPower + Vector3.up * 3f * knockBackPower, ForceMode2D.Impulse);
        }

        public void LogDebug(string s)
        {
            if (false)
            {
                Debug.Log("PlayerController2D: " + s);
            }
        }

        #region Player Actions

        public virtual void Stop()
        {
            m_rigidbody.velocity = Vector2.zero;
        }

        /// <summary>
        /// 플레이어 이동
        /// </summary>
        public virtual void Move()
        {
            if (player.isDie || base.isStun)
            {
                return;
            }
            Vector2 velocity = m_rigidbody.velocity;

            // Apply acceleration directly as we'll want to clamp
            // prior to assigning back to the body.
            velocity.x += input_move.x * moveSpeed * Time.fixedDeltaTime * base.speedMultiplier;

            // Clamp horizontal speed.
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);

            if (isSprint)
            {
                maxVelocity = 5;
            }
            if (!isSprint)
            {
                maxVelocity = 2;
            }

            if (velocity.x > maxVelocity)
            {
                velocity.x = maxVelocity;
            }
            if (velocity.x < -maxVelocity)
            {
                velocity.x = -maxVelocity;
            }

            //if (Mathf.Abs(_velocity.x) >= maxVelocity && !isSprint)
            //{
            //    isSprint = true;
            //}


            m_rigidbody.velocity = new Vector2(velocity.x , m_rigidbody.velocity.y);

            if (stageMgr.statStage == StageStat.EVENT)
            {
                m_rigidbody.velocity = Vector2.zero;
            }

            m_animator.SetFloat("MoveSpeed", Mathf.Abs(m_rigidbody.velocity.x));
            m_animator.SetFloat("fHeight", m_rigidbody.velocity.y);
        }

        void MoveDirectionAnim()
        {
            if (input_move.x != 0)
            {
                if (input_move.x > 0)
                {
                    transform.GetChild(0).localScale = originScale;
                    isLeft = false;
                }
                else if (input_move.x < 0)
                {
                    transform.GetChild(0).localScale = new Vector3(-originScale.x, originScale.y, originScale.z);
                    isLeft = true;
                }
                m_animator.SetBool("isMove", true);
            }
            else
            {
                m_animator.SetBool("isMove", false);
            }
        }

        public void Jump()
        {
            if (player.isDie || base.isStun)
            {
                return;
            }
            if (!isGround)
            {
                if (StageManager.Instance.perkChecker.perk_fly)
                {
                    // Fly();
                }
                return;
            }

            isJumpClicking = true;
            jumpTimer = 0.3f;

            LogDebug("Jump");
            m_animator.SetTrigger("tJump");

            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            m_rigidbody.AddForce(Vector3.up * jumpPower * base.jumpMultiplier, ForceMode2D.Impulse);
        }

        /// <summary>
        /// 점프버튼 유지시
        /// </summary>
        public void Jumping(float jumpTime)
        {
            if (jumpTime > 0)
            {
                m_rigidbody.AddForce(Vector3.up * jumpingPower * base.jumpMultiplier, ForceMode2D.Impulse);
            }
        }

        public void Fly()
        {
            if (flyCount <= 0)
            {
                return;
            }

            flyCount--;

            //SetColliderActive(1);
            LogDebug("Fly");
            m_animator.SetTrigger("tFly");
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            m_rigidbody.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
        }

        //IEnumerator coJumpHeight()
        //{
        //    Vector3 _startVec = transform.position;
        //    float _height = 0f;
        //    while (!isGround)
        //    {
        //        _height = transform.position.y - _startVec.y;
        //        m_animator.SetFloat("fHeight", _height);

        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}


        /// <summary>
        /// 가시 개수에 따른 깃털 표시 변경
        /// </summary>
        public void ChangeFeatherState()
        {
            if (currentFeatherCount > maxFeatherCount)
            {
                currentFeatherCount = maxFeatherCount;
            }
            //맨 몸
            for (int i = 0; i < arr_featherState.Length; i++)
            {
                arr_featherState[i].gameObject.SetActive(false);
            }

            if (currentFeatherCount > 0 &&
                currentFeatherCount <= maxFeatherCount * 0.5f)
            {
                //털 빠짐 2
                arr_featherState[2].SetActive(true);
            }
            else if (currentFeatherCount > maxFeatherCount * 0.5f &&
                currentFeatherCount < maxFeatherCount * 0.9f)
            {
                //털 빠짐 1
                arr_featherState[1].SetActive(true);
            }
            else if (currentFeatherCount > maxFeatherCount * 0.9f &&
                currentFeatherCount <= maxFeatherCount)
            {
                //가시 최대, 털이 빽빽
                arr_featherState[0].SetActive(true);
            }
        }


        /// <summary>
        /// 현재 콜라이더 변경
        /// </summary>
        /// <param name="_coll">0:Standing/1:BodyOnly</param>
        void SetColliderActive(int _coll)
        {
            if (arr_collider[_coll].gameObject.activeSelf)
            {
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                arr_collider[i].gameObject.SetActive(false);
            }

            arr_collider[_coll].gameObject.SetActive(true);
            // arr_collider[_coll + 2].enabled = true;
        }

        /// <summary>
        /// 바닥 감지 후 닿아있는 바닥 레이어 변경
        /// 공중에서 착지 중에도 체크, 가속도 그대로 떨어지도록
        /// 바닥 감지용 레이캐스트 중에 작동 될 수 있도록 할 것
        /// </summary>
        /// <param name="isActive"></param>
        private void PlatformDown(bool isActive)
        {
            if (platformColl != null &&
               !isActive)
            {
                LogDebug("Platform Down");
                //for (int i = 0; i < platformColl.Length; i++)
                //{
                //    platformColl[i].gameObject.GetComponent<FallthroughReseter>().StartFall();
                //}
                platformColl.gameObject.GetComponent<FallthroughReseter>().StartFall();
            }
        }

        #endregion


        #region PC/Joystic Input Actions
        public void OnPlayerMove(InputAction.CallbackContext _context)
        {
            input_move = _context.ReadValue<Vector2>().x * Vector2.right;
            MoveDirectionAnim();

            LogDebug("Move:" + input_move);

        }

        public void OnJump(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started)
            {
                Jump();
            }
        }

        public void OnSprint(InputAction.CallbackContext _context)
        {
            LogDebug("Sprint");
            switch (_context.phase)
            {
                case InputActionPhase.Performed:
                    isSprint = true;
                    break;

                default:
                    isSprint = false;
                    break;
            }
        }


        public void OnAim(InputAction.CallbackContext _context)
        {
            if (!isAimClicking)
            {
                return;
            }
            input_aim = _context.ReadValue<Vector2>();
            // LogDebug("Aim:" + input_aim);
        }

        public void OnFire(InputAction.CallbackContext _context)
        {
            switch (_context.phase)
            {
                case InputActionPhase.Started:
                    isAimClicking = true;
                    LogDebug("FireStart");
                    //조준선 보여주기, 조준

                    break;
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    isAimClicking = false;
                    LogDebug("FireEnd");
                    //발사!
                    break;
                default:
                    break;
            }
        }

        #endregion


        #region Mobile Input Actions
        public virtual void MobileJoystickInput()
        {
            if (InputSystem.devices[0].IsPressed())
            {
                return;
            }

            input_move = variableJoystick.Direction;

            if (Mathf.Abs(input_move.x) > 0.5f)
            {
                isSprint = true;
            }
            else
            {
                isSprint = false;
            }

            MoveDirectionAnim();

            if (variableJoystick.Vertical < -0.6f)
            {
                downTime += Time.deltaTime;
                if (downTime < 0.2f)
                {
                    PlatformDown(false);
                }
            }
            else
            {
                downTime = 0;
                PlatformDown(true);
            }
        }

        public void MoveRight()
        {
            input_move = Vector2.right;
            transform.localScale = new Vector3(1, 1, 1);
            m_animator.SetBool("isMove", true);

            //Debug.Log("Move:" + input_move);
        }
        public void MoveLeft()
        {
            input_move = Vector2.left;
            transform.localScale = new Vector3(-1, 1, 1);
            m_animator.SetBool("isMove", true);
            //Debug.Log("Move:" + input_move);
        }
        public void MoveEnd()
        {
            input_move = Vector2.zero;
            // isSprint = false;

            m_animator.SetBool("isMove", false);
        }
        #endregion


        /// <summary>
        /// 3/22/2023-LYI
        /// 캐릭터 바닥 체크 isGround
        /// 플랫폼 체크
        /// </summary>
        void RayCheckGround()
        {
            Vector3 colliderBound = Vector3.down *( arr_collider[0].bounds.size.y * 0.4f+0.2f);
            Vector2 platformBound = arr_collider[0].bounds.center + Vector3.up * arr_collider[0].bounds.size.y;
            Vector2 downBound = arr_collider[0].bounds.center + colliderBound;

            int platformMask = 1 << LayerMask.NameToLayer("Platform");
            int characterMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Ignore Raycast"));

            float raygap = 0.2f;
            //플랫폼 레이 체크
            RaycastHit2D ray_platform = Physics2D.Raycast(platformBound, Vector2.down, 3f, platformMask);
            RaycastHit2D ray_groundLeft = Physics2D.Raycast(downBound + Vector2.left * raygap, Vector2.down, 0.2f, ~characterMask);
            RaycastHit2D ray_groundRight = Physics2D.Raycast(downBound + Vector2.right * raygap, Vector2.down, 0.2f, ~characterMask);


            Color rayPlatform = Color.blue;
            if (ray_platform)
            {
                if (ray_platform.collider.gameObject.layer == 10)
                {
                    //LogDebug("Platform Colled");
                    //ray_platform.collider.attachedRigidbody.GetAttachedColliders(platformColl);
                    platformColl = ray_platform.collider;
                    rayPlatform = Color.green;
                }
            }

            Color rayColorLeft = Color.blue;
            Color rayColorRight = Color.blue;
            if (ray_groundLeft)
            {
                if (ray_groundLeft.collider.gameObject.CompareTag("Ground"))
                {
                    flyCount = 3;
                    isGround = true;
                    m_animator.SetBool("isGround", isGround);
                   // SetColliderActive(0);
                    rayColorLeft = Color.green;
                }
            }
            if (ray_groundRight)
            {
                if (ray_groundRight.collider.gameObject.CompareTag("Ground"))
                {
                    flyCount = 3;
                    isGround = true;
                    m_animator.SetBool("isGround", isGround);
                  //  SetColliderActive(0);
                    rayColorRight = Color.green;
                }
            }
            if (!ray_groundLeft && !ray_groundRight)
            {
                if (isGround)
                {
                    isGround = false;
                    m_animator.SetBool("isGround", isGround);
                }
                rayColorLeft = Color.red;
                rayColorRight = Color.red;
            }
           // Debug.DrawRay(platformBound, Vector2.down * 3f, rayPlatform, 0.1f);
            Debug.DrawRay(arr_collider[0].bounds.center + Vector3.left * raygap + colliderBound, Vector2.down * 0.2f, rayColorLeft, 0.1f);
            Debug.DrawRay(arr_collider[0].bounds.center + Vector3.right * raygap + colliderBound, Vector2.down * 0.2f, rayColorRight, 0.1f);
        }
    }
}