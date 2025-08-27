using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabyrinth.ReadOnlys;

public class NPC_BigDog : NPC
{
    public int Char_Index;

    public ErekiBall erekiBall;

    protected override void Init()
    {
        UpdateMat(); // Emission 초기화

        Status.Type = GameMgr.PlayData.GameData.lCharData[Char_Index].Type;

        if (GameMgr.isEvent)
        {
            Status.MaxHP = Status.HP = 360000;

            Status.AttackPoint = 60000;

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
    protected override void ChildAwake()
    {
        magicPos = transform.GetChild(2).GetChild(0);
        //erekiBall = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<ErekiBall>();
        erekiBall.transform.SetParent(transform.root.parent);
    }

    protected override bool FindTarget()
    {
        //Debug.Log("Call");
        RaycastHit rangeRay;
        Vector3 rayDirection;
        // rayDirectioni = 1001과 플레이어사이의 거리
        rayDirection = GameMgr.Player.transform.position - transform.position;
        rayDirection.y += 1.0f;

        //Debug.DrawRay(transform.position, rayDirection * Status.AttackRange, Color.red);

        int layerMask = (1 << 9) | (1 << 11);
        layerMask = ~layerMask;
        // 1001의 위치부터 플레이어의 방향으로 AttackRange만큼 Ray를 쏨
        if (Physics.Raycast(transform.position, rayDirection, out rangeRay, Status.AttackRange, layerMask))
        {
            // Raycasthit이 tag.Player면
            if (rangeRay.collider.CompareTag(Defines.TAG_PLAYER))
                return true;
            else
                return false;
        }

        return false;
    }

    protected void EnemyRangeAttack()
    {
        //Debug.Log("EnemyRangeAttack");
        // firePos를 초기화

        if (GameMgr.Player.Status.HP > 0)
            erekiBall.Use(magicPos.position, GameMgr.Player.transform.position, Status.AttackPoint);

        if (GameMgr.Player.Status.HP - Status.AttackPoint <= 0)
            PlayerDie();
    }
}
