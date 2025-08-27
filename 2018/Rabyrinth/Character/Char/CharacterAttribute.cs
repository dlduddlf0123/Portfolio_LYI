using UnityEngine;
using System.Collections;

public class CharacterAttribute
{
    // 오브젝트의 타입를 구분하기 위한 변수(근거리, 원거리 등)
    public int Type { get; set; }
    // 이름
    public string Name { get; set; }
    // 이동속도
    public float MoveSpeed { get; set; }
    // 공격속도
    public float AttackSpeed { get; set; }
    // 공격범위
    public float AttackRange { get; set; }
    // 타겟 검색범위
    public float FindRange { get; set; }
    // 공격력
    public int AttackPoint { get; set; }
    // 방어력
    public int Defense { get; set; }

    // 치명타 확률
    public float CriticalChance { get; set; }
    // 치명타 배수
    public float CriticalBonus { get; set; }
    // HP
    public int MaxHP { get; set; }
    // HP
    public int HP { get; set; }
}

public class PlayerAttribute : CharacterAttribute
{
    public static PlayerAttribute Create( /*Types characterType*/ )
    {
        var status = new PlayerAttribute();
        return status;
    }

    public PlayerAttribute()
    {
    }
}

public class EnemyAttribute : CharacterAttribute
{
    public float TraceDist { get; set; }
    public float AttackDist { get; set; }

    public static EnemyAttribute Create()
    {
        var status = new EnemyAttribute();
        return status;
    }

    public EnemyAttribute()
    {
    }
}