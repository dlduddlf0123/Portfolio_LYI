using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    GameManager gameMgr;
    CSVparser parse = new CSVparser();

    public List<List<object>> ReadDialogDatas(string _path)
    {
        Table table;
        table = parse.ParsingCSV(_path);
        List<List<object>> _datas = new List<List<object>>();
        for (int i = 0; i < table.Row.Count; i++)
        {
            List<object> _data = table.Row[i].Col;
            _datas.Add(_data);
        }
        return _datas;
    }

    public List<object> ReadDialogData(string _path,int _level)
    {
        Table table;
        table = parse.ParsingCSV(_path);
        List<object> _data = table.Row[_level].Col;
        return _data;
    }

    //string 잘라서 줄 바꾸기,출력
    public void SetText(List<object> _list,Text _txt, int _index)
    {
        string t = _list[_index].ToString();
        string[] _t = t.Split('@');
        _txt.text = null;
        for (int i = 0; i < _t.Length; i++)
        {
            _txt.text += _t[i];
            if (i < _t.Length - 1)
            {
                _txt.text += '\n';
            }
        }
    }
}
