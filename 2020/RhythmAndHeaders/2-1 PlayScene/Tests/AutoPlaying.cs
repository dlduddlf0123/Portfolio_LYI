using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlaying : MonoBehaviour
{
    PlayManager ingameMgr;

    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
        ingameMgr.inputMgr[0].stateInput = StateInput.NONE;
        ingameMgr.inputMgr[1].stateInput = StateInput.NONE;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (ingameMgr.currentNote != null&&ingameMgr.isCheckingNote==0)
        {
            ingameMgr.inputMgr[0].stateInput = StateInput.NONE;
            ingameMgr.inputMgr[1].stateInput = StateInput.NONE;
            if (Mathf.Abs(ingameMgr.currentNote.transform.position.x - ingameMgr.currentNote.GetComponent<Note>().checkPosition.position.x) < ingameMgr.judgeThreshold * 0.1f)
            {

                    if (ingameMgr.currentNote.GetComponent<Note>().lineNum == 1 || ingameMgr.currentNote.GetComponent<Note>().lineNum == 3)
                    {
                        ingameMgr.inputMgr[1].stateInput = StateInput.CLICK;
                    }
                    else if (ingameMgr.currentNote.GetComponent<Note>().lineNum == 0 || ingameMgr.currentNote.GetComponent<Note>().lineNum == 2)
                    {
                        ingameMgr.inputMgr[0].stateInput = StateInput.CLICK;
                    }
                    ingameMgr.currentNote.GetComponent<Note>().Judge(StateInput.CLICK);
                    ingameMgr.currentNote = null;
                    ingameMgr.inputMgr[1].stateInput = StateInput.NONE;
                    ingameMgr.inputMgr[0].stateInput = StateInput.NONE;
                
            }
        }
        else if(ingameMgr.isCheckingNote==1)
        {
            if (Mathf.Abs(ingameMgr.currentLongnote.transform.position.x - ingameMgr.currentLongnote.GetComponent<LongNote>().checkPosition.position.x) < ingameMgr.judgeThreshold * 0.1f)
            {
                if (ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 1 || ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 3)
                {
                    ingameMgr.inputMgr[1].stateInput = StateInput.CLICK;
                }
                else if (ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 0 || ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 2)
                {
                    ingameMgr.inputMgr[0].stateInput = StateInput.CLICK;
                }
                ingameMgr.currentLongnote.GetComponent<LongNote>().Judge(StateInput.CLICK);
            }
            if (ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 1 || ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 3)
            {
                ingameMgr.inputMgr[1].stateInput = StateInput.DRAG;
            }
            else if (ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 0 || ingameMgr.currentLongnote.GetComponent<Note>().lineNum == 2)
            {
                ingameMgr.inputMgr[0].stateInput = StateInput.DRAG;
            }
            ingameMgr.currentLongnote.GetComponent<LongNote>().Judge(StateInput.DRAG);
        }
    }
}
