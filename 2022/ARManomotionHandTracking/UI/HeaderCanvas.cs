using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogoDigital.Lipsync;

/// <summary>
/// 대가리들 대사, 머리 위 UI 관련 클래스
/// </summary>
public class HeaderCanvas : MonoBehaviour
{
    GameManager gameMgr;
    DialogManager dialogMgr;

    public LipSync lipMaster;
    public LipSyncData currentLipData;

    public DialogBallon dialogBallon; //parent
    public Text dialogText;

    //public Image dialogBg;
    public List<List<object>> list__currentDialog = new List<List<object>>();

    Coroutine dialogCoroutine = null;

    public float canvasSize = 0.001f;


    private void Awake()
    {
        gameMgr = GameManager.Instance;
        dialogMgr = gameMgr.dialogMgr;
    }

    private void Start()
    {
        lipMaster = transform.GetComponentInParent<LipSync>();
    }

    private void OnEnable()
    {
        if (gameMgr.statGame == GameStatus.CUTSCENE)
        {
            if (gameMgr.currentEpisode.episodeNum == 0 &&
                gameMgr.currentEpisode.currentStageNum == 0)
            {
                gameMgr.currentEpisode.currentStage.NextDialog(this);
            }
            else
            {
                gameMgr.currentEpisode.currentStage.NextDialogLip(this);
            }

            //if (lipMaster != null &&
            //    currentLipData != null)
            //{
            //    lipMaster.Play(currentLipData);
            //}
        }
    }

    private void OnDisable()
    {
        if (gameMgr.uiMgr.ui_game.game_txt_dialog != null)
        {
            gameMgr.uiMgr.ui_game.game_txt_dialog.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - GameManager.Instance.mainCam.transform.position);
        dialogText.transform.rotation = Quaternion.LookRotation(dialogText.transform.position - gameMgr.arMainCamera.transform.position);

        //float dist = Vector3.Distance(transform.position, Camera.main.transform.position);

        //if (dist < 1.5f)
        //{
        //   // transform.localScale = new Vector2(dist, dist) * canvasSize;
        //    dialogBallon.transform.parent.localScale = new Vector3(0.5f,0.5f,0.5f);
        //    dialogBallon.transform.parent.localPosition = Vector3.up*1.2f;
        //}
        //else
        //{
        //   // transform.localScale = new Vector2(canvasSize, canvasSize);
        //    dialogBallon.transform.parent.localScale = Vector3.one;
        //    dialogBallon.transform.parent.localPosition = Vector3.up*1.4f;
        //}
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
            return;
        }
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }
        if (list__currentDialog[_state][_index].ToString() == "")
        {
            _index = 1;
        }
        dialogCoroutine = StartCoroutine(DialogTextOn(list__currentDialog[_state][_index].ToString()));
    }
    public void ShowText(string _text, float _time = 3.0f)
    {
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }
        dialogCoroutine = StartCoroutine(DialogTextOn(_text,_time));
    }

    //언어에 따른 대사 설정
    public void SetDialogLanguage()
    {
        //if (header == null || header.list___dialog_kor == null) { return; }
        //switch (gameMgr.language)
        //{
        //    case 0:
        //        list__currentDialog = header.list___dialog_kor[2];
        //        break;
        //    case 1:
        //        list__currentDialog = header.list___dialog_eng[2];
        //        break;
        //    default:
        //        list__currentDialog = header.list___dialog_eng[2];
        //        break;
        //}
    }

    /// <summary>
    /// 컷씬용 대사출력
    /// </summary>
    /// <param name="_state"></param>
    /// <param name="_index"></param>
    public void ShowCutsceneText(List<List<object>> __list, int _state, int _index)
    {
        _index++;

        if (_index >= __list[_state].Count)
        {
            Debug.Log("대사 Index 초과, 대화 종료");
            gameMgr.currentEpisode.currentStage.EndCutScene();
            return;
        }

        if (__list[_state] == null)
        {
            Debug.Log("올바른 State를 입력할 것");
        }

        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }

        dialogText = gameMgr.dialogMgr.SetText(__list[_state], dialogText, _index); //@ 띄어쓰기변환
        dialogCoroutine = StartCoroutine(DialogTextOn(dialogText.text));

    }


    //대사 한줄
    public IEnumerator DialogTextOn(string _str, float _time = 3.0f)
    {
        if (_str == null)
        {
            Debug.Log("올바른 Index를 입력할 것");
            yield break;
        }

        dialogBallon.StopAllCoroutines();
        dialogBallon.gameObject.SetActive(true);
        dialogText.gameObject.SetActive(true);
        dialogText.text = _str;

        Text _name = gameMgr.uiMgr.ui_game.game_txt_dialog.transform.GetChild(0).GetComponent<Text>();
        if (gameMgr.currentEpisode.currentStage.IsMultipleCharacterTalk())
        {
            _name.text = "Headers";
            _name.color = Color.white;
        }
        else
        {
            if (GetComponentInParent<Character>())
            {
                Character _header = GetComponentInParent<Character>();
                _name.text = _header.gameObject.name;
                _name.color = _header.characterColor;
            }
            else
            {
                _name.text =  transform.parent.gameObject.name;
                _name.color = transform.parent.GetComponent<Talker>().nameColor;
            }

        }

        gameMgr.uiMgr.ui_game.game_txt_dialog.text = dialogText.text;
        gameMgr.uiMgr.ui_game.game_txt_dialog.gameObject.SetActive(true);
        yield return new WaitForSeconds(_time);  // lipData.length

        if (gameMgr.statGame == GameStatus.INTERACTION)
        {
            gameMgr.uiMgr.ui_game.game_txt_dialog.gameObject.SetActive(false);
            dialogBallon.Off();
        }

    }

    //대사가 여러줄 
    public IEnumerator DialogTextOn(string[] _str)
    {
        for (int i = 0; i < _str.Length; i++)
        {
            dialogBallon.gameObject.SetActive(true);
            dialogText.gameObject.SetActive(true);
            dialogText.text = _str[i];
            yield return new WaitForSeconds(_str[i].Length * 0.3f);
            dialogBallon.Off();
            dialogText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

}
