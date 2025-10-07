using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

namespace Burbird
{

    /// <summary>
    /// 스테이지에 대한 정보를가진 클래스
    /// 스테이지매니저에서 참조하여 룸, 적 캐릭터 등을 불러온다
    /// 스테이지 번호와 클리어 여부, 최대 스테이지 정보는 데이터매니저에서 알고 있어야 함?
    /// </summary>
    public class StageData
    {
        AddressableManager addressMgr;

        //ReadFromJson
        public JsonStage currentStageJson;

        public int stageNum; //스테이지 번호
        public string stageName; //스테이지 이름
        public int maxRoom; //최대 스테이지
        public int steminaForPlay; //입장 시 소모할 스테미너 기본 5

        public List<Enemy> list_enemy = new List<Enemy>(); //스폰할 적 캐릭터 목록(원본 프리팹)
        public List<Perk> list_perk = new List<Perk>(); //등장할 퍽 목록
        public List<InventoryItem> list_dropItem = new List<InventoryItem>(); //드랍할 아이템 목록

        public List<Room> list_room = new List<Room>(); //생성할 방 목록
        public List<BossRoom> list_bossRoom = new List<BossRoom>(); //보스방 목록(보스 포함 사용)
        public RestRoom restRoom; //휴식방

        public List<List<object>> list__roomEnemy = new List<List<object>>(); //방에서 등장할 적 캐릭터 목록

        public Status enemyStatBonus = new Status(); //스테이지별 적 스탯 가중치


        // public bool isHardMode; //하드모드 여부
        public int clearedRoom; //클리어한 스테이지
        public bool isClear; //클리어 여부


        public void SaveStageData()
        {
            ES3.Save(stageName, this);
        }

        public StageData LoadStageData()
        {
            if (ES3.KeyExists(stageName))
            {
                return ES3.Load<StageData>(stageName);
            }

            Debug.Log("Can't find saved stage data");
            return null;
        }

        public StageData SetStageData(int num)
        {
            currentStageJson = JsonUtility.FromJson<JsonStage>(
                   GameManager.Instance.addressMgr.dic_jsonStageData["JsonStage" + num].text);

            list__roomEnemy = GameManager.Instance.csvLoader.ReadCSVDatas2("CSV" + "Stage" + num);

            stageNum = currentStageJson.stageNum;
            stageName = currentStageJson.stageName;

            maxRoom = currentStageJson.maxRoom;
            steminaForPlay = currentStageJson.steminaForPlay;

            addressMgr = GameManager.Instance.addressMgr;

            list_enemy = LoadEnemyData();
            list_perk = LoadPerkData();
            list_dropItem = LoadDropItemData();

            clearedRoom = 0;

            isClear = false;

            return this;
        }

        List<Enemy> LoadEnemyData()
        {
            List<Enemy> list = new List<Enemy>();

            for (int i = 0; i < currentStageJson.list_enemy.Count; i++)
            {
                list.Add(addressMgr.dic_enemy[currentStageJson.list_enemy[i]]);
            }

            return list;
        }
        List<Perk> LoadPerkData()
        {
            List<Perk> returnList = new List<Perk>();

            for (int i = 0; i < currentStageJson.list_perk.Count; i++)
            {
                returnList.Add(addressMgr.dic_perk[currentStageJson.list_perk[i]]);
            }

            return returnList;
        }
        List<InventoryItem> LoadDropItemData()
        {
            List<InventoryItem> returnList = new List<InventoryItem>();

            for (int i = 0; i < currentStageJson.list_dropItem.Count; i++)
            {
                returnList.Add(addressMgr.dic_inventoryItem[currentStageJson.list_dropItem[i]]);
            }

            return returnList;
        }
    }
}