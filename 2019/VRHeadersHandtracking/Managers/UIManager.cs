using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모든 캔버스들, UI 관련 동작함수들을 가지고 있다
/// </summary>
public class UIManager : MonoBehaviour
{
    GameManager gameMgr;
    GameObject mainCam;

    public HeaderCanvas headerUI;


    public Canvas mainCanvas;
    public RectTransform cmdUI { get; set; }
    Image[] arr_cmd { get; set; }

    //Image char_img_judge;
    //Sprite good;
    //Sprite perfect;
    //Sprite miss;

    //public RectTransform raceUI { get; set; }
    //Text race_time;
    //public Text race_count { get; set; }


    //public RectTransform startUI;
    //public Text start_count { get; set; }

    //public RectTransform goalUI;
    //public Text txt_goal { get; set; }
    //public Text goal_time { get; set; }

    void Awake()
    {
        gameMgr = GameManager.Instance;
        mainCam = gameMgr.mainCam.gameObject;

        cmdUI = mainCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        arr_cmd = new Image[4];
        for (int idx = 0; idx < arr_cmd.Length; idx++)
        {
            arr_cmd[idx] = cmdUI.GetChild(idx).GetComponent<Image>();
        }

        cmdUI.gameObject.SetActive(false);
    }

    private void Start()
    {
    }

    public void SetImageSprite(Image _i, Sprite _s)
    {
        _i.sprite = _s;
    }

    /// <summary>
    /// 캐릭터의 상태에 맞춰 커맨드 변경
    /// </summary>
    /// <param name="_state"></param>
    public void SetCommandUI(int _state)
    {
        switch (_state)
        {
            case 0:
                cmdUI.gameObject.SetActive(false);
                break;
            case 1:
                cmdUI.gameObject.SetActive(true);
                break;
            case 2:
                cmdUI.gameObject.SetActive(true);
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// 일정시간 UI 보여주기
    /// </summary>
    /// <param name="_img"></param>
    /// <returns></returns>
    public IEnumerator ActiveImage(Image _img)
    {
        _img.gameObject.SetActive(true);

        float _t = 0.0f;
        while (_t < 0.5f)
        {
            _t += Time.deltaTime;
            _img.transform.rotation = Quaternion.LookRotation(_img.transform.position - mainCam.transform.position);
            //_img.transform.Translate(Vector3.up * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        //_img.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        _img.gameObject.SetActive(false);
    }

    /// <summary>
    /// 깜빡거리게 하기
    /// </summary>
    /// <param name="_go">깜빡일 물체</param>
    /// <param name="_isActive">깜빡인 후 활성화 여부</param>
    /// <returns></returns>
    public IEnumerator Fleek(GameObject _go, bool _isActive)
    {
        for (int i = 0; i < 5; i++)
        {
            _go.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            _go.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);
        _go.gameObject.SetActive(_isActive);
    }

    /// <summary>
    /// 게이지 증감함수
    /// </summary>
    /// <param name="_img">게이지 이미지</param>
    /// <param name="_gauge">증감 값(0.00f~1.00f)</param>
    public IEnumerator SetGauge(Image _img, float _start, float _end, int _change)
    {
        //_img.fillAmount += _gauge;
        _img.transform.parent.gameObject.SetActive(true);

        float t = 0.0f;
        float spd = 2f;
        if (_change != 0)
        {
            if (_change == 1)//상승
            {
                while (t < 1f)
                {
                    t += Time.deltaTime * spd;
                    _img.fillAmount = Mathf.Lerp(_start, 1, t);
                    yield return new WaitForSeconds(0.02f);
                }

                _img.fillAmount = 1;
                gameMgr.PlayEffect(_img.transform.GetChild(0).position, gameMgr.particles[1]);
                gameMgr.soundMgr.PlaySfx(_img.transform.GetChild(0).position, gameMgr.soundMgr.LoadClip("Sounds/SFX/14short3NL"));

                yield return new WaitForSeconds(1f);

                t = 0f;
                _img.fillAmount = 0f;
                headerUI.img_likeGauge.color = headerUI.arr_likeColors[(int)gameMgr.selectHeader.statLike];

                while (t < 1f)
                {
                    t += Time.deltaTime* spd;
                    _img.fillAmount = Mathf.Lerp(0, _end, t);
                    yield return new WaitForSeconds(0.02f);
                }
            }
            else if (_change == 2)//하락
            {
                while (t <1f)
                {
                    t += Time.deltaTime* spd;
                    _img.fillAmount = Mathf.Lerp(_start, 0, t);
                    yield return new WaitForSeconds(0.02f);
                }
                _img.fillAmount = 0;
                yield return new WaitForSeconds(0.1f);
                t = 0f;
                _img.fillAmount = 1f;
                headerUI.img_likeGauge.color = headerUI.arr_likeColors[(int)gameMgr.selectHeader.statLike];
                gameMgr.soundMgr.PlaySfx(_img.transform.GetChild(0).position, gameMgr.soundMgr.LoadClip("Sounds/SFX/14short2NL"));
                while (t < 1f)
                {
                    t += Time.deltaTime* spd;
                    _img.fillAmount = Mathf.Lerp(1, _end, t);
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime* spd;
                _img.fillAmount = Mathf.Lerp(_start, _end, t);
                yield return new WaitForSeconds(0.02f);
            }
            _img.fillAmount = _end;
        }
        headerUI.img_likeGauge.color = headerUI.arr_likeColors[(int)gameMgr.selectHeader.statLike];
        yield return new WaitForSeconds(1f);
        _img.transform.parent.gameObject.SetActive(false);
        gameMgr.currentCoroutine = null;

    }

    //public void UIPerfect()
    //{
    //    StartCoroutine(JudgeImage(char_img_judge, perfect));
    //}
    //public void UIGood()
    //{
    //    StartCoroutine(JudgeImage(char_img_judge, good));
    //}
    //public void UIMiss()
    //{
    //    StartCoroutine(JudgeImage(char_img_judge, miss));
    //}

    ////텍스트 변경 함수
    //public void UICount(string _t)
    //{
    //    race_count.text = _t;
    //    start_count.text = _t;
    //}


    //public void ChangeCmdImage()
    //{
    //    switch (gameMgr.statGame)
    //    {
    //        case GameState.NONE:
    //            rock.gameObject.SetActive(true);
    //            paper.gameObject.SetActive(true);
    //            finger.gameObject.SetActive(true);
    //            SetImageSprite(finger, Resources.Load<Sprite>("Finger_Run"));
    //            doubleRock.gameObject.SetActive(true);
    //            break;
    //        case GameState.GAMING:
    //            rock.gameObject.SetActive(false);
    //            paper.gameObject.SetActive(false);
    //            SetImageSprite(finger, Resources.Load<Sprite>("Finger_Jump"));
    //            doubleRock.gameObject.SetActive(false);
    //            break;
    //    }
    //}

    //이미지 변경 함수
    //public IEnumerator RaceUI()
    //{
    //    float t = 0f;

    //    while (gameMgr.selectHeader.statObstacle != ObstacleState.NONE)
    //    {
    //        t += Time.deltaTime;
    //        int m = (int)(t / 60) % 60;
    //        int s = (int)(t % 60);
    //        int ms = (int)(t * 100) % 100;
    //        race_time.text = "TIME " + m.ToString("D2") + ":" + s.ToString("D2") + ":" + ms.ToString("D2");
    //        goal_time.text = "TIME " + m.ToString("D2") + ":" + s.ToString("D2") + ":" + ms.ToString("D2");
    //        //main_speed.text = "Speed: " + gameMgr.chara.nvAgent.speed.ToString();
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    StartCoroutine(UIFleek(race_time.gameObject, true));
    //    StartCoroutine(UIFleek(goal_time.gameObject, true));

    //    if (t < gameMgr.timeRecord)
    //    {
    //        gameMgr.timeRecord = t;
    //        PlayerPrefs.SetFloat("TimeRecord", t);
    //        Debug.Log("!!New Record!!");
    //    }
    //}

}
