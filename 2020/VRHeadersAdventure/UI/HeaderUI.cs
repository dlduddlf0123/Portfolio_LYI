using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderUI : MonoBehaviour
{
    GameManager gameMgr;
    PlatformUI uiMgr;
    DialogManager dialogMgr;
    Character header;

    public Image dialogBg;
    public Text dialogText { get; set; }

    Image img_emote; //child(3)
    Animator emoteAnim;

    public List<List<object>> list__currentDialog;

    Coroutine dialogCoroutine = null;

    public float canvasSize = 0.001f;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        dialogMgr = gameMgr.dialogMgr;
        header = this.GetComponentInParent<Character>();

        dialogText = dialogBg.transform.GetChild(0).GetComponent<Text>();
        list__currentDialog = new List<List<object>>();
    }


    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - gameMgr.mainCam.transform.position);
        float dist = Vector3.Distance(transform.position, gameMgr.player.transform.position);

        if (dist < 5f)
        {
            transform.localScale = new Vector2(0.5f,0.5f) * canvasSize;
        }
        else
        {
            transform.localScale = new Vector2(canvasSize, canvasSize);
        }
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
    //언어에 따른 대사 설정
    public void SetDialogLanguage()
    {
        if (header == null || header.list___dialog_kor == null) { return; }
        switch (gameMgr.language)
        {
            case 0:
                list__currentDialog = header.list___dialog_kor[0];
                break;
            case 1:
                list__currentDialog = header.list___dialog_eng[0];
                break;
            default:
                list__currentDialog = header.list___dialog_eng[0];
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
        if (gameMgr.statGame != GameState.CLEAR)
        {
            return;
        }
        _index++;
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
        if (_str == null)
        {
            Debug.Log("올바른 Index를 입력할 것");
            yield break;
        }
        dialogBg.gameObject.SetActive(true);
        dialogText.text = _str;
        yield return new WaitForSeconds(3.0f);
        dialogBg.gameObject.SetActive(false);
    }

    //대사가 여러줄 
    public IEnumerator DialogTextOn(string[] _str)
    {
        for (int i = 0; i < _str.Length; i++)
        {
            dialogBg.gameObject.SetActive(true);
            dialogText.text = _str[i];
            yield return new WaitForSeconds(2.0f);
            dialogBg.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetLikeGauge(float _start, float _end, int _change)
    {

    }
}
