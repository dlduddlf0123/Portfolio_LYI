using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 똥, 처리해야한다
/// 배변봉투로 수거하기
/// 손에 봉투를 달아서 똥에 닿으면 묶인 모양으로 변경
/// 
/// 물로 씻어내기?
/// </summary>
public class Poop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHand hand = other.GetComponent<PlayerHand>();
            if (!hand.isDirty)
            {
                hand.isDirty = true;
                hand.ChangeHandColor(hand.handColor[3], hand.handColor[2]);
                GameManager.Instance.HandEffect(hand);
            }
        }
    }

    //private void OnDestroy()
    //{
    //    Debug.Log("똥 제거됨");
    //    Quest poop = GameManager.Instance.questMgr.dic_quest["Poop"];
    //    poop.questStep = 2;
    //    poop.CheckQuestStep();
    //}
}
