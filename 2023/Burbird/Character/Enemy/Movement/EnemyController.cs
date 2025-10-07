using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public enum EnemyState
    {
        IDLE,
        MOVE,
        CHASE,
        ATTACK,
        DIE,
    }


    /// <summary>
    /// ?? ?????? ????, AI ???? ????
    /// </summary>
    public class EnemyController : CharacterController
    {
        protected StageManager stageMgr;

        protected Enemy enemyStat;
        protected EnemyRangedAttack enemyAttack;

        [Header("Physics2D")]
        public Rigidbody2D m_rigidbody2D;
        public Collider2D[] arr_collider;

        [Header("Status")]
        public EnemyState e_state;
        public float moveSpeed = 1f;

        protected float moveTime = 1f;

        public int direction = 1; //1 or -1
        public bool isGround = false;

        protected Coroutine currentCoroutine = null;

        /// <summary>
        /// 3/21/2023-LYI
        /// 넉백 가능 여부
        /// </summary>
        public bool isKnockBackable = true;
        /// <summary>
        /// 3/21/2023-LYI
        /// 현재 플레이어가 추적 범위 내에 있는지
        /// </summary>
        public bool isPlayerCheck = false;

        public Vector3 originScale { get; set; }

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            enemyStat = GetComponent<Enemy>();
            enemyAttack = GetComponent<EnemyRangedAttack>();

            m_rigidbody2D = GetComponent<Rigidbody2D>();
            arr_collider = GetComponentsInChildren<Collider2D>();

            originScale = transform.GetChild(0).localScale;

            DoAwake();
        }

        protected virtual void DoAwake() { }

        public virtual void Stop()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }


        public virtual void AI_Move(EnemyState AIstate)
        {
            //같은 동작일 경우 실행 안함, 중복 발사 방지용
            if (e_state == AIstate) { return; }

            Stop();

            if (enemyStat.isDie) { return; }

            e_state = AIstate;

            if (stageMgr.isEnemyDebug)
                Debug.Log(gameObject.name + "AI Move:" + AIstate);
            switch (AIstate)
            {
                case EnemyState.IDLE: //IDLE
                    currentCoroutine = StartCoroutine(Idle());
                    break;
                case EnemyState.MOVE: //WALK
                    currentCoroutine = StartCoroutine(Movement());
                    break;
                case EnemyState.CHASE: //CHASE
                    currentCoroutine = StartCoroutine(Chase());
                    break;
                case EnemyState.ATTACK: //ATTACK
                    currentCoroutine = StartCoroutine(Attack());
                    break;
                default:
                    currentCoroutine = StartCoroutine(Idle());
                    break;
            }
        }

        /// <summary>
        /// 현재 플레이어 상태에 따른 다음 움직임 받기
        /// </summary>
        public virtual void CheckNextMove(EnemyState OnNext = EnemyState.MOVE)
        {
            if (isPlayerCheck)
            {
                AI_Move(EnemyState.ATTACK);
            }
            else
            {
                AI_Move(OnNext);
            }
        }

        protected virtual void ChangeDirectionToPlayer()
        {
            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;
            ChangeSpriteDirection();
        }

        protected void ChangeSpriteDirection()
        {
            if (direction == -1)
            {
                transform.GetChild(0).localScale = new Vector3(originScale.x, originScale.y);
            }
            else
            {

                transform.GetChild(0).localScale = new Vector3(-originScale.x, originScale.y);
            }
        }

        protected virtual IEnumerator Idle()
        {
            yield return new WaitForSeconds(1f);

            CheckNextMove();
        }

        /// <summary>
        /// ?????? ????
        /// ???? ???????? ???????? ???? ????
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Movement()
        {
            direction = (Random.Range(0, 2) == 0) ? -1 : 1;
            ChangeSpriteDirection();

            float t = 0;
            float randomTime = moveTime * Random.Range(0.7f, 1.3f);
            while (t < randomTime)
            {
                t += 0.01f;
                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            CheckNextMove(EnemyState.IDLE);
        }

        /// <summary>
        /// ???????? ?????? ????, ????
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Chase()
        {
            float t = 0;

            ChangeDirectionToPlayer();

            while (t < moveTime)
            {
                t += 0.01f;

                transform.Translate(Vector3.left * direction * moveSpeed * speedMultiplier * 2 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            CheckNextMove(EnemyState.CHASE);
        }

        /// <summary>
        /// 3/21/2023-LYI
        /// 적 캐릭터 공격
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Attack()
        {
            //원거리 공격이면 원거리 공격 진행
            if (enemyAttack != null)
            {
                currentCoroutine = StartCoroutine(enemyAttack.Attack());
            }
            else
            {

            }
            yield return null;

        }

        #region Raycast Check

        /// <summary>
        /// 전방 체크 시 작동할 동작들
        /// </summary>
        protected virtual void FrontCheckAction()
        {
            switch (e_state)
            {
                case EnemyState.MOVE:
                case EnemyState.CHASE:
                    direction *= -1;
                    ChangeSpriteDirection();
                    break;
                case EnemyState.ATTACK:
                    //Heading to wall, get stun
                    AI_Move(EnemyState.IDLE);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 지면 체크 시 작동할 동작들
        /// </summary>
        protected virtual void GroundCheckAction()
        {
            switch (e_state)
            {
                case EnemyState.MOVE:
                    direction *= -1;
                    ChangeSpriteDirection();
                    break;
                case EnemyState.CHASE:
                    //Heading to wall, get stun
                   // AI_Move(EnemyState.IDLE);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 4/19/2023-LYI
        /// 전방 체크, 벽에 닿았을 때 뒤로 돌아갈 때 사용
        /// </summary>
        protected virtual void FrontCheck()
        {
            Vector2 frontBound = arr_collider[0].bounds.center + Vector3.right * arr_collider[0].bounds.size.x * 0.5f * direction;

            int characterMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Ignore Raycast"));

            RaycastHit2D ray_front = Physics2D.Raycast(frontBound, Vector2.right * direction, 0.3f, ~characterMask);

            Color frontColor = Color.blue;

            if (ray_front)
            {
                frontColor = Color.red;
                FrontCheckAction();
            }
            Debug.DrawRay(frontBound, Vector2.right * direction * 0.3f, frontColor, 0.1f);
        }

        /// <summary>
        /// 4/19/2023-LYI
        /// 지면 체크, 바닥으로 떨어지지않고 돌아갈 때 사용
        /// </summary>
        protected virtual void GroundCheck()
        {
            Vector2 downBound = arr_collider[0].bounds.center + Vector3.right * arr_collider[0].bounds.size.x * 0.5f * direction;

            int characterMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Ignore Raycast"));

            RaycastHit2D ray_ground = Physics2D.Raycast(downBound, Vector2.down, arr_collider[0].bounds.size.y, ~characterMask);

            Color rayColor = Color.blue;

            if (ray_ground)
            {
                if (ray_ground.collider.gameObject.CompareTag("Ground"))
                {
                    rayColor = Color.green;
                }
            }
            else
            {
                rayColor = Color.red;
                GroundCheckAction();
            }

            Debug.DrawRay(downBound, Vector2.down * arr_collider[0].bounds.size.y, rayColor, 0.1f);
        }

        #endregion

    }
}