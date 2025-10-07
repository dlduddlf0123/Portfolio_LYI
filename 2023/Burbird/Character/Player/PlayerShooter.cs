using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 깃털, 조준, 발사 관련 스크립트
    /// </summary>
    public class PlayerShooter : MonoBehaviour
    {
        StageManager stageMgr;
        public PlayerController2D playerControll { get; set; }
        public PlayerParticleHolder particleHolder;

        public AimSprites aimSprites;
        public Enemy fireTarget = null;

        //깃털
        [Header("Feather")]
        public GameObject[] arr_featherOrigin;

        public Transform tr_active;
        public Transform tr_disable;
        public List<GameObject> list_feather = new List<GameObject>();
        public List<Feather> list_active = new List<Feather>(); //현재 활성화 된 깃털을 따로 저장? 떨구기 시 적용?

        public AudioClip sfx_featherShot;

        public List<Debuff> list_debuff = new List<Debuff>();

        public float maxFirePower = 500f;
        public float minFirePower = 50f;

        public float fireTime = 0.3f;
        public float fireTic = 0f;

        public bool isFireReady = false;
        void Awake()
        {
            stageMgr = StageManager.Instance;
            playerControll = GetComponent<PlayerController2D>();
        }

        private void FixedUpdate()
        {
            AutoFire();
            ChangeHeadRotation();
        }

        /// <summary>
        /// 조준중인 적 향해 바라보기
        /// </summary>
        void ChangeHeadRotation()
        {
            if (fireTarget == null)
            {
                aimSprites.ActiveAimSprite(false);
                return;
            }
            aimSprites.ActiveAimSprite(true);

            //각도기, 캐릭터 머리 돌리기
            Vector3 fireVec = (fireTarget.centerTr.position - playerControll.centerTr.position).normalized;
            float angle = Mathf.Atan2(fireVec.y, fireVec.x) * Mathf.Rad2Deg;
            if (playerControll.isLeft)
            {
                angle += 180f;
            }
            aimSprites.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        /// <summary>
        /// 시간 체크
        /// 쿨타임마다 발사 준비
        /// 일정 거리 안쪽에 적들이 들어올 경우 발사
        /// </summary>
        public void AutoFire()
        {
            //사망 시 발사 안함
            if (playerControll.player.isDie || 
                !playerControll.player.isShooting)
            {
                return;
            }

            if (fireTic < fireTime)
            {
                fireTic += Time.deltaTime * playerControll.player.playerStatus.ATKSpeed;
            }
            else
            {
                isFireReady = true;
                CheckEnemy();
            }

        }

        /// <summary>
        /// 적 확인, 타겟 변경
        /// </summary>
        void CheckEnemy()
        {
            if (!isFireReady || stageMgr.statStage != StageStat.FIGHT)
            {
                return;
            }
            float currentDist = 0.0f;
            float targetDist = 50.0f;

            int rayCount = 0;

            //거리 체크, 타겟 설정
            //레이캐스트 그라운드가 걸리면 다름 타겟으로 이동
            for (int i = 0; i < stageMgr.enemySpawner.list_activeEnemy.Count; i++)
            {
                RaycastHit2D ray = RayCheckGround(stageMgr.enemySpawner.list_activeEnemy[i].centerTr.position);
                //적이 지형지물로 인해 사이가 막힌 경우
                if (ray)
                {
                    if (fireTarget != null)
                    {
                        if (fireTarget == stageMgr.enemySpawner.list_activeEnemy[i])
                        {
                            fireTarget = null;
                        }

                    }
                    rayCount++;
                }
                else
                {
                    //적과의 거리 계산, 가까우면 저장
                    currentDist = Vector3.Distance(transform.position, stageMgr.enemySpawner.list_activeEnemy[i].transform.position);
                    if (fireTarget != null)
                    {
                        targetDist = Vector3.Distance(transform.position, fireTarget.transform.position);
                    }

                    if (currentDist < targetDist)
                    {
                        fireTarget = stageMgr.enemySpawner.list_activeEnemy[i];
                    }
                }
            }
            //모든 적이 가로막힌 상태일 경우 가장 가까운 타겟으로 설정
            if (rayCount == stageMgr.enemySpawner.list_activeEnemy.Count)
            {
                fireTarget = null;
                targetDist = 50;

                for (int i = 0; i < stageMgr.enemySpawner.list_activeEnemy.Count; i++)
                {
                    currentDist = Vector3.Distance(transform.position, stageMgr.enemySpawner.list_activeEnemy[i].transform.position);
                    if (fireTarget != null)
                    {
                        targetDist = Vector3.Distance(transform.position, fireTarget.transform.position);
                    }

                    if (currentDist < targetDist)
                    {
                        fireTarget = stageMgr.enemySpawner.list_activeEnemy[i];
                    }
                }
            }


            if (isFireReady)
            {
                //발사 명령 실행
                Vector3 fireVec = Vector3.zero;
                if (fireTarget == null)
                {
                    fireVec = playerControll.isLeft ? Vector3.left: Vector3.right;
                    
                }
                else
                {
                    fireVec = Vector3.Normalize(fireTarget.centerTr.position - playerControll.centerTr.position);
                }

                StartCoroutine(Fire(fireVec, minFirePower));

                isFireReady = false;
                fireTic = 0;
            }
        }


        /// <summary>
        /// 타겟과 플레이어 사이의 지형 검사
        /// </summary>
        /// <param name="targetVec">사이를 검사할 적</param>
        /// <returns></returns>
        RaycastHit2D RayCheckGround(Vector3 targetVec)
        {
            int groundMask = 
                (1 << LayerMask.NameToLayer("Player")) |
                (1 << LayerMask.NameToLayer("Character")) | 
                (1 << LayerMask.NameToLayer("Ignore Raycast") |
                (1 << LayerMask.NameToLayer("Platform")));

            Vector3 v = Vector3.Normalize(targetVec - playerControll.centerTr.position);
            float d = Vector3.Distance(targetVec, playerControll.centerTr.position);

            RaycastHit2D ray_fire = Physics2D.Raycast(playerControll.centerTr.position, v, d, ~groundMask);

            Debug.DrawRay(playerControll.centerTr.position, v * d, Color.blue, 0.1f);

            return ray_fire;
        }

        /// <summary>
        /// 깃털 초기화
        /// </summary>
        /// <param name="feather"></param>
        public void FeatherListInit(Feather feather)
        {
            //Debug.Log("Feather List Init!");

            feather.Init();
            GameManager.Instance.objPoolingMgr.ObjectInit(list_feather, feather.gameObject, tr_disable);
            list_active.Remove(feather);
        }

        public void AllFeatherInit()
        {
            for (int i = 0; i < list_active.Count; i++)
            {
                FeatherListInit(list_active[i]);
            }
        }

        /// <summary>
        /// 오브젝트 풀링 깃털 생성
        /// </summary>
        /// <returns></returns>
        Feather CreateFeather()
        {
            Feather feather;

            feather = GameManager.Instance.objPoolingMgr.CreateObject(list_feather, arr_featherOrigin[0], playerControll.centerTr.position, tr_active).GetComponent<Feather>();
            
            FeatherStatInit(feather);
            list_active.Add(feather);

            return feather;
        }

        /// <summary>
        /// 깃털 활성화 및 초기화
        /// </summary>
        /// <param name="feather"></param>
        void FeatherStatInit(Feather feather)
        {
            feather.transform.position = playerControll.centerTr.position; 
            feather.transform.GetChild(0).rotation = playerControll.centerTr.rotation;

            feather.shooter = this;
            feather.featherDamage = (int)playerControll.player.playerStatus.ATKDamage;
            feather.featherSpeed = (int)playerControll.player.playerStatus.shotSpeed;

            feather.isBounce = stageMgr.perkChecker.perk_bounceShot;
            feather.isPierce = stageMgr.perkChecker.perk_piercingShot;
            feather.isChain = stageMgr.perkChecker.perk_chainShot;


            if (playerControll.player.perk_deathShot > 0)
            {
                if (Random.Range(0, 100) < playerControll.player.perk_deathShot)
                {
                    feather.isDeath = true;
                }
                else
                {
                    feather.isDeath = false;
                }
            }
            feather.isDeath = false;

            feather.list_debuff = list_debuff;
            feather.gameObject.SetActive(true);
        }

        /// <summary>
        /// 깃털 발사 방향으로 정렬 계산
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        float FeatherAngle(Vector2 target)
        {
            return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 투사체 개수 만큼 위치 정렬
        /// 2개면 || 3개면 ||| 조금 떨어지기
        ///짝수 홀수 구분 %2
        ///나눈 뒤 /2 중앙에서 떨어진 만큼 곱해서 거리 벌리기
        ///기준 트랜스폼 생성
        /// </summary>
        /// <param name="playerConroll"></param>
        /// <param name="arr_feather"></param>
        void SetMultipleFeatherPosition(Quaternion rot, Feather[] arr_feather)
        {
            for (int i = 0; i < arr_feather.Length; i++)
            {
                arr_feather[i] = CreateFeather();
                arr_feather[i].transform.GetChild(0).rotation = rot;

                //짝수 체크
                if (arr_feather.Length % 2 == 0)
                {
                    int checkNum = i + 1;

                    //짝수
                    int centerNext = arr_feather.Length / 2; //2 = 1, 4 = 2, 6 = 3 중간값 다음 값 중간 +1
                    int centerPrev = centerNext - 1; //2 = 0, 4 = 1, 6 = 2 중간값 이전 값 중간 -1

                    Vector3 moveVec = Vector3.zero;

                    if (centerPrev - checkNum >= 0)
                    {
                        //배열 중앙에서 왼쪽 값
                        moveVec = arr_feather[i].transform.GetChild(0).up * 0.2f * (centerPrev - checkNum + 1);
                    }
                    else if (centerNext - checkNum < 0)
                    {
                        //배열 중앙에서 오른쪽 값
                        moveVec = arr_feather[i].transform.GetChild(0).up * 0.2f * (centerNext - checkNum);
                    }
                    arr_feather[i].transform.position += moveVec;
                }
                else
                {
                    //홀수
                    int center = arr_feather.Length / 2; //3 = 1, 5 = 2, 7 = 3 중간값

                    Vector3 moveVec = Vector3.zero;

                    if (center - i >= 0)
                    {
                        //배열 중앙에서 왼쪽 값
                        moveVec = arr_feather[i].transform.GetChild(0).up * 0.2f * (center - i);
                    }
                    else if (center - i < 0)
                    {
                        //배열 중앙에서 오른쪽 값
                        moveVec = arr_feather[i].transform.GetChild(0).up * 0.2f * (center - i);
                    }
                    arr_feather[i].transform.position += moveVec;
                }
            }
        }

        #region Fire Types
        public IEnumerator Fire(Vector2 fireVec, float firePower)
        {
            if (!isFireReady)
            {
                yield break;
            }
            //가시 각도 설정
            float angle = FeatherAngle(fireVec);
            playerControll.centerTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            CheckShotType(fireVec, firePower);


            if (stageMgr.perkChecker.perk_multiShot > 0)
            {
                for (int i = 0; i < stageMgr.perkChecker.perk_multiShot; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    CheckShotType(fireVec, firePower);
                }
            }
        }

        void CheckShotType(Vector2 fireVec, float firePower)
        {
            if (stageMgr.perkChecker.perk_doubleShot > 0)
            {
                DoubleShot(fireVec, firePower);
            }
            else
            {
                SingleShot(fireVec, firePower);
            }
            if (stageMgr.perkChecker.perk_sideShot > 0)
            {
                SideShot(fireVec, firePower);
            }
            if (stageMgr.perkChecker.perk_diagonalShot > 0)
            {
                DiagonalShot(fireVec, firePower);
            }
            if (stageMgr.perkChecker.perk_backShot > 0)
            {
                BackShot(fireVec, firePower);
            }
        }
        public void ChainShot(Enemy target, Vector2 pos, int chainCount, Vector2 fireVec, float firePower)
        {
            Feather feather;
            feather = CreateFeather();
            feather.transform.position = pos;
            feather.hitEnemy = target;
            feather.chainCount = chainCount;

            //체인샷일 경우 체인 카운트만큼 데미지 감소
            feather.featherDamage = (int)(feather.featherDamage * (1 - feather.chainCount * feather.chainReduceDMG));

            //가시 발사
            feather.FireFeather(fireVec, firePower);
            //stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }
        void SingleShot(Vector2 fireVec, float firePower)
        {
            Feather feather = CreateFeather();

            //가시 발사
            feather.FireFeather(fireVec, firePower);
            stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }
        void DoubleShot(Vector2 fireVec, float firePower)
        {
            //더블샷 개수만큼 생성
            Feather[] arr_feather = new Feather[stageMgr.perkChecker.perk_doubleShot + 1];

            SetMultipleFeatherPosition(playerControll.centerTr.rotation, arr_feather);

            for (int i = 0; i < arr_feather.Length; i++)
            {
                //가시 발사
                arr_feather[i].FireFeather(fireVec, firePower);
            }

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }

        void SideShot(Vector2 fireVec, float firePower)
        {
            //사이드샷 2배 만큼 생성
            Feather[] arr_featherUp = new Feather[stageMgr.perkChecker.perk_sideShot];
            Feather[] arr_featherDown = new Feather[stageMgr.perkChecker.perk_sideShot];

            Vector3 upTarget = playerControll.centerTr.up.normalized;
            Vector3 downTarget = -playerControll.centerTr.up.normalized;

            SetMultipleFeatherPosition(Quaternion.AngleAxis(FeatherAngle(upTarget), Vector3.forward), arr_featherUp);
            SetMultipleFeatherPosition(Quaternion.AngleAxis(FeatherAngle(downTarget), Vector3.forward), arr_featherDown);

            for (int i = 0; i < arr_featherUp.Length; i++)
            {
                //발사
                arr_featherUp[i].FireFeather(upTarget, firePower);
                arr_featherDown[i].FireFeather(downTarget, firePower);
            }

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }

        void DiagonalShot(Vector2 fireVec, float firePower)
        {
            //사선샷 2배 만큼 생성
            Feather[] arr_featherUp = new Feather[stageMgr.perkChecker.perk_diagonalShot];
            Feather[] arr_featherDown = new Feather[stageMgr.perkChecker.perk_diagonalShot];

            Vector2 upTarget = (playerControll.centerTr.up + playerControll.centerTr.right).normalized;
            Vector2 downTarget = (-playerControll.centerTr.up + playerControll.centerTr.right).normalized;

            SetMultipleFeatherPosition(Quaternion.AngleAxis(FeatherAngle(upTarget), Vector3.forward), arr_featherUp);
            SetMultipleFeatherPosition(Quaternion.AngleAxis(FeatherAngle(downTarget), Vector3.forward), arr_featherDown);

            for (int i = 0; i < arr_featherUp.Length; i++)
            {
                //발사
                arr_featherUp[i].FireFeather(upTarget, firePower);
                arr_featherDown[i].FireFeather(downTarget, firePower);
            }

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }

        void BackShot(Vector2 fireVec, float firePower)
        {
            //백샷 개수만큼 생성
            Feather[] arr_feather = new Feather[stageMgr.perkChecker.perk_backShot];

            Vector2 backTarget = -playerControll.centerTr.right.normalized;

            SetMultipleFeatherPosition(Quaternion.AngleAxis(FeatherAngle(backTarget), Vector3.forward), arr_feather);

            for (int i = 0; i < arr_feather.Length; i++)
            {
                //가시 발사
                arr_feather[i].FireFeather(backTarget, firePower);
            }

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }

        #endregion


        #region Debuff Effects

        /// <summary>
        /// 해당 효과 획득 시 호출
        /// </summary>
        /// <param name="effect"></param>
        public virtual void GetEffect(Debuff effect)
        {

            if (list_debuff.Count > 0)
            {
                //리스트에서 이미 해당 효과가 있을 때
                if (list_debuff.Contains(effect))
                {
                    //중첩 가능한 경우 획득
                    if (effect.isStackable)
                    {
                        AdditionalEffect stackEffect = list_debuff.Find(e => e == effect);
                        if (stackEffect.stack < stackEffect.maxStack)
                        {
                            stackEffect.stack++;
                            stackEffect.ActiveEffect();
                        }
                        else
                        {
                            Debug.Log("Effect stack is full:" + effect.name);
                        }
                        return;
                    }
                    else
                    {
                        //중첩 불가 안내
                        Debug.Log("Aleady has effect:" + effect.name);
                        return;
                    }
                }
            }


            effect.damage = (int)playerControll.player.playerStatus.ATKDamage;

            //리스트에서 이 효과가 없을 때 획득
            list_debuff.Add(effect);

            //이펙트 사라질 때 처리 추가
            //if (effect.onEffectRemove == null)
            //{
            //    effect.onEffectRemove.AddListener(() => list_debuff.Remove(effect));
            //}

        }


        public virtual void LostEffect(Debuff effect)
        {
            //리스트에서 이미 해당 효과가 있을 때
            if (list_debuff.Contains(effect))
            {
                //중첩 가능한 경우 중첩 제거
                if (effect.isStackable)
                {
                    AdditionalEffect stackEffect = list_debuff.Find(e => e == effect);
                    if (stackEffect.stack > 1)
                    {
                        stackEffect.stack--;
                    }
                    else
                    {
                        //남은 중첩 없으면 제거
                        list_debuff.Remove(effect);
                        Destroy(effect);
                    }
                    return;
                }
                else
                {
                    //중첩 불가 안내
                    Debug.Log("Effect Removed:" + effect.name);
                    list_debuff.Remove(effect);
                    Destroy(effect);
                    return;
                }
            }
        }

        #endregion
    }
}
