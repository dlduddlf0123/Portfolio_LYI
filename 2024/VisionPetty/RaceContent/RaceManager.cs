using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{
    /// <summary>
    /// 8/22/2024-LYI
    /// Race 게임 플레이 시 호출
    /// 레이스 게임 진행 관련 코드들
    /// 캐릭터 상태 참조
    /// 플레이어 데이터 참조
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        GameManager gameMgr;

        private void Awake()
        {
            RaceInit();
        }

        public void RaceInit()
        {
            gameMgr = GameManager.Instance;
        }

    }
}