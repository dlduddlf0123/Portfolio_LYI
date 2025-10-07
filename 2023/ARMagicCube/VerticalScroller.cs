using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VerticalScroller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    GameManager gameMgr;

    RectTransform moveTarget;

    RectTransform startTr;
    Vector3 startVec;
    Vector3 endVec;

    float scrollSpeed = 15f;
    float moveScale = 0.6f;
    float clickTime;

    public UnityAction onVerticalChanged = null;
    bool isDrag = false;

    public bool isVerticalMove = false;

    private void Start()
    {
        gameMgr = GameManager.Instance;
        moveTarget = transform.parent.GetComponent<RectTransform>();

        //onVerticalChanged += VerticalChange;
        //onVerticalChanged += gameMgr.wordCardMgr.VerticalWordCardChangeActive;
    }


    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        startVec = eventData.position;
        startTr = moveTarget;

        clickTime = 0;

        Debug.Log("PointerDown:" + startVec);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        endVec = eventData.position;

        if (Mathf.Abs(startVec.y - endVec.y) < 20f &&
            !isDrag)
        {
            moveTarget.anchoredPosition =
                  Vector3.up * Screen.height;
            isVerticalMove = false;
            return;
        }

        isDrag = true;
        isVerticalMove = true;

        moveTarget.anchoredPosition =
            Vector3.up * Screen.height //현재 위치
            + Vector3.up * (endVec.y - startVec.y) * moveScale; //현재 위치서 거리만큼 이동
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        endVec = eventData.position;
        float accelation = Vector3.Distance(startVec, endVec);
        isVerticalMove = false;
        isDrag = false;

        if (clickTime < 1f &&
            clickTime > 0)
        {
            if (startVec.y > endVec.y)
            {
                //위로
                gameMgr.ui_librarySelect.ToggleScrollView(true);
            }
            else
            {
                //아래로
                gameMgr.ui_librarySelect.ToggleScrollView(false);
            }
        }

        //if (accelation < 300f)
        //{
        //    //되돌리기
        //   // StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.up * Screen.height, 5, false));
        //    return;
        //}

        if (moveTarget.anchoredPosition.y > Screen.height)
        {
            //Move Up
            gameMgr.ui_librarySelect.ToggleScrollView(true);
        }
        else if (moveTarget.anchoredPosition.y < Screen.height)
        {
            //Move Down
            gameMgr.ui_librarySelect.ToggleScrollView(false);
        }

        clickTime = 0;

        Debug.Log("PointerUp:" + endVec);
    }

    //IEnumerator LerpImageMove(RectTransform _target, Vector3 _startVec, Vector3 _endVec, float _speed = 1, bool _isMove = true)
    //{
    //    if (_isMove)
    //    {
    //        gameMgr.wordCardMgr.currentWord.StopTimeline();
    //    }

    //    float t = 0;
    //    while (t < 1f)
    //    {
    //        _target.anchoredPosition3D = Vector3.Lerp(_startVec, _endVec, t);
    //        t += 0.01f * scrollSpeed * _speed;
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    _target.anchoredPosition3D = _endVec;

    //    if (_isMove)
    //    {
    //        onVerticalChanged.Invoke();
    //    }
    //}

    IEnumerator Timer(float _t)
    {
        while (_t < 3)
        {
            _t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
