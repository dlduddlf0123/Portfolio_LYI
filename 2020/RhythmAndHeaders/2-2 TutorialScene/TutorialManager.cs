using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEngine.UI;
public enum TutorialPhase
{
    Sync =0,
    Note,
    Fever,
    Test,
}
public class TutorialManager : MonoBehaviour
{
    PlayManager ingameMgr;
    GameManager gameMgr;
    public TutorialPhase currentPhase;

    public GameObject notePool;
    public List<GameObject> musicNoteList = new List<GameObject>();
    public List<float> syncpointList = new List<float>();

    public Image guideArrow;
    public Transform[] guideArrowPos;
    public int guideArrowOrderNum = 0;
    [Header("- Sync Phase")]
    public GameObject ui_syncPhase; 
    public float syncSongBpm = 60;
    public float tutorialSongBpm = 170;
    [Header("-Dialogue")]
    public NPCConversation syncDialogue;
    public NPCConversation noteDialogue;
    public NPCConversation feverDialogue;
    public NPCConversation testDialogue;

    public Button bt_start; //싱크 노트 나오는 버튼(Sync 페이즈)
    public Button bt_next; //다음 튜토리얼 페이즈 넘어가는 버튼
    public int feverLife = 3;


    void Awake()
    {
        ingameMgr = PlayManager.Instance;
        gameMgr = GameManager.Instance;
        //ConversationManager.Instance.StartConversation(dialogue1);
        currentPhase = TutorialPhase.Sync;
        TutorialInit();
    }
    private void Start()
    {
        HideNote();
    }
    public void TutorialInit() // 튜토리얼 시작 시 초기화 
    {
        StartCoroutine(TutorialDialogue(syncDialogue));
        ingameMgr.mAudio.enabled = false;
    }

    IEnumerator TutorialDialogue(NPCConversation dialogue)
    {
        yield return new WaitForSeconds(2);
        ConversationManager.Instance.StartConversation(dialogue);
    }
    public void SyncNoteStart()
    {
        ingameMgr.platformDistance = syncSongBpm / 60;
        ingameMgr.bpm = syncSongBpm;
        this.gameObject.GetComponent<SyncSetting>().isTutorial = true;
        for (int i=0;i<4;i++)
        {
            if (i % 2 != 0)
            {
                GameObject go;
                go = Instantiate(ingameMgr.platform[1], new Vector3(i * 32 - 8, ingameMgr.guidePoint[0].position.y, ingameMgr.guidePoint[0].position.z), Quaternion.identity, ingameMgr.platformTr.GetChild(0));
                go.GetComponent<Note>().checkPosition = ingameMgr.guidePoint[0];
                go.GetComponent<Note>().lineNum = 0;
            }
            else
            {
                GameObject go;
                go = Instantiate(ingameMgr.platform[1], new Vector3(i * 32 - 8, ingameMgr.guidePoint[1].position.y, ingameMgr.guidePoint[1].position.z), Quaternion.identity, ingameMgr.platformTr.GetChild(0));
                go.GetComponent<Note>().checkPosition = ingameMgr.guidePoint[1];
                go.GetComponent<Note>().lineNum = 1;
            }
        }       
    }
    /// <summary>
    /// sync 부분은 prefab 뒷부분은 csv읽어와서 노트생성, csv부분 노트 숨기기
    /// </summary>
    public void HideNote()
    {
        Transform temp = ingameMgr.platformTr.gameObject.transform.GetChild(0).gameObject.transform;

        Debug.Log(ingameMgr.platformTr.gameObject.transform.GetChild(0).gameObject.transform.childCount);
        foreach (Transform child in temp)
        {
            musicNoteList.Add(child.gameObject);
        }
        foreach (GameObject note in musicNoteList)
        {
            note.transform.SetParent(notePool.transform);
        }
    }
    public void ShowNote()
    {
        ingameMgr.platformDistance = ingameMgr.bpm / 60;
        Transform temp = ingameMgr.platformTr.gameObject.transform.GetChild(0).gameObject.transform;
        foreach (GameObject note in musicNoteList)
        {
            note.transform.SetParent(temp.transform);
            note.transform.localPosition = note.transform.localPosition + Vector3.right * gameMgr.userSync * 0.02f;
        }
    }

    public void NoteInputPhase()
    {
        ingameMgr.bpm = tutorialSongBpm;
        bt_start.gameObject.SetActive(false);
        currentPhase = TutorialPhase.Note;
        GetComponent<SyncSetting>().enabled = false;
        ingameMgr.feverPoint = 0;
        ingameMgr.mAudio.enabled = true;
        ingameMgr.mAudio.Play();
        ui_syncPhase.transform.parent.gameObject.SetActive(false);
        ConversationManager.Instance.StartConversation(noteDialogue);     
    }

    public void ShowGuidePoint(float delaytime)
    {
        StartCoroutine(GuidePoint(delaytime));
    }
    IEnumerator GuidePoint(float delaytime)
    {
        guideArrow.transform.localPosition = guideArrowPos[guideArrowOrderNum].localPosition;
        guideArrow.transform.localRotation = guideArrowPos[guideArrowOrderNum].localRotation;
        guideArrow.gameObject.SetActive(true);
        yield return new WaitForSeconds(delaytime);
        guideArrow.gameObject.SetActive(false);
        guideArrowOrderNum++;
    }

    public void FeverInputPhase()
    {
        currentPhase = TutorialPhase.Fever;
        ingameMgr.mAudio.Play();
        ConversationManager.Instance.StartConversation(feverDialogue);
    }
    public void TestPhase()
    {
        currentPhase = TutorialPhase.Test;
        StartCoroutine(TutorialDialogue(testDialogue));
        foreach(GameObject note in musicNoteList)
        {
            note.transform.SetParent(notePool.transform);
            note.SetActive(false);
        }
        ingameMgr.score = 0;
        ingameMgr.uiMgr.txt_score.gameObject.SetActive(true);
    }
    public void ShowTestNote()
    {
        Transform temp = ingameMgr.platformTr.gameObject.transform.GetChild(0).gameObject.transform;
        foreach (GameObject note in musicNoteList)
        {
            ingameMgr.platformTr.position = new Vector3(-31.9f, 0, 0);
            note.transform.SetParent(temp.transform);
            note.SetActive(true);
        }
    }
    public void SettingSync()
    {
        float sum = 0;
        foreach(float syncpoint in syncpointList)
        {
            sum += syncpoint;
        }
        gameMgr.userSync = sum / syncpointList.Count;
    }
    public void NextButton()
    {
        switch(currentPhase)
        {
            case TutorialPhase.Sync:
                bt_next.onClick.AddListener(() => { NoteInputPhase(); });
                break;
            case TutorialPhase.Note:
                bt_next.onClick.RemoveAllListeners();
                bt_next.onClick.AddListener(() => { FeverInputPhase(); });
                break;
            case TutorialPhase.Fever:
                bt_next.onClick.RemoveAllListeners();
                bt_next.onClick.AddListener(() => { ingameMgr.FeverOn();ShowNote(); });
                break;
            case TutorialPhase.Test:
                bt_next.onClick.RemoveAllListeners();
                bt_next.onClick.AddListener(() => {; });
                break;

        }
    }
}
