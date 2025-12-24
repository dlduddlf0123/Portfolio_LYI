using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵 생성 클래스
/// CSV파일에서 숫자를 읽어와 오브젝트 생산
/// 프리팹 형태로 오브젝트 관리
/// 
/// 오브젝트 풀링
/// 코인, 하트 등 아이템 생성
/// </summary>
public class RunMapCreator : MonoBehaviour
{
    //맵 스크롤러 위치값
    public RunMapScroller run_scroller;

    public GameObject prefab_stage;

    private void Awake()
    {
        
    }

    public void CreateStage(int stageNum)
    {

    }


    IEnumerator CreateMap()
    {
        while (true)
        {
            //현재 맵 생성 블럭이 진행 되었을 경우
            yield return new WaitForSeconds(0.1f);
        }

    }
}
