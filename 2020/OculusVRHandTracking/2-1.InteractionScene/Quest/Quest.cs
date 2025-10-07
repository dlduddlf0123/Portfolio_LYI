using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestStatus statQuest = QuestStatus.NONE;

    public Character header; //이 퀘스트에 해당하는 캐릭터(들)

    public int questNum;    //이 퀘스트 내용에 할당된 퀘스트 번호
    public int questStep = 0;   //현재 진행중인 퀘스트 단계
    public int lastStep;    //퀘스트가 종료되는 단계

    protected virtual void QuestInit() { }
    public virtual void CheckQuestStep() { }    //현재 questStep에 맞는 함수 호출 //questStep이 변경될 때 호출할 것
    
    /// <summary>
    /// 조건 체크 이후 퀘스트 시작
    /// </summary>
    public virtual void StartQuest()
    {

    }

    /// <summary>
    /// 퀘스트 종료 보상 수령
    /// </summary>
    public virtual void EndQuest()
    {

    }
}
