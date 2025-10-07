using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction
{
    public enum ShootingType
    {
        ONCE = 0,
        REPEAT,
    }

    public class Trap_Shooting : Tok_Interact
    {
        GameManager gameMgr;

        [Header("Trap_Shooting")]

        [Header("Projectile")]
        public List<GameObject> list_disable = new List<GameObject>();
        public Transform tr_active;
        public Transform tr_disable;

        public GameObject origin_projectile;
        public Transform tr_shoot;

        [Header("Type")]
        public ShootingType typeShoot = ShootingType.ONCE;

        [Header("Once")]
        public GameObject missileHead; //단발 미사일인 경우 튀어나온 미사일 표시
        public LineRenderer line_missile; //단발 미사일인 경우 궤적 표시
        
        [Header("Effects")]
        public AudioClip sfx_projectile;
        public AudioClip sfx_hit;
        public ParticleSystem efx_shot;
        public ParticleSystem efx_hit;

        [Header("Properties")]
        public bool isOnce = false;

        public float startDelay;
        public float shotDelay;
        public float shotSpeed;

        public int projectileNum;
        float shotTimer = 0;
        Vector3 shotVec;


        Coroutine currentCoroutine = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        // Start is called before the first frame update
        void Start()
        {
            InteractInit();
        }


        public override void InteractInit()
        {
            base.InteractInit();

            isOnce = false;
            shotVec = transform.forward * 0.1f;

            for (int i = 0; i < tr_active.childCount; i++)
            {
                MissileInit(tr_active.GetChild(i).gameObject);
            }

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            switch (typeShoot)
            {
                case ShootingType.ONCE:
                    missileHead.SetActive(true);
                   // currentCoroutine = StartCoroutine(OnceShoot());
                    break;
                case ShootingType.REPEAT:
                    currentCoroutine = StartCoroutine(RepeatShoot());
                    break;
                default:
                    break;
            }
        }

        private void Update()
        {
            MissileRayCheck();
        }

        public void Shoot()
        {
            switch (typeShoot)
            {
                case ShootingType.ONCE:
                    if (isOnce)
                    {
                        return;
                    }
                    isOnce = true;
                    break;
                case ShootingType.REPEAT:
                    break;
                default:
                    break;
            }

            if (projectileNum > 1)
            {
                StartCoroutine(MissileShots());
            }
            else
            {
                MissileShot();
            }
        }

        public void MissileInit(GameObject go)
        {
            gameMgr.objPoolingMgr.ObjectInit(list_disable, go, tr_disable);
        }

        /// <summary>
        /// 6/21/2024-LYI
        /// 미사일이 폭발 시
        /// </summary>
        public void MissileHit(Trap_Projectile missile)
        {
            if (efx_hit != null)
            {
                PlayParticle(efx_hit.gameObject, missile.transform);
            }
            gameMgr.soundMgr.PlaySfxRandomPitch(missile.transform.position, sfx_hit);

        }


        void MissileShot()
        {
            missileHead.SetActive(false);

            shotVec = tr_shoot.forward;
            Trap_Projectile missile = gameMgr.objPoolingMgr.CreateObject(
                list_disable, origin_projectile, tr_shoot.position, tr_active).GetComponent<Trap_Projectile>();
    
            missile.transform.localRotation = Quaternion.identity;
           
            missile.shooter = this;
            missile.speed = shotSpeed;
            missile.TargetShot(shotVec);

            if (sfx_projectile != null)
            {
                gameMgr.soundMgr.PlaySfxRandomPitch(missile.transform.position, sfx_projectile);
            }
            else
            {
                gameMgr.soundMgr.PlaySfxRandomPitch(missile.transform.position, Constants.Sound.SFX_TRAP_MISSILE_SHOOT);
            }

            if (efx_shot != null)
            {
                PlayParticle(efx_shot.gameObject, tr_shoot);
            }

        }

        IEnumerator MissileShots()
        {
            for (int i = 0; i < projectileNum; i++)
            {
                MissileShot();
                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator RepeatShoot()
        {
            yield return new WaitForSeconds(startDelay);
            //WaitForSeconds wait = new WaitForSeconds(shotDelay);

            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(shotDelay);
                Shoot();
            }
        }


        IEnumerator OnceShoot()
        {
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (!isOnce)
            {
                yield return wait;

                int trapMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                       (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                       (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                         (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                           (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));


                RaycastHit ray_front;
                //Physics.Raycast(transform.position, transform.forward, out ray_front, Mathf.Infinity, ~trapMask);

                Color frontColor = Color.blue;

                line_missile.gameObject.SetActive(true);
                line_missile.SetPosition(0, tr_shoot.position);

                if (Physics.Raycast(tr_shoot.position, tr_shoot.forward, out ray_front, Mathf.Infinity, ~trapMask))
                {
                    if (ray_front.transform.CompareTag("Wall"))
                    {
                        frontColor = Color.red;

                        line_missile.SetPosition(1, ray_front.point);

                        Debug.DrawRay(tr_shoot.position, tr_shoot.forward * Vector3.Distance(transform.position, ray_front.point),
                            frontColor, 0.1f);
                    }
                    else
                    {
                        line_missile.SetPosition(1, tr_shoot.position + tr_shoot.forward * 0.6f);
                        Debug.DrawRay(tr_shoot.position, tr_shoot.forward * 0.6f, frontColor, 0.1f);
                    }

                    if (ray_front.transform.CompareTag("Header"))
                    {
                        line_missile.gameObject.SetActive(false);
                        Shoot();
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.forward * 1f, frontColor, 0.1f);

                    line_missile.SetPosition(1, tr_shoot.position + tr_shoot.forward * 0.6f);
                }


            }
        }

        public void MissileRayCheck()
        {
            if (isOnce || typeShoot != ShootingType.ONCE)
            {
                return;
            }

            int trapMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                         (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                         (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                           (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                             (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));


            RaycastHit ray_front;
            //Physics.Raycast(transform.position, transform.forward, out ray_front, Mathf.Infinity, ~trapMask);

            Color frontColor = Color.blue;

            line_missile.gameObject.SetActive(true);
            line_missile.SetPosition(0, tr_shoot.position);

            if (Physics.Raycast(tr_shoot.position, tr_shoot.forward, out ray_front, Mathf.Infinity, ~trapMask))
            {
                if (ray_front.transform.CompareTag("Wall") ||
                    ray_front.transform.CompareTag("Ground"))
                {
                    frontColor = Color.red;

                    line_missile.SetPosition(1, ray_front.point);

                    Debug.DrawRay(tr_shoot.position, tr_shoot.forward * Vector3.Distance(transform.position, ray_front.point),
                        frontColor, 0.1f);
                }
                else
                {
                    line_missile.SetPosition(1, ray_front.point);
                    Debug.DrawRay(tr_shoot.position, tr_shoot.forward * Vector3.Distance(transform.position, ray_front.point),
                        frontColor, 0.1f);
                }

                if (ray_front.transform.CompareTag("Header"))
                {
                    line_missile.gameObject.SetActive(false);
                    Shoot();
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * 1f, frontColor, 0.1f);

                line_missile.SetPosition(1, tr_shoot.position + tr_shoot.forward * 0.6f);
            }
        }


        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }


        /// <summary>
        /// 6/17/2024-LYI
        /// 미사일 파티클 출력
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="tr_start"></param>
        public void PlayParticle(GameObject origin, Transform tr_start)
        {
            GameObject go = gameMgr.objPoolingMgr.CreateObject(list_disable, origin, tr_start.position, tr_active);
            go.transform.rotation = Quaternion.LookRotation(tr_start.forward);

            ParticleSystem p = go.GetComponentInParent<ParticleSystem>();
            if (p == null)
            {
                p = go.GetComponentInChildren<ParticleSystem>();
            }
            PlayParticle(p);

            StartCoroutine(ResetTokParticle(p));
        }



        IEnumerator ResetTokParticle(ParticleSystem p)
        {
            yield return new WaitForSeconds(p.main.duration + 1f); //유예시간 1초 추가

            gameMgr.objPoolingMgr.ObjectInit(list_disable, p.gameObject, tr_disable);
        }

        public void PlayParticle(ParticleSystem p)
        {
            ParticleSystem[] arr_p = p.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < arr_p.Length; i++)
            {
                arr_p[i].Play();
            }
        }

    }
}