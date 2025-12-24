using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Spawner : MonoBehaviour
{
    public MiniSoundManager soundMgr;
    public MiniUIManager uiMgr;

    public GameState gameState;

    //스폰 위치
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnpoint3;
    public Transform spawnpoint4;

    //오브젝트 풀링 관련 변수
    public GameObject rock;
    public GameObject wood;
    public List<GameObject> list_Rock = null;
    public List<GameObject> list_Wood = null;

    public HandInteract[] hands = new HandInteract[2];
    public ParticleSystem handEffect;


    public int pooledAmount = 7;

    void Awake()
    {
        gameState = GameState.NONE;
        list_Rock = new List<GameObject>();
        list_Wood = new List<GameObject>();
    }

    void Start()
    {
        //지정한 수 만큼 객체 생성해서 리스트에 추가
        for(int i =0; i<pooledAmount; i++) ////돌
        {
            GameObject obj = (GameObject)Instantiate(rock);
            obj.SetActive(false);
            list_Rock.Add(obj);
        }
        for(int  ix = 0; ix<4;ix++) /////통나무
        {
            GameObject obj1 = (GameObject)Instantiate(wood);
            obj1.SetActive(false);
            list_Wood.Add(obj1);
        }
    }

    public void HandEffect(int _hand)
    {
        PlayEffect(hands[_hand].transform.position, handEffect);
        soundMgr.PlaySfx(hands[_hand].transform.position, soundMgr.LoadClip("Sounds/SFX/jump_15"));
    }
    public void PlayEffect(Vector3 _position, ParticleSystem _p)
    {
        _p.transform.position = _position;
        _p.Play();
    }


    public void PushToPool(List<GameObject> _list,GameObject _item)
    {
        _item.SetActive(false);
        _item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        _list.Add(_item);
    }

    public GameObject PopFromPool()
    {
        GameObject item = list_Rock[0];
        list_Rock.RemoveAt(0);
        return item;
    }

    public GameObject PopFromPool1()
    {
        GameObject item = list_Wood[0];
        list_Wood.RemoveAt(0);
        return item;
    }
    /// <summary>
    /// 세가지 패턴에 관한 함수
    /// </summary>
    IEnumerator Pattern1()
    {
        Debug.Log("Pattern1");
        int spawnpoint = Random.Range(0, 3);

        GameObject temprock = PopFromPool();
        switch(spawnpoint)
        {
            case 0:
                temprock.transform.position = spawnPoint1.transform.position;
                break;
            case 1:
                temprock.transform.position = spawnPoint2.transform.position;
                break;
            case 2:
                temprock.transform.position = spawnpoint3.transform.position;
                break;
        }
        temprock.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);
        RandomPattern();
    }
    IEnumerator Pattern2()
    {
        Debug.Log("Pattern2");
        int spawnpoint = Random.Range(0, 3);
        GameObject temprock = PopFromPool();
        GameObject temprock1 = PopFromPool();
        switch (spawnpoint)
        {
            case 0:
                temprock.transform.position = spawnPoint1.transform.position;
                temprock1.transform.position = spawnPoint2.transform.position;
                break;
            case 1:
                temprock.transform.position = spawnPoint2.transform.position;
                temprock1.transform.position = spawnpoint3.transform.position;
                break;
            case 2:
                temprock.transform.position = spawnPoint1.transform.position;
                temprock1.transform.position = spawnpoint3.transform.position;
                break;
        }
        temprock.SetActive(true);
        temprock1.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        RandomPattern();

    }
    IEnumerator Pattern3()
    {

        Debug.Log("Pattern3");
        GameObject tempwood = PopFromPool1();
        tempwood.transform.position = spawnpoint4.transform.position;
        tempwood.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);
        RandomPattern();
    }
    /// <summary>
    /// 랜덤으로 세가지 패턴중 하나를 선택 후 실행
    /// </summary>
    public void RandomPattern()
    {
        int CurrentPattern;
        CurrentPattern = Random.Range(0, 3);
        switch(CurrentPattern)
        {
            case 0:
                StartCoroutine("Pattern1");
                break;
            case 1:
                StartCoroutine("Pattern2");
                break;
            case 2:
                StartCoroutine("Pattern3");
                break;
        }
    }

    public void GameChange(GameState state)
    {
        
        switch (state)
        {
            case GameState.PLAYING:
                {
                    StartCoroutine(Pattern2());
                }
                break;
            case GameState.GAMEOVER:
                {
                    uiMgr.GameOver();
                    gameState = GameState.GAMEOVER;
                }
                break;
            case GameState CLEAR:
                {
                    uiMgr.GameClear();
                }
                break;
        }
    }
}
