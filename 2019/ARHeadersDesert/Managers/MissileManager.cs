using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using Project.ReadOnly;

public enum BubbleType
{
    NORMAL = 0,
    SPREAD,
    SNIPE,
    REPEAT,
}

//플레이어의 동작. 즉, 총을 쏘게하는 코드
public class MissileManager : MonoBehaviour, IPointerDownHandler
{
    //클래스 호출
    GameManager gameMgr;

    //내부 
    //GameObject particle = null;
    public Camera mainCam;
    public AimMove aim; //조준점
    public Transform firePos; //발사하는 지점
    public GameObject aims;

    public AudioClip sfx_fire { get; set; }

    public List<GameObject> list_missile { get; set; }

    //장착 시 변경되는 변수들
    public BubbleType bubbleType;
    public int missileCount { get; set; }

    public int missileDamage; //데미지
    public float missileSpeed;  //탄속
    public float shotDelay;//장전 속도

    //Spread Shot
    public float spreadNum = 5;
    public float spreadRange = 0.3f; //Change Shooting Range

    //Power Shot
    float maxDmg = 2f; //x3 on Max range damage
    float maxSize = 0.2f;

    //Repeat Shot
    public float repeatNum = 3;

    //Snipe Shot
    int snipeDmg = 3;
    float minSize = 0.1f;
    public float snipeMaxTime = 0.1f;

    private float delay;    //시간 세기용 변수

    void Awake()
    {
        gameMgr = GameManager.Instance;
        //mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        list_missile = new List<GameObject>();

        missileDamage = 10;
        missileSpeed = gameMgr.stage.transform.localScale.x * 20f;
        shotDelay = 0.3f;
        delay = 0f;
        bubbleType =(BubbleType)PlayerPrefs.GetInt("BubbleType",0);

        aims = mainCam.transform.GetChild(0).gameObject;
        firePos = aims.transform.GetChild(0);
    }

    void Start()
    {
        sfx_fire = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_BUBBLE) as AudioClip;
        //mainCam.transform.localScale = gameMgr.stage.transform.localScale;
        aims.transform.localScale = gameMgr.stage.transform.localScale;

        if (gameMgr.shopMgr.currentItem == null)
        {
            gameMgr.shopMgr.currentItem = gameMgr.shopMgr.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(PlayerPrefs.GetInt("BubbleType")).GetComponent<ShopItem>();
        }
        gameMgr.shopMgr.ItemInit(gameMgr.shopMgr.currentItem, PlayerPrefs.GetInt("BubbleType"));
        SetMissileStat(gameMgr.shopMgr.currentItem);

        //총알 생성 
        for (int i = 0; i < 20; i++)
        {
            GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
            missile.name = "missile";
            list_missile.Add(missile);
            MissileInit(missile);
        }
    }

    private void OnEnable()
    {
        delay = 0f;
        aim.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        aim.gameObject.SetActive(false);
    }

    //미사일 초기화
    public void RefreshMissile()
    {
        if (list_missile == null || list_missile.Count == 0) { return; }

        foreach (var _missile in list_missile)
        {
            MissileInit(_missile);
        }
    }


    //미사일 초기화
    public void MissileInit(GameObject _go)
    {
        _go.transform.position = mainCam.transform.position;
        _go.transform.rotation = mainCam.transform.rotation;
        _go.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
        _go.transform.parent = this.transform.GetChild(0);

        _go.GetComponent<Missile>().damage = missileDamage;
        _go.GetComponent<MeshRenderer>().enabled = true;
        _go.GetComponent<SphereCollider>().enabled = true;
        _go.SetActive(false);
    }

    /// <summary>
    /// 각 미사일 별 능력치 설정
    /// CSV파일 능력치 적용
    /// </summary>
    public void SetMissileStat(ShopItem _item)
    {
        bubbleType = _item.type;

        if (gameMgr == null)
        {
            gameMgr = GameManager.Instance;
        }

        missileDamage = _item.damage;
        missileSpeed = gameMgr.stage.transform.localScale.x * _item.speed;
        shotDelay = _item.delay;

        switch (bubbleType)
        {
            case BubbleType.NORMAL:
                break;
            case BubbleType.SPREAD:
                spreadNum = gameMgr.ReadMissileData(1, 4) + _item.level / 5 * 2;
                break;
            case BubbleType.SNIPE:
                snipeDmg = (int)gameMgr.ReadMissileData(2, 3);
                minSize = 0.1f;
                break;
            case BubbleType.REPEAT:
                repeatNum = gameMgr.ReadMissileData(3, 4) + _item.level / 5;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 화면 클릭 시 동작
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnMouseDown");
        if (delay > 0)
        {
            return;
        }
        delay = shotDelay;
        StartCoroutine(ShotCooltime());

        if (gameMgr.limit_missiles == 0)
        {
            ShotMissile(bubbleType);
        }
        else //미사일 개수 제한 모드
        {
            //미사일이 0개 이상일 때
            if (missileCount >= 0)
            {
                ShotMissile(bubbleType);
                missileCount--;
                gameMgr.uiMgr.game_missiles.text = missileCount.ToString();
            }

            //미사일을 다썼을 때
            if (missileCount < 0)
            {
                gameMgr.isClear = false;
                gameMgr.GameOver(gameMgr.isClear);
            }
        }
    }

    /// <summary>
    /// 각 방울 타입에 따른 미사일 발사
    /// </summary>
    /// <param name="_b"> 방울 타입 </param>
    void ShotMissile(BubbleType _b)
    {
        switch (_b)
        {
            case BubbleType.NORMAL:
                ShotNormalMissile();
                break;
            case BubbleType.SPREAD:
                ShotSpreadMissile();
                break;
            case BubbleType.SNIPE:
                StartCoroutine(ShotSnipeMissile());
                break;
            case BubbleType.REPEAT:
                StartCoroutine(ShotRepeatMissile());
                break;
            default:
                ShotNormalMissile();
                break;
        }
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

    #region ShotMissileTypes
    /// <summary>
    /// 화면 중앙을 향해 미사일을 발사한다
    /// </summary>
    public void ShotNormalMissile()
    {
        if (list_missile.Count == 0)
        {
            GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
            missile.transform.parent = this.transform.GetChild(0);
            missile.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
            missile.name = "missile";
            missile.SetActive(false);
            list_missile.Add(missile);
        }
        list_missile[0].GetComponent<Missile>().durationTime =3f;

        list_missile[0].SetActive(true);
        list_missile[0].GetComponent<Rigidbody>().velocity = (aim.transform.position - firePos.position) * missileSpeed;
        list_missile[0].transform.rotation = Quaternion.LookRotation(list_missile[0].GetComponent<Rigidbody>().velocity);
        list_missile.RemoveAt(0);

        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
    }

    /// <summary>
    /// 한번에 퍼지는 5개의 미사일을 발사한다.
    /// </summary>
    public void ShotSpreadMissile()
    {
        for (int i = 0; i < spreadNum; i++)
        {
            if (list_missile.Count <= spreadNum)
            {
                GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
                missile.transform.parent = this.transform.GetChild(0);
                missile.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
                missile.name = "missile";
                missile.SetActive(false);
                list_missile.Add(missile);
            }
            float randX = Random.Range(- spreadRange, spreadRange);
            float randY = Random.Range(-spreadRange, spreadRange);
            float randZ = Random.Range(-spreadRange, spreadRange);

            Vector3 targetPos = new Vector3(
                aim.transform.position.x + randX,
                aim.transform.position.y + randY,
                aim.transform.position.z + randZ);

            list_missile[i].GetComponent<Missile>().durationTime = 0.5f;

            list_missile[i].SetActive(true);
            list_missile[i].GetComponent<Rigidbody>().velocity = (targetPos - firePos.position) * missileSpeed;
            list_missile[i].transform.rotation = Quaternion.LookRotation(list_missile[i].GetComponent<Rigidbody>().velocity);
            list_missile.RemoveAt(i);
        }

        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
    }

    /// <summary>
    /// 점점 커지면서 데미지가 상승하는 미사일을 발사한다
    /// </summary>
    public IEnumerator ShotPowerMissile()
    {
        if (list_missile.Count  == 0)
        {
            GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
            missile.transform.parent = this.transform.GetChild(0);
            missile.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
            missile.name = "missile";
            missile.SetActive(false);
            list_missile.Add(missile);
        }

        Missile _m = list_missile[0].GetComponent<Missile>();
        _m.transform.localScale = new Vector3(maxSize, maxSize, maxSize);
        _m.damage = (int)maxDmg;
        _m.durationTime = 3.0f;

        list_missile[0].SetActive(true);
        list_missile[0].transform.parent = gameMgr.transform;
        list_missile[0].GetComponent<Rigidbody>().velocity = (aim.transform.position - firePos.position) * missileSpeed;
        list_missile[0].transform.rotation = Quaternion.LookRotation(list_missile[0].GetComponent<Rigidbody>().velocity);
        list_missile.RemoveAt(0);


        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
        yield break;

        //float t = 0.0f;
        //while (_m.gameObject.activeSelf == true
        //    && _m.transform.localScale.x < maxSize)
        //{
        //    //_m.transform.localScale = new Vector3(
        //    //    _m.transform.localScale.x+ missileSize,
        //    //    _m.transform.localScale.y + missileSize,
        //    //    _m.transform.localScale.z + missileSize);
        //    _m.transform.localScale *= missileSize;
        //    t += 0.01f;

        //    if (_m.damage < maxDmg
        //        && t >= 0.25f)
        //    {
        //        t = 0.0f;
        //        _m.damage += _m.damage;
        //    }
        //    yield return new WaitForSeconds(0.01f);
        //}
    }

    /// <summary>
    /// 한번의 클릭으로 여러발의 미사일을 발사한다
    /// </summary>
    public IEnumerator ShotRepeatMissile()
    {
        for (int i = 0; i < repeatNum; i++)
        {
            if (list_missile.Count <= repeatNum)
            {
                GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
                missile.transform.parent = this.transform.GetChild(0);
                missile.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
                missile.name = "missile";
                missile.SetActive(false);
                list_missile.Add(missile);
            }
            list_missile[i].GetComponent<Missile>().durationTime = 3.0f;

            list_missile[i].SetActive(true);
            list_missile[i].GetComponent<Rigidbody>().velocity = (aim.transform.position - firePos.position) * missileSpeed;
            list_missile[i].transform.rotation = Quaternion.LookRotation(list_missile[0].GetComponent<Rigidbody>().velocity);
            list_missile.RemoveAt(0);
            yield return new WaitForSeconds(0.1f);
        }

        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
    }

    /// <summary>
    /// 작고 빠르고 강력한 미사일을 발사한다
    /// </summary>
    public IEnumerator ShotSnipeMissile()
    {
        if (list_missile.Count == 0)
        {
            GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
            missile.transform.parent = this.transform.GetChild(0);
            missile.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
            missile.name = "missile";
            missile.SetActive(false);
            list_missile.Add(missile);
        }

        Missile _m = list_missile[0].GetComponent<Missile>();
        _m.transform.localScale = new Vector3(minSize, minSize, minSize);
        _m.durationTime = 5.0f;

        list_missile[0].SetActive(true);
        list_missile[0].GetComponent<Rigidbody>().velocity = (aim.transform.position - firePos.position) * missileSpeed;
        list_missile[0].transform.rotation = Quaternion.LookRotation(list_missile[0].GetComponent<Rigidbody>().velocity);
        list_missile.RemoveAt(0);

        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);

        while (_m.popTime < snipeMaxTime)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("MaxDmg");
        _m.damage *= snipeDmg;
    }
    #endregion
}
