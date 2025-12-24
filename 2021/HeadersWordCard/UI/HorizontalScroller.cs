using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class HorizontalScroller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    GameManager gameMgr;
    RawImageManager rawImgMgr;

    RectTransform moveTarget;

    RectTransform startTr;
    Vector3 startVec;
    Vector3 endVec;

    public float scrollSpeed = 1f;
    public float moveScale = 3f;
    float clickTime;

    public List<RawImage> list_rawImg = new List<RawImage>();

    private void Start()
    {
        gameMgr = GameManager.Instance;
        rawImgMgr = gameMgr.wordCardMgr.rawImgMgr;
        moveTarget = GetComponent<RectTransform>();
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

        if (Mathf.Abs(startVec.x - endVec.x) < 200f ||
            rawImgMgr.isVerticalMove)
        {
           rawImgMgr.isHorizontalMove = false;
            return;
        }

        rawImgMgr.isHorizontalMove = true;

        if (startVec.x > endVec.x)
        {
            //오른쪽으로
            moveTarget.anchoredPosition = Vector3.left * (rawImgMgr.currentImageNum * gameMgr.screenWidth) + moveScale * Vector3.left * (startTr.position.x + endVec.x - startVec.x) * Time.deltaTime + Vector3.up * moveTarget.anchoredPosition.y;
        }
        if (startVec.x < endVec.x)
        {
            //왼쪽으로
            moveTarget.anchoredPosition = Vector3.left * (rawImgMgr.currentImageNum * gameMgr.screenWidth) + moveScale * Vector3.left * (startTr.position.x + endVec.x - startVec.x) * Time.deltaTime + Vector3.up * moveTarget.anchoredPosition.y;
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        endVec = eventData.position;
        float accelation = Vector3.Distance(startVec, endVec);
        rawImgMgr.isHorizontalMove = false;

        if (clickTime < 1f &&
            clickTime > 0)
        {
            if (startVec.x > endVec.x)
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
            StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.left * rawImgMgr.currentImageNum * gameMgr.screenWidth + Vector3.up * moveTarget.anchoredPosition.y, 5,false));
            return;
        }

        if (moveTarget.anchoredPosition.x > rawImgMgr.currentImageNum * -gameMgr.screenWidth)
        {
            if (rawImgMgr.currentImageNum > 0)
            {
                rawImgMgr.currentImageNum--;

                //위로 이동
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.left * rawImgMgr.currentImageNum * gameMgr.screenWidth + Vector3.up * moveTarget.anchoredPosition.y));

            }
            else
            {
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.left * rawImgMgr.currentImageNum * gameMgr.screenWidth + Vector3.up * moveTarget.anchoredPosition.y, 5, false));
            }
        }
        else if (moveTarget.anchoredPosition.x < rawImgMgr.currentImageNum * -gameMgr.screenWidth)
        {
            if (rawImgMgr.transform.GetChild(0).GetChild(rawImgMgr.currentSubjectNum).childCount > rawImgMgr.currentImageNum+1)
            {
                rawImgMgr.currentImageNum++;

                //아래로 이동
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.left * rawImgMgr.currentImageNum * gameMgr.screenWidth + Vector3.up * moveTarget.anchoredPosition.y));

            }
            else
            {
                StartCoroutine(LerpImageMove(moveTarget, startTr.anchoredPosition, Vector3.left * rawImgMgr.currentImageNum * gameMgr.screenWidth + Vector3.up * moveTarget.anchoredPosition.y, 5, false));
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
            t += 0.01f * (Vector3.Distance(startVec, endVec) * 0.05f) * scrollSpeed * _speed;
            yield return new WaitForSeconds(0.01f);
        }
        _target.anchoredPosition3D = _endVec;
        if (_isMove)
        {
            gameMgr.wordCardMgr.currentWord = gameMgr.wordCardMgr.list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum];
            gameMgr.wordCardMgr.currentWord.PlayTimeline();
        }
    }

    public void ResetHorizontalPosition()
    {
        moveTarget.anchoredPosition = new Vector2(0, moveTarget.anchoredPosition.y);
    }
}
