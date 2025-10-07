using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대가리들 대사, 머리 위 UI 관련 클래스
/// </summary>
public class HeaderCanvas : MonoBehaviour
{
    GameManager gameMgr;
    DialogManager dialogMgr;

    StatusUI statUI;
    Character header;
    //public Image img_likeGauge;

    public DialogBallon dialogBallon; //parent
    public Text dialogText;

    public GameObject circleUI;
    Animator circleUIAnim;
    GameObject[] arr_circleUI = new GameObject[2];
    Button circle_btn_travel;

    //public Image dialogBg;
    public List<List<object>> list__currentDialog = new List<List<object>>();

    public Color[] arr_likeColors = new Color[3];

    Coroutine dialogCoroutine = null;

    public float canvasSize = 0.001f;


    private void Awake()
    {
        circleUIAnim = circleUI.GetComponent<Animator>();

        arr_circleUI[0] = circleUI.transform.GetChild(0).gameObject;
        arr_circleUI[1] = circleUI.transform.GetChild(1).gameObject;

        circle_btn_travel = arr_circleUI[0].transform.GetChild(1).GetComponent<Button>();
    }

    private void Start()
    {
        gameMgr = GameManager.Instance;
        statUI = gameMgr.currentPlay.GetComponent<StageManager>().statUI;
        dialogMgr = gameMgr.dialogMgr;
        header = this.GetComponentInParent<Character>();

        // img_likeGauge.fillAmount = 0f;//(0%)
        // img_likeGauge.color = arr_likeColors[0];

        arr_likeColors[0] = new Color(1, 0.1f, 0.1f);
        arr_likeColors[1] = new Color(0.3f, 1f, 0.3f);
        arr_likeColors[2] = new Color(0.3f, 0.5f, 1f);

        circle_btn_travel.onClick.AddListener(() => { gameMgr.LoadScene(3); });
    }

    //private void OnEnable()
    //{
    //    if (gameMgr.statGame == GameState.PLAYING)
    //    {
    //        ShowCutSceneText(gameMgr.cutSceneMgr.list__cutSceneDialog, gameMgr.cutSceneMgr.currentSequence, gameMgr.cutSceneMgr.textNum);
    //        gameMgr.cutSceneMgr.textNum++;
    //    }
    //}

    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - GameManager.Instance.mainCam.transform.position);
        dialogText.rectTransform.rotation = Quaternion.LookRotation(dialogText.rectTransform.position - GameManager.Instance.mainCam.transform.position);
        circleUI.transform.rotation = Quaternion.LookRotation(circleUI.transform.position - GameManager.Instance.mainCam.transform.position);

        float dist = Vector3.Distance(transform.position, GameManager.Instance.mainCam.transform.position);

        if (dist < 1.5f)
        {
           // transform.localScale = new Vector2(dist, dist) * canvasSize;
            dialogBallon.transform.parent.localScale = new Vector3(0.5f,0.5f,0.5f);
            dialogBallon.transform.parent.localPosition = Vector3.up*1.2f;
        }
        else
        {
           // transform.localScale = new Vector2(canvasSize, canvasSize);
            dialogBallon.transform.parent.localScale = Vector3.one;
            dialogBallon.transform.parent.localPosition = Vector3.up*1.4f;
        }
    }

    public void SetLikeGauge(float _start, float _end, int _change)
    {
        if (statUI == null)
        {
            return;
        }
       // statUI.StartCoroutine(statUI.SetGauge(img_likeGauge, _start, _end, _change));
        statUI.StartCoroutine(statUI.SetGauge(statUI.arr_gaugeImgs[0], _start, _end, _change));
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
        if (list__currentDialog[_state][_index].ToString() == "")
        {
            _index = 1;
        }
        dialogCoroutine = StartCoroutine(DialogTextOn(list__currentDialog[_state][_index].ToString()));
    }
    public void ShowText(string _text)
    {
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }
        dialogCoroutine = StartCoroutine(DialogTextOn(_text));
    }

    //언어에 따른 대사 설정
    public void SetDialogLanguage()
    {
        if (header == null || header.list___dialog_kor == null) { return; }
        switch (gameMgr.language)
        {
            case 0:
                list__currentDialog = header.list___dialog_kor[2];
                break;
            case 1:
                list__currentDialog = header.list___dialog_eng[2];
                break;
            default:
                list__currentDialog = header.list___dialog_eng[2];
                break;
        }
    }

    //대사 한줄
    public IEnumerator DialogTextOn(string _str)
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
        yield return new WaitForSeconds(3.0f);
        dialogBallon.Off();
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

    #region CircleUI관련 함수
    bool isCircleUIOff = false;
    public void ToggleCircleUI()
    {
        if (isCircleUIOff) return;
        StopAllCoroutines();

        if (!circleUI.gameObject.activeSelf)
        {
            circleUI.gameObject.SetActive(true);

            circleUIAnim.SetBool("isActive", true);
            circleUIAnim.SetTrigger("isTrigger");
            StartCoroutine(Timer());
            Debug.Log("CircleUI On" + isCircleUIOff);
        }
        else
        {
            isCircleUIOff = true;
            StartCoroutine(CircleUIOff());
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(5f);
        isCircleUIOff = true;
        StartCoroutine(CircleUIOff());
    }


    public IEnumerator CircleUIOff()
    {
        Debug.Log("CircleUI Off" + isCircleUIOff);

        circleUIAnim.SetBool("isActive", false);
        circleUIAnim.SetTrigger("isTrigger");

        yield return new WaitForSeconds(0.5f);

        circleUI.gameObject.SetActive(false);
        isCircleUIOff = false;
        ToggleCircleMenu(0);

        Debug.Log("CircleUI Off end" + isCircleUIOff);
    }

    public void ToggleCircleMenu(int _uiNum)
    {
        StartCoroutine(ToggleCircleMenuAction(_uiNum));
    }

    /// <summary>
    /// 해당 UI 활성화, 그 외 비활성화
    /// </summary>
    IEnumerator ToggleCircleMenuAction(int _uiNum)
    {
        circleUIAnim.SetBool("isActive", false);
        circleUIAnim.SetTrigger("isTrigger");

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < arr_circleUI.Length; i++)
        {
            arr_circleUI[i].SetActive(false);
        }

        circleUIAnim.SetBool("isActive", true);
        circleUIAnim.SetTrigger("isTrigger");
        arr_circleUI[_uiNum].SetActive(true);
    }
    #endregion
}
