using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;
using System.Threading;
using Valve.VR;

public class MyRightHand : MonoBehaviour
{
    public Character header;

    public GameObject laserPrefab;
    GameObject laser;
    Transform laserTransform;
    Vector3 hitPoint;
    GameObject hitObj;

    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean moveAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Move");
    public SteamVR_Action_Boolean callAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Call");

    public SteamVR_Input_Sources myHandType;

    // Start is called before the first frame update
    void Start()
    {
       laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    // Update is called once per frame
    void Update()
    {
        MoveAction();
        CallAction();
    }

    //대가리 선택하기
    //누를 시 레이져 발사, 떼면 이동
    public void MoveAction()
    {
        //레이져 발사
        if (moveAction.GetState(SteamVR_Input_Sources.RightHand))
        {
            RaycastHit hit;
            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                hitObj = hit.transform.gameObject;
                ShowLaser(hit);
            }
        }
        else
        {
            laser.SetActive(false);
        }

        if (moveAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            if (header == null)
            {
                return;
            }
            //이동 함수
            header.MoveCharacter(hitPoint,hitObj);
        }
    }

    //대가리 선택하기
    //누르면 부르기
    public void CallAction()
    {
        if (callAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            header.MoveCharacter(transform.position,hitObj);
        }
    }

    void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laser.transform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, 0.5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    /// <summary>
    /// 레이저가 부딪힌 사물 체크
    /// 캐릭터, 아이템, 상자, 벽 등
    /// </summary>
    public void CheckLaserTarget()
    {
        if (hitObj.GetComponent<Character>() != null)
        {
            header = hitObj.GetComponent<Character>();
        }
        else
        {
            header.MoveCharacter(hitPoint,hitObj);
        }
    }
}
