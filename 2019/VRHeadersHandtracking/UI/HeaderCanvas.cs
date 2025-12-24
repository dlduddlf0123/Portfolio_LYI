using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderCanvas : MonoBehaviour
{
    GameManager gameMgr;
    UIManager uiMgr;
    DialogManager dialogMgr;
    Character header;
    public Image img_likeGauge;

    public Image dialogBg;
    public Text dialogText { get; set; }

    public List<List<object>> list__CurrentDialog;

    public Color[] arr_likeColors = new Color[3];


    private void Awake()
    {
        gameMgr = GameManager.Instance;
        uiMgr = gameMgr.uiMgr;
        dialogMgr = gameMgr.dialogMgr;
        header = this.GetComponentInParent<Character>();
        
        dialogText = dialogBg.transform.GetChild(0).GetComponent<Text>();
        list__CurrentDialog = new List<List<object>>();

        arr_likeColors[0] = new Color(1, 0.1f, 0.1f);
        arr_likeColors[1] = new Color(0.3f, 1f, 0.3f);
        arr_likeColors[2] = new Color(0.3f, 0.5f, 1f);
    }

    private void Start()
    {
        img_likeGauge.fillAmount = 0f;//(0%)
        img_likeGauge.color = arr_likeColors[0];
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - GameManager.Instance.mainCam.transform.position);
        float dist = Vector3.Distance(transform.position, GameManager.Instance.mainCam.transform.position);

        if (dist < 1.5f)
        {
            transform.localScale = new Vector3(dist, dist, dist) * 0.001f;
        }
        else
        {
            transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }
    }

    public void SetLikeGauge(float _start, float _end, int _change)
    {
        if (gameMgr.currentCoroutine != null)
        {
            StopCoroutine(gameMgr.currentCoroutine);
        }
        gameMgr.currentCoroutine = StartCoroutine(uiMgr.SetGauge(img_likeGauge, _start, _end, _change));
    }

    /// <summary>
    /// 대사 출력, 대사창 활성화
    /// </summary>
    /// <param name="_state">0:이동/1:만짐/2:음식/3:친밀해짐/4:서먹해짐</param>
    /// <param name="_index">몇번째 대사</param>
    public void ShowText(int _state, int _index)
    {
        _index++;
        if (list__CurrentDialog[_state] == null)
        {
            Debug.Log("올바른 State를 입력할 것");
        }
        StartCoroutine(DialogTextOn(list__CurrentDialog[_state][_index].ToString()));
    }

    //대사 한줄
    public IEnumerator DialogTextOn(string _str)
    {
        if (_str == null)
        {
            Debug.Log("올바른 Index를 입력할 것");
            yield return null;
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
}
