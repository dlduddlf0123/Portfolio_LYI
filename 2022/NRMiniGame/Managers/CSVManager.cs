using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CSVManager : MonoBehaviour
{
    CSVparser parse = new CSVparser();

    /// <summary>
    /// 파일 이름을 넣으면 해당 파일의 내용을 2차원 배열로 가져온다
    /// </summary>
    /// <param name="name">CSV file name</param>
    /// <returns></returns>
    public List<List<object>> ReadCSVDatas(string name)
    {
        Table table;
        table = parse.ParsingCSV(name);
        List<List<object>> _datas = new List<List<object>>();
        for (int i = 0; i < table.Row.Count; i++)
        {
            List<object> _data = table.Row[i].Col;
            _datas.Add(_data);
        }
        return _datas;
    }
    public List<List<object>> ReadCSVDatas2(string path)
    {
        Table table;
        table = parse.ParsingCSV(path);
        List<List<object>> _datas = new List<List<object>>();
        for (int i = 0; i < table.Row.Count; i++)
        {
            List<object> _data = table.Row[i].Col;
            _data.RemoveAll(d => d.Equals(""));
            _data.RemoveAll(d => d.Equals("\r"));
            _datas.Add(_data);
        }
        return _datas;
    }

    public List<object> ReadCSVData(string path,int level)
    {
        Table table;
        table = parse.ParsingCSV(path);
        List<object> _data = table.Row[level].Col;
        return _data;
    }

    //string 잘라서 줄 바꾸기,출력
    //public void SetText(List<object> _list,Text _txt, int _index)
    //{
    //    string t = _list[_index].ToString();
    //    string[] _t = t.Split('@');
    //    _txt.text = null;
    //    for (int i = 0; i < _t.Length; i++)
    //    {
    //        _txt.text += _t[i];
    //        if (i < _t.Length - 1)
    //        {
    //            _txt.text += '\n';
    //        }
    //    }
    //}

    //string 잘라서 줄 바꾸기,출력
    public Text SetText(List<object> list, Text txt, int index)
    {
        string t = list[index].ToString();
        string[] arr_t = t.Split('@');
        txt.text = null;
        for (int i = 0; i < arr_t.Length; i++)
        {
            txt.text += arr_t[i];
            if (i < arr_t.Length - 1)
            {
                txt.text += '\n';
            }
        }
        return txt;
    }
}
