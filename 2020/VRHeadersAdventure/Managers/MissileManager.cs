using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어의 동작. 즉, 총을 쏘게하는 코드
public class MissileManager : MonoBehaviour
{
    //클래스 호출
    GameManager gameMgr;

    //내부 
    public GameObject bubblePrefab;
    public GameObject firePos;

    public AudioClip sfx_fire { get; set; }

    public List<GameObject> list_missile { get; set; }

    //장착 시 변경되는 변수들
    public int missileCount { get; set; }

    public int missileDamage { get; set; }  //데미지
    public float missileSpeed { get; set; }  //탄속
    public float shotDelay { get; set; } //장전 속도

    private float delay;    //시간 세기용 변수

    int spreadNum = 5;
    float spreadRange = 0.15f;

    void Awake()
    {
        gameMgr = GameManager.Instance;
        //mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        list_missile = new List<GameObject>();

        missileDamage = 10;
        missileSpeed = 5f;
        shotDelay = 0.0001f;
        delay = 0f;
    }

    void Start()
    {
        firePos = FindObjectOfType<MyRightHand>().gameObject;
        //총알 생성 
        for (int i = 0; i < 20; i++)
        {
            GameObject missile = Instantiate(bubblePrefab, firePos.transform.position, firePos.transform.rotation);
            missile.name = "missile";
            MissileInit(missile);
        }
    }

    private void OnEnable()
    {
        delay = 0f;
    }

    public void SetFirePos(GameObject _go)
    {
        firePos = _go;
    }

    //미사일 초기화
    public void MissileInit(GameObject _go)
    {
        _go.transform.position = firePos.transform.position;
        _go.transform.rotation = firePos.transform.rotation;
        _go.transform.parent = this.transform;

        _go.GetComponent<Missile>().damage = missileDamage;
        _go.GetComponent<MeshRenderer>().enabled = true;
        _go.GetComponent<SphereCollider>().enabled = true;
        _go.SetActive(false);

        list_missile.Add(_go);
    }
    /// <summary>
    /// 화면 클릭 시 동작
    /// </summary>
    public void Fire()
    {
        if (delay > 0)
        {
            return;
        }
        delay = shotDelay;
        StartCoroutine(ShotCooltime());
        ShotSpreadMissile();
    }

    /// <summary>
    /// 한번에 퍼지는 5개의 미사일을 발사한다.
    /// </summary>
    public void ShotSpreadMissile()
    {
        for (int i = 0; i < spreadNum; i++)
        {
            if (this.transform.childCount <= spreadNum)
            {
                GameObject missile = Instantiate(bubblePrefab, firePos.transform.position, firePos.transform.rotation);
                missile.name = "missile";
                MissileInit(missile);
            }
            float randX = Random.Range(-spreadRange, spreadRange);
            float randY = Random.Range(-spreadRange, spreadRange);
            float randZ = Random.Range(-spreadRange, spreadRange);
            float randSpd = Random.Range(missileSpeed*0.5f, missileSpeed*2f);
            Vector3 targetPos = new Vector3(
                firePos.transform.forward.x + randX,
                firePos.transform.forward.y + randY,
                 firePos.transform.forward.z + randZ);

            list_missile[i].SetActive(true);
            list_missile[i].transform.parent = gameMgr.transform;
            list_missile[i].GetComponent<Rigidbody>().velocity = targetPos * randSpd;
            list_missile[i].transform.rotation = Quaternion.LookRotation(list_missile[i].GetComponent<Rigidbody>().velocity);
            list_missile.RemoveAt(i);
        }

       // gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
    }

    /// <summary>
    /// 화면 중앙을 향해 미사일을 발사한다
    /// </summary>
    void ShotNormalMissile()
    {
        if (this.transform.childCount == 0)
        {
            GameObject missile = Instantiate(bubblePrefab, firePos.transform.position, firePos.transform.rotation);
            missile.name = "missile";
            MissileInit(missile);
        }

        list_missile[0].SetActive(true);
        list_missile[0].transform.parent = gameMgr.transform;
        list_missile[0].GetComponent<Rigidbody>().velocity = firePos.transform.forward * missileSpeed;
        list_missile[0].transform.rotation = Quaternion.LookRotation(list_missile[0].GetComponent<Rigidbody>().velocity);
        list_missile.RemoveAt(0);

       // gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
    }
    
    //쿨타임 돌리기
    IEnumerator ShotCooltime()
    {
        while (delay > 0)
        {
            yield return new WaitForSeconds(0.1f);
            delay -= 0.1f;
        }
    }


}
