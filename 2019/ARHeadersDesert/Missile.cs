using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.ReadOnly;

public class Missile : MonoBehaviour
{
    GameManager gameMgr;
    // 물리기반을 이용하기 위해 붙인 리지드바디 활용위해

    ParticleSystem pBubble;
    Rigidbody rb;
    MeshRenderer mesh;
    SphereCollider coll;

    AudioClip sfx_pop;
    AudioClip sfx_splash;

    //public float speed;
    public float popTime = 0.0f; //시간세기용 변수
    public float durationTime = 3.0f;
    public int damage = 1;  //데미지

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        pBubble = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        rb = this.GetComponent<Rigidbody>();
        mesh = this.GetComponent<MeshRenderer>();
        coll = this.GetComponent<SphereCollider>();
        sfx_pop = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_POP) as AudioClip;
        sfx_splash = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_WATERSPLASH) as AudioClip;
    }

    void OnEnable()
    {
        popTime = 0.0f;
        //활성화 시 좌표 보정
        this.transform.position = gameMgr.missileMgr.firePos.position;
        this.transform.rotation = gameMgr.missileMgr.firePos.rotation;
    }

    //일정 시간 이후에 총알 없애기
    private void Update()
    {
        if (popTime < durationTime)
        {
            popTime += Time.deltaTime;
            transform.LookAt(gameMgr.missileMgr.mainCam.transform);
        }
        else
        {
            StartCoroutine(Pop());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //지형과 캐릭터 외에는 충돌하지 않는다
        if (other.CompareTag("Ground"))
        {
            StartCoroutine(Pop());
            //방울 터지는 소리
            gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_pop);
            gameMgr.PlayEffect(this.transform.position, gameMgr.particles[3]);
        }
        else if (!other.CompareTag("Point")
            && !other.CompareTag("ball")
            && !other.CompareTag("Watch")
            && !other.CompareTag("Rain")
            && !other.CompareTag("MainCamera"))
        {
            StartCoroutine(Pop());
            //방울 터지는 소리
            gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_pop);
            gameMgr.PlayEffect(this.transform.position, gameMgr.particles[2]);
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    /// <summary>
    /// 방울이 터질 때의 동작
    /// </summary>
    public IEnumerator Pop()
    {
        //pBubble.Play(); //이펙트 재생

        popTime = 0.0f;

        mesh.enabled = false;
        coll.enabled = false;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(1f);
        mesh.enabled = true;
        gameMgr.missileMgr.MissileInit(gameObject);
        gameMgr.missileMgr.list_missile.Add(this.gameObject);
    }
}
