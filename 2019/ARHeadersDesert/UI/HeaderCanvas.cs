using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderCanvas : MonoBehaviour
{
    GameManager gameMgr;
    UIManager uiMgr;
    DialogManager dialogMgr;

    Character header;   //이 캔버스를 소지한 캐릭터

    public Image hpbar; //child(1)

    Image dialogBg; //child(2)
    Text dialogText;

    Image img_emote; //child(3)
    Animator emoteAnim;

    Camera mainCamera;
    RectTransform rtr;

    public List<List<object>> list__currentDialog;  //현재 재생할 대사 리스트
    Coroutine dialogCoroutine = null;   //현재 진행중인 대사 코루틴(대사 중 다른대사 등장시 코루틴 갱신 버그 방지)

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        uiMgr = gameMgr.uiMgr;
        dialogMgr = gameMgr.dialogMgr;

        header = transform.parent.GetComponent<Character>();
        hpbar = transform.GetChild(1).GetComponent<Image>();
        
        dialogBg = transform.GetChild(2).GetComponent<Image>();
        dialogText = dialogBg.transform.GetChild(0).GetComponent<Text>();

        img_emote = transform.GetChild(3).GetComponent<Image>();
        emoteAnim = img_emote.GetComponent<Animator>();

        mainCamera = gameMgr.missileMgr.mainCam;
        rtr = GetComponent<RectTransform>();

        SetDialogLanguage();
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (gameMgr.statGame == GameState.DIALOG)
        {
            ShowCutSceneText(gameMgr.dialogNum - 1, gameMgr.cutSceneMgr.cutSceneCount);
            gameMgr.cutSceneMgr.cutSceneCount++;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Init();
    }

    public void Init()
    {
        hpbar.fillAmount = header.Status.maxHp;

        if (gameMgr.statGame != GameState.DIALOG)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    //언어에 따른 대사 설정
    public void SetDialogLanguage()
    {
        if (header == null || header.list__dialog_kor == null) { return; }
        switch (gameMgr.language)
        {
            case 0:
                list__currentDialog = header.list__dialog_kor;
                break;
            case 1:
                list__currentDialog = header.list__dialog_eng;
                break;
            default:
                list__currentDialog = header.list__dialog_eng;
                break;
        }
    }

    public void SetHP(float _maxHp, float _hp)
    {
        hpbar.fillAmount = _hp/_maxHp ;
    }

    private void Update()
    {
        rtr.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }

    public void ShowEmote(int _emoteNum)
    {
        switch (_emoteNum)
        {
            case 0: //놀람
                emoteAnim.SetTrigger("tFrighten");
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// 대사 출력, 대사창 활성화
    /// </summary>
    /// <param name="_state">0:이동/1:만짐/2:음식/3:친밀해짐/4:서먹해짐</param>
    /// <param name="_index">몇번째 대사</param>
    public void ShowText(int _state, int _index)
    {
        _index++;
        if (list__currentDialog[_state] == null)
        {
            Debug.Log("올바른 State를 입력할 것");
        }
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }
        //dialogCoroutine = StartCoroutine(DialogTextOn(list__currentDialog[_state][_index].ToString()));
    }

    /// <summary>
    /// 컷씬용 대사출력
    /// </summary>
    /// <param name="_state"></param>
    /// <param name="_index"></param>
    public void ShowCutSceneText(int _state, int _index)
    {
        _index++;
        list__currentDialog = GameManager.Instance.dialogMgr.ReadDialogDatas("KantoDialog_kor");

        if (_index > list__currentDialog[_state].Count)
        {
            gameMgr.cutSceneMgr.EndCutScene();
            return;
        }

        if (list__currentDialog[_state] == null)
        {
            Debug.Log("올바른 State를 입력할 것");
        }
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }
        dialogCoroutine = StartCoroutine(DialogTextOn(list__currentDialog[_state][_index].ToString()));
    }

    //대사 한줄
    public IEnumerator DialogTextOn(string _str)
    {
        dialogBg.gameObject.SetActive(true);
        dialogText.text = _str;
        yield return new WaitForSeconds(2.0f);
        dialogBg.gameObject.SetActive(false);
    }

    //대사가 여러줄 
    public IEnumerator DialogTextOn(string[] _str)
    {
        for (int i = 0; i < _str.Length; i++)
        {
            dialogBg.gameObject.SetActive(true);
            dialogText.text = _str[i];
            yield return new WaitForSeconds(1.0f);
            dialogBg.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
