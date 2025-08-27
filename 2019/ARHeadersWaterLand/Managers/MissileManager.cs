using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using Project.ReadOnly;

//플레이어의 동작. 즉, 총을 쏘게하는 코드
public class MissileManager : MonoBehaviour, IPointerDownHandler
{
    //클래스 호출
    GameManager gameMgr;
    public AimMove aim;

    //내부 
    //GameObject particle = null;
    public Camera mainCam;

    public AudioClip sfx_fire { get; set; }

    public List<GameObject> list_missile { get; set; }

    public int missileCount { get; set; }
    public float missileSpeed;
    public float shotDelay;

    private float delay;

    void Awake()
    {
        gameMgr = GameManager.Instance;
        //mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        list_missile = new List<GameObject>();

        missileSpeed = gameMgr.stage.transform.localScale.x * 20f;
        shotDelay = 0.3f;
        delay = 0f;

        sfx_fire = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_BUBBLE) as AudioClip;

        //mainCam.transform.localScale = gameMgr.stage.transform.localScale;
        //총알 생성
        for (int i = 0; i < 20; i++)
        {
            GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
            missile.transform.parent = this.transform.GetChild(0);
            missile.transform.localScale = gameMgr.stage.transform.localScale * 0.3f;
            missile.name = "missile[" + i + "]";
            missile.SetActive(false);
            list_missile.Add(missile);
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
    public void MissileInit(GameObject _go)
    {
        _go.transform.position = mainCam.transform.position;
        _go.transform.rotation = mainCam.transform.rotation;
        _go.transform.parent = this.transform.GetChild(0);

        _go.GetComponent<MeshRenderer>().enabled = true;
        _go.GetComponent<SphereCollider>().enabled = true;
        _go.SetActive(false);

        list_missile.Add(_go);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnMouseDown");
        if (delay > 0)
        {
            return;
        }
        delay = shotDelay;
        StartCoroutine(ShotCooltime());


        if (gameMgr.limit_missiles == 0)
        {
            ShotMissile();
        }
        else //미사일 개수 제한 모드
        {
            //미사일이 0개 이상일 때
            if (missileCount >= 0)
            {
                ShotMissile();
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
    /// 화면 중앙을 향해 미사일을 발사한다
    /// </summary>
    public void ShotMissile()
    {
        if (this.transform.GetChild(0).GetChild(0) == null)
        {
            GameObject missile = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_MISSILE) as GameObject, mainCam.transform.position, mainCam.transform.rotation);
            missile.transform.parent = this.transform.GetChild(0);
            list_missile.Add(missile);
        }
        else
        {
            list_missile[0].SetActive(true);
            list_missile[0].transform.parent = gameMgr.transform;
            list_missile[0].GetComponent<Rigidbody>().velocity = (mainCam.transform.GetChild(1).position - mainCam.transform.GetChild(0).position) * missileSpeed;
            list_missile[0].transform.rotation = Quaternion.LookRotation(list_missile[0].GetComponent<Rigidbody>().velocity);
            list_missile.RemoveAt(0);
        }
        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fire);
    }

    IEnumerator ShotCooltime()
    {
        while (delay > 0)
        {
            yield return new WaitForSeconds(0.1f);
            delay-=0.1f;
        }
    }
}
