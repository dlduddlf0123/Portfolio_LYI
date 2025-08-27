using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{
    //각 필드의 스폰 포인트 위치값을 받아올 리스트
    public List<Transform> FieldList { get; private set; }

    // 스폰위치는 각 맵의 순/역 방향 2가지 이기 때문에 따로 위치를 저장
    public Transform StartPos { get; set; }
    // 각 맵에 따로 존재하는 NPC풀의 객체 Trandform 정보
    public Transform trNPC_Pool { get; private set; }

    // 활성화 된 NPC를 저장할 List
    public List<NPC> listActives { get; private set; }
    // 근접 NPC Pool List
    public List<NPC> meleeNPC_Pool { get; set; }
    // 원거리 NPC Pool List
    public List<NPC> rangeNPC_Pool { get; set; }

    public NPC NPC_Boss { get; private set; }

    // 순방향 스폰인지 역방향 스폰인지 구별을 위한 변수
    public bool isReverse { get; set; }
    public bool isSceneReverse { get; set; }
    // 현재 스폰되어 있는 NPC의 개체 수를 저장하기 위한 변수
    //public int nCurrentSpawn { get; set; }

    //게임 매니저
    private GameManager GameMgr;

    // 스폰 개체 수를 랜덤으로 선택하여 쓰기 위한 List
    private List<int> listSpawnNum;

    // 스폰 구역내에 6개의 스폰 위치를 랜덤으로 선택하여 쓰기 위한 List
    private List<int> listSpawnPoint;

    private int currentField;

    private int EventKPM;
    private void Awake()
    {
        FieldList = new List<Transform>();
        listActives = new List<NPC>();
        GameMgr = MonoSingleton<GameManager>.Inst;

        isSceneReverse = false;
        currentField = 0;
        EventKPM = 0;
    }

    // 4~6사이의 18개의 스폰 개체 수를 랜덤으로 선택하여 쓰기 위한 List 초기화
    public void initSpawnNum()
    {
        if (listSpawnNum != null)
            listSpawnNum.Clear();
        else
            listSpawnNum = new List<int>();

        for (int index = 0; index < 18; index++)
            listSpawnNum.Add(4 + index / 6);
    }

    // 스폰 구역내에 6개의 스폰 위치를 랜덤으로 선택하여 쓰기 위한 List 초기화
    public void initSpawnPoint()
    {
        if (listSpawnPoint != null)
            listSpawnPoint.Clear();
        else
            listSpawnPoint = new List<int>();

        for (int index = 0; index < 6; index++)
            listSpawnPoint.Add(index);
    }

    // 근접, 원거리 NPC Pool(List)을 초기화한다. Scene 전환시 Map Class Awake()에서 호출
    public void setNPC_Pool(Transform _tr)
    {
        trNPC_Pool = _tr;

        if (meleeNPC_Pool != null)
            meleeNPC_Pool.Clear();
        if (rangeNPC_Pool != null)
            rangeNPC_Pool.Clear();

        meleeNPC_Pool = new List<NPC>(_tr.GetChild(1).GetChild(0).GetComponentsInChildren<NPC>(true));
        rangeNPC_Pool = new List<NPC>(_tr.GetChild(1).GetChild(1).GetComponentsInChildren<NPC>(true));
        NPC_Boss = _tr.GetChild(2).GetComponent<NPC>();
    }

    public void SpawnStart()
    {
        this.StopAllCoroutines();   

        GameMgr.Main_UI.SkillCtrl.ResetSkillCoolTime();

        if (GameMgr.PlayData.PlayerData.KPM >= Rabyrinth.ReadOnlys.Defines.BOSS_KPM)
            GameMgr.Main_UI.SetBossButton(true);
        else
            GameMgr.Main_UI.SetBossButton(false);

        if(GameMgr.isEvent)
            GameMgr.Main_UI.SetBossButton(false);

        StartCoroutine(Spawn());
    }

    public void BossSpawn()
    {
        if (GameMgr.PlayData.PlayerData.KPM < Rabyrinth.ReadOnlys.Defines.BOSS_KPM)
            return;

        StopAllCoroutines();

        GameMgr.Main_UI.Cur_KPM.text = "Knight";

        for (int index = 0; index < listActives.Count; index++)
            listActives[index].EnemyDeath(true);

        listActives.Clear();
        GameMgr.Player.listTarget.Clear();

        NPC_Boss.transform.position =
            FieldList[currentField < 5 ? currentField + 1 : 4].
            GetChild(UnityEngine.Random.Range(0, 3)).
            GetChild(UnityEngine.Random.Range(0, 6)).transform.position;

        NPC_Boss.gameObject.SetActive(true);

        listActives.Add(NPC_Boss);
        GameMgr.Player.listTarget.Add(NPC_Boss);

        GameMgr.Player.Status.HP = GameMgr.Player.Status.MaxHP;
        GameMgr.Main_UI.PlayerHpBar.Hp_Bar.value = 1.0f;
        GameMgr.Player.SP = 0;

        StartCoroutine(WaitBossKill());
    }

    public void GStarEvent()
    {
        StopAllCoroutines();

        for (int index = 0; index < listActives.Count; index++)
            listActives[index].EnemyDeath(true);

        listActives.Clear();
        GameMgr.Player.listTarget.Clear();

        System.Action action = () =>
        {
            GameMgr.Main_UI.popUpController.PopButton(0);
            GameMgr.Main_UI.popUpController.callBack_GEvent = () =>
            {
                GameMgr.Main_UI.GstarButton.SetActive(false);
                GameMgr.Main_UI.GstarButton_BG.SetActive(false);

                GameMgr.Player.SetTrail(0.0f, Rabyrinth.ReadOnlys.WeaponState.Default);

                StartCoroutine(GameMgr.Main_UI.SetUIText(true));

                Time.timeScale = 1.0f;
                GameMgr.isPlay = false;
                GameMgr.isEvent = true;
                GameMgr.Main_UI.Reset_UI();

                StartCoroutine(GameMgr.ChangeScene(5));
            };
        };

        GameMgr.Main_UI.popUpController.PopUpEvent(action);
    }

    private IEnumerator WaitBossKill()
    {
        while (listActives.Count > 0)
        {
            GameMgr.Main_UI.KpmSet((int)((float)listActives[0].Status.HP / (float)listActives[0].Status.MaxHP * 100.0f),
                 (int)((float)GameMgr.Player.Status.HP / (float)GameMgr.Player.Status.MaxHP * 100.0f));
            yield return null;
        }
        StartCoroutine(SpawnEnd(true));
    }

    private IEnumerator Spawn()
    {
        //스폰 수 리스트 초기화
        initSpawnNum();

        StartCoroutine(RegisterStagePlayTime());
        

        //플레이어가 생성됬는지 획인
        while (GameMgr.Player == null)
            yield return null;

        //플래이어 스폰(위치 지정) 및 활성, Trace 시작
        GameMgr.Player.transform.position = StartPos.position;
        GameMgr.Main_Cam.transform.position = new Vector3(
            GameMgr.Player.transform.position.x,
            GameMgr.Player.transform.position.y + 7.0f,
            GameMgr.Player.transform.position.z - 8.0f);
        GameMgr.Player.gameObject.SetActive(true);
        GameMgr.Player.StartTrace();

        // 필드 수 만큼 루프
        for (int row = 0; row < FieldList.Count; row++)
        {
            currentField = row;
            // Actives의 하위 객체를 모두 비운다.
            //setEmptyActives();
            // 플레이어가 타겟을 삼을 적 리스트를 초기화한다.
            //GameMgr.Player.emptyListNPC();

            //스폰 구역 수 만큼 루프
            for (int col = 0; col < FieldList[row].childCount - 1; col++) //마지막 자식 객체는 필드이기 때문에 -1
            {
                // 스폰 위치(총 6개) 만큼의 정수형 리스트를 초기화한다.
                // 6개 지점에 랜덤으로 배치하기 위한 것 이며, 한 사이클의 스폰 시 마다 리스트를 재설정한다.
                initSpawnPoint();

                // 4~6사이의 랜덤한 18개 정수형 리스트에서 랜덤한 1개의 수를 설정한다.
                int rand = UnityEngine.Random.Range(0, listSpawnNum.Count);


                //nCurrentSpawn += listSpawnNum[rand];

                // 원거리 NPC와 근거리 NPC 각각의 스폰 수를 지정한다.
                int nRangeSpawn = UnityEngine.Random.Range(1, listSpawnNum[rand] != 4 ? 4 : 3);
                int nMeleeSpawn = listSpawnNum[rand] - nRangeSpawn;

                // 스폰 할 개체 수 만큼 루프
                for (int index = 0; index < listSpawnNum[rand]; index++)
                {
                    // 스폰 구역안에 총 6개 스폰 위치 중 한 곳을 랜덤으로 설정한다. 
                    int randSpawnPoint = UnityEngine.Random.Range(0, listSpawnPoint.Count);

                    // 원거리 몬스터 스폰수가 아직 남았다면 원거리 NPC Pool을 넘기고, 아니면 근접 Pool을 넘긴다. 
                    if (nRangeSpawn-- > 0)
                        activeNPC(rangeNPC_Pool,
                            FieldList[row].GetChild(col).GetChild(listSpawnPoint[randSpawnPoint]).position);
                    else if (nMeleeSpawn-- > 0)
                        activeNPC(meleeNPC_Pool,
                            FieldList[row].GetChild(col).GetChild(listSpawnPoint[randSpawnPoint]).position);

                    // 한번 스폰 된 위치에 중복 스폰되지 않도록 스폰 위치 리스트에서 선택 되었던 위치를 삭제한다.
                    listSpawnPoint.Remove(randSpawnPoint);
                }

                // 랜덤 선택했던 스폰 개체 수를 리스트에서 삭제한다. 
                listSpawnNum.Remove(rand);
            }

            //스폰될 몬스터를 모두 정한후 플레이어 캐릭터 타겟풀에 저장
            GameMgr.Player.SetListNPC(listActives);

            if(row == 5 && GameMgr.isEvent)
            {
                NPC_Boss.transform.position = FieldList[row].GetChild(1).GetChild(5).transform.position;

                NPC_Boss.gameObject.SetActive(true);

                listActives.Add(NPC_Boss);
                GameMgr.Player.listTarget.Add(NPC_Boss);
            }

            //현재 스폰 수가 0이 될때까지 대기
            while (GameMgr.spawnManager.listActives.Count != 0)
                yield return null;
        }

        Time.timeScale = 0.1f;
        //더이상 스폰 할 것이 없으면 다음 스테이지로 이동.
        GameMgr.isPlay = false;

        if(GameMgr.isEvent)
            GameMgr.AWS_Mgr.SaveRankingData(GameMgr.EventName, GameMgr.fCurrentStagePlayTime, EventKPM);
        //랭킹 데이터 등록
    }

    private IEnumerator RegisterStagePlayTime()
    {
        GameMgr.isPlay = true;
        GameMgr.fCurrentStagePlayTime = 0.0f;
        GameMgr.nCurrentDeadNPC = 0;
        int currentKPM = 0;
        while (GameMgr.isPlay)
        {
            yield return new WaitForSeconds(0.1f);
            GameMgr.fCurrentStagePlayTime = (float)Math.Round(GameMgr.fCurrentStagePlayTime + 0.1f, 2);

            //if(GameMgr.nCurrentDeadNPC > 0)
            //    currentKPM = (int)(( 60.0f / ((float)(90 / GameMgr.nCurrentDeadNPC) * GameMgr.fCurrentStagePlayTime)) * 90.0f);

            if (GameMgr.nCurrentDeadNPC > 0)
            {
                float MaxNPC_Div_CurDead = (float)Math.Round(90.0f / (float)GameMgr.nCurrentDeadNPC, 2);
                float MDC_Mul_PlayTime = (float)Math.Round(MaxNPC_Div_CurDead * GameMgr.fCurrentStagePlayTime, 2);
                float Minute_Mul_MMP = (float)Math.Round(60.0f / MDC_Mul_PlayTime, 2);
                currentKPM = (int)Math.Round(Minute_Mul_MMP * 90.0f);
                GameMgr.Main_UI.Cur_KPM.text = currentKPM.ToString();

                if (GameMgr.isEvent)
                    EventKPM = currentKPM;
            }

            GameMgr.Main_UI.KpmSet(GameMgr.isEvent ? GameMgr.PlayData.lRankingData[0].Score : GameMgr.PlayData.PlayerData.KPM, currentKPM);
        }
        if (!GameMgr.isEvent)
            GameMgr.PlayData.PlayerData.KPM = currentKPM;

        StartCoroutine(SpawnEnd());
    }

    public IEnumerator SpawnEnd(bool isBoss = false)
    {
        int nextStage = 0;
        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)
        {
            case 2:
                if (isSceneReverse)
                    nextStage = 5;
                else
                    nextStage = 3;

                isSceneReverse = false;
                break;
            case 3:
                if (isSceneReverse)
                    nextStage = 2;
                else
                    nextStage = 4;
                break;
            case 4:
                nextStage = 3;
                isSceneReverse = true;
                break;
            case 5:
                nextStage = 6;
                break;
            case 6:
                nextStage = 2;
                break;
        }

        System.Action action = () =>
        {
            GameMgr.Main_UI.GstarButton.SetActive(true);
            GameMgr.Main_UI.GstarButton_BG.SetActive(true);

            StartCoroutine(GameMgr.Main_UI.SetUIText());

            Time.timeScale = 1.0f;
            GameMgr.isPlay = false;

            GameMgr.Main_UI.Reset_UI();
            GameMgr.Player.listTarget.Clear();

            listActives.Clear();

            StartCoroutine(GameMgr.ChangeScene(nextStage));
        };

        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1.0f;

        if(GameMgr.isEvent)
        {
            GameMgr.isEvent = false;
            DateTime time = DateTime.MinValue + TimeSpan.FromSeconds(GameMgr.fCurrentStagePlayTime);
            GameMgr.Main_UI.popUpController.PopUpMiddle("P, ,L, ,A, ,Y,   ,T, ,I, ,M, ,E,    ," +
                time.ToString("m:ss") + "\nSCORE\t\t\t\t\t\t" + EventKPM.ToString(), action);
        }
        else if (isBoss)
        {
            PlayerPrefs.SetInt("BossButton", 1);
            GameMgr.PlayData.PlayerData.CurrentFloor++;
            GameMgr.PlayData.PlayerData.MaxFloor++;
            GameMgr.PlayData.PlayerData.KPM = 0;
            GameMgr.Main_UI.popUpController.PopUpMiddle("G,o, ,t,o, ,n,e,x,t, ,f,l,o,o,r.", action);
        }
        else if(GameMgr.Player.Status.HP > 0)
        {
            DateTime time = DateTime.MinValue + TimeSpan.FromSeconds(GameMgr.fCurrentStagePlayTime);

            GameMgr.Main_UI.popUpController.PopUpMiddle(
                "P, ,L, ,A, ,Y,\t,T, ,I, ,M, ,E,\t\t," +
                time.ToString("m:ss") +
                "\n,S,C,O,R,E,\t\t\t\t\t\t," +
                GameMgr.PlayData.PlayerData.KPM +
                "\n,B,l,a,c,k, ,c,r,i,s,t,a,l,\t\t," + GameMgr.Main_UI.getCrystal,
                action);

            GameMgr.Main_UI.getCrystal = 0;
        }
        else
        {
            StopAllCoroutines();

            string str = "D,E,A,D,!,\n,D,o, ,y,o,u, ,w,a,n,t, ,r,e,v,i,v,e,?";

            //if (GameMgr.PlayData.PlayerData.KPM == 0)
            //{
            //    //마력도 1 깍아야함.
            //    GameMgr.PlayData.PlayerData.CurrentFloor--;
            //    str = "D,E,A,D,!,\n,T,h,e, ,c,u,r,r,e,n,t, ,f,l,o,o,r, ,s,c,o,r,e, ,0,\n,G,o, ,d,o,w,n,s,t,a,i,r,s.";
            //}
            //else
            //{
            //    str = "D,E,A,D,!,\n,D,o, ,y,o,u, ,w,a,n,t, ,r,e,v,i,v,e,?";
            //}

            GameMgr.Main_UI.popUpController.PopUpMiddle(str, action);
        }

        GameMgr.Player.SetTrail(0.0f, Rabyrinth.ReadOnlys.WeaponState.Default);

        GameMgr.AWS_Mgr.SavePlayerDataSecu(GameMgr.PlayData.PlayerData, true);
    }
    

    // NPC 스폰시 활성화 역할 및 Pool List 간의 이동 수행.
    private void activeNPC(List<NPC> _listNPC, Vector3 _pos)
    {
        int rand = UnityEngine.Random.Range(0, _listNPC.Count);

        _listNPC[rand].transform.parent = trNPC_Pool.GetChild(0);

        _listNPC[rand].transform.position = _pos;

        listActives.Add(_listNPC[rand]);

        _listNPC[rand].gameObject.SetActive(true);

        _listNPC.RemoveAt(rand);
    }

    // 활성화 NPC Pool을 비우고 NPC 속성에 맞는 NPC Pool로 다시 넣는다.  
    private void setEmptyActives()
    {
        for (int index = 0; index < listActives.Count; index++)
        {
            if (listActives[index].Status.Type == 1000)
            {
                listActives[index].transform.parent = GameMgr.spawnManager.trNPC_Pool.GetChild(1).GetChild(0);
                GameMgr.spawnManager.meleeNPC_Pool.Add(listActives[index]);
            }
            else
            {
                listActives[index].transform.parent = GameMgr.spawnManager.trNPC_Pool.GetChild(1).GetChild(1);
                GameMgr.spawnManager.rangeNPC_Pool.Add(listActives[index]);
            }

            listActives[index].gameObject.SetActive(false);
        }

        listActives.Clear();
    }
}