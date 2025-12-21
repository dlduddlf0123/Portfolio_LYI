using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongNote : Note
{
    public GameObject includedNote;

    List<GameObject> list_miniCube = new List<GameObject>();

    Color cubeColor;

    public int beatCount; // 롱노트 1개가 몇 개의 일반 노트 역할을 할지
    public int currentBeatCount = 0;
    public bool isJudging =false; //롱노트 판정~ing

    public Vector3 startDragPos;
    public Vector3 endDragPos;
    public float checkPos;

    void Start()
    {
        if (lineNum == 0 || lineNum == 2)
        {
            cubeColor = Color.red;
        }
        else if (lineNum == 1 || lineNum == 3)
        {
            cubeColor = Color.blue;
        }
        for (int i = 0; i < beatCount; i++)
        {
            GameObject go;
            go = Instantiate(includedNote,
                new Vector3(gameObject.transform.position.x + i * includedNote.transform.localScale.x+(includedNote.transform.localScale.x-gameObject.transform.localScale.x)/2,
                gameObject.transform.position.y, gameObject.transform.position.z),
                Quaternion.identity);
            go.transform.parent = gameObject.transform;
            go.GetComponent<Renderer>().material.color = cubeColor;
            list_miniCube.Add(go);
            if (i == beatCount - 1)
                go.GetComponent<SubNote>().isLastnote = true;
            //go.GetComponent<Note>().checkPosition = checkPosition;
            // go.GetComponent<Note>().lineNum = lineNum;
        }

    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.CompareTag("CheckTrigger"))
    //    {
    //        isChecked = true;
    //        ingameMgr.player.attackMotion = 3; //롱노트 공격모션
    //    }
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        GetComponent<Collider>().enabled = false;
    //        other.GetComponent<Character>().Hit();
    //        ingameMgr.PopNote();
    //        this.gameObject.SetActive(false);
    //    }
    //    if (other.gameObject.CompareTag("Pass"))
    //    {
    //        ingameMgr.Pass();
    //        this.gameObject.SetActive(false);
    //        ingameMgr.PopNote();
    //    }
    //}
    private void OnTriggerEnter(Collider collision)
    {
        //장애물에 맞았을 때
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            collision.GetComponent<Character>().Hit();
            ingameMgr.PopNote();
            ingameMgr.isLongNote = false;
            this.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Pass"))
        {
            ingameMgr.Pass();
            ingameMgr.PopNote();
            this.gameObject.SetActive(false);
        }

        CheckTrigger(collision);
    }
    protected override void CheckTrigger(Collider collision)
    {
        if (collision.gameObject.CompareTag("CheckTrigger"))
        {
            isChecked = true;
            ingameMgr.player.attackMotion = 3; //롱노트 공격모션
            ingameMgr.isCheckingNote = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
            {
            startDragPos = Input.mousePosition;
                if (isChecked)
                {
                    if (((lineNum == 1 || lineNum == 3) && (Input.mousePosition.x > ingameMgr.uiMgr.canvaswidth / 2)) ||
                            ((lineNum == 0 || lineNum == 2) && (Input.mousePosition.x < ingameMgr.uiMgr.canvaswidth / 2)))
                    {
                        ingameMgr.isLongNote = true;

                        ingameMgr.player.mAnimator.SetTrigger("isTrigger");
                        ingameMgr.player.mAnimator.SetBool("LongNoteMove", true);

                        ingameMgr.PopNote();

                        ingameMgr.soundMgr.PlaySfx(transform, sfx, 1,1);
                        ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[0]); ;

                        Judge(transform.position);

                        GetComponent<BoxCollider>().enabled = false;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isChecked)
                {
                    ingameMgr.isLongNote = false;
                    ingameMgr.player.mAnimator.SetBool("LongNoteMove", false);
                    if (currentBeatCount == beatCount)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
            endDragPos = Input.mousePosition;
            checkPos = startDragPos.x - endDragPos.x;
                if (isChecked&&Mathf.Abs(checkPos)>30)
                {
                    ingameMgr.isLongNote = true;
                    if (isChecked && (currentBeatCount < beatCount))
                    {
                        if (Mathf.Abs(list_miniCube[currentBeatCount].transform.position.x - checkPosition.position.x) < 0.4f)
                        {
                            ingameMgr.PlayEffect(list_miniCube[currentBeatCount].transform.position, ingameMgr.particles[1]);
                            Judge(list_miniCube[currentBeatCount].transform.position);
                            currentBeatCount++;
                        }
                        if (currentBeatCount == list_miniCube.Count)
                        {
                            ingameMgr.player.mAnimator.SetBool("LongNoteMove", false);
                            ingameMgr.isLongNote = false;
                        }
                    }
                }
            }
       */
        CreateGuidePoint();
        
        if (checkPosition.position.x-transform.position.x>50)
        {
            ingameMgr.isLongNote = false;
            gameObject.SetActive(false);
        }
    }
    
    public override void Judge(StateInput _inputstate)
    {
        switch (_inputstate)
        {
            case StateInput.CLICK:
                if (((lineNum == 1 || lineNum == 3) && (ingameMgr.inputMgr[1].stateInput == StateInput.CLICK)) ||
                                        ((lineNum == 0 || lineNum == 2) && (ingameMgr.inputMgr[0].stateInput == StateInput.CLICK)))
                {
                    ingameMgr.isLongNote = true;
                    PositionCheck(transform.position);
                    ingameMgr.player.mAnimator.SetTrigger("isTrigger");
                    ingameMgr.player.mAnimator.SetBool("LongNoteMove", true);

                    ingameMgr.PopNote();

                    ingameMgr.soundMgr.PlaySfx(transform, sfx, 1.5f);
                    ingameMgr.PlayEffect(this.transform.position, ingameMgr.particles[0]);

                    GetComponent<BoxCollider>().enabled = false;
                    ingameMgr.player.TouchMove1();
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
                }
               
                break;
            case StateInput.DRAG:
                if ((((lineNum == 1 || lineNum == 3) && (ingameMgr.inputMgr[1].stateInput == StateInput.DRAG)) ||
                   ((lineNum == 0 || lineNum == 2) && (ingameMgr.inputMgr[0].stateInput == StateInput.DRAG))) &&ingameMgr.isLongNote==true)
                {
                    ingameMgr.isLongNote = true;
                    if (isChecked && (currentBeatCount < beatCount))
                    {
                        if (Mathf.Abs(list_miniCube[currentBeatCount].transform.position.x - checkPosition.position.x) < 0.4f)
                        {
                            ingameMgr.PlayEffect(list_miniCube[currentBeatCount].transform.position, ingameMgr.particles[1]);
                            PositionCheck(checkPosition.position);
                            ingameMgr.soundMgr.PlaySfx(transform, sfx, 1, 1);
                            currentBeatCount++;
                        }
                        if (currentBeatCount == list_miniCube.Count)
                        {
                            ingameMgr.player.mAnimator.SetBool("LongNoteMove", false);
                            ingameMgr.isLongNote = false;
                        }
                    }
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
                    
                }
                break;
                //ingameMgr.player.TouchMove1();                



        }
    }
    public void LongNoteEnd()
    {
        ingameMgr.isLongNote = false;
        ingameMgr.player.mAnimator.SetBool("LongNoteMove", false);
        if (currentBeatCount == beatCount)
        {
            gameObject.SetActive(false);
            ingameMgr.currentLongnote = null;
        }
    }

}
