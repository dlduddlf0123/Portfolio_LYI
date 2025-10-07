using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnly;

/// <summary>
/// 손목캔버스
/// </summary>
public class StatusUI : MonoBehaviour
{
    GameManager gameMgr;
    StageManager stageMgr;
    HeaderCanvas headerUI;

    GameObject mainCam;

    public Image[] arr_gaugeImgs; //0:like, 1:hunger, 2:energy
    
    void Awake()
    {
        
    }

    private void Start()
    {
        gameMgr = GameManager.Instance;
        stageMgr = gameMgr.currentPlay.GetComponent<StageManager>();
        mainCam = gameMgr.mainCam.gameObject;

        headerUI = stageMgr.interactHeader.headerCanvas;
        arr_gaugeImgs[0].fillAmount = 0;
        arr_gaugeImgs[0].color = headerUI.arr_likeColors[0];
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - GameManager.Instance.mainCam.transform.position);
    }



    public void SetImageSprite(Image _i, Sprite _s)
    {
        _i.sprite = _s;
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
        //_img.transform.parent.gameObject.SetActive(true);

        float t = 0.0f;
        float spd = 2f;
        float likeMax = 1f;
        switch (stageMgr.interactHeader.statLike)
        {
            case LikeState.HATE:
                likeMax = 0.5f;
                break;
            case LikeState.NORMAL:
                likeMax = 1f;
                break;
            case LikeState.FRIEND:
                likeMax = 1;
                break;
        }


        if (_change != 0)
        {
            if (_change == 1)//상승
            {
                gameMgr.soundMgr.PlaySfx(_img.transform, Defines.SOUND_SFX_GAUGEUP);
                while (t < 1f)
                {
                    t += Time.deltaTime * spd;
                    _img.fillAmount = Mathf.Lerp(_start, likeMax, t);
                    yield return new WaitForSeconds(0.02f);
                }

                _img.fillAmount = likeMax;
                gameMgr.PlayEffect(_img.transform.GetChild(0).position, gameMgr.particles[1]);
                gameMgr.soundMgr.PlaySfx(_img.transform, Defines.SOUND_SFX_LIKEUP);

                yield return new WaitForSeconds(1f);

                t = 0f;
                _img.fillAmount = 0f;
                _end -= likeMax;
                _img.color = headerUI.arr_likeColors[(int)stageMgr.interactHeader.statLike];

                while (t < 1f)
                {
                    t += Time.deltaTime * spd;
                    _img.fillAmount = Mathf.Lerp(0, _end, t);
                    yield return new WaitForSeconds(0.02f);
                }
            }
            else if (_change == 2)//하락
            {
                gameMgr.soundMgr.PlaySfx(_img.transform.GetChild(0), gameMgr.soundMgr.LoadClip(Defines.SOUND_SFX_GAUGEDOWN));
                while (t < 1f)
                {
                    t += Time.deltaTime * spd;
                    _img.fillAmount = Mathf.Lerp(_start, 0, t);
                    yield return new WaitForSeconds(0.02f);
                }
                _img.fillAmount = 0;
                yield return new WaitForSeconds(0.1f);

                t = 0f;
                _img.fillAmount = likeMax;
                _end += likeMax;

                _img.color = headerUI.arr_likeColors[(int)stageMgr.interactHeader.statLike];
                gameMgr.soundMgr.PlaySfx(_img.transform.GetChild(0), gameMgr.soundMgr.LoadClip(Defines.SOUND_SFX_LIKEDOWN));
                while (t < 1f)
                {
                    t += Time.deltaTime * spd;
                    _img.fillAmount = Mathf.Lerp(likeMax, _end, t);
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        else
        {
            if (_start < _end)
                gameMgr.soundMgr.PlaySfx(_img.transform, Defines.SOUND_SFX_GAUGEUP);
            else
                gameMgr.soundMgr.PlaySfx(_img.transform, Defines.SOUND_SFX_GAUGEDOWN);

            while (t < 1f)
            {
                t += Time.deltaTime * spd;
                _img.fillAmount = Mathf.Lerp(_start, _end, t);
                yield return new WaitForSeconds(0.02f);
            }
            _img.fillAmount = _end;
        }
        _img.color = headerUI.arr_likeColors[(int)stageMgr.interactHeader.statLike];
        yield return new WaitForSeconds(1f);
        //_img.transform.parent.gameObject.SetActive(false);
        _img.color = headerUI.arr_likeColors[(int)stageMgr.interactHeader.statLike];
        gameMgr.currentCoroutine = null;
    }

    public IEnumerator SetGauge(Image _img, float _start, float _end)
    {
        float gaugeTime = 0.0f;
        float spd = 2f;

        _start *= 0.01f;
        _end *= 0.01f;

        while (Mathf.Round(_start - _end) > 10 &&
            gaugeTime < 1f)
        {
            gaugeTime += Time.deltaTime * spd;
            _img.fillAmount = Mathf.Lerp(_start, _end, gaugeTime);
            yield return new WaitForSeconds(0.02f);
        }
        _img.fillAmount = _end;

    }


}
