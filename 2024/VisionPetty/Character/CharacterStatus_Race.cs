using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{


    public enum  RaceGestureType
    {
        NONE = 0,
    }


    /// <summary>
    /// 9/6/2024-LYI
    /// 훈련, 레이스 컨텐츠 관련 스탯
    /// 제스쳐 별 숙련도 등
    /// 숙련도 기반 성공률 계산 후 bool 콜백
    /// </summary>
    public class CharacterStatus_Race : MonoBehaviour
    {

        HandGestureType type;
        float[] gestureEXP;
        
        float speed;
        float stemina;
        float power;


        public virtual void Init()
        {


        }

        public void LoadStatus()
        {

        }


        public bool IsSuccess(HandGestureType type)
        {
            bool isSuccess = false;


            return isSuccess;
        }
    }
}