using UnityEngine;
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Rabyrinth.ReadOnlys;

public class DataManager : MonoBehaviour
{
    public PlayerData PlayerData { get; private set; }
    public GameData GameData { get;  set; }
    public List<RankingData> lRankingData { get; set; }

    public string SecurityID { get; set; }

    private GameManager GameMgr;

    private void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;
    }

    public void SetPlayerData(PlayerData _plyerData)
    {
        if (_plyerData == null)
            return;

        if (_plyerData.Skill == null)
        {
            List<Skill> lSkill = new List<Skill>();

            for (int index = 0; index < Defines.SKILL_MAX_INDEX; index++)
                lSkill.Add(new Skill { level = 1 });

            _plyerData.Skill = lSkill;
        }

        PlayerData = _plyerData;
    }

    public void SavePlayerData()
    {
        if (PlayerData == null)
            return;

        GameMgr.AWS_Mgr.SavePlayerData(PlayerData);
    }
}

[DynamoDBTable("RabyrinthPlayerData")]
public class PlayerData
{
    [DynamoDBHashKey]   // Hash key.
    public string ID { get; set; }

    [DynamoDBProperty]
    public string SecurityID { get; set; }

    [DynamoDBProperty]
    public int MaxFloor { get; set; }

    [DynamoDBProperty]
    public int CurrentFloor { get; set; }

    [DynamoDBProperty]
    public int KPM { get; set; }

    [DynamoDBProperty]
    public int Gold { get; set; }

    [DynamoDBProperty]
    public int Gem { get; set; }

    [DynamoDBProperty]
    public int StatusPoint { get; set; }

    [DynamoDBProperty("Skill")]    // Multi-valued (set type) attribute. 
    public List<Skill> Skill { get; set; }

    //하단 코드 무시
    //public static explicit operator PlayerDataAuth(PlayerData _data)
    //{
    //    PlayerDataAuth output = new PlayerDataAuth()
    //    {
    //        ID = _data.ID,
    //        Name = _data.Name,
    //        Level = _data.Level
    //    };
    //    return output;
    //}

}

[DynamoDBTable("RabyrinthAuthData")]
public class AuthData
{
    [DynamoDBHashKey]   // Hash key.
    public string Token { get; set; }
    [DynamoDBProperty]
    public string ID { get; set; }

}


[DynamoDBTable("RabyrinthRankingData")]
public class RankingData
{
    [DynamoDBHashKey]   // Hash key.
    public string Name { get; set; }

    [DynamoDBProperty]
    public float Time { get; set; }

    [DynamoDBProperty]
    public int Score { get; set; }

}

[DynamoDBTable("RabyrinthGameData")]
public class GameData
{
    [DynamoDBHashKey]   // Hash key.
    public int Index { get; set; }

    [DynamoDBProperty]
    public int Score { get; set; }

    [DynamoDBProperty]
    public float Version { get; set; }

    [DynamoDBProperty("Skill")]    // Multi-valued (set type) attribute. 
    public List<Skill> lSkillData { get; set; }

    [DynamoDBProperty("Character")]    // Multi-valued (set type) attribute. 
    public List<CharacterData> lCharData { get; set; }
}

public class CharacterData
{
    public string Name;
    public int Type;
    public int HP;
    public int Attack;
    public int Defense;
    public float MoveSpeed;
    public float AttackSpeed;
    public float AttackRange;
    public float Critical;
    public float Critical_Damage;
    public float FindRange;
}

public class Item
{
    string Name;
    int Index;
    int Type;
}