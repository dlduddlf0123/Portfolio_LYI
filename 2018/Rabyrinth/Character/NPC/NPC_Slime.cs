using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabyrinth.ReadOnlys;

public class NPC_Slime : NPC
{
    public int Char_Index;

    protected override void Init()
    {
        UpdateMat(); // Emission 초기화

        Status.Type = GameMgr.PlayData.GameData.lCharData[Char_Index].Type;

        if (GameMgr.isEvent)
        {
            Status.MaxHP = Status.HP = 720000;

            Status.AttackPoint = 36000;

            Status.Defense = 18000;

        }
        else
        {
            Status.MaxHP = Status.HP =
                GameMgr.PlayData.GameData.lCharData[Char_Index].HP * GameMgr.PlayData.PlayerData.CurrentFloor;
            Status.AttackPoint =
                GameMgr.PlayData.GameData.lCharData[Char_Index].Attack * GameMgr.PlayData.PlayerData.CurrentFloor;
            Status.Defense =
                GameMgr.PlayData.GameData.lCharData[Char_Index].Defense * GameMgr.PlayData.PlayerData.CurrentFloor;
        }

        Status.MoveSpeed =
            GameMgr.PlayData.GameData.lCharData[Char_Index].MoveSpeed;
        Status.AttackSpeed =
            GameMgr.PlayData.GameData.lCharData[Char_Index].AttackSpeed;
        Status.AttackRange =
            GameMgr.PlayData.GameData.lCharData[Char_Index].AttackRange;
        Status.CriticalChance =
            GameMgr.PlayData.GameData.lCharData[Char_Index].Critical;
        Status.CriticalBonus =
            GameMgr.PlayData.GameData.lCharData[Char_Index].Critical_Damage;
        Status.FindRange =
            GameMgr.PlayData.GameData.lCharData[Char_Index].FindRange;

        InitAttakSpeed = Status.AttackSpeed;
        InitMoveSpeed = Status.MoveSpeed;

        EnemyState = CharacterState.idle;

        StartCoroutine(SetPlayer());
    }
    // 플레이어의 HP가 0 이상이면 PlyaerCharacter의 TakeDamage호출, 0이하라면 코루틴을 종료
    protected override void Attack()
    {
        //Debug.Log("Attack");
        if (GameMgr.Player.Status.HP > 0)
            GameMgr.Player.TakeDamage(Status.AttackPoint, HitEffect.Default);

        if (GameMgr.Player.Status.HP - Status.AttackPoint <= 0)
            PlayerDie();
    }
}
