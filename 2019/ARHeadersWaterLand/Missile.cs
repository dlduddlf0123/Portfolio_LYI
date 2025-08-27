using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FXV;
using Project.ReadOnly;

public class Missile : MonoBehaviour
{
    GameManager gameMgr;
    FXVShield water;
    // 물리기반을 이용하기 위해 붙인 리지드바디 활용위해

    ParticleSystem pBubble;
    Rigidbody rb;
    MeshRenderer mesh;
    SphereCollider coll;

    AudioClip sfx_pop;
    AudioClip sfx_splash;

    //public float speed;
    float popTime; //미사일 지속시간
    public int damage { get; set; }  //데미지

    float missileSize;

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        pBubble = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        rb = this.GetComponent<Rigidbody>();
        mesh = this.GetComponent<MeshRenderer>();
        coll = this.GetComponent<SphereCollider>();
        sfx_pop = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_POP) as AudioClip;
        sfx_splash = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_WATERSPLASH) as AudioClip;

        popTime = 0.0f;
        damage = 1;
        missileSize = gameMgr.stage.transform.localScale.x*10f;
        water = gameMgr.water;
        //speed = 30;
    }

    void OnEnable ()
    {
        //활성화 시 좌표 보정
        this.transform.position = gameMgr.missileMgr.mainCam.transform.GetChild(0).position;
        this.transform.rotation = gameMgr.missileMgr.mainCam.transform.GetChild(0).rotation;
    }

    //일정 시간 이후에 총알 없애기
    private void Update()
    {
        if(popTime < 3.0f)
        {
            popTime += Time.deltaTime;
            transform.LookAt(gameMgr.missileMgr.mainCam.transform);
        }
        else
        {
            popTime = 0.0f;
            gameMgr.missileMgr.MissileInit(gameObject);
        }
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //지형과 플레이어 외에는 충돌하지 않는다
        if (!other.CompareTag("Point")
            && !other.CompareTag("ball")
            && !other.CompareTag("Watch")
            && !other.CompareTag("MainCamera")
            && !other.CompareTag("Water"))
        {
            StartCoroutine(Pop());
            //방울 터지는 소리
            gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_pop);
        }

        if (other.CompareTag("Water"))
        {
            //물과 충돌시 속도 저하
            rb.velocity *= 0.3f;
            water.OnHit(this.transform.position, gameMgr.stage.transform.localScale.x);
            //물 튀기는 소리
            gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_splash);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            //물과 충돌시 속도 저하
            rb.velocity *= 3f;
        }
    }

    /// <summary>
    /// 방울이 터질 때의 동작
    /// </summary>
    public IEnumerator Pop()
    {
        pBubble.Play();
        popTime = 0.0f;
        mesh.enabled = false;
        rb.velocity = Vector3.zero;
        coll.enabled = false;
        yield return new WaitForSeconds(1f);
        mesh.enabled = true;
        gameMgr.missileMgr.MissileInit(gameObject);
    }
}
