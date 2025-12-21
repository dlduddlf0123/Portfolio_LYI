using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum StateInput
{
    NONE=0,
    CLICK,
    DRAG,
    DOUBLEHIT,
}
public class InputManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    
    public StateInput stateInput = StateInput.NONE;
    public PlayManager ingameMgr { get; set; }
    public bool isLeftTouch;
    public bool isPressed;

    public float dragGauge;

    public void Awake()
    {
        ingameMgr = PlayManager.Instance;
    }

    public void Update()
    {
        if(isPressed ==true)
        {
            dragGauge += Time.deltaTime;
            if(dragGauge>0.1f)
            {
                stateInput = StateInput.DRAG;
                NoteCheck();
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        if (stateInput != StateInput.CLICK)
        {
            stateInput = StateInput.CLICK;
            if(ingameMgr.inputMgr[0].stateInput==StateInput.CLICK&&ingameMgr.inputMgr[1].stateInput==StateInput.CLICK)
            {
                ingameMgr.doubleNoteHit.SetActive(true);
            }
            else
            {
                NoteCheck();
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        dragGauge = 0;
        stateInput = StateInput.NONE;
        if(ingameMgr.isCheckingNote==1)
        {
            ingameMgr.currentLongnote.GetComponent<LongNote>().LongNoteEnd();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        stateInput = StateInput.DRAG;
        NoteCheck();
        dragGauge += Time.deltaTime;
       // Debug.Log(dragGauge);
    }
    
    public void NoteCheck()
    {
        if(ingameMgr.currentNote==null)
        {
            return;
        }
        if (ingameMgr.currentNote.activeSelf==true && ingameMgr.isCheckingNote == 0)
        {
            ingameMgr.currentNote.GetComponent<Note>().Judge(stateInput);
        }
        else if (ingameMgr.isCheckingNote == 1&&ingameMgr.currentLongnote.activeSelf==true)
            ingameMgr.currentLongnote.GetComponent<LongNote>().Judge(stateInput);            
    }
}
