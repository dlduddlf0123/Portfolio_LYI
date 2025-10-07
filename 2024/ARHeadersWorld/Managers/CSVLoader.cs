using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Burbird
{
    public class CSVLoader : MonoBehaviour
    {
        CSVparser parse = new CSVparser();

        public List<List<object>> ReadCSVDatas(string path)
        {
            Table table;
            table = parse.ParsingCSV(path);
            if (table == null)
                return null;

            List<List<object>> datas = new List<List<object>>();
            for (int i = 0; i < table.Row.Count; i++)
            {
                List<object> data = table.Row[i].Col;
                datas.Add(data);
            }
            return datas;
        }
        public List<List<object>> ReadCSVDatas2(string path)
        {
            Table table;
            table = parse.ParsingCSV(path);
            if (table == null)
                return null;


            List<List<object>> datas = new List<List<object>>();
            for (int i = 0; i < table.Row.Count; i++)
            {
                List<object> data = table.Row[i].Col;
                data.RemoveAll(d => d.Equals(""));
                data.RemoveAll(d => d.Equals("\r"));
                data.RemoveAll(d => d.Equals(" \r"));
                datas.Add(data);
            }
            return datas;
        }

        /// <summary>
        /// 4/5/2023-LYI
        /// 적 캐릭터 스탯 불러오기 위해 제작
        /// CSV에서 각 행의 첫번째에 구분코드를 쓸 경우 사용
        /// Dictionary 형태로 저장, 구분코드로 내용 검색하도록 정리
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<int, List<object>> ReadCSVDataDic(string path)
        {
            Table table;
            table = parse.ParsingCSV(path);
            if (table == null)
                return null;


            Dictionary<int, List<object>> datas = new();
            for (int i = 1; i < table.Row.Count; i++)
            {
                List<object> data = table.Row[i].Col;
                data.RemoveAll(d => d.Equals(""));
                data.RemoveAll(d => d.Equals("\r"));
                data.RemoveAll(d => d.Equals(" \r"));
                datas.Add(Convert.ToInt32(table.Row[i].Col[0]), data);
            }
            return datas;
        }


        public List<object> ReadCSVData(string path, int level)
        {
            Table table;
            table = parse.ParsingCSV(path);
            List<object> data = table.Row[level].Col;
            return data;
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
        public Text SetText(List<object> _list, Text _txt, int _index)
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
            return _txt;
        }
    }
}