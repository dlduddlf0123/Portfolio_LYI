using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VerticalScroller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    GameManager gameMgr;
    RawImageManager rawImgMgr;

    RectTransform moveTarget;

    RectTransform startTr;
    Vector3 startVec;
    Vector3 endVec;

    public float scrollSpeed = 1f;
    public float moveScale = 10f;
    float clickTime;


    private void Start()
    {
        gameMgr = GameManager.Instance;
        rawImgMgr = gameMgr.wordCardMgr.rawImgMgr;
        moveTarget = transform.parent.GetComponent<RectTransform>();
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
        if (Mathf.Abs(startVec.y - endVec.y) < 200f ||
             rawImgMgr.isHorizontalMove)
        {
            rawImgMgr.isVerticalMove = false;
            return;
        }

        rawImgMgr.isVerticalMove = true;

        if (startVec.y > endVec.y)
        {
            //위로
            moveTarget.anchoredPosition = Vector3.up * (rawImgMgr.currentSubjectNum * gameMgr.screenHeight) + moveScale * Vector3.down * (startTr.position.y + endVec.y - startVec.y) * Time.deltaTime;
        }
        if (startVec.y < endVec.y)
        {
            //아래로
            moveTarget.anchoredPosition = Vector3.up * (rawImgMgr.currentSubjectNum * gameMgr.screenHeight) + moveScale * Vector3.down * (startTr.position.y + endVec.y - startVec.y) * Time.deltaTime;
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        endVec = eventData.position;
        float accelation = Vector3.Distance(startVec, endVec);
        rawImgMgr.isVerticalMove = false;

        if (clickTime < 1f &&
            clickTime > 0)
        {
            if (startVec.y > endVec.y)
            {
                //위로
            }
            else
            {
                //아래로
            }
        }

        if (accelation < 300f)
        {
            StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.up * rawImgMgr.currentSubjectNum * gameMgr.screenHeight, 5, false));
            return;
        }

        if (moveTarget.anchoredPosition.y > rawImgMgr.currentSubjectNum * gameMgr.screenHeight)
        {
            if (rawImgMgr.currentSubjectNum > 0)
            {
                rawImgMgr.currentSubjectNum--;
                rawImgMgr.currentImageNum = 0;

                //위로 이동
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition,  Vector3.up * rawImgMgr.currentSubjectNum * gameMgr.screenHeight));

            }
            else
            {
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.up * rawImgMgr.currentSubjectNum * gameMgr.screenHeight,5,false));
            }
        }
        else if (moveTarget.anchoredPosition.y < rawImgMgr.currentSubjectNum * gameMgr.screenHeight)
        {
            if (rawImgMgr.transform.GetChild(0).childCount > rawImgMgr.currentSubjectNum +1)
            {
                rawImgMgr.currentSubjectNum++;
                rawImgMgr.currentImageNum = 0;

                //아래로 이동
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.up * rawImgMgr.currentSubjectNum * gameMgr.screenHeight));

            }
            else
            {
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.up * rawImgMgr.currentSubjectNum * gameMgr.screenHeight,5, false));
            }
        }

        clickTime = 0;

        Debug.Log("PointerUp:" + endVec);
    }

    IEnumerator LerpImageMove(RectTransform _target, Vector3 _startVec, Vector3 _endVec, float _speed = 1, bool _isMove = true)
    {
        if (_isMove)
        {
            gameMgr.wordCardMgr.currentWord.StopTimeline();
        }

        float t = 0;
        while (t < 1f)
        {
            _target.anchoredPosition3D = Vector3.Lerp(_startVec, _endVec, t);
            t += 0.01f *(Vector3.Distance(startVec, endVec)*0.05f)* scrollSpeed * _speed;
            yield return new WaitForSeconds(0.01f);
        }
        _target.anchoredPosition3D = _endVec;

        if (_isMove)
        {
            gameMgr.wordCardMgr.currentWord = gameMgr.wordCardMgr.list__renderWordCard[rawImgMgr.currentSubjectNum][0];
            gameMgr.wordCardMgr.currentWord.PlayTimeline();

            //다른 곳의 가로열 되돌리기
            for (int i = 0; i < rawImgMgr.transform.GetChild(0).childCount; i++)
            {
                if (i == rawImgMgr.currentSubjectNum)
                {
                    continue;
                }
                rawImgMgr.transform.GetChild(0).GetChild(i).GetComponent<HorizontalScroller>().ResetHorizontalPosition();
            }
        }
    }

    IEnumerator Timer(float _t)
    {
        while (_t < 3)
        {
            _t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
