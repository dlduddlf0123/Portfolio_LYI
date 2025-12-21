using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Note : MonoBehaviour
{
    protected PlayManager ingameMgr;
    public AudioClip sfx;
    public Transform checkPosition;  //어떤 판정선에서 판정될지

    public int lineNum;
    public bool isChecked; // 캐릭터 앞의 판정체에 닿아야 isChecked 활성화, true면은 노트판정이 가능해짐
    public bool isGuidePointMade; //이 변수로 판정 가이드 프리팹 생성 한번만되게 함
    public bool isDoubleNote; //동시타 노트인지 체크

    protected void Awake()
    {
        ingameMgr = PlayManager.Instance;
        isChecked = false;
        isGuidePointMade = false;
    }

    protected void OnEnable()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //장애물에 맞았을 때
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            collision.GetComponent<Character>().Hit();
            ingameMgr.PopNote();
            this.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Pass"))
        {
            ingameMgr.Pass();
            this.gameObject.SetActive(false);
            ingameMgr.PopNote();
        }

        CheckTrigger(collision);
    }

    void Update()
    {
        //if (!EventSystem.current.IsPointerOverGameObject())
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (isChecked)
        //        {
        //            if (Input.touchCount > 1 && (Input.GetTouch(0).position.x -))
        //                if (((lineNum == 1 || lineNum == 3) && (Input.mousePosition.x > ingameMgr.uiMgr.canvaswidth / 2)) ||
        //                        ((lineNum == 0 || lineNum == 2) && (Input.mousePosition.x < ingameMgr.uiMgr.canvaswidth / 2)))
        //                {
        //                    ingameMgr.PopNote();
        //                    Judge(transform.position);
        //                    ingameMgr.soundMgr.PlaySfx(transform, sfx, 1, 1);
        //                    if (ingameMgr.isFever == true)  //피버모드일때 카메라 흔들리는 연출
        //                    {
        //                        ingameMgr.cameraList[0].GetComponent<Animation>().Play();
        //                    }
        //                    if (lineNum == 1 || lineNum == 3)
        //                    {
        //                        ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[1]);
        //                    }
        //                    else if (lineNum == 0 || lineNum == 2)
        //                    {
        //                        ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[2]);
        //                    }
        //                    ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[0]);
        //                    gameObject.SetActive(false);
        //                }
        //        }


        //    }
        //}
        CreateGuidePoint();
    }
    
    /// <summary>
    /// 가이드 포인트 생성함수
    /// </summary>
    protected virtual void CreateGuidePoint()
    {
        if ((Mathf.Abs(checkPosition.position.x - transform.position.x)) < Mathf.Abs(ingameMgr.hitChecker.position.x - checkPosition.position.x) * 2 && isGuidePointMade == false)
        {
            isGuidePointMade = true;
            GameObject go;
            go = ingameMgr.uiMgr.circleGuide.Dequeue();
            ingameMgr.uiMgr.currentHitGuide = go;
            go.SetActive(true);
            //go.transform.position = ingameMgr.uiMgr.HitGuide.transform.GetChild(lineNum).transform.position;
            go.GetComponent<CircleGuide>().lineNum = lineNum;
            go.transform.position = ingameMgr.currentCam.WorldToScreenPoint(ingameMgr.uiMgr.HitGuide.transform.GetChild(lineNum).transform.position);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    
    public virtual void Judge(StateInput _inputstate)
    {
        switch(_inputstate)
        {
            case StateInput.CLICK:
                ingameMgr.player.TouchMove1();
                if (((lineNum == 1 || lineNum == 3) && (ingameMgr.inputMgr[1].stateInput==StateInput.CLICK)) ||
                                        ((lineNum == 0 || lineNum == 2) && (ingameMgr.inputMgr[0].stateInput == StateInput.CLICK)))
                {
                    PositionCheck(transform.position);
                    ingameMgr.PopNote();
                    ingameMgr.soundMgr.PlaySfx(transform, sfx, 1, 1);
                    if (ingameMgr.isFever == true)  //피버모드일때 카메라 흔들리는 연출
                    {
                        ingameMgr.cameraList[0].GetComponent<Animation>().Play();
                    }
                    if (lineNum == 1 || lineNum == 3)
                    {
                        ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[1]);
                    }
                    else if (lineNum == 0 || lineNum == 2)
                    {
                        ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[2]);
                    }
                    ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[0]);
                    gameObject.SetActive(false);
                }
                break;
            case StateInput.DOUBLEHIT:
                PositionCheck(transform.position);
                ingameMgr.PopNote();
                ingameMgr.soundMgr.PlaySfx(transform, sfx, 1, 1);
                if (ingameMgr.isFever == true)  //피버모드일때 카메라 흔들리는 연출
                {
                    ingameMgr.cameraList[0].GetComponent<Animation>().Play();
                }
                if (lineNum == 1 || lineNum == 3)
                {
                    ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[1]);
                }
                else if (lineNum == 0 || lineNum == 2)
                {
                    ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[2]);
                }
                ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[0]);
                gameObject.SetActive(false);
                break;
        }
        
    }

    protected virtual void CheckTrigger(Collider collision)
    {
        if (collision.gameObject.CompareTag("CheckTrigger"))
        {
            isChecked = true;
            if (lineNum == 0 || lineNum == 1)
            {
                ingameMgr.player.attackMotion = 2; //공중공격모션
            }
            else if (lineNum == 2 || lineNum == 3)
            {
                ingameMgr.player.attackMotion = 0; //지상공격모션
            }
            ingameMgr.isCheckingNote = 0;
        }
    }
    public void PositionCheck(Vector3 _pos)
    {
        checkPosition.gameObject.GetComponent<Animation>().Play();
        ingameMgr.judgeUI.judgeText.GetComponent<Animation>().Play();
        if (Mathf.Abs(checkPosition.position.x - _pos.x) > (ingameMgr.judgeThreshold)*0.8f)
        {
            ingameMgr.judgeUI.ChangeJudgeText(0);
            ingameMgr.count_bad++;
            ingameMgr.GetPoint(2000 / ingameMgr.count_note, ingameMgr.isFever);
            if(ingameMgr.isFever==true)
            {
                ingameMgr.FeverOff();
            }
        }
        else if(Mathf.Abs(checkPosition.position.x - _pos.x) > (ingameMgr.judgeThreshold)*0.6f)
        {
            ingameMgr.judgeUI.ChangeJudgeText(1);
            ingameMgr.count_good++;
            ingameMgr.GetPoint(4000 / ingameMgr.count_note, ingameMgr.isFever);
            if (ingameMgr.isFever == true)
            {
                ingameMgr.FeverOff();                           
            }
        }
        else if (Mathf.Abs(checkPosition.position.x - _pos.x) > (ingameMgr.judgeThreshold) * 0.4f)
        {
            ingameMgr.judgeUI.ChangeJudgeText(2);
            ingameMgr.count_great++;
            ingameMgr.GetPoint(6000 / ingameMgr.count_note, ingameMgr.isFever);
        }
        else if (Mathf.Abs(checkPosition.position.x - _pos.x) > (ingameMgr.judgeThreshold) * 0.2f)
        {
            ingameMgr.judgeUI.ChangeJudgeText(3);
            ingameMgr.count_perfect++;
            ingameMgr.GetPoint(8000 / ingameMgr.count_note, ingameMgr.isFever);
        }
        else
        {
            ingameMgr.judgeUI.ChangeJudgeText(4);
            ingameMgr.count_fantastic++;
            if(ingameMgr.isFever==false)
            {
                ingameMgr.feverPoint += 1;
                ingameMgr.uiMgr.SetFeverGuage(ingameMgr.feverPoint);
            }
            ingameMgr.GetPoint(10000 / ingameMgr.count_note, ingameMgr.isFever);
        }
    }
}
