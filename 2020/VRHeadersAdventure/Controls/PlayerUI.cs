using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// 각 캐릭터 호감도 표시, 나가기 등
/// </summary>
public class PlayerUI : MonoBehaviour
{
    SteamVR_Action_Boolean toggleUI = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ToggleUI");
    Hand hand;

    public Vector3 camPos;
    public Vector3 handPos;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = handPos;
        transform.rotation = Quaternion.LookRotation(transform.position -camPos);
    }

    /// <summary>
    /// UI를 활성화 시키고 반대편 손에 커서(막대기) 활성화
    /// UI 초기화
    /// </summary>
    /// <param name="_hand">버튼을 누른 손</param>
    public void ToggleUI(Hand  _hand)
    {
        hand = _hand;
        handPos = hand.transform.GetChild(0).position;
        hand.otherHand.transform.GetChild(0).gameObject.SetActive(true);
    }


}
