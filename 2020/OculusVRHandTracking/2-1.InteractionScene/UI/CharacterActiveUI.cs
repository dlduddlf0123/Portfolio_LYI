using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
public class CharacterActiveUI : MonoBehaviour
{
    GameManager gameMgr;
    StageManager stageMgr;
    HandInteract rightHand;

    public Transform respawnPoint;
    public Transform[] respawnPoints;
    private bool characterOn; //캐릭터 On/Off
    private bool istouched; //연속체크 방지

    public Character[] Headers;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        stageMgr = gameMgr.currentPlay.GetComponent<StageManager>();

        rightHand = gameMgr.hand[1].GetComponent<HandInteract>();

        Headers = stageMgr.headers;

        respawnPoints = respawnPoint.GetComponentsInChildren<Transform>();
        characterOn = false;
        istouched = false;
    }

    public void Appear(Character header)
    {
        if (rightHand.handGesture == HandGesture.PINCH && characterOn == false && istouched == false)
        {
            StartCoroutine(switchTimer());
            int randPoint = Random.Range(0, respawnPoints.Length);
            Transform target = respawnPoints[randPoint];
            header.gameObject.transform.position = target.position;
            header.gameObject.SetActive(true);
            characterOn = true;
        }
        else if (rightHand.handGesture == HandGesture.PINCH && characterOn == true && istouched == false)
        {
            StartCoroutine(switchTimer());
            header.gameObject.SetActive(false);
            characterOn = false;
        }

    }
    IEnumerator switchTimer()
    {
        istouched = true;
        yield return new WaitForSeconds(0.5f);
        istouched = false;
    }
}
