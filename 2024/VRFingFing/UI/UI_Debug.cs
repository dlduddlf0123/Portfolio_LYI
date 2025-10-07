using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using VRTokTok.Character;
using VRTokTok.Manager;

namespace VRTokTok.UI
{
    /// <summary>
    /// 2/29/2024-LYI
    /// 게임 플레이 체크용 디버그
    /// </summary>
    public class UI_Debug : MonoBehaviour
    {
        GameManager gameMgr;

        //check FPS for debug
        [SerializeField]
        TextMeshProUGUI txt_fps;

        [SerializeField]
        TextMeshProUGUI[] arr_hand;
        [SerializeField]
        TextMeshProUGUI[] arr_controller;


        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        void Start()
        {

        }

        float pollingTime = 1f;
        float time;
        int frameCount;
        private void Update()
        {
            time += Time.deltaTime;
            frameCount++;

            if (time >= pollingTime)
            {
                int frameRate = Mathf.RoundToInt(frameCount / time);
                txt_fps.text = "FPS: " + frameRate.ToString();

                time -= pollingTime;
                frameCount = 0;
            }

            arr_hand[0].text = "HandL: " + gameMgr.playMgr.tokMgr.arr_pokeHand[0].State.ToString();
            arr_hand[1].text = "HandR: " + gameMgr.playMgr.tokMgr.arr_pokeHand[1].State.ToString();
            arr_controller[0].text = "ControllerL: " + gameMgr.playMgr.tokMgr.arr_pokeController[0].State.ToString();
            arr_controller[1].text = "ControllerR: " + gameMgr.playMgr.tokMgr.arr_pokeController[1].State.ToString();
        }

    }
}