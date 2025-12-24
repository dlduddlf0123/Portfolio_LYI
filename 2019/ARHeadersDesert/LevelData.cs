using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour {
    CSVparser parse = new CSVparser();

    public void ReadTestData()
    {
        //파싱 데이터파일 호출
        Table table = parse.ParsingCSV("LevelData");

        for (int i = 0; i < table.Row.Count; i++)
        {
            //데이터 출력
            //raw == 레벨
            //col0 레벨, col2 시간, col3 미사일,col4 AI
            Debug.Log(table.Row[i].Col[0] + " // " +
                table.Row[i].Col[1] + " // " +
                table.Row[i].Col[2] + " // " +
                table.Row[i].Col[3]);
        }
    }

    /// <summary>
    /// 레벨을 넣으면 해당 레벨에 맞는 시간, 미사일, AI 값을 설정
    /// </summary>
    /// <param name="_level"> 현재 스테이지 레벨값</param>
    /// <param name="_type">0: Level, 1: Time, 2: Missiles, 3: AI</param>
    /// <returns></returns>
    public int ReadLevelData(int _level, int _type)
    {
        Table table = parse.ParsingCSV("LevelData");
        int _data =Convert.ToInt32(table.Row[_level].Col[_type]);
        return _data;
    }
}
