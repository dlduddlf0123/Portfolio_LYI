using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPoop : Quest
{
    StageManager stageMgr;

    private void Awake()
    {
        QuestInit();
        stageMgr = GameManager.Instance.currentPlay.GetComponent<StageManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void QuestInit()
    {
        questNum = 1;
        questStep = 0;
        lastStep = 3;
    }

    //퀘스트 시작 동작
    IEnumerator Pooping()
    {
        Debug.Log("Poop Start");
        header.PlayTriggerAnimation(8);
        header.headerCanvas.ShowText("받아, 이거 네 선물이야");   //똥 싼 뒤 대사
        yield return new WaitForSeconds(4.0f);  //똥싸는애니메이션 재생 시간 or 애니메이션에서 콜백받기
        questStep = 1;
        header.isEvent = false;
        yield return new WaitForSeconds(1.0f);
        CheckQuestStep();
    }

    /// <summary>
    /// 조건 체크 이후 퀘스트 시작(외부 호출)
    /// </summary>
    public override void StartQuest()
    {
        Debug.Log("Quest Start");
        if (statQuest != QuestStatus.NONE) { return; }
        statQuest = QuestStatus.ACTIVE;

        QuestInit();
        header = stageMgr.interactHeader;
        header.Stop();
        header.isEvent = true;
        header.currentAI = header.StartCoroutine(Pooping());
    }

    public override void CheckQuestStep()
    {
        Debug.Log("CheckQuestStep" + questStep);
        if (questStep >= lastStep)
        {
            EndQuest();
            return;
        }
        switch (questStep)
        {
            case 1: //똥 싸고감
                header.AI_Move(0);
                break;
            case 2: //똥 치운 뒤
                //손이 지저분한 상태
                //만지거나 음식을 줄 시 더럽다고 도망가며 questStep = 3 호출 / 엔딩
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 퀘스트 종료 보상 수령
    /// </summary>
    public override void EndQuest()
    {
        if (statQuest != QuestStatus.ACTIVE) { return; }
        statQuest = QuestStatus.CLEAR;

    }
}
