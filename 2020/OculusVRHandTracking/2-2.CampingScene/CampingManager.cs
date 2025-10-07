using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TravelTime
{
    MORNING,
    AFTERNOON,
    EVENING,
}

public class CampingManager : StageManager
{
    public Material[] arr_skybox = new Material[3];  //오전, 오후, 저녁 머테리얼
    public GameObject[] arr_directionalLight = new GameObject[3];
    public Color[] arr_forColor = new Color[3];

    public TravelTime campingTime; 
    public int clearCount = 0;

    protected override void DoAwake()
    {
        headers = headersTransform.GetComponentsInChildren<Character>();

        ChangeSelectCharacter(headers[0]);

        gameMgr.hand = hand;
        gameMgr.mainCam = mainCam;
        gameMgr.statGame = GameState.TRAVEL;

        //StartCoroutine(testTime());
    }

    IEnumerator testTime()
    {
        int time = 0;
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (time < 2)
            {
                time++;
            }
            else
            {
                time = 0;
            }
            ChangeTime(time);
        }
    }


    /// <summary>
    /// 미니게임 종료 후 시간 체크
    /// </summary>
    public void CheckTime()
    {
        if (clearCount > 1)
        {
            clearCount = 0;
            campingTime++;
            ChangeTime((int)campingTime);
        }
    }


    /// <summary>
    /// 스카이박스 변경
    /// </summary>
    /// <param name="_time">0:오전/1:오후/2:밤</param>
    public void ChangeTime(int _time)
    {
        for (int i = 0; i < arr_directionalLight.Length; i++)
        {
            arr_directionalLight[i].SetActive(false);
        }
        arr_directionalLight[_time].SetActive(true);

        RenderSettings.sun = arr_directionalLight[_time].GetComponent<Light>();
        RenderSettings.skybox = arr_skybox[_time];
        RenderSettings.fogColor = arr_forColor[_time];

    }

    
    /// <summary>
    /// 캠핑 시작
    /// </summary>
    public override void PlayStart()
    {
        base.PlayStart();

        gameMgr.statGame = GameState.TRAVEL;
        campingTime = TravelTime.MORNING;

        ChangeTime((int)campingTime);

    }

    /// <summary>
    /// 캠핑 종료
    /// </summary>
    public override void PlayEnd()
    {
        base.PlayEnd();


    }

}
