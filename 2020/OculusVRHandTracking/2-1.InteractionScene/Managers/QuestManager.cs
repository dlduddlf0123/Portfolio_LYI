using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestStatus
{
    NONE,
    ACTIVE,
    CLEAR,
}

/// <summary>
/// 이벤트 종류, 완료여부, 이벤트의 시작과 종료 관리
/// </summary>
public class QuestManager : MonoBehaviour
{
    GameManager gameMgr;
    List<Quest> list_quest = new List<Quest>(); //전체 퀘스트
    List<Quest> list_clearQuest = new List<Quest>(); // 클리어한 퀘스트
    public Dictionary<string, Quest> dic_quest = new Dictionary<string, Quest>();

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        MakeQuest();
    }

    public void MakeQuest()
    {
        dic_quest.Add("Poop", new QuestPoop { questNum = 1, questStep = 0});

    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartQuest(int _questNum)
    {
        if (list_quest.Count < _questNum)
        {
            Debug.Log("Quest Not Exist");
            return;
        }
        list_quest[_questNum].StartQuest();
    }

    public void StartQuest(string _questName)
    {
        if (dic_quest.ContainsKey(_questName) &&
            dic_quest[_questName].statQuest == QuestStatus.NONE)
        {
            dic_quest[_questName].StartQuest();
        }
        else
        {
            Debug.Log(_questName + "Quest Not Exist");
        }
    }

    public void EndQuest(int _questNum)
    {
        if (list_quest.Count < _questNum)
        {
            Debug.Log("Quest Not Exist");
            return;
        }
        list_quest[_questNum].EndQuest();
    }

    public void EndQuest(string _questName)
    {
        if (dic_quest.ContainsKey(_questName))
        {
            dic_quest[_questName].EndQuest();
        }
        else
        {
            Debug.Log(_questName + "Quest Not Exist");
        }
    }

}
