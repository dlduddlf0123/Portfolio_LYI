using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok;
using UnityEngine.Events;

public class GrabConstraints : MonoBehaviour
{    
    public Transform moveObject = null;

    public UnityAction onGrabStart = null;
    public UnityAction onGrabEnd = null;


    Vector3 startPos;
    Quaternion startRot;
    Vector3 startScale;

    Vector3 moveObjStartPos;

    public bool isPosition;
    public bool moveX, moveY, moveZ;

    //public bool isScale;
    //public bool scaleX, scaleY, scaleZ;

    private void Start()
    {
        startPos = this.transform.position;
        startRot = this.transform.rotation;
        startScale = this.transform.lossyScale;

        moveObjStartPos = moveObject.transform.position;

        //if (moveX)
        //{
        //    moveObjStartPos = new Vector3(ES3.Load("TableX", moveObjStartPos.x),
        //        moveObjStartPos.y,
        //        moveObjStartPos.z);
        //    moveObject.transform.position = moveObjStartPos;
        //}
        //if (moveY)
        //{
        //    moveObjStartPos = new Vector3(moveObjStartPos.x,
        //          ES3.Load("TableY", moveObjStartPos.y),
        //         moveObjStartPos.z);
        //    moveObject.transform.position = moveObjStartPos;
        //}
        //if (moveZ)
        //{
        //    moveObjStartPos = new Vector3(moveObjStartPos.x,
        //          moveObjStartPos.y,
        //           ES3.Load("TableZ", moveObjStartPos.z));
        //    moveObject.transform.position = moveObjStartPos;
        //}

    }

    // Update is called once per frame
    void Update()
    {
        if (moveObject == null)
        {
            return;
        }

        if (isPosition)
        {
            Vector3 movedPos = moveObjStartPos + (transform.position - startPos);

            if (moveX)
            {
                moveObject.position = new Vector3
                    (movedPos.x,
                   moveObject.position.y,
                    moveObject.position.z);
            }
            if (moveY)
            {
                moveObject.position = new Vector3
                    (moveObject.position.x,
                    movedPos.y,
                    moveObject.position.z);
            }
            if (moveZ)
            {
                moveObject.position = new Vector3
                    (moveObject.position.x,
                   moveObject.position.y,
                   movedPos.z);
            }
        }


        //if (isScale)
        //{
        //    Vector3 changedScale = transform.lossyScale - startScale;

        //    if (scaleX)
        //    {
        //        moveObject.lossyScale += Vector3.right * changedScale.x;
        //    }
        //    if (scaleY)
        //    {
        //        moveObject.position += Vector3.up * changedScale.y;
        //    }
        //    if (scaleZ)
        //    {
        //        moveObject.position += Vector3.forward * changedScale.z;
        //    }
        //}
        
    }

    //private void OnApplicationQuit()
    //{
    //    if (moveX)
    //    {
    //        ES3.Save("TableX", moveObject.transform.position.x);
    //    }
    //    if (moveY)
    //    {
    //        ES3.Save("TableY", moveObject.transform.position.y);
    //    }
    //    if (moveZ)
    //    {
    //        ES3.Save("TableZ", moveObject.transform.position.z);
    //    }
    //}

    public void OnGrabStart()
    {
        GameManager.Instance.playMgr.OnGrabStart();

        if (onGrabStart != null)
        {
            onGrabStart.Invoke();
        }

        //if (GameManager.Instance.isTutorial)
        //{
        //    if (tutorialHandle != null)
        //    {
        //        tutorialHandle.EndMark();
        //    }
        //}
    }

    public void OnGrabEnd()
    {
        GameManager.Instance.playMgr.OnGrabEnd();
        if (onGrabEnd != null)
        {
            onGrabEnd.Invoke();
        }
        
        //if (GameManager.Instance.isTutorial)
        //{
        //    if (tutorialHandle != null)
        //    {
        //        tutorialHandle.EndTutorial();
        //    }
        //}
    }


}
