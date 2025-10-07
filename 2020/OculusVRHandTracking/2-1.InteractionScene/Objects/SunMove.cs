using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeState
{
    NIGHT,
    MORNING,
    NOON,
    AFTERNOON,
}

public class SunMove : MonoBehaviour
{
    GameManager gameMgr;
    public TimeState statTime;

    bool isNoon = false;

    public System.DateTime dateTime = new System.DateTime(2020, 01, 01, 6, 0, 0); //게임 맨처음 시작할때 시간설정
    public System.TimeSpan ts = new System.TimeSpan(0, 10, 0); //10분씩 지날때마다 시간 바뀜 (나중가서 바꿀수도 있음)

    private string time; //콘솔에서 시간체크용

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        CheckTimeState();
        
        StartCoroutine("Checktime");
    }

    private void Update()
    {
        if (gameMgr.statGame == GameState.INTERACTION)
        {
            transform.Rotate(Time.deltaTime * 0.2f, 0, 0);

            CheckTimeState();

        }
    }

    public void CheckTimeState()
    {
        //0 = 일출
        //90 = 정오 12:00
        if (transform.eulerAngles.x > 190 &&
            transform.eulerAngles.x < 350)
        {
            statTime = TimeState.NIGHT;
            isNoon = false;
        }
        else if (transform.eulerAngles.x > -10 &&
            transform.eulerAngles.x < 80)
        {
            statTime = (isNoon) ? TimeState.AFTERNOON : TimeState.MORNING;
        }
        else if (transform.eulerAngles.x > 80 &&
           transform.eulerAngles.x < 100)
        {
            statTime = TimeState.NOON;
            isNoon = true;
        }
    }

    IEnumerator Checktime()
    {
        while (true)
        {
            if (dateTime.Hour > 17)
            {
                gameMgr.StartCoroutine("NextDay");  //18시 지나면 날짜 바뀜
            }
            time = dateTime.ToString("MM/dd HH:mm");
            //Debug.Log(time);
            dateTime = dateTime + ts;
            yield return new WaitForSeconds(6.527f);    //게임 시간 12시간, 현실 시간 7분 50초 ->게임시간 10분, 현실 시간 6.527초   
        }
    }

}