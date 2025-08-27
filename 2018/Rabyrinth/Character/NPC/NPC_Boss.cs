using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabyrinth.ReadOnlys;

public class NPC_Boss : NPC
{
    public int Char_Index;

    public FireBallCtrl fireArrow;

    protected override void Init()
    {
        UpdateMat(); // Emission 초기화

        Status.Type = GameMgr.PlayData.GameData.lCharData[Char_Index].Type;

        if (GameMgr.isEvent)
        {
            Status.MaxHP = Status.HP = 2400000;

            Status.AttackPoint = 120000;

            Status.Defense = 72000;
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

        // 코루틴을 시작
        StartCoroutine(SetPlayer());
    }

    protected override void ChildAwake()
    {
        isBoss = true;
        magicPos = transform.GetChild(2).GetChild(0);
        //fireArrow = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<FireBallCtrl>();
        fireArrow.transform.SetParent(transform.root.parent);
    }

    protected override bool FindTarget()
    {
        //Debug.Log("Call");
        RaycastHit rangeRay;
        Vector3 rayDirection;
        // rayDirectioni = 1001과 플레이어사이의 거리
        rayDirection = GameMgr.Player.transform.position - transform.position;
        rayDirection.y += 1.0f;

        int layerMask = (1 << 9) | (1 << 11);
        layerMask = ~layerMask;

        //Debug.DrawRay(transform.position, rayDirection * Status.AttackRange, Color.red);

        // 1001의 위치부터 플레이어의 방향으로 AttackRange만큼 Ray를 쏨
        if (Physics.Raycast(transform.position, rayDirection, out rangeRay, Status.AttackRange, layerMask))
        {
            // Raycasthit이 tag.Player면
            if (rangeRay.collider.CompareTag(Defines.TAG_PLAYER)|| rangeRay.collider.CompareTag("NPC"))
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
            fireArrow.Use(magicPos.position, GameMgr.Player.transform.position, Status.AttackPoint);

        if (GameMgr.Player.Status.HP - Status.AttackPoint <= 0)
            PlayerDie();
    }
}