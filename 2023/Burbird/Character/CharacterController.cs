using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    /// <summary>
    /// 220714
    /// 플레이어와 적캐릭터에게 공통적으로 이동 관련 버프, 디버프를 적용 시키기 위해 생성
    /// Character.controller
    /// </summary>
    public class CharacterController : MonoBehaviour
    {
        [Header("Base Character Controller")]
        public float speedMultiplier = 1f; //이동, 공격속도, 애니메이션 속도에 영향
        public float jumpMultiplier = 1f; //점프 속도에 영향
        public bool isStun = false;

    }
}