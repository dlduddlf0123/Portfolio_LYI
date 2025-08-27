using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVparser
{
    public Table ParsingCSV(string path)
    {
        //csv파일 호출
        TextAsset csvFile = (TextAsset)GameManager.Instance.b_csvdata.LoadAsset<TextAsset>(path) as TextAsset;
        //예외처리
        if (csvFile == null)
            return null;

        //클래스 호출
        Table CSVData = new Table();

        // string.Split(); 파라미터(string) 기준으로 문자열을 잘라서 string[] 으로 반환
        string[] strWhole = csvFile.text.Split('\n');
        string[] strLine = strWhole[0].Split(',');
        //최대값 설정
        int col_max = strWhole.Length; //행 길이
        int row_max = strLine.Length;  //열 길이
        //마지막 문자가 비었을 때 삭제
        if (strWhole[col_max - 1] == "")
            col_max--;

        // 파싱 구현
        //열 최대 길이 미만일때
        for (int col = 0; col < col_max; col++)
        {
            //Col 호출, Col 객체 할당
            Column col_data = new Column();
            //','마다 라인 자르기
            strLine = strWhole[col].Split(',');
            //행 최대 길이 미만일때
            for (int row = 0; row < row_max; row++)
            {
                //Col에 strLine 데이터 저장
                col_data.Col.Add(strLine[row]);
            }
            //Raw에 Col데이터 저장
            CSVData.Row.Add(col_data);
        }
        //값 반환
        return CSVData;
    }
}

//Table 클래스
public class Table
{
    //리스트 Table 선언
    public List<Column> Row;
    //생성자 함수
    public Table()
    {
        //Raw 초기화
        Row = new List<Column>();
    }
}

//Col 클래스
public class Column
{
    //리스트 Column 선언
    public List<object> Col;
    //생성자 함수
    public Column()
    {
        //Column 초기화
        Col = new List<object>();
    }
}
