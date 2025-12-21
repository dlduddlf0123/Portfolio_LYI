using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 5마리 대가리들 준비자세, 클릭 시 방귀 발사, 색 일정 부분 빠짐, 방귀 사운드는 캐릭터 마다 다름
/// 색이 원래대로 돌아오면 바람빠지는 소리가 남
/// 
/// /// </summary>
public class FartInteraction : InteractionManager
{
    HeaderFart[] arr_fart;

    protected override void DoAwake()
    {
        arr_fart = GetComponentsInChildren<HeaderFart>();
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
    }

    public override void EndInteraction()
    {
        base.EndInteraction();
    }
}
